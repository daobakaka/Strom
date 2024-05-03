using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct UpSkill : IComponentData
{
    public UpSkillName upSkill_Name;
    public bool Is_UpSkill;
    public float InjuryRecord;//ÉËº¦¼ÇÂ¼
}
