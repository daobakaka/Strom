using GPUInstancer;
using GPUInstancer.CrowdAnimations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class GpuAnimationSystemBase : SystemBase
{
    public GPUICrowdManager gpuiCrowdManager;
    private GPUICrowdPrototype _crowdPrototype; // reference to a prototype that is defined and baked on the gpuiCrowdManager.
    private GPUICrowdRuntimeData _runtimeData; // reference to the runtime data of the _crowdPrototype.do something for the job// the matrix array for the instances. Since we are not using GameObjects, we will store and 
    private GPUICrowdPrototype _crowdPrototype1; // reference to a prototype that is defined and baked on the gpuiCrowdManager.
    private GPUICrowdRuntimeData _runtimeData1; // reference to the runtime data of the _crowdPrototype.do something for the job// the matrix array for the instances. Since we are not using GameObjects, we will store and 
    private GPUICrowdPrototype _crowdPrototype2; // reference to a prototype that is defined and baked on the gpuiCrowdManager.
    private GPUICrowdRuntimeData _runtimeData2; // reference to the runtime data of the _crowdPrototype.do something for the job// the matrix array for the instances. Since we are not using GameObjects, we will store and 
    private GPUICrowdPrototype _crowdPrototype3; // reference to a prototype that is defined and baked on the gpuiCrowdManager.
    private GPUICrowdRuntimeData _runtimeData3; // reference to the runtime data of the _crowdPrototype.do something for the job// the matrix array for the instances. Since we are not using GameObjects, we will store and 
    private NativeList<float4x4> _instanceMatrixArray;
    private NativeList<CrowdInstanceState> _instanceStateArray; // CrowdInstanceState array to determine next state of the animator
    private NativeList<float4x4> _instanceMatrixArray1;
    private NativeList<CrowdInstanceState> _instanceStateArray1; // CrowdInstanceState array to determine next state of the animator
    private NativeList<float4x4> _instanceMatrixArray2;
    private NativeList<CrowdInstanceState> _instanceStateArray2; // CrowdInstanceState array to determine next state of the animator
    private NativeList<float4x4> _instanceMatrixArray3;
    private NativeList<CrowdInstanceState> _instanceStateArray3; // CrowdInstanceState array to determine next state of the animator
    private int _instanceCount;
    // demo specific internal fields
    private int _selectedPrototypeIndex = 0;
    private int _rowCount = 300;
    private int _collumnCount = 300;
    private float _space = 3f;
    private readonly int _bufferSize = 100000;
    private bool _isStateModified; // set to true when the state array is modified
    private int animatorType;
    private bool untest;
    private int jobnum, regularnum, forechnum;
    private int buffersize;

    public int num;
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireForUpdate<UnitSpawner>();
        // RequireForUpdate<StartSystemTag>();
        _instanceMatrixArray = new NativeList<float4x4>(Allocator.Persistent);
        _instanceStateArray = new NativeList<CrowdInstanceState>(Allocator.Persistent);
        _instanceMatrixArray1 = new NativeList<float4x4>(Allocator.Persistent);
        _instanceStateArray1 = new NativeList<CrowdInstanceState>(Allocator.Persistent);
        _instanceMatrixArray2 = new NativeList<float4x4>(Allocator.Persistent);
        _instanceStateArray2 = new NativeList<CrowdInstanceState>(Allocator.Persistent);
        _instanceMatrixArray3 = new NativeList<float4x4>(Allocator.Persistent);
        _instanceStateArray3 = new NativeList<CrowdInstanceState>(Allocator.Persistent);
    }
    protected override void OnStartRunning()
    {
        gpuiCrowdManager = GpuInstancerCrowd.Instance.gpuiCrowdManager;
        InsSubForGpuMonster();
        buffersize = 1000;
        num = 10000;
 
    }
    protected override void OnStopRunning()
    {
     
    }
    protected override void OnDestroy()
    {
        _instanceMatrixArray.Dispose();
        _instanceStateArray.Dispose();
        _instanceMatrixArray1.Dispose();
        _instanceStateArray1.Dispose();
        _instanceMatrixArray2.Dispose();
        _instanceStateArray2.Dispose();
        _instanceMatrixArray3.Dispose();
        _instanceStateArray3.Dispose();
    }
    protected override void OnUpdate()
    {
      
         _instanceMatrixArray.Clear();
         _instanceStateArray.Clear();
         _instanceMatrixArray1.Clear();
         _instanceStateArray1.Clear();
        _instanceMatrixArray2.Clear();
        _instanceStateArray2.Clear();
        _instanceMatrixArray3.Clear();
        _instanceStateArray3.Clear();

        var random = new Unity.Mathematics.Random(1234);
        #region the module of entities.foreach
        //Entities.ForEach((ref LocalTransform transform, ref CrowdInstanceState animation, in Unit1Component unit) =>
        //{
        //    if (unit.order == 61)
        //    {

        //        entityMatrixArray.Add(transform.ToMatrix());
        //        _instanceMatrixArray.Add(transform.ToMatrix());
        //        if (_instanceStateArray.Length < entityMatrixArray.Length)
        //            _instanceStateArray.Add(new CrowdInstanceState
        //            {
        //                animationIndex = animation.animationIndex,
        //                animationSpeed = 1,
        //                animationStartTimeMultiplier = random.NextFloat(0, 1f),
        //                modificationType = StateModificationType.All
        //            });
        //    }
        //}).WithoutBurst().Run();
        #endregion
        foreach (var (unit, transform,crowdInstance) in SystemAPI.Query<RefRW<Unit1Component>, RefRW<LocalTransform>,RefRW<CrowdInstanceState>>())
        {
            if (unit.ValueRO.order == 61)
            {
                var data = transform.ValueRO.ToMatrix();
                _instanceMatrixArray.Add(data);
                _instanceStateArray.Add(crowdInstance.ValueRO);
                //if (_instanceStateArray.Length < _instanceMatrixArray.Length)
                //    _instanceStateArray.Add(new CrowdInstanceState
                //    {
                //        animationIndex = crowdInstance.ValueRW.animationIndex,
                //        animationSpeed = 1,
                //        animationStartTimeMultiplier = crowdInstance.ValueRW.initanimationStartTimeMultiplier,
                //        modificationType = StateModificationType.All
                //    });
            }
            if (unit.ValueRO.order == 62)
            {
                var data = transform.ValueRO.ToMatrix();
                _instanceMatrixArray1.Add(data);
                _instanceStateArray1.Add(crowdInstance.ValueRO);
                //if (_instanceStateArray1.Length < _instanceMatrixArray1.Length)
                //    _instanceStateArray1.Add(new CrowdInstanceState
                //    {
                //        animationIndex = crowdInstance.ValueRW.animationIndex,
                //        animationSpeed = 1,
                //        animationStartTimeMultiplier = crowdInstance.ValueRW.initanimationStartTimeMultiplier,
                //        modificationType = StateModificationType.All
                //    });
            }
            if (unit.ValueRO.order == 63)
            {
                _instanceMatrixArray2.Add(transform.ValueRO.ToMatrix());
                _instanceStateArray2.Add(crowdInstance.ValueRO);
                //if (_instanceStateArray2.Length < _instanceMatrixArray2.Length)
                //    _instanceStateArray2.Add(new CrowdInstanceState
                //    {
                //        animationIndex = crowdInstance.ValueRW.animationIndex,
                //        animationSpeed = 1,
                //        animationStartTimeMultiplier = crowdInstance.ValueRW.initanimationStartTimeMultiplier,
                //        modificationType = StateModificationType.All
                //    });
            }
            if (unit.ValueRO.order == 64)
            {
                _instanceMatrixArray3.Add(transform.ValueRO.ToMatrix());
                _instanceStateArray3.Add(crowdInstance.ValueRO);
                //if (_instanceStateArray3.Length < _instanceMatrixArray3.Length)
                //    _instanceStateArray3.Add(new CrowdInstanceState
                //    {
                //        animationIndex = crowdInstance.ValueRW.animationIndex,
                //        animationSpeed = 1,
                //        animationStartTimeMultiplier = crowdInstance.ValueRW.initanimationStartTimeMultiplier,
                //        modificationType = StateModificationType.All
                //    });
            }


        }
        //foreach (var item in _instanceMatrixArray)
        //{
        //    item.SetRow(1,new Vector4(20, 20, 20, 20));
        //    if (item.lossyScale.x < 0) 
        //    {
        //    }
        //    Debug.LogError(item.lossyScale);
        //    Debug.LogError(item.m11);
        //}
        //foreach (var item in _instanceMatrixArray1)
        //{
        //    item.SetRow(1, new Vector4(20, 20, 20, 20));
        //    Debug.LogError(item.lossyScale);
        //    if (item.lossyScale.x < 0)
        //    {
        //    }
        //    Debug.LogError(item.m11);
        //}
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype,  _instanceMatrixArray.Length+1, _instanceMatrixArray.Length);
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype1, _instanceMatrixArray1.Length+1, _instanceMatrixArray1.Length);
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype2, _instanceMatrixArray2.Length+1, _instanceMatrixArray2.Length);
        GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype3, _instanceMatrixArray3.Length+1, _instanceMatrixArray3.Length);
        if (!_runtimeData.clipDatas.IsCreated)
            _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);
        if (!_runtimeData1.clipDatas.IsCreated)
            _runtimeData1.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype1.animationData.clipDataList.ToArray(), Allocator.Persistent);
        if (!_runtimeData2.clipDatas.IsCreated)
            _runtimeData2.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype2.animationData.clipDataList.ToArray(), Allocator.Persistent);
        if (!_runtimeData3.clipDatas.IsCreated)
            _runtimeData3.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype3.animationData.clipDataList.ToArray(), Allocator.Persistent);
        if (_isStateModified)
            {
            _runtimeData.dependentJob = new GpuCrowdInstancerAnimaionBase()
            {
                clipDatas = _runtimeData.clipDatas,
                animationData = _runtimeData.animationData,
                crowdAnimatorControllerData = _runtimeData.crowdAnimatorControllerData,
                instanceStateArray = _instanceStateArray.AsArray(),
            }.Schedule(_instanceMatrixArray.Length, 64, _runtimeData.dependentJob);
               // _isStateModified = false;
                _runtimeData.animationDataModified = true;
                _runtimeData.crowdAnimatorDataModified = true;
            }
    
        if (_isStateModified)
        {
            _runtimeData1.dependentJob = new GpuCrowdInstancerAnimaionBase()
            {
                clipDatas = _runtimeData1.clipDatas,
                animationData = _runtimeData1.animationData,
                crowdAnimatorControllerData = _runtimeData1.crowdAnimatorControllerData,
                instanceStateArray = _instanceStateArray1.AsArray(),
            }.Schedule(_instanceMatrixArray1.Length, 64, _runtimeData1.dependentJob);
            // _isStateModified = false;
            _runtimeData1.animationDataModified = true;
            _runtimeData1.crowdAnimatorDataModified = true;
        }
        if (_isStateModified)
        {
            _runtimeData2.dependentJob = new GpuCrowdInstancerAnimaionBase()
            {
                clipDatas = _runtimeData2.clipDatas,
                animationData = _runtimeData2.animationData,
                crowdAnimatorControllerData = _runtimeData2.crowdAnimatorControllerData,
                instanceStateArray = _instanceStateArray2.AsArray(),
            }.Schedule(_instanceMatrixArray2.Length, 64, _runtimeData2.dependentJob);
            // _isStateModified = false;
            _runtimeData2.animationDataModified = true;
            _runtimeData2.crowdAnimatorDataModified = true;
        }
        if (_isStateModified)
        {
            _runtimeData3.dependentJob = new GpuCrowdInstancerAnimaionBase()
            {
                clipDatas = _runtimeData3.clipDatas,
                animationData = _runtimeData3.animationData,
                crowdAnimatorControllerData = _runtimeData3.crowdAnimatorControllerData,
                instanceStateArray = _instanceStateArray3.AsArray(),
            }.Schedule(_instanceMatrixArray3.Length, 64, _runtimeData3.dependentJob);
            // _isStateModified = false;
            _runtimeData3.animationDataModified = true;
            _runtimeData3.crowdAnimatorDataModified = true;
        }

        _runtimeData.dependentJob.Complete();
        _runtimeData1.dependentJob.Complete();
        _runtimeData2.dependentJob.Complete();
        _runtimeData3.dependentJob.Complete();
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype, _instanceMatrixArray.AsArray()); // Set Matrix array to the visibility buffer                                                                                                                ////                                                                                                                          //_isStateModified = true;
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype1, _instanceMatrixArray1.AsArray());
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype2, _instanceMatrixArray2.AsArray());
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype3, _instanceMatrixArray3.AsArray());

        #region the module of test
        if (Input.GetKeyDown(KeyCode.M))
        {
            GpuCrowdIns();
            _isStateModified = true;
                Debug.Log($"instacneState length:{_instanceStateArray.Length}," +
                $"and the instanceMatricx.length:{_instanceMatrixArray.Length} ," +
                $"and the GpuanimationClipData.length£∫{_runtimeData.clipDatas.Length}," +
                $"and the _runtimeData.animationData.length is:{_runtimeData.animationData.Length}");
        }
        #endregion
        #region the module of dispose
        #endregion
    }
    void InsSubForGpuMonster()
    {
        _isStateModified = true;
        _crowdPrototype = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex + 4];
        _crowdPrototype.animationData.useCrowdAnimator = true;
        _runtimeData = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype);
        //--
        _crowdPrototype1 = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex+1];
        _crowdPrototype1.animationData.useCrowdAnimator = true;
        _runtimeData1 = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype1);
        //
        _crowdPrototype2 = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex+2];
        _crowdPrototype2.animationData.useCrowdAnimator = true;
        _runtimeData2 = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype2);
        //--
        _crowdPrototype3 = (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[_selectedPrototypeIndex+3];
        _crowdPrototype3.animationData.useCrowdAnimator = true;
        _runtimeData3 = (GPUICrowdRuntimeData)gpuiCrowdManager.GetRuntimeData(_crowdPrototype3);
        //
        //if (!_runtimeData.clipDatas.IsCreated)
        //    _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);
        //if (!_runtimeData1.clipDatas.IsCreated)
        //    _runtimeData1.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype1.animationData.clipDataList.ToArray(), Allocator.Persistent);
        //if (!_runtimeData2.clipDatas.IsCreated)
        //    _runtimeData2.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype2.animationData.clipDataList.ToArray(), Allocator.Persistent);
        //if (!_runtimeData3.clipDatas.IsCreated)
        //    _runtimeData3.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype3.animationData.clipDataList.ToArray(), Allocator.Persistent);
    }
    public void GpuCrowdIns()
    {
            _runtimeData.dependentJob.Complete();
            //for (int i = 0; i < _instanceMatrixArray.Length; i++)
            //{
            //   // persisentArrayMatrix[i] = _instanceMatrixArray[i];
            //    persisentArrayState[i] = _instanceStateArray[i];
            
            //}
            
            GPUInstancerAPI.InitializePrototype(gpuiCrowdManager, _crowdPrototype, _bufferSize, _instanceMatrixArray.Length);
            //if (!_runtimeData.clipDatas.IsCreated)
            //    _runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(_crowdPrototype.animationData.clipDataList.ToArray(), Allocator.Persistent);
            GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager, _crowdPrototype, _instanceMatrixArray.AsArray()); // Set Matrix array to the visibility buffer
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
}
public partial struct GpuCrowdInstancerAnimaionBase : IJobParallelFor
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
        //if (index * 4 >= crowdAnimatorControllerData.Length)
        //{
        //    Debug.LogError("∂Øª≠…Ë÷√ ß∞‹" + index); 
        //    return;
        //}
        //else 
        //{
        //    Debug.LogError("∂Øª≠" + index);
        //}
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
