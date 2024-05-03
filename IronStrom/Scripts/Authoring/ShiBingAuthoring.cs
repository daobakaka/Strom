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
    Shield,//����
    JiDi,


}


public class ShiBingAuthoring : MonoBehaviour
{
    public ShiBingName Name;
    public Entity TarEntity;//����Ŀ��

    public GameObject FirePoint_R;//�����
    public GameObject FirePoint_L;//�����
    public GameObject FirePoint_R2;//�����
    public GameObject FirePoint_L2;//�����
    public GameObject CenterPoint;//���ĵ�
    public GameObject CameraPoint;//�����λ��
    public GameObject Foot_R;//���ҽ�
    public GameObject Foot_L;
    public GameObject Particle_1;
    public GameObject DeadPoint;//����λ��
    public GameObject DeadParticle;//������Ч
    public GameObject DeadLanguage;//����Ч��
    public GameObject AvatarNamePoint;//ͷ������λ��
    //[Tooltip("-�Ƿ���������ת����--")] public bool Is_AniMoveRL;//�Ƿ���������ת����

    [Tooltip("--�Ƿ�Ϊ����������λ--")] public bool Is_Parasitic;
    [Tooltip("--����Ч����Ϊ�ӵ�----")] public bool Is_LanguageNoBullet;



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
