using GPUInstancer;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;

public partial class GPUIInstancingSystem : SystemBase
{
    Matrix4x4[] instanceMatrices = new Matrix4x4[100];
    NativeList<float3> positions;
    NativeList<float4> rotations;
    NativeList<float3> scales;
    public GPUInstancerPrefab GPUInstancerPrefab;
    protected override void OnCreate()
    {
        RequireForUpdate<TestSystemGpu>();
     

    }
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TestSystemGpu testSystem, ref LocalTransform transform) =>
        {
            transform.Position += new float3(0, 0, 1) * SystemAPI.Time.DeltaTime;
        }).Schedule();
    }
    void GetPicture()
    {
        for (int i = 0; i < 100; i++)
        {
            instanceMatrices[i] = Matrix4x4.Identity; //positions[i]* rotations[i]* scales[i]);
        }

        foreach (var matrix in instanceMatrices)
        {
            //GPUInstancerAPI.SetInstanceCount(gpuiCrowdManager,
            //       (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[prototypes.prototypeIndex - 1], _length);
            //GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(gpuiCrowdManager,
            //    (GPUICrowdPrototype)gpuiCrowdManager.prototypeList[prototypes.prototypeIndex - 1], bigArray, _offset, 0, _length);
        }

    }
}

