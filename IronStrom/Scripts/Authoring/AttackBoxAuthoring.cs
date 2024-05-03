using DashGame;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum AttackBoxID
{
    Null,
    BaoLei_AttackBox,
    BaoYu_AttackBox,
    BaZhu_AttackBox,
    Grid_FireBox,
    HuGuang_AttackBox,
    HuGuang_AttackBox_2,
    Monster1_AttackBox,
    Monster3_AttackBox,
    Monster4_AttackBox,
    HuoShen_AttackBox,
    NuclearBomb_AttackBox,//音浪武器 原子弹
    PaChong_AttackBox,
    SulfuricAcid_AttackBox,
    TieChui_AttackBox,
    TOWER_AttackBox,
    ZhanZhengGongChang_AttackBox,
    Missile_AttackBox,//音浪武器 导弹
    Laser_AttackBox,//音浪武器 激光
    Monster5_AttackBox,
    Monster6_AttackBox,
}

public class AttackBoxAuthoring : MonoBehaviour
{
    public AttackBoxID AttackBoxid;
    [Tooltip("--攻击盒子的形状------")] public AttactBoxShape BoxShape;
    [Tooltip("--攻击盒子的长宽高----")] public float3 halfExtents;//盒子的长宽高;
    [Tooltip("--攻击盒子偏移值------")] public float3 Offset;
    [Tooltip("--攻击盒子的半径------")] public float R;
    [Tooltip("--什么类型的攻击矩形--")] public AttactBoxState BoxState;
    [Tooltip("--Box存在时间---------")] public float ExistenceTime;//Box存在时间
    [Tooltip("--持续伤害的间隔------")] public float SustainTime;//持续伤害的间隔
    [Tooltip("--每次的攻击力--------")] public float AT;//盒子的攻击力
    [Tooltip("--是否攻击检测到的全部")] public bool Is_All;//是否攻击检测到的全部
    [Tooltip("--是否为回血效果------")] public bool Is_Restore;//是否为恢复效果
    [Tooltip("--不倒计时存活时间----")] public bool Is_NoTime;//不倒计时存活时间
    [Tooltip("--Box是否逐渐变远-----")] public bool Is_Remote;
    [Tooltip("--Box最远距离---------")] public float MaxRemote;
}

public class AttackBoxBaker : Baker<AttackBoxAuthoring>
{
    public override void Bake(AttackBoxAuthoring authoring)
    {
        TextAsset ta = Resources.Load<TextAsset>("Config/AttackBoxData");
        JsonData Jdata = JsonMapper.ToObject(ta.text);
        JsonData jsondata = null;
        for (int i = 0; i < Jdata.Count; ++i)
        {
            jsondata = Jdata[i];
            var boxid = JsonUtil.ToEnum<AttackBoxID>(jsondata, "id");
            if (authoring.AttackBoxid == boxid)
                break;
            if (i == Jdata.Count - 1)
                jsondata = null;
        }
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var attackBox = new AttackBox
        {
            BoxShape = authoring.BoxShape,
            halfExtents = authoring.halfExtents,
            R = authoring.R,
            Offset = authoring.Offset,
            BoxState = authoring.BoxState,
            ExistenceTime = authoring.ExistenceTime,
            SustainTime = authoring.SustainTime,
            Cur_SustainTime = authoring.SustainTime,
            AT = authoring.AT,
            Is_All = authoring.Is_All,
            Is_Restore= authoring.Is_Restore,
            Is_NoTime = authoring.Is_NoTime,
            Is_Remote = authoring.Is_Remote,
            MaxRemote = authoring.MaxRemote,
        };
        if (jsondata != null && authoring.AttackBoxid != AttackBoxID.Null)
        {
            attackBox.BoxShape = JsonUtil.ToEnum<AttactBoxShape>(jsondata, "BoxShape");
            attackBox.halfExtents.x = JsonUtil.ToFloat(jsondata, "HalfExtentsX");
            attackBox.halfExtents.y = JsonUtil.ToFloat(jsondata, "HalfExtentsY");
            attackBox.halfExtents.z = JsonUtil.ToFloat(jsondata, "HalfExtentsZ");
            attackBox.Offset.x = JsonUtil.ToFloat(jsondata, "OffsetX");
            attackBox.Offset.y = JsonUtil.ToFloat(jsondata, "OffsetY");
            attackBox.Offset.z = JsonUtil.ToFloat(jsondata, "OffsetZ");
            attackBox.R = JsonUtil.ToFloat(jsondata, "R");
            attackBox.BoxState = JsonUtil.ToEnum<AttactBoxState>(jsondata, "BoxState");
            attackBox.ExistenceTime = JsonUtil.ToFloat(jsondata, "ExistenceTime");
            attackBox.SustainTime = JsonUtil.ToFloat(jsondata, "SustainTime");
            attackBox.AT = JsonUtil.ToFloat(jsondata, "AT");
            attackBox.Is_All = JsonUtil.ToBool(jsondata, "Is_All");
            attackBox.Is_Restore = JsonUtil.ToBool(jsondata, "Is_Restore");
            attackBox.Is_Remote = JsonUtil.ToBool(jsondata, "Is_Remote");
            attackBox.MaxRemote = JsonUtil.ToFloat(jsondata, "MaxRemote");
            //Debug.Log(authoring.AttackBoxid + "  的攻击半径R为：" + attackBox.R + ", Is_All: "+ JsonUtil.ToBool(jsondata, "Is_All")+
            //    ", Is_Restore: " + JsonUtil.ToBool(jsondata, "Is_Remote") + ", Is_Remote: " + JsonUtil.ToBool(jsondata, "Is_Remote"));
        }
        AddComponent(entity, attackBox);
    }
}