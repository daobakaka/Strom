using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AppearAuthoring : MonoBehaviour
{
    [Tooltip("MonsterAppear：怪物出场，MonsterAirForceAppear：飞行怪物出场，BaZhuApper：霸主出场")]
    public AppearName appearName;
    [Tooltip("--出场行走的距离--")] public float AppearDistance;//出场行走的距离
    [Tooltip("--出场速度--")]public float AppearSpeed;//出场速度
}
public class AppearBaker : Baker<AppearAuthoring>
{
    public override void Bake(AppearAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var appear = new Appear
        {
            appearName = authoring.appearName,
            AppearDistance = authoring.AppearDistance,
            AppearSpeed = authoring.AppearSpeed,
        };
        AddComponent(entity, appear);
    }
}
