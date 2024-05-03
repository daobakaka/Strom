using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct ShiBingChange : IComponentData
{
    public float3 Dir;
    public ActState Act;//行为
    public Entity enemyJiDi;//目标基地

}
