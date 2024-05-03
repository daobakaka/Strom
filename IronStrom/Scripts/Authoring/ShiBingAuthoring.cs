using Unity.Entities;
using UnityEngine;
public enum ShiBingName
{
    Null,
    ChangGong,
    BaoLei,
    JianYa,
    HuGuang,
    PaChong,
    TieChui,
    YeMa,
    BaoYu,
    BingFeng,
    BaZhu,
    GangQiu,
    FengHuang,
    ZhanZhengGongChang,
    HuoShen,
    HaiKe,
    RongDian,
    XiNiu,
    FengHuangRebirth,
    TOWER,
    Monster_1,
    Monster_2,
    Monster_3,
    Monster_4,
    Monster_5,
    Monster_6,
    Monster_7,
    Shield,//护盾
    JiDi,


}


public class ShiBingAuthoring : MonoBehaviour
{
    public ShiBingName Name;
    public Entity TarEntity;//攻击目标

    public GameObject FirePoint_R;//发射点
    public GameObject FirePoint_L;//发射点
    public GameObject FirePoint_R2;//发射点
    public GameObject FirePoint_L2;//发射点
    public GameObject CenterPoint;//中心点
    public GameObject CameraPoint;//摄像机位置
    public GameObject Foot_R;//左右脚
    public GameObject Foot_L;
    public GameObject Particle_1;
    public GameObject DeadPoint;//死亡位置
    public GameObject DeadParticle;//死亡特效
    public GameObject DeadLanguage;//亡语效果
    public GameObject AvatarNamePoint;//头像名字位置
    //[Tooltip("-是否有左右旋转动画--")] public bool Is_AniMoveRL;//是否有左右旋转动画

    [Tooltip("--是否为寄生攻击单位--")] public bool Is_Parasitic;
    [Tooltip("--亡语效果不为子弹----")] public bool Is_LanguageNoBullet;



    public PlayerData player;
}

class ShiBingBaker : Baker<ShiBingAuthoring>
{
    public override void Bake(ShiBingAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var shibing = new ShiBing
        {
            Name = authoring.Name,
            TarEntity = Entity.Null,
            ShootEntity = Entity.Null,
            FirePoint_R = GetEntity(authoring.FirePoint_R, TransformUsageFlags.Dynamic),
            FirePoint_L = GetEntity(authoring.FirePoint_L, TransformUsageFlags.Dynamic),
            FirePoint_R2 = GetEntity(authoring.FirePoint_R2, TransformUsageFlags.Dynamic),
            FirePoint_L2 = GetEntity(authoring.FirePoint_L2, TransformUsageFlags.Dynamic),
            CenterPoint = GetEntity(authoring.CenterPoint, TransformUsageFlags.Dynamic),
            CameraPoint = GetEntity(authoring.CameraPoint, TransformUsageFlags.Dynamic),
            Foot_R = GetEntity(authoring.Foot_R, TransformUsageFlags.Dynamic),
            Foot_L = GetEntity(authoring.Foot_L, TransformUsageFlags.Dynamic),
            Particle_1 = GetEntity(authoring.Particle_1, TransformUsageFlags.Dynamic),
            DeadPoint = GetEntity(authoring.DeadPoint, TransformUsageFlags.Dynamic),
            DeadParticle = GetEntity(authoring.DeadParticle, TransformUsageFlags.Dynamic),
            DeadLanguage = GetEntity(authoring.DeadLanguage, TransformUsageFlags.Dynamic),
            AvatarNamePoint = GetEntity(authoring.AvatarNamePoint, TransformUsageFlags.Dynamic),
            Is_Parasitic = authoring.Is_Parasitic,
            Is_LanguageNoBullet = authoring.Is_LanguageNoBullet,
            
        };
        AddComponent(entity, shibing);

    }
}
