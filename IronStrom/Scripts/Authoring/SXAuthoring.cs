using DashGame;
using LitJson;
using Unity.Entities;
using UnityEngine;

public enum SkyGround
{
    AirAndSurface,//�ԿնԵ�
    Anti_Surface,//�Ե�
    Anti_Air,//�Կ�
}
public enum AirGroundAdvantage
{
    Null,
    To_Ground,//�Ե�����
    To_Air,//�Կ�����
}


public class SXAuthoring : MonoBehaviour
{
    public ShiBingName Name;
    public float HP;//����ֵ
    [System.NonSerialized]public float Cur_HP;//����ֵ
    public float DP;//������
    public float AT;//������
    public float DB;//��ֵ
    public float Tardis;//��ⷶΧ
    public float Shootdis;//������Χ
    public float ShootTime;
    public float Speed;
    [Tooltip("--��λ�������--")] public float VolumetricDistance;
    [Tooltip("--���ع�����λ��������������--")] public int Fire_TakeTurnsIntNum;//������������
    [Tooltip("--���ع�����λ�������������--")] public float AttackNumberTime;
    [Tooltip("--�ԿնԵ����ԣ�Anti_Air(�Կ�),Anti_Surface(�Ե�),AirAndSurface(�ԿնԵ�)------")]
    public SkyGround Anti_SX;
    [Tooltip("--�ԿնԵ����ƣ�To_Air(�Կ�����),To_Ground(�Կ�����)")]
    public AirGroundAdvantage Advantage;
    [Tooltip("--�Ƿ�Ϊ�վ�------")] public bool Is_AirForce;//�Ƿ�Ϊ�վ�
    [Tooltip("--buff��Ч�Ĵ�С--")] public float BuffParticleScale;//buff��Ч��С
    [Tooltip("--Walk�����Ĳ����ٶ�--")] public float AinWalkSpeed;


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

