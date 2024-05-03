using DashGame;
using LitJson;
using Unity.Entities;
using UnityEngine;

public class CommanderSkillAuthoring : MonoBehaviour
{
    [Tooltip("--ָ�ӹټ���--")] public SceneBombType comdSkillname;
    [Tooltip("--ָ�ӹټ���ǰҡʱ��--")] public float InitTime;//ָ�ӹټ���ǰҡʱ��
    [Tooltip("--��������--")] public int MissileNum;
    [Tooltip("--���ʱ��--")] public float IntervalTime;
    [Tooltip("--����ʱ��--")] public float SustainTime;//����ʱ��
    [Tooltip("--�ٶ�--")] public float Speed;
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
