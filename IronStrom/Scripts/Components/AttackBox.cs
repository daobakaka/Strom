using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public enum AttactBoxShape
{
    NUll,
    Box,//����
    Capsule,//����
    Sphere,//Բ��
}

public enum AttactBoxState
{
    NUll,
    OnceDamage,//�����˺�

    ManyDamage,//����˺�

}


public struct AttackBox : IComponentData
{
    public AttactBoxShape BoxShape;//������״
    public float3 halfExtents;//���ӵĳ����
    public float3 Offset;//����ƫ��ֵ
    public float R;
    public AttactBoxState BoxState;
    public float ExistenceTime;//Box����ʱ��
    public float SustainTime;//�����˺��ļ��
    public float Cur_SustainTime;
    public float AT;//���ӵĹ�����
    public bool Is_All;//�Ƿ񹥻���⵽��ȫ����λ
    public bool Is_Restore;//�Ƿ�Ϊ�ָ�Ч��
    public bool Is_NoTime;
    public bool Is_Remote;
    public float MaxRemote;
}
