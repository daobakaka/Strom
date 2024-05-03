using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public enum AttactBoxShape
{
    NUll,
    Box,//矩形
    Capsule,//胶量
    Sphere,//圆形
}

public enum AttactBoxState
{
    NUll,
    OnceDamage,//单次伤害

    ManyDamage,//多次伤害

}


public struct AttackBox : IComponentData
{
    public AttactBoxShape BoxShape;//盒子形状
    public float3 halfExtents;//盒子的长宽高
    public float3 Offset;//盒子偏移值
    public float R;
    public AttactBoxState BoxState;
    public float ExistenceTime;//Box存在时间
    public float SustainTime;//持续伤害的间隔
    public float Cur_SustainTime;
    public float AT;//盒子的攻击力
    public bool Is_All;//是否攻击检测到的全部单位
    public bool Is_Restore;//是否为恢复效果
    public bool Is_NoTime;
    public bool Is_Remote;
    public float MaxRemote;
}
