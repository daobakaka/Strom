using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum AppearName
{
    NUll,
    MonsterAppear,//�������
    MonsterAirForceAppear,//�������
    BaZhuApper,//��������
    BaZhuApper2,//��������2
}

public struct Appear : IComponentData//������Ϊ
{
    public AppearName appearName;
    public float AppearDistance;//�������ߵľ���
    public float AppearSpeed;//�����ٶ�

    [HideInInspector] public bool IsOver_AppearAnimPlay;//���������Ƿ񲥷����
    [HideInInspector] public float3 AppearPos;//��������������󣬳��ֵ�λ��
    [HideInInspector] public quaternion AppearRot;//��������������󣬳��ֵ�λ��
}
