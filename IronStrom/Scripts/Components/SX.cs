using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SX : IComponentData
{
    public float HP;//����ֵ
    public float Cur_HP;
    public float DP;//������
    public float AT;//������
    public float DB;//��ֵ
    public float Tardis;//��ⷶΧ
    public float Shootdis;//������Χ
    public float ShootTime;//�ӵ�����ʱ��
    public float Cur_ShootTime;
    public float Speed;
    public float Init_Speed;//��ʼ�ƶ��ٶ�
    public float Record_Speed;//��¼�ƶ��ٶ��Ƿ����һ֡һ��
    public float VolumetricDistance;//�������
    public int Fire_TakeTurnsIntNum;//������������
    public int Cur_Fire_TakeTurnsIntNum;//������������
    public float AttackNumberTime;//�����������ʱ��
    public float Cur_AttackNumberTime;//�����������ʱ��

    public bool Is_Die;//�Ƿ�����
    public SkyGround Anti_SX;//�ԿնԵ�����
    public AirGroundAdvantage Advantage;//�ԿնԵ�����
    public bool Is_AirForce;//�Ƿ�Ϊ�վ�
    public bool Is_UpSkill;//�Ƿ��Ѿ�����

    public float BuffParticleScale;//buff��Ч��С
    public float MutinyValue;//�߷���ֵ

    public float AinWalkSpeed;//�ƶ������ٶ�
    public float Cur_AinWalkSpeed;//�ƶ������ٶ�
    public bool Is_ChangedAinWalkSpeed;//�Ƿ�ı����ƶ��������ٶ�

}
