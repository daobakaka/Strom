using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public struct DirectShoot : IComponentData//表示攻击行为 为直接作用的角色
{
    public Entity DirectParticle_Parfab;//直接攻击的特效，由子弹同步和士兵的位置关系
    public Entity DirectParticle_Entity;//我实例化的子弹
    public bool Is_CumulativeDamage;//是否为积累伤害
    public float AT_Min;//伤害最小
    public float AT_Max;//伤害最上线
    public float IntervalTime;//伤害累加的累加间隔
    public float Cur_IntervalTime;//伤害累加的累加间隔
    public Entity CD_ShootEntity;//累积伤害的目标，用于检测是否是同一个单位，如果不是累积的伤害重新开始

    public bool Is_ShootEntityChanges;//攻击目标是否发生变化
}
