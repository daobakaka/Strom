using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Rebirth : IComponentData
{
    public float RebirthTime;//重生时间
    public Entity RebirthEntity;//重生成的Entity
    public Entity RebirthParticle;//重生的特效

    public Entity FollowEntity;//跟随单位
    public float3 FollowOffDistance;//跟随点的偏移距离
    public bool Is_HaveRebirthParticle;//是否已经有特效了
    public UpSkillName upSkill_Name;

}
