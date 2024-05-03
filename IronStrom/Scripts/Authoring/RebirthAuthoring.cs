using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RebirthAuthoring : MonoBehaviour
{
    public float RebirthTime;//重生时间
    public GameObject RebirthEntity;//重生成的Entity
    public GameObject RebirthParticle;//重生的特效
    public UpSkillName upSkill_Name;
}

public class RebirthBaker : Baker<RebirthAuthoring>
{
    public override void Bake(RebirthAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var rebirth = new Rebirth
        {
            RebirthTime = authoring.RebirthTime,
            RebirthEntity = GetEntity(authoring.RebirthEntity.gameObject, TransformUsageFlags.Dynamic),
            RebirthParticle = GetEntity(authoring.RebirthParticle.gameObject,TransformUsageFlags.Dynamic),
            upSkill_Name = authoring.upSkill_Name,
        };
        AddComponent(entity, rebirth);
    }
}