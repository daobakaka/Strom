using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct DeadAspects : IAspect
{
    readonly RefRO<Die> m_Die;
    public readonly Entity Die_Entity;


    public Entity DeadParticle { get { return m_Die.ValueRO.DeadParticle; } }//死亡特效
    public Entity DeadPoint { get { return m_Die.ValueRO.DeadPoint; } }
    public Entity DeadLanguage { get { return m_Die.ValueRO.DeadLanguage; } }//亡语效果
    public Entity DeadLanguage2 { get { return m_Die.ValueRO.DeadLanguage2; } }//亡语效果
    public bool Is_DieDirectly { get { return m_Die.ValueRO.Is_DieDirectly; } }
    public bool Is_LanguageNoBullet { get { return m_Die.ValueRO.Is_LanguageNoBullet; } }//亡语效果不为子弹
}
