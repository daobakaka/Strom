using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct PutUpShiBing : IComponentData
{
    public Entity GroundSpawnPoint;//���������
    public Entity AirSpawnPoint;//���г�����
    public Entity SpecifyShiBing;//ָ����λ
    public float PutUpTime;//��������ʱ
    public float Cur_PutUpTime;
    public int PutUpNum;//����ʿ������
}
