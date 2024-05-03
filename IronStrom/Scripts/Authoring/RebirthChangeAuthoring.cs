using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RebirthChangeAuthoring : MonoBehaviour
{

}

public class RebirthChangeBaker : Baker<RebirthChangeAuthoring>
{
    public override void Bake(RebirthChangeAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var rebirthChange = new RebirthChange();
        AddComponent(entity, rebirthChange);
    }
}