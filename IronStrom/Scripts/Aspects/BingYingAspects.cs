using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct BingYingAspects : IAspect
{
    readonly RefRW<BingYing> m_BingYing;

    public int PlayerID { get { return m_BingYing.ValueRW.PlayerID; } set { m_BingYing.ValueRW.PlayerID = value; } }
    public int TeamID { get { return m_BingYing.ValueRW.TeamID; } }
    public Entity FirePoint { get { return m_BingYing.ValueRW.FirePoint; } }
    public float CountDownTime { get { return m_BingYing.ValueRW.CountDownTime; } set { m_BingYing.ValueRW.CountDownTime = value; } }
    public float Cur_CountDownTime { get { return m_BingYing.ValueRW.Cur_CountDownTime; } set { m_BingYing.ValueRW.Cur_CountDownTime = value; } }
    public float InitSpeed { get { return m_BingYing.ValueRW.InitSpeed; } set { m_BingYing.ValueRW.InitSpeed = value; } }

}
