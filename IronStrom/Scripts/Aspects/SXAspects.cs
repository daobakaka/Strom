using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct SXAspects : IAspect
{
    readonly RefRW<SX> m_SX;

    public float TarDis { get { return m_SX.ValueRW.Tardis; } set { m_SX.ValueRW.Tardis = value; } }
    public float ShootDis { get { return m_SX.ValueRW.Shootdis; } set { m_SX.ValueRW.Shootdis = value; } }
    public float ShootTime { get { return m_SX.ValueRW.ShootTime; } set { m_SX.ValueRW.ShootTime = value; } }
    public float Cur_ShootTime { get { return m_SX.ValueRW.Cur_ShootTime; } set { m_SX.ValueRW.Cur_ShootTime = value; } }
    public float Speed { get { return m_SX.ValueRW.Speed; } set { m_SX.ValueRW.Speed = value; } }
    public bool Is_Die { get { return m_SX.ValueRW.Is_Die; } set { m_SX.ValueRW.Is_Die = value; } }
    public float Cur_AinWalkSpeed { get { return m_SX.ValueRW.Cur_AinWalkSpeed; } set { m_SX.ValueRW.Cur_AinWalkSpeed = value; } }
    public bool Is_ChangedAinWalkSpeed { get { return m_SX.ValueRW.Is_ChangedAinWalkSpeed; } set { m_SX.ValueRW.Is_ChangedAinWalkSpeed = value; } }
}
