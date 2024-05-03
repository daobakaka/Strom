using DashGame;
using LitJson;
using Unity.Entities;
using UnityEngine;

public class CommanderSkillAuthoring : MonoBehaviour
{
    [Tooltip("--指挥官技能--")] public SceneBombType comdSkillname;
    [Tooltip("--指挥官技能前摇时间--")] public float InitTime;//指挥官技能前摇时间
    [Tooltip("--导弹个数--")] public int MissileNum;
    [Tooltip("--间隔时间--")] public float IntervalTime;
    [Tooltip("--持续时间--")] public float SustainTime;//持续时间
    [Tooltip("--速度--")] public float Speed;
}

public class CommanderSkillBaker : Baker<CommanderSkillAuthoring>//CommaderSkillData
{
    public override void Bake(CommanderSkillAuthoring authoring)
    {
        TextAsset ta = Resources.Load<TextAsset>("Config/CommaderSkillData");
        JsonData Jdata = JsonMapper.ToObject(ta.text);
        JsonData jsondata = null;
        for (int i = 0; i < Jdata.Count; ++i)
        {
            jsondata = Jdata[i];
            var CommaderSkillID = JsonUtil.ToEnum<SceneBombType>(jsondata, "id");
            if (authoring.comdSkillname == CommaderSkillID)
                break;
            if (i == Jdata.Count - 1)
                jsondata = null;
        }

        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var commanderSkill = new CommanderSkill
        {
            SceneBombtype = authoring.comdSkillname,
            InitTime = authoring.InitTime,
            Cur_InitTime = authoring.InitTime,
            IntervalTime = authoring.IntervalTime,
            Cur_IntervalTime = authoring.IntervalTime,
            MissileNum = authoring.MissileNum,
            SustainTime = authoring.SustainTime,
            Cur_SustainTime = authoring.SustainTime,
            Speed = authoring.Speed,
        };
        if (jsondata != null && authoring.comdSkillname != SceneBombType.NUll)
        {
            commanderSkill.InitTime = JsonUtil.ToFloat(jsondata, "InitTime");
            commanderSkill.Cur_InitTime = commanderSkill.InitTime;
            commanderSkill.MissileNum = JsonUtil.ToInt(jsondata, "MissileNum");
            commanderSkill.IntervalTime = JsonUtil.ToFloat(jsondata, "IntervalTime");
            commanderSkill.Cur_IntervalTime = commanderSkill.IntervalTime;
            commanderSkill.SustainTime = JsonUtil.ToFloat(jsondata, "SustainTime");
            commanderSkill.Cur_SustainTime = commanderSkill.SustainTime;
            commanderSkill.Speed = JsonUtil.ToFloat(jsondata, "Speed");
        }
        AddComponent(entity, commanderSkill);
    }
}
