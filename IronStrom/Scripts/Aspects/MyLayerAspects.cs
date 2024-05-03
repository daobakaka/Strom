using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct MyLayerAspects : IAspect
{
    readonly RefRW<MyLayer> m_MyLayer;

    public layer BelongsTo { get { return m_MyLayer.ValueRW.BelongsTo; }set { m_MyLayer.ValueRW.BelongsTo = value; } }
    public layer CollidesWith_1 { get { return m_MyLayer.ValueRW.CollidesWith_1; } set { m_MyLayer.ValueRW.CollidesWith_1 = value; } }
    public layer CollidesWith_2 { get { return m_MyLayer.ValueRW.CollidesWith_2; } set { m_MyLayer.ValueRW.CollidesWith_2 = value; } }
    public layer BulletCollidesWith { get { return m_MyLayer.ValueRW.BulletCollidesWith; } set { m_MyLayer.ValueRW.BulletCollidesWith = value; } }
}
