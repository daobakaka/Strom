using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WalkAuthoring : MonoBehaviour
{

}

public class WalkBaker : Baker<WalkAuthoring>
{
    public override void Bake(WalkAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var walk = new Walk();
        AddComponent(entity, walk);
    }
}