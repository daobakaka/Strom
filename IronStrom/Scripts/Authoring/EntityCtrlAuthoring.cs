using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityCtrlAuthoring : MonoBehaviour
{
    [Header("û�ж����Ľ�ɫֱ�ӿ���Entity--------------------------------")]
    public GameObject PaoTai;//��̨
    public GameObject CheShen;//����
    public GameObject Bullet;//�ӵ�
    public GameObject Muzzle;//ǹ����Ч
    public GameObject Particle_1;
    public GameObject AttackBox;//�������� 

    [Header("û�ж����Ľ�ɫ�����еĶ���----------------------------------")]
    public bool EntityCtrlAni_Idle;
    public bool EntityCtrlAni_Walk;
    public bool EntityCtrlAni_Move;
    public bool EntityCtrlAni_Ready;
    public bool EntityCtrlAni_Fire;

    [Header("�����DOTS�����ڴ�ͳ�����Ľ�ɫ------------------------------")]
    public bool Is_TraditionalAnimation;

}

public class EntityCtrlBaker : Baker<EntityCtrlAuthoring>
{
    public override void Bake(EntityCtrlAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var entityCtrl = new EntityCtrl
        {
            PaoTai = GetEntity(authoring.PaoTai.gameObject, TransformUsageFlags.Dynamic),
            CheShen = GetEntity(authoring.CheShen.gameObject, TransformUsageFlags.Dynamic),
            Bullet = GetEntity(authoring.Bullet.gameObject, TransformUsageFlags.Dynamic),
            Muzzle = GetEntity(authoring.Muzzle.gameObject, TransformUsageFlags.Dynamic),
            Particle_1 = GetEntity(authoring.Particle_1.gameObject, TransformUsageFlags.Dynamic),
            AttackBox = GetEntity(authoring.AttackBox.gameObject, TransformUsageFlags.Dynamic),
            
            EntityCtrlAni_Idle = authoring.EntityCtrlAni_Idle,
            EntityCtrlAni_Walk = authoring.EntityCtrlAni_Walk,
            EntityCtrlAni_Move = authoring.EntityCtrlAni_Move,
            EntityCtrlAni_Ready = authoring.EntityCtrlAni_Ready,
            EntityCtrlAni_Fire = authoring.EntityCtrlAni_Fire,

            Is_TraditionalAnimation = authoring.Is_TraditionalAnimation,
        };
        AddComponent(entity, entityCtrl);
    }
}