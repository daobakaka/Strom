using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BezierFiringChange : IComponentData
{
    public float3 StartPosition;//起点
    public float3 EndPosition;//终点
    public float3 ControlPoint;//控制点
    public float DirenDistance;//敌人距离，用来求飞行总时间
    public float ElapsedTime;//飞行经过时间
    public bool Is_RandomBezier;// 是否为随机贝塞尔曲线
}
