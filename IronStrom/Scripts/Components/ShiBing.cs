using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ShiBing : IComponentData
{
    public ShiBingName Name;
    public Entity TarEntity;//检测到的目标
    public Entity ShootEntity;//要攻击的目标
    public Entity FirePoint_R;//子弹发生点
    public Entity FirePoint_L;//子弹发生点
    public Entity FirePoint_R2;//子弹发生点
    public Entity FirePoint_L2;//子弹发生点
    public Entity CenterPoint;//每个单位的中心点
    public Entity CameraPoint;//摄像机位置
    public Entity Foot_R;//左右脚
    public Entity Foot_L;
    public Entity Particle_1;
    public Entity DeadPoint;//死亡位置
    public Entity DeadParticle;//死亡特效
    public Entity DeadLanguage;//亡语
    public Entity JidiPoint;//攻击的基地位子
    public Entity AvatarNamePoint;//头像名字位置

    public float Injuries;//收到的伤害
    public bool Fire_TakeTurnsBool;//轮流攻击，只有一个攻击点的默认右
    public int Fire_TakeTurnsInt;

    public bool Is_Parasitic;//是否为寄生攻击单位
    public bool Is_UpSkill;//是否已经升级
    public Entity CorrectPosEntity;//跟随我的物体的Entity
    public bool CorrectPosition_IsDie;//跟随我的物体是否可以死亡

    public int AttackNumber;//攻击次数


    public bool Is_LanguageNoBullet;//不是子弹？

    //public bool Is_HasAppear;//是否有出场行为

}
