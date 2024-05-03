using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GpuMonsterAuthoring : MonoBehaviour
{
    class GpuMonsterBaker : Baker<GpuMonsterAuthoring>
    {
        public override void Bake(GpuMonsterAuthoring authoring)
        {
            var newentity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(newentity, new CrowdInstanceState
            {

            });
        }
    }
}
