using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float Speed;
    public float Radius;//胶囊体半径
    public float Height;//胶囊体高度
    [Tooltip("--子弹中心点位置--")] public GameObject CenterPoint;//中心点的位置
    [Tooltip("--子弹击中特效----")] public GameObject CannonHit;//子弹击中特效
    [Tooltip("--子弹亡语效果----")] public GameObject DeadLanguage;//亡语效果
    [Tooltip("--子弹亡语效果----")] public GameObject DeadLanguage2;//亡语效果
    [Tooltip("--子弹死亡特效----")] public GameObject DeadParticle;//子弹死亡效果
    [Tooltip("--是否为攻击地面--")] public bool Is_NOAttack;//是否为不攻击的子弹
}


public class BulletBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var bullet = new Bullet
        {
            Speed = authoring.Speed,
            Radius = authoring.Radius,
            Height = authoring.Height,
            Is_NOAttack = authoring.Is_NOAttack,
            CannonHit = GetEntity(authoring.CannonHit.gameObject, TransformUsageFlags.Dynamic),
            CenterPoint = GetEntity(authoring.CenterPoint.gameObject, TransformUsageFlags.Dynamic),
            DeadLanguage = GetEntity(authoring.DeadLanguage.gameObject, TransformUsageFlags.Dynamic),
            DeadLanguage2 = GetEntity(authoring.DeadLanguage2.gameObject, TransformUsageFlags.Dynamic),
            DeadParticle = GetEntity(authoring.DeadParticle.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, bullet);
    }
}
