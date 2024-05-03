using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GpuECSAniAuthoring : MonoBehaviour
{

}

public class GpuECSAniBaker : Baker<GpuECSAniAuthoring>
{
    public override void Bake(GpuECSAniAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var gpuECSAni = new GpuECSAni();
        AddComponent(entity, gpuECSAni);
    }
}
