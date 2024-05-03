using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct ParticleComponent : IComponentData
{
    public float DieTime;//������ʱ
    public bool Is_Follow;//�Ƿ�Ҫ����Ŀ��
    public Entity FollowPoint;
    public bool Is_CasterDead;//ʩ�����Ƿ�����
    public Entity Caster;//ʩ����
    public float ParScale;//��Ч�Ĵ�С
    public bool Is_NoDieTime;//��Ҫ����ʱ
}
