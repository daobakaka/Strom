using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BuffAuthoring : MonoBehaviour
{

}

public class BuffBaker : Baker<BuffAuthoring>
{
    public override void Bake(BuffAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        AddBuffer<Buff>(entity);
    }
}