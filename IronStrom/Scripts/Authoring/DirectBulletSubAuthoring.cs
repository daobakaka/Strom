using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DirectBulletSubAuthoring : MonoBehaviour
{
    public GameObject parBullet;
}

public class DirectBulletSubBaker : Baker<DirectBulletSubAuthoring>
{
    public override void Bake(DirectBulletSubAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var dirBullSub = new DirectBulletSub
        {
            parBullet = GetEntity(authoring.parBullet.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, dirBullSub);
    }
}
