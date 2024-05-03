using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletChange : IComponentData
{
    //直线
    public float AT;
    public float3 Dir;
    public Entity DeadLanguage2;//亡语效果
}
