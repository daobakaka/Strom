using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class RotateTiresAuthoring : MonoBehaviour
{
    public GameObject Owner;//这个轮胎的拥有者
}

public class RotateTiresBaker : Baker<RotateTiresAuthoring>
{
    public override void Bake(RotateTiresAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var rotatetires = new RotateTires 
        {
            Owner = GetEntity(authoring.Owner.gameObject,TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, rotatetires);
    }
}