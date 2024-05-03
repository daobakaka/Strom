using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FengHuangAuthoring : MonoBehaviour
{
    public GameObject FollowPoint;//¸úËæµã
}

public class FengHuangBaker : Baker<FengHuangAuthoring>
{
    public override void Bake(FengHuangAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var fenghuang = new FengHuang
        {
            FollowPoint = GetEntity(authoring.FollowPoint.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, fenghuang);
    }
}