using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public readonly partial struct BulletAspects : IAspect
{
    readonly RefRW<Bullet> m_Bullet;
    readonly RefRW<BulletChange> m_BulletChange;
    //readonly RefRW<LocalTransform> m_transform;
    public readonly Entity Bullet_Entity;

    public float Speed { get { return m_Bullet.ValueRW.Speed; } set { m_Bullet.ValueRW.Speed = value; } }
    public RefRW<Bullet> bullet { get { return m_Bullet; } }
    public float BulletAT { get { return m_BulletChange.ValueRW.AT; }set { m_BulletChange.ValueRW.AT = value; } }
    public float3 BulletDir { get { return m_BulletChange.ValueRW.Dir; }set { m_BulletChange.ValueRW.Dir = value; } }
    public float Radius { get { return m_Bullet.ValueRW.Radius; } }
    public float Height { get { return m_Bullet.ValueRW.Height; } }
    //public Entity DeadPoint { get { return m_Bullet.ValueRW.DeadPoint; } }
    public Entity CannonHit { get { return m_Bullet.ValueRW.CannonHit; } }
    public Entity DeadLanguage { get { return m_Bullet.ValueRW.DeadLanguage; } }
    public Entity DeadLanguage2 { get { return m_Bullet.ValueRW.DeadLanguage2; } }
    public Entity TarGet { get { return m_Bullet.ValueRW.TarGet; } set { m_Bullet.ValueRW.TarGet = value; } }
    public bool Is_NOAttack { get { return m_Bullet.ValueRW.Is_NOAttack; } }

}
