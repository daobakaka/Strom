using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEditor.Rendering;
using UnityEngine;

public enum commandType
{
    NUll,
    Attack,//����
    Jungle,//��Ұ
}
public struct BarrageCommand : IComponentData
{
    public commandType command;
    public Entity Comd_ShootEntity;//�����ʿ��������Entity
    
    //ʧȥĿ�꣬������ٸ�һ��Ŀ��
    //public 
}
