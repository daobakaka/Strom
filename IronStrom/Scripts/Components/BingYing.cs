using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BingYing : IComponentData
{
    public int TeamID;//哪方队伍
    public int PlayerID;//玩家ID
    public Entity FirePoint;//出兵点
    public float CountDownTime;//出兵倒计时
    public float Cur_CountDownTime;//出兵倒计时
    public float InitSpeed;//初始化速度
}
