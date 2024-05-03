using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ShiBing : IComponentData
{
    public ShiBingName Name;
    public Entity TarEntity;//��⵽��Ŀ��
    public Entity ShootEntity;//Ҫ������Ŀ��
    public Entity FirePoint_R;//�ӵ�������
    public Entity FirePoint_L;//�ӵ�������
    public Entity FirePoint_R2;//�ӵ�������
    public Entity FirePoint_L2;//�ӵ�������
    public Entity CenterPoint;//ÿ����λ�����ĵ�
    public Entity CameraPoint;//�����λ��
    public Entity Foot_R;//���ҽ�
    public Entity Foot_L;
    public Entity Particle_1;
    public Entity DeadPoint;//����λ��
    public Entity DeadParticle;//������Ч
    public Entity DeadLanguage;//����
    public Entity JidiPoint;//�����Ļ���λ��
    public Entity AvatarNamePoint;//ͷ������λ��

    public float Injuries;//�յ����˺�
    public bool Fire_TakeTurnsBool;//����������ֻ��һ���������Ĭ����
    public int Fire_TakeTurnsInt;

    public bool Is_Parasitic;//�Ƿ�Ϊ����������λ
    public bool Is_UpSkill;//�Ƿ��Ѿ�����
    public Entity CorrectPosEntity;//�����ҵ������Entity
    public bool CorrectPosition_IsDie;//�����ҵ������Ƿ��������

    public int AttackNumber;//��������


    public bool Is_LanguageNoBullet;//�����ӵ���

    //public bool Is_HasAppear;//�Ƿ��г�����Ϊ

}
