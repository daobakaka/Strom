using Unity.Entities;
using UnityEngine;

public class BaZhuAuthoring : MonoBehaviour
{
    public GameObject FirePoint_1;
    public GameObject FirePoint_2;
    public GameObject MainGunBullet;//主炮子弹
    public GameObject MainGunMuzzle;//主炮枪口
}

public class BaZhuBaker : Baker<BaZhuAuthoring>
{
    public override void Bake(BaZhuAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var bazhu = new BaZhu
        {
            FirePoint_1 = GetEntity(authoring.FirePoint_1.gameObject, TransformUsageFlags.Dynamic),
            FirePoint_2 = GetEntity(authoring.FirePoint_2.gameObject, TransformUsageFlags.Dynamic),
            MainGunBullet = GetEntity(authoring.MainGunBullet, TransformUsageFlags.Dynamic),
            MainGunMuzzle = GetEntity(authoring.MainGunMuzzle, TransformUsageFlags.Dynamic),
            MainGunShootNum = 1f,
            MainGunPointOfTime = 0,
        };
        AddComponent(entity, bazhu);
    }
}