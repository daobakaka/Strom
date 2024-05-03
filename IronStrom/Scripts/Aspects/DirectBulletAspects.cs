using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct DirectBulletAspects : IAspect
{
    readonly RefRW<DirectBullet> m_DirectBullet;
    readonly RefRW<DirectBulletChange> m_DirectBulletChange;

    public Entity Owner { get { return m_DirectBulletChange.ValueRW.Owner; } set { m_DirectBulletChange.ValueRW.Owner = value; } }
    public Entity BulletHit { get { return m_DirectBullet.ValueRW.BulletHit; } }
    public float StartLifetime { get { return m_DirectBullet.ValueRW.StartLifetime; } set { m_DirectBullet.ValueRW.StartLifetime = value; } }//子弹特效的粒子存活时间
    public float StartSpeed { get { return m_DirectBullet.ValueRW.StartSpeed; } set { m_DirectBullet.ValueRW.StartSpeed = value; } }
    public float StartOffset { get { return m_DirectBullet.ValueRW.StartOffset; } }
    public DirectBullet_FirePoint DB_FirePoint { get { return m_DirectBulletChange.ValueRW.DB_FirePoint; }set { m_DirectBulletChange.ValueRW.DB_FirePoint = value; } }

}