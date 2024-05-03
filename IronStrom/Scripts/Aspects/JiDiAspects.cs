using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct JiDiAspects : IAspect
{
    readonly RefRO<JiDi> m_JiDi;
    public readonly Entity JiDi_Entity;
    public Entity firePoint { get { return m_JiDi.ValueRO.FirePoint; } }
}
