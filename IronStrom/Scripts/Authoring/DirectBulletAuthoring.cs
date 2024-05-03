using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DirectBulletAuthoring : MonoBehaviour
{
    public GameObject Bullet_Hit;//子弹击中特效
    public float StartSpeed;//子弹特效的粒子速度
    [Tooltip("--子弹特效的前后偏移百分比------")] public float StartOffset;

}

public class DirectBulletBaker : Baker<DirectBulletAuthoring>
{
    public override void Bake(DirectBulletAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var dirBullet = new DirectBullet
        {
            BulletHit = GetEntity(authoring.Bullet_Hit.gameObject, TransformUsageFlags.Dynamic),
            StartSpeed = authoring.StartSpeed,
            StartOffset = authoring.StartOffset,
        };
        AddComponent(entity, dirBullet);
    }
}