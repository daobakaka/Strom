using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BezierFiring : IComponentData
{
    public float heightFactor;//高度
    public float LerpFactor;//和敌人之间的什么距离开始弯曲
}
