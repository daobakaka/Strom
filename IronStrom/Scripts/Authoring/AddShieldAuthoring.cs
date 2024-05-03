using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AddShieldAuthoring : MonoBehaviour
{
    public GameObject ShieldParent;//��
    public float ShieldScale;//���ܰ뾶
    public float ShieldHP;//����ֵ
    [Tooltip("--���������ٶ�--")] public float ShieldExpandSpeed;
}

public class AddShieldBaker : Baker<AddShieldAuthoring>
{
    public override void Bake(AddShieldAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var addshield = new AddShield
        {
            ShieldParent = GetEntity(authoring.ShieldParent, TransformUsageFlags.Dynamic),
            ShieldScale = authoring.ShieldScale,
            ShieldHP = authoring.ShieldHP,
            ShieldExpandSpeed = authoring.ShieldExpandSpeed,
        };
        AddComponent(entity, addshield);
    }
}