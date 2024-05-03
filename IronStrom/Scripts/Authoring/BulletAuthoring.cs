using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float Speed;
    public float Radius;//������뾶
    public float Height;//������߶�
    [Tooltip("--�ӵ����ĵ�λ��--")] public GameObject CenterPoint;//���ĵ��λ��
    [Tooltip("--�ӵ�������Ч----")] public GameObject CannonHit;//�ӵ�������Ч
    [Tooltip("--�ӵ�����Ч��----")] public GameObject DeadLanguage;//����Ч��
    [Tooltip("--�ӵ�����Ч��----")] public GameObject DeadLanguage2;//����Ч��
    [Tooltip("--�ӵ�������Ч----")] public GameObject DeadParticle;//�ӵ�����Ч��
    [Tooltip("--�Ƿ�Ϊ��������--")] public bool Is_NOAttack;//�Ƿ�Ϊ���������ӵ�
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
