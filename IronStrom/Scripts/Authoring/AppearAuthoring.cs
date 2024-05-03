using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AppearAuthoring : MonoBehaviour
{
    [Tooltip("MonsterAppear�����������MonsterAirForceAppear�����й��������BaZhuApper����������")]
    public AppearName appearName;
    [Tooltip("--�������ߵľ���--")] public float AppearDistance;//�������ߵľ���
    [Tooltip("--�����ٶ�--")]public float AppearSpeed;//�����ٶ�
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
