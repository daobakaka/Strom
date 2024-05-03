using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct BezierFiringAspects : IAspect
{
    readonly RefRW<BezierFiring> m_BezierFiring;
    readonly RefRW<BezierFiringChange> m_BezierFiringChange;
    
    public float3 StartPosition { get { return m_BezierFiringChange.ValueRW.StartPosition; } set { m_BezierFiringChange.ValueRW.StartPosition = value; } }
    public float3 EndPosition { get { return m_BezierFiringChange.ValueRW.EndPosition; }set { m_BezierFiringChange.ValueRW.EndPosition = value; } }
    public float3 ControlPoint { get { return m_BezierFiringChange.ValueRW.ControlPoint; } set { m_BezierFiringChange.ValueRW.ControlPoint = value; } }
    public float ElapsedTime { get { return m_BezierFiringChange.ValueRW.ElapsedTime; } set { m_BezierFiringChange.ValueRW.ElapsedTime = value; } }
    public float DirenDistance { get { return m_BezierFiringChange.ValueRW.DirenDistance; } set { m_BezierFiringChange.ValueRW.DirenDistance = value; } }
    public float heightFactor { get { return m_BezierFiring.ValueRW.heightFactor; } set { m_BezierFiring.ValueRW.heightFactor = value; } }
    public float LerpFactor { get { return m_BezierFiring.ValueRW.LerpFactor; }set { m_BezierFiring.ValueRW.LerpFactor = value; } }
    public bool Is_Random { get { return m_BezierFiringChange.ValueRW.Is_RandomBezier; } }
}
