using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BezierFiringAuthoring : MonoBehaviour
{
    [Tooltip("--���������߿��Ƶ�ĸ߶�--------")][Range(0.0f, 1.0f)] public float heightFactor;//�߶�
    [Tooltip("--�͵���֮���ʲô���뿪ʼ����--")][Range(0.0f, 1.0f)] public float LerpFactor;//�͵���֮���ʲô���뿪ʼ����
}

public class BezierFiringBaker : Baker<BezierFiringAuthoring>
{
    public override void Bake(BezierFiringAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var bezierFiring = new BezierFiring
        {
            heightFactor = authoring.heightFactor,
            LerpFactor = authoring.LerpFactor,
        };
        AddComponent(entity, bezierFiring);
    }
}