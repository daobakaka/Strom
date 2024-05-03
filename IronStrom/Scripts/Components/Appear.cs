using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum AppearName
{
    NUll,
    MonsterAppear,//怪物出场
    MonsterAirForceAppear,//怪物出场
    BaZhuApper,//霸主出场
    BaZhuApper2,//霸主出场2
}

public struct Appear : IComponentData//出场行为
{
    public AppearName appearName;
    public float AppearDistance;//出场行走的距离
    public float AppearSpeed;//出场速度

    [HideInInspector] public bool IsOver_AppearAnimPlay;//出场动画是否播放完毕
    [HideInInspector] public float3 AppearPos;//出场动画播放完后，出现的位置
    [HideInInspector] public quaternion AppearRot;//出场动画播放完后，出现的位置
}
