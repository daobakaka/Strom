using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct MyLayer : IComponentData
{
    public layer BelongsTo;//�����㼶
    public layer CollidesWith_1;//��Բ㼶
    public layer CollidesWith_2;//��Բ㼶

    public layer BulletCollidesWith;//��Ե��ӵ�
    public layer ParasiticBelongsTo;
}
