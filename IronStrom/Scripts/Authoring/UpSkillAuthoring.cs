using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
public enum UpSkillName
{
    Null,
    PaChong,
    YeMa,
    BaoYu,
    BaoYuBullet,
    GangQiu,
    BaZhu,
    WarFactory,
    HuGuang,
    FengHuang,

}
public class UpSkillAuthoring : MonoBehaviour
{
    [Tooltip("--Ë­Éý¼¶--------------")] public UpSkillName upSkill_Name;
    [Tooltip("--ÊÇ·ñÉý¼¶------------")] public bool Is_UpSkill;

}

public class UpSkillBaker : Baker<UpSkillAuthoring>
{
    public override void Bake(UpSkillAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var upskill = new UpSkill
        {
            upSkill_Name = authoring.upSkill_Name,
            Is_UpSkill = authoring.Is_UpSkill,
        };
        AddComponent(entity, upskill);
    }
}