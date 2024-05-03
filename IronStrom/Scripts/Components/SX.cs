using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SX : IComponentData
{
    public float HP;//生命值
    public float Cur_HP;
    public float DP;//防御力
    public float AT;//攻击力
    public float DB;//格挡值
    public float Tardis;//检测范围
    public float Shootdis;//攻击范围
    public float ShootTime;//子弹发射时间
    public float Cur_ShootTime;
    public float Speed;
    public float Init_Speed;//初始移动速度
    public float Record_Speed;//记录移动速度是否和上一帧一样
    public float VolumetricDistance;//体积距离
    public int Fire_TakeTurnsIntNum;//轮流攻击次数
    public int Cur_Fire_TakeTurnsIntNum;//轮流攻击次数
    public float AttackNumberTime;//攻击次数间隔时间
    public float Cur_AttackNumberTime;//攻击次数间隔时间

    public bool Is_Die;//是否死亡
    public SkyGround Anti_SX;//对空对地属性
    public AirGroundAdvantage Advantage;//对空对地优势
    public bool Is_AirForce;//是否为空军
    public bool Is_UpSkill;//是否已经升级

    public float BuffParticleScale;//buff特效大小
    public float MutinyValue;//策反的值

    public float AinWalkSpeed;//移动动画速度
    public float Cur_AinWalkSpeed;//移动动画速度
    public bool Is_ChangedAinWalkSpeed;//是否改变了移动动画的速度

}
