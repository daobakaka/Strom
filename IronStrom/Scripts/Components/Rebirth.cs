using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Rebirth : IComponentData
{
    public float RebirthTime;//����ʱ��
    public Entity RebirthEntity;//�����ɵ�Entity
    public Entity RebirthParticle;//��������Ч

    public Entity FollowEntity;//���浥λ
    public float3 FollowOffDistance;//������ƫ�ƾ���
    public bool Is_HaveRebirthParticle;//�Ƿ��Ѿ�����Ч��
    public UpSkillName upSkill_Name;

}
