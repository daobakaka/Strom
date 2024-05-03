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
    /// 用户名字
    /// </summary>
    public FixedString32Bytes playerName;
    /// <summary>
    ///101所有冰系精英怪物 102所有火系精英怪 61冰步兵 63冰骑兵  62火步兵 64火骑兵
    /// </summary>
    public int order;
    /// <summary>
    /// 阵营
    /// </summary>
    public EliteUnitPortalMan.Team team;
    /// <summary>
    /// 兵种类型
    /// </summary>
    public MonsterType monsterType;

    public float health;
    public float speed;
    public float damage;
    /// <summary>
    /// 死亡动画时间
    /// </summary>
    public float parameterTime;
    public float height;
    /// <summary>
    /// 积分关联
    /// </summary>
    public BlobAssetReference<PlayerID> PlayerName;

    public float intergalTimes;
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool death;
    /// <summary>
    /// 攻击目标
    /// </summary>
    public bool iftarget;
    public Entity targetEntity;
    /// <summary>
    /// 是否正在攻击Boss
    /// </summary>
    public bool isAttackBoss;
    /// <summary>
    /// 是否进入攻击流程
    /// </summary>
    public bool isIntoAttack;
    /// <summary>
    /// 攻击动画剩余时间（需要和动画时间匹配）
    /// </summary>
    public float attackTimer;
    public float tempattackTimer;
    /// <summary>
    /// 最后一个攻击我的人 用于积分计算
    /// </summary>
    public FixedString32Bytes lastAttacker;
    /// <summary>
    /// 敌人索引
    /// </summary>
    public int random_index;
    /// <summary>
    /// 是否攻击CD目前机甲在用
    /// </summary>
    public bool isAttackCD;
    public bool iscontinueMove;
    //动画初始随机值
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
    /// 阵营
    /// </summary>
    public EliteUnitPortalMan.Team team;
    public float4 rotation;
    public float health;
}
/// <summary>
/// 精英怪标签
/// </summary>
public struct UnitSprint :IComponentData
{
    public bool sprint;
    public MonsterType monsterType;
    public EliteUnitPortalMan.Team team;
    /// <summary>
    /// 目前用于子弹的目标
    /// </summary>
    public Entity targetEntity;
    /// <summary>
    /// 通知现实世界里播放动画
    /// </summary>
    public bool attack;
    /// <summary>
    /// 现实世界是否播放的普通攻击回调（每回调一次检测一下Enity是否有目标对其产生伤害）
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
/// 机甲子弹爆炸产生的伤害计算 死神技能伤害
/// </summary>
public struct BulletSkill : IComponentData
{
    /// <summary>
    /// 用户名字 唯一ID
    /// </summary>
    public FixedString32Bytes playerName;
    public EliteUnitPortalMan.Team team;
    public MonsterType monsterType;
    //爆炸范围
    public float range;
    //伤害
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
//    public fixed char Name[64]; // 定义固定长度的字符数组
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