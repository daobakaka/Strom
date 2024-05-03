using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MyLayerAuthoring : MonoBehaviour
{
    public layer BelongsTo;//�����㼶
    public layer CollidesWith_1;//��Բ㼶
    public layer CollidesWith_2;//��Բ㼶

    public layer BulletCollidesWith;//��Ե��ӵ�
    [Tooltip("--�����ӵ�����������㼶--")] public layer ParasiticBelongsTo;
}

public class MyLayerBaker : Baker<MyLayerAuthoring>
{
    public override void Bake(MyLayerAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var mylayer = new MyLayer
        {
            BelongsTo = authoring.BelongsTo,
            CollidesWith_1 = authoring.CollidesWith_1,
            CollidesWith_2 = authoring.CollidesWith_2,
            BulletCollidesWith = authoring.BulletCollidesWith,
            ParasiticBelongsTo = authoring.ParasiticBelongsTo,
        };
        AddComponent(entity, mylayer);
    }
}