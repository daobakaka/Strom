using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DirectBulletAuthoring : MonoBehaviour
{
    public GameObject Bullet_Hit;//�ӵ�������Ч
    public float StartSpeed;//�ӵ���Ч�������ٶ�
    [Tooltip("--�ӵ���Ч��ǰ��ƫ�ưٷֱ�------")] public float StartOffset;

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