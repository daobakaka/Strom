using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShiBingChangeAuthoring : MonoBehaviour
{

}

public class ShiBingChangeBaker : Baker<ShiBingChangeAuthoring>
{
    public override void Bake(ShiBingChangeAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var shibingChange = new ShiBingChange();

        AddComponent(entity, shibingChange);
    }
}
