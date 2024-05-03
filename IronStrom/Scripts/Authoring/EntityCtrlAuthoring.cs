using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityCtrlAuthoring : MonoBehaviour
{
    [Header("没有动画的角色直接控制Entity--------------------------------")]
    public GameObject PaoTai;//炮台
    public GameObject CheShen;//车身
    public GameObject Bullet;//子弹
    public GameObject Muzzle;//枪口特效
    public GameObject Particle_1;
    public GameObject AttackBox;//攻击矩形 

    [Header("没有动画的角色可能有的动画----------------------------------")]
    public bool EntityCtrlAni_Idle;
    public bool EntityCtrlAni_Walk;
    public bool EntityCtrlAni_Move;
    public bool EntityCtrlAni_Ready;
    public bool EntityCtrlAni_Fire;

    [Header("灵魂在DOTS肉体在传统动画的角色------------------------------")]
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