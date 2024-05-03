using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct DeadAspects : IAspect
{
    readonly RefRO<Die> m_Die;
    public readonly Entity Die_Entity;


    public Entity DeadParticle { get { return m_Die.ValueRO.DeadParticle; } }//������Ч
    public Entity DeadPoint { get { return m_Die.ValueRO.DeadPoint; } }
    public Entity DeadLanguage { get { return m_Die.ValueRO.DeadLanguage; } }//����Ч��
    public Entity DeadLanguage2 { get { return m_Die.ValueRO.DeadLanguage2; } }//����Ч��
    public bool Is_DieDirectly { get { return m_Die.ValueRO.Is_DieDirectly; } }
    public bool Is_LanguageNoBullet { get { return m_Die.ValueRO.Is_LanguageNoBullet; } }//����Ч����Ϊ�ӵ�
}
