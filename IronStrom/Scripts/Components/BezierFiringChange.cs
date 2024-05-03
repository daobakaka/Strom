using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BezierFiringChange : IComponentData
{
    public float3 StartPosition;//���
    public float3 EndPosition;//�յ�
    public float3 ControlPoint;//���Ƶ�
    public float DirenDistance;//���˾��룬�����������ʱ��
    public float ElapsedTime;//���о���ʱ��
    public bool Is_RandomBezier;// �Ƿ�Ϊ�������������
}
