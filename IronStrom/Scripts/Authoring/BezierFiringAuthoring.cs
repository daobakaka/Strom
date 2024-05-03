using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BezierFiringAuthoring : MonoBehaviour
{
    [Tooltip("--贝塞尔曲线控制点的高度--------")][Range(0.0f, 1.0f)] public float heightFactor;//高度
    [Tooltip("--和敌人之间的什么距离开始弯曲--")][Range(0.0f, 1.0f)] public float LerpFactor;//和敌人之间的什么距离开始弯曲
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