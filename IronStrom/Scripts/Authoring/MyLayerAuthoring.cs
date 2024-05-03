using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MyLayerAuthoring : MonoBehaviour
{
    public layer BelongsTo;//所属层级
    public layer CollidesWith_1;//针对层级
    public layer CollidesWith_2;//针对层级

    public layer BulletCollidesWith;//针对的子弹
    [Tooltip("--寄生子弹父类的所属层级--")] public layer ParasiticBelongsTo;
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