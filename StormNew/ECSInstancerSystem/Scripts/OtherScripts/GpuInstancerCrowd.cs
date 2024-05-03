using GPUInstancer;
using GPUInstancer.CrowdAnimations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Entities;
using System.ComponentModel;
using ReadOnlyAttribute = Unity.Collections.ReadOnlyAttribute;
public enum StateModificationType // example enum to determine what to update inside the Job
{
    None = 0,
    All = 1,
    Clip = 2,
    Speed = 3,
    StartTime = 4
}

public class GpuInstancerCrowd : MonoBehaviour
{
    [HideInInspector]
    public static GpuInstancerCrowd Instance;
    //---The GPU Instance
    public GPUICrowdManager gpuiCrowdManager;
    public GPUICrowdPrototype _crowdPrototype; // reference to a prototype that is defined and baked on the gpuiCrowdManager.
    public GPUICrowdRuntimeData _runtimeData; // reference to the runtime data of the _crowdPrototype.do something for the job
    private NativeArray<Matrix4x4> _instanceMatrixArray; // the matrix array for the instances. Since we are not using GameObjects, we will store and 
                                                         // reference the instances by their transform matrices. 
                                                         // (Each matrix hold the position, scale and rotation information for an instance.)
    private NativeArray<CrowdInstanceState> _instanceStateArray; // CrowdInstanceState array to determine next state of the animator
    private int _instanceCount;
    // demo specific internal fields
    private int _selectedPrototypeIndex = 0;
    private int _rowCount = 300;
    private int _collumnCount = 300;
    private float _space = 3f;
    private readonly int _bufferSize = 100000;
    public bool _isStateModified; // set to true when the state array is modified
    private int animatorType;
    public bool iftest;
    private bool testJob = true;
    private void Awake()
    {
        Instance = this;
        if (!iftest)
            InsSubForGpuMonster();
    }
    void Start()
    {
        if(iftest)
        GpuCrowdIns();
    }
    void InsSubForGpuMonster()
    {
        _crowdPrototype = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex];
        _crowdPrototype.animationData.useCrowdAnimator = true;
        _runtimeData = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype);

    }

    // Update is called once per frame
    void Update()
    {
        if (iftest)
            RunJobGpuAnimator();
    }
    private GpuInstancerCrowd() { }

    private struct ApplyAnimationStateJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<GPUIAnimationClipData> clipDatas;
        [ReadOnly] public NativeArray<CrowdInstanceState> instanceStateArray;
        /// <summary>
        /// <para>index: 0 x -> frameNo1, y -> frameNo2, z -> frameNo3, w -> frameNo4</para> 
        /// <para>index: 1 x -> weight1, y -> weight2, z -> weight3, w -> weight4</para> 
        /// </summary>
        [NativeDisableParallelForRestriction] public NativeArray<Vector4> animationData;
        /// <summary>
        /// 0 to 4: x ->  minFrame, y -> maxFrame (negative if not looping), z -> speed, w -> startTime
        /// </summary>
        [NativeDisableParallelForRestriction] public NativeArray<Vector4> crowdAnimatorControllerData;

        public void Execute(int index)
        {
            CrowdInstanceState state = instanceStateArray[index];
            if (state.modificationType == StateModificationType.None)
                return;

            Vector4 activeClip0 = crowdAnimatorControllerData[index * 4];
            Vector4 clipFrames = animationData[index * 2];
            Vector4 clipWeights = animationData[index * 2 + 1];
            GPUIAnimationClipData clipData = clipDatas[state.animationIndex];

            switch (state.modificationType)
            {
                case StateModificationType.All:
                    clipFrames.x = clipData.clipStartFrame;
                    activeClip0.x = clipData.clipStartFrame;
                    activeClip0.y = clipData.clipStartFrame + clipData.clipFrameCount - 1;
                    activeClip0.z = state.animationSpeed;
                    activeClip0.w = clipDatas[state.animationIndex].length * state.animationStartTimeMultiplier;
                    clipWeights = new Vector4(1f, 0f, 0f, 0f);
                    break;
                case StateModificationType.Clip:
                    clipFrames.x = clipData.clipStartFrame;
                    activeClip0.x = clipData.clipStartFrame;
                    activeClip0.y = clipData.clipStartFrame + clipData.clipFrameCount - 1;
                    activeClip0.w = 0f;
                    clipWeights = new Vector4(1f, 0f, 0f, 0f);
                    break;
                case StateModificationType.Speed:
                    activeClip0.z = state.animationSpeed;
                    break;
                case StateModificationType.StartTime:
                    activeClip0.w = clipData.length * state.animationStartTimeMultiplier;
                    break;
            }

            animationData[index * 2] = clipFrames;
            animationData[index * 2 + 1] = clipWeights;
            crowdAnimatorControllerData[index * 4] = activeClip0;

        }
    }
  public void GpuCrowdIns()
    {

        if (gpuiCrowdManager == null)
            return;

        _crowdPrototype = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex]; // Get the crowd prototype at the current index,
        //--
        _crowdPrototype.animationData.useCrowdAnimator = true; // and set its Animator Workflow to be the Crowd Animator Workflow by default.
        _instanceCount = _rowCount * _collumnCount;
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype, _bufferSize, _instanceCount); // Initialize buffers with the buffer size
        _runtimeData = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype); // get the runtime data for the instances of the current prototype

        // Make sure the GPUIAnimationClipData array is created to be able to access clip data inside Jobs
        if (!_runtimeData.clipDatas.IsCreated)
            _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);

        _instanceMatrixArray = new NativeArray<Matrix4x4>(_bufferSize, Allocator.Persistent); // initialize the matrix arrays for a maximum of 10k instances,
        _instanceStateArray = new NativeArray<CrowdInstanceState>(_bufferSize, Allocator.Persistent); // and initialize the state array for the same amount of instances. 
                                                                                                      // The indexes will be the same for the instances in each array.

        // Create the matrices for all the potential 10k instances. 
        // Rest of the matrix array other than the initial 900 instances will be ignored by GPUI 
        // so there will be no performance overhang (See GPUInstancerAPI.SetInstanceCount below).
        // Memory will be reserved for the whole array. However, caching the instances as such will 
        // result in lightning fast add/remove operations on the instances.
        GameObject prefabObject = _crowdPrototype.prefabObject;
        Vector3 pos = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(0, 180, 0) * prefabObject.transform.rotation; // we refer to the prototype's prefab to account for the rotation in the original prefab.
        int index = 0;
        for (int cycle = 1; cycle <= 10; cycle++)
        {
            int count = cycle * 10;
            for (int r = 0; r < count; r++)
            {
                for (int c = (r < count - 10 ? count - 10 : 0); c < count; c++)
                {
                    pos.x = _space * r;
                    pos.z = _space * c;
                    _instanceMatrixArray[index] = Matrix4x4.TRS(pos, rotation, Vector3.one); // create a transform matrix
                    _instanceStateArray[index] = new CrowdInstanceState()
                    {
                        animationIndex = _crowdPrototype.animationData.crowdAnimatorDefaultClip,
                        animationSpeed = 1,
                        animationStartTimeMultiplier = UnityEngine.Random.Range(0.0f, 0.99f),
                        modificationType = StateModificationType.All
                    }; // and a state for each instance.
                    index++;
                }
            }
        }

        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype, _instanceMatrixArray); // Set Matrix array to the visibility buffer
        _isStateModified = true;
    }
      public void GpuCrowdIns(NativeArray<Matrix4x4> _instanceMatrixArray)
    {if (testJob)
        {
            if (gpuiCrowdManager == null)
                return;
            GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype, _bufferSize, _instanceMatrixArray.Length);
            if (!_runtimeData.clipDatas.IsCreated)
                _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);
            GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype, _instanceMatrixArray); // Set Matrix array to the visibility buffer
            _instanceCount = _instanceMatrixArray.Length;
            testJob = false;
        }
    }
    public void RunJobGpuAnimator(NativeArray<CrowdInstanceState> crowdInstanceState)
    {
        
        if (_isStateModified)
        {
            // Schedule the ApplyAnimationStateJob
            _runtimeData.dependentJob = new ApplyAnimationStateJob()
            {
                clipDatas = _runtimeData.clipDatas,
                animationData = _runtimeData.animationData,
                crowdAnimatorControllerData = _runtimeData.crowdAnimatorControllerData,
                instanceStateArray = crowdInstanceState
                //instanceStateArray = _instanceStateArray
            }.Schedule(_instanceCount, 64, _runtimeData.dependentJob);
            _isStateModified = false;

            // Notify Crowd Manager that the NativeArrays are modified
            _runtimeData.animationDataModified = true;
            _runtimeData.crowdAnimatorDataModified = true;
        }

    }
    public void RunJobGpuAnimator()
    {
        if (_isStateModified)
        {
            // Schedule the ApplyAnimationStateJob
            _runtimeData.dependentJob = new ApplyAnimationStateJob()
            {
                clipDatas = _runtimeData.clipDatas,
                animationData = _runtimeData.animationData,
                crowdAnimatorControllerData = _runtimeData.crowdAnimatorControllerData,
                instanceStateArray = _instanceStateArray
            }.Schedule(_instanceCount, 64, _runtimeData.dependentJob);
            _isStateModified = false;

            // Notify Crowd Manager that the NativeArrays are modified
            _runtimeData.animationDataModified = true;
            _runtimeData.crowdAnimatorDataModified = true;
        }

    }
    public void ResetAnimations()
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();

        for (int i = 0; i < _instanceCount; i++)
        {
            // Staring the default animation clip for the current prototype for this instance
            CrowdInstanceState previousState = _instanceStateArray[i];
            previousState.animationIndex = _crowdPrototype.animationData.crowdAnimatorDefaultClip;
            previousState.animationSpeed = 1f;
            previousState.animationStartTimeMultiplier = 0;
            previousState.modificationType = StateModificationType.All;
            _instanceStateArray[i] = previousState;
        }
        _isStateModified = true;
    }
    public void RandomizeFrames()
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();

        for (int i = 0; i < _instanceCount; i++)
        {
            // Starts the current animation from a random frame index for this instance
            CrowdInstanceState previousState = _instanceStateArray[i];
            previousState.animationStartTimeMultiplier = UnityEngine.Random.Range(0.0f, 0.99f);
            previousState.modificationType = StateModificationType.StartTime;
            _instanceStateArray[i] = previousState;
        }
        _isStateModified = true;
    }
    public void RandomizeClips()
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();

        int clipCount = _crowdPrototype.animationData.clipDataList.Count;
        for (int i = 0; i < _instanceCount; i++)
        {
            // Start a random animation for this instance
            CrowdInstanceState previousState = _instanceStateArray[i];
            previousState.animationIndex = UnityEngine.Random.Range(0, clipCount);
            previousState.animationStartTimeMultiplier = 0;
            previousState.modificationType = StateModificationType.Clip;
            _instanceStateArray[i] = previousState;
        }
        _isStateModified = true;
    }
    public void ChnageClips()
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();
        if (animatorType == 3)
            animatorType = 0;

        int clipCount = _crowdPrototype.animationData.clipDataList.Count;
        for (int i = 0; i < _instanceCount; i++)
        {
            // Start a random animation for this instance
            CrowdInstanceState previousState = _instanceStateArray[i];
            previousState.animationIndex = animatorType;
            previousState.animationStartTimeMultiplier = 0;
            previousState.modificationType = StateModificationType.Clip;
            _instanceStateArray[i] = previousState;
        }
        _isStateModified = true;
        animatorType++;
    }
    public void SetCrowdAnimatorSpeed(float speed)
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();

        for (int i = 0; i < _instanceCount; i++)
        {
            // Set the animation speed for the current animation for this instance
            CrowdInstanceState previousState = _instanceStateArray[i];
            previousState.animationSpeed = speed;
            previousState.modificationType = StateModificationType.Speed;
            _instanceStateArray[i] = previousState;
        }

        _isStateModified = true;
    }
    public void SwitchProtoype()
    {
        if (gpuiCrowdManager == null || !gpuiCrowdManager.enabled)
            return;
        _runtimeData.dependentJob.Complete();

        // set the rendered instance count to 0 for the current prototype, ignoring all matrices
        GPUInstancerAPI.SetInstanceCount(gpuiCrowdManager, _crowdPrototype, 0);

        // switch prototype index
        _selectedPrototypeIndex++;
        if (_selectedPrototypeIndex >= gpuiCrowdManager.prototypeList.Count)
            _selectedPrototypeIndex = 0;

        // get the next prototype from the manager with the switched index
        _crowdPrototype = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex];

        // initialize the manager with the new prototype and the same transform matrix array
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype, _bufferSize, _instanceCount);
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype, _instanceMatrixArray);

        // get the runtime data for the current prototype
        _runtimeData = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype);
        // Set GPUIAnimationClipData array to be able to access clip data inside Jobs
        if (!_runtimeData.clipDatas.IsCreated)
            _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);

        for (int i = 0; i < _instanceCount; i++)
        {
            // start the default animation clip from a random frame for this instance
            _instanceStateArray[i] = new CrowdInstanceState()
            {
                animationIndex = _crowdPrototype.animationData.crowdAnimatorDefaultClip,
                animationSpeed = 1,
                animationStartTimeMultiplier = UnityEngine.Random.Range(0.0f, 0.99f),
                modificationType = StateModificationType.All
            };
        }
        _isStateModified = true;
    }
}
//public struct CrowdInstanceState : IComponentData// example struct that determines the state for crowd animator
//{
//    public int animationIndex;
//    public float animationSpeed;
//    public float animationStartTimeMultiplier;
//    public StateModificationType modificationType;

//}