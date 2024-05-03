using DashGame;
using LitJson;
using Unity.Entities;
using UnityEngine;

public enum SkyGround
{
    AirAndSurface,//对空对地
    Anti_Surface,//对地
    Anti_Air,//对空
}
public enum AirGroundAdvantage
{
    Null,
    To_Ground,//对地优势
    To_Air,//对空优势
}


public class SXAuthoring : MonoBehaviour
{
    public ShiBingName Name;
    public float HP;//生命值
    [System.NonSerialized]public float Cur_HP;//生命值
    public float DP;//防御力
    public float AT;//攻击力
    public float DB;//格挡值
    public float Tardis;//检测范围
    public float Shootdis;//攻击范围
    public float ShootTime;
    public float Speed;
    [Tooltip("--单位体积距离--")] public float VolumetricDistance;
    [Tooltip("--多重攻击单位的轮流攻击次数--")] public int Fire_TakeTurnsIntNum;//轮流攻击次数
    [Tooltip("--多重攻击单位的轮流攻击间隔--")] public float AttackNumberTime;
    [Tooltip("--对空对地属性：Anti_Air(对空),Anti_Surface(对地),AirAndSurface(对空对地)------")]
    public SkyGround Anti_SX;
    [Tooltip("--对空对地优势：To_Air(对空优势),To_Ground(对空优势)")]
    public AirGroundAdvantage Advantage;
    [Tooltip("--是否为空军------")] public bool Is_AirForce;//是否为空军
    [Tooltip("--buff特效的大小--")] public float BuffParticleScale;//buff特效大小
    [Tooltip("--Walk动画的播放速度--")] public float AinWalkSpeed;


}

public class SXBaker : Baker<SXAuthoring>
{
    public override void Bake(SXAuthoring authoring)
    {
        TextAsset ta = Resources.Load<TextAsset>("Config/SoldierData");
        JsonData Jdata = JsonMapper.ToObject(ta.text);
        JsonData jsondata = null;
        for (int i = 0; i < Jdata.Count; ++i)
        {
            jsondata = Jdata[i];
            var SBName = JsonUtil.ToEnum<ShiBingName>(jsondata, "id");
            if (authoring.Name == SBName)
                break;
            if (i == Jdata.Count - 1)
                jsondata = null;
        }
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var sx = new SX
        {
            HP = authoring.HP,
            Cur_HP = authoring.HP,
            DP = authoring.DP,
            AT = authoring.AT,
            DB = authoring.DB,
            Tardis = authoring.Tardis,
            Shootdis = authoring.Shootdis,
            ShootTime = authoring.ShootTime,
            Cur_ShootTime = authoring.ShootTime,
            Speed = authoring.Speed,
            Init_Speed = authoring.Speed,
            Record_Speed = authoring.Speed,
            Anti_SX = authoring.Anti_SX,
            Advantage = authoring.Advantage,
            Is_AirForce = authoring.Is_AirForce,
            Fire_TakeTurnsIntNum = authoring.Fire_TakeTurnsIntNum,
            Cur_Fire_TakeTurnsIntNum = authoring.Fire_TakeTurnsIntNum,
            AttackNumberTime = authoring.AttackNumberTime,
            Cur_AttackNumberTime = authoring.AttackNumberTime,
            BuffParticleScale = authoring.BuffParticleScale,
            AinWalkSpeed = authoring.AinWalkSpeed,
            Cur_AinWalkSpeed = authoring.AinWalkSpeed,
            VolumetricDistance = authoring.VolumetricDistance,
        };
        if (jsondata != null && authoring.Name != ShiBingName.Null)
        {
            sx.HP = JsonUtil.ToFloat(jsondata, "HP");
            sx.Cur_HP = sx.HP;
            sx.Speed = JsonUtil.ToFloat(jsondata, "Speed");
            sx.AT = JsonUtil.ToFloat(jsondata, "AT");
            sx.DP = JsonUtil.ToFloat(jsondata, "DP");
            sx.DB = JsonUtil.ToFloat(jsondata, "DB");
            sx.Tardis = JsonUtil.ToFloat(jsondata, "Tardis");
            sx.Shootdis = JsonUtil.ToFloat(jsondata, "Shootdis");
            sx.ShootTime = JsonUtil.ToFloat(jsondata, "ShootTime");
            sx.Cur_ShootTime = sx.ShootTime;
            sx.VolumetricDistance = JsonUtil.ToFloat(jsondata, "VolumetricDistance");
            sx.Anti_SX = JsonUtil.ToEnum<SkyGround>(jsondata, "Anti");
            sx.Advantage = JsonUtil.ToEnum<AirGroundAdvantage>(jsondata, "Advantage");
            sx.BuffParticleScale = JsonUtil.ToFloat(jsondata, "BuffParticleScale");
        }
        AddComponent(entity, sx);
    }
}

