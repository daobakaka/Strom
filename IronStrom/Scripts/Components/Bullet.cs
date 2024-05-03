using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

public struct Bullet : IComponentData
{
    public float Speed;
    public float Radius;//������뾶
    public float Height;//������߶�

    public Entity CannonHit;//�ӵ�������Ч
    public Entity DeadLanguage;//����Ч��
    public Entity DeadLanguage2;//����Ч��
    public Entity TarGet;//�ӵ�����Ŀ��
    public Entity DeadParticle;//�ӵ�����Ч��

    public Entity CenterPoint;//���ĵ��λ��

    public bool Is_NOAttack;//�Ƿ�Ϊ���������ӵ�
}
