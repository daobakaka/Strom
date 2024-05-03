using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public enum EntityCtrlAni//Entity控制的士兵可能需要的动画
{
    NUll,
    GpuAni_Idle,
    GpuAni_Walk,
    GpuAni_Move,
    GpuAni_Ready,
    GpuAni_Fire,
}
public struct EntityCtrl : IComponentData
{
    public Entity PaoTai;//炮台
    public Entity CheShen;//车身
    public Entity Bullet;//子弹
    public Entity Muzzle;//枪口特效
    public Entity Particle_1;
    public Entity AttackBox;//攻击矩形 

    public quaternion PaoTaiRotation;
    public bool EntityCtrlAni_Idle;
    public bool EntityCtrlAni_Walk;
    public bool EntityCtrlAni_Move;
    public bool EntityCtrlAni_Ready;
    public bool EntityCtrlAni_Fire;

    public bool Is_TraditionalAnimation;
}
