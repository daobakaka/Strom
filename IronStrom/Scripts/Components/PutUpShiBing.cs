using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct PutUpShiBing : IComponentData
{
    public Entity GroundSpawnPoint;//地面出生点
    public Entity AirSpawnPoint;//空中出生点
    public Entity SpecifyShiBing;//指定单位
    public float PutUpTime;//出兵倒计时
    public float Cur_PutUpTime;
    public int PutUpNum;//建造士兵个数
}
