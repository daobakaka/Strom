using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public enum EntityCtrlAni//Entity���Ƶ�ʿ��������Ҫ�Ķ���
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
    public Entity PaoTai;//��̨
    public Entity CheShen;//����
    public Entity Bullet;//�ӵ�
    public Entity Muzzle;//ǹ����Ч
    public Entity Particle_1;
    public Entity AttackBox;//�������� 

    public quaternion PaoTaiRotation;
    public bool EntityCtrlAni_Idle;
    public bool EntityCtrlAni_Walk;
    public bool EntityCtrlAni_Move;
    public bool EntityCtrlAni_Ready;
    public bool EntityCtrlAni_Fire;

    public bool Is_TraditionalAnimation;
}
