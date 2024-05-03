using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct Integral : IComponentData//积分组件
{
    public float ATIntegral;//伤害积分
    public Entity AttackMeEntity;//攻击我的人的Entity

}
