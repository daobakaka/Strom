using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BezierFiring : IComponentData
{
    public float heightFactor;//�߶�
    public float LerpFactor;//�͵���֮���ʲô���뿪ʼ����
}
