using DashGame;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum DirectShootID
{
    NUll,
    HaiKe,
    GangQiu,
    RongDian,
}

public class DirectShootAuthoring : MonoBehaviour
{
    public DirectShootID DirectShootid;
    public GameObject DirectParticle;//ֱ�ӹ�������Ч���Լ�ͬ����ʿ����λ�ù�ϵ
    [Tooltip("----�Ƿ�Ϊ�����˺�-----")] public bool Is_CumulativeDamage;//�Ƿ�Ϊ�����˺�
    [Tooltip("----�˺���Сֵ---------")] public float AT_Min;
    [Tooltip("----�˺�������---------")] public float AT_Max;//�˺�������
    [Tooltip("----�˺��ۼӼ��-------")] public float IntervalTime;//�˺��ۼӵ��ۼӼ��
}


public class DirectShootBaker : Baker<DirectShootAuthoring>
{
    public override void Bake(DirectShootAuthoring authoring)
    {
        TextAsset ta = Resources.Load<TextAsset>("Config/DirectShootData");
        JsonData Jdata = JsonMapper.ToObject(ta.text);
        JsonData jsondata = null;
        for (int i = 0; i < Jdata.Count; ++i)
        {
            jsondata = Jdata[i];
            var DSID = JsonUtil.ToEnum<DirectShootID>(jsondata, "id");
            if (authoring.DirectShootid == DSID)
                break;
            if (i == Jdata.Count - 1)
                jsondata = null;
        }
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var directShoot = new DirectShoot
        {
            DirectParticle_Parfab = GetEntity(authoring.DirectParticle, TransformUsageFlags.Dynamic),
            Is_CumulativeDamage = authoring.Is_CumulativeDamage,
            AT_Min = authoring.AT_Min,
            AT_Max = authoring.AT_Max,
            IntervalTime = authoring.IntervalTime,
            Cur_IntervalTime = authoring.IntervalTime,
        };
        if (jsondata != null && authoring.DirectShootid != DirectShootID.NUll)
        {
            directShoot.Is_CumulativeDamage = JsonUtil.ToBool(jsondata, "Is_CumulativeDamage");
            directShoot.AT_Max = JsonUtil.ToFloat(jsondata, "AT_Max");
            directShoot.AT_Min = JsonUtil.ToFloat(jsondata, "AT_Min");
            directShoot.IntervalTime = JsonUtil.ToFloat(jsondata, "IntervalTime");
        }


        AddComponent(entity, directShoot);
    }
}