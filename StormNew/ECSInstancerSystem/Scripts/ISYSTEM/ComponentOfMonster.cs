using Games.Characters.EliteUnits;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public enum EntitySkillName 
{ 
    bullet,
    sprike,
    rock
}
public enum EntityCamp
{
   ice,
   fire
}
public enum EntityLevel
{ 
    level1,
    level2,
    eliteLevel1,
    eliteLevel2,
    eliteLevel3,
    eliteLevel4,
    eliteLevel5,


}
public struct Unit1Component:IComponentData
{
    /// <summary>
    /// �û�����
    /// </summary>
    public FixedString32Bytes playerName;
    /// <summary>
    ///101���б�ϵ��Ӣ���� 102���л�ϵ��Ӣ�� 61������ 63�����  62�𲽱� 64�����
    /// </summary>
    public int order;
    /// <summary>
    /// ��Ӫ
    /// </summary>
    public EliteUnitPortalMan.Team team;
    /// <summary>
    /// ��������
    /// </summary>
    public MonsterType monsterType;

    public float health;
    public float speed;
    public float damage;
    /// <summary>
    /// ��������ʱ��
    /// </summary>
    public float parameterTime;
    public float height;
    /// <summary>
    /// ���ֹ���
    /// </summary>
    public BlobAssetReference<PlayerID> PlayerName;

    public float intergalTimes;
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool death;
    /// <summary>
    /// ����Ŀ��
    /// </summary>
    public bool iftarget;
    public Entity targetEntity;
    /// <summary>
    /// �Ƿ����ڹ���Boss
    /// </summary>
    public bool isAttackBoss;
    /// <summary>
    /// �Ƿ���빥������
    /// </summary>
    public bool isIntoAttack;
    /// <summary>
    /// ��������ʣ��ʱ�䣨��Ҫ�Ͷ���ʱ��ƥ�䣩
    /// </summary>
    public float attackTimer;
    public float tempattackTimer;
    /// <summary>
    /// ���һ�������ҵ��� ���ڻ��ּ���
    /// </summary>
    public FixedString32Bytes lastAttacker;
    /// <summary>
    /// ��������
    /// </summary>
    public int random_index;
    /// <summary>
    /// �Ƿ񹥻�CDĿǰ��������
    /// </summary>
    public bool isAttackCD;
    public bool iscontinueMove;
    //������ʼ���ֵ
    public float animationStartTimeMultiplier;
}
public struct UnitSpawner : IComponentData
{
    public Entity entity1;
    public Entity entity2;
    public int insnum;
    public float spawnRate;
    public float lengthRate;
    public float nextSpawnerTime;
    public Entity killer;
    public Entity GpuMonsterIce;
    public Entity GpuMonsterFire;
    public Entity bullet;
    public Entity deadBodyice1;
    public Entity deadBodyice2;
    public Entity deadBodyfire1;
    public Entity deadBodyfire2;
}
public struct unitTagBoss:IComponentData 
{
    /// <summary>
    /// ��Ӫ
    /// </summary>
    public EliteUnitPortalMan.Team team;
    public float4 rotation;
    public float health;
}
/// <summary>
/// ��Ӣ�ֱ�ǩ
/// </summary>
public struct UnitSprint :IComponentData
{
    public bool sprint;
    public MonsterType monsterType;
    public EliteUnitPortalMan.Team team;
    /// <summary>
    /// Ŀǰ�����ӵ���Ŀ��
    /// </summary>
    public Entity targetEntity;
    /// <summary>
    /// ֪ͨ��ʵ�����ﲥ�Ŷ���
    /// </summary>
    public bool attack;
    /// <summary>
    /// ��ʵ�����Ƿ񲥷ŵ���ͨ�����ص���ÿ�ص�һ�μ��һ��Enity�Ƿ���Ŀ���������˺���
    /// </summary>
    public bool generalattackBack;
}
public struct EntitySkill : IComponentData
{
    public EntitySkillName skill;
    public EntityCamp camp;
    public float damdage;
    public float3 position;

}
/// <summary>
/// �����ӵ���ը�������˺����� �������˺�
/// </summary>
public struct BulletSkill : IComponentData
{
    /// <summary>
    /// �û����� ΨһID
    /// </summary>
    public FixedString32Bytes playerName;
    public EliteUnitPortalMan.Team team;
    public MonsterType monsterType;
    //��ը��Χ
    public float range;
    //�˺�
    public int damage;
}

public struct StartSystemTag: IComponentData
{ 


}
public struct TestSystemGpu : IComponentData
{ 

}
public struct CrowdInstanceState : IComponentData// example struct that determines the state for crowd animator
{
    public float initanimationStartTimeMultiplier;
    public int animationIndex;
    public float animationSpeed;
    public float animationStartTimeMultiplier;
    public StateModificationType modificationType;

}
public struct IceCampDistinguish : ISharedComponentData 
{
    public int value; 
}
public struct PlayerID
{
    public BlobArray<char> Name;
}

public struct PlayerIDReference : IComponentData
{
    public BlobAssetReference<PlayerID> PlayerName;

}
//unsafe public struct NameComponent : IComponentData
//{
//    public fixed char Name[64]; // ����̶����ȵ��ַ�����
//}
public struct PlayerDic : IComponentData
{ 



}

#region the physicl check
public struct EntityAttack : IComponentData
{
    public float entityDamage;
    public EntityLevel myLevel;
    public float collisionHealth;
    public bool ifIce;
}
#endregion