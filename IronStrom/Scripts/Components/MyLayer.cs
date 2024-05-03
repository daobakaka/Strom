using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct MyLayer : IComponentData
{
    public layer BelongsTo;//所属层级
    public layer CollidesWith_1;//针对层级
    public layer CollidesWith_2;//针对层级

    public layer BulletCollidesWith;//针对的子弹
    public layer ParasiticBelongsTo;
}
