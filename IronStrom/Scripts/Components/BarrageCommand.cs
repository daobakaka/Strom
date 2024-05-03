using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEditor.Rendering;
using UnityEngine;

public enum commandType
{
    NUll,
    Attack,//攻击
    Jungle,//打野
}
public struct BarrageCommand : IComponentData
{
    public commandType command;
    public Entity Comd_ShootEntity;//玩家让士兵攻击的Entity
    
    //失去目标，让玩家再给一个目标
    //public 
}
