using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FireAuthoring : MonoBehaviour
{

}

public class FireBaker : Baker<FireAuthoring>
{
    public override void Bake(FireAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var fire = new Fire();
        AddComponent(entity, fire);
    }
}