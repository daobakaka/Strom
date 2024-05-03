using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ParasiticAuthoring : MonoBehaviour
{
    public GameObject Owner;
}

public class ParasiticBaker : Baker<ParasiticAuthoring>
{
    public override void Bake(ParasiticAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject,TransformUsageFlags.Dynamic);
        var paras = new Parasitic
        {
            Owner = GetEntity(authoring.Owner.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, paras);
    }
}