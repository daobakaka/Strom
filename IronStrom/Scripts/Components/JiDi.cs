using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct JiDi : IComponentData
{
    public Entity FirePoint;//出兵点
    public Entity WalkPoint;//走向基地的位置
    public Entity shieldPoint;//基地护盾的位置
    public Entity JidiShield;//基地的护盾
}
