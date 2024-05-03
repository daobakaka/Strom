using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TOWERAuthoring : MonoBehaviour
{
    [Tooltip("--·ÀÓùËþÅÚ¹Ü--")] public GameObject GunBarrel;
}

public class TOWERBaker : Baker<TOWERAuthoring>
{
    public override void Bake(TOWERAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var Towerb = new TOWER
        {
            GunBarrel = GetEntity(authoring.GunBarrel.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, Towerb);
    }
}