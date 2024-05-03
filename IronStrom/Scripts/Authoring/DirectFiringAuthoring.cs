using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DirectFiringAuthoring : MonoBehaviour
{

}

public class DirectFiringBaker : Baker<DirectFiringAuthoring>
{
    public override void Bake(DirectFiringAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var directFiring = new DirectFiring();
        AddComponent(entity, directFiring);
    }
}
