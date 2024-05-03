using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;

public partial class AttackBoxSystem : SystemBase
{
    ComponentLookup<SX> m_SX;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;

    void UpDataComponentLookup()
    {
        m_SX.Update(this);
        m_shibing.Update(this);
        m_UpSkill.Update(this);
        m_LocaltoWorld.Update(this);
    }
    protected override void OnCreate()
    {
        m_SX = GetComponentLookup<SX>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
    }

    protected override void OnUpdate()
    {
        UpDataComponentLookup();



        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//创建一个碰撞检测需要的单例

        Entities.ForEach((Entity entity,LocalToWorld loacltoworld ,MyLayer mylayer,ref AttackBox abox) =>
        {
            if (abox.BoxState == AttactBoxState.NUll)
                return;
            var time = SystemAPI.Time.DeltaTime;
            if(!abox.Is_NoTime)
                abox.ExistenceTime -= time;//box持续时间
            abox.Cur_SustainTime -= time;//box间隔时间
            if (abox.Cur_SustainTime > 0)
                return;
            else
                abox.Cur_SustainTime = abox.SustainTime;

            NativeList<ColliderCastHit> outHitsAll = new NativeList<ColliderCastHit>(Allocator.Temp);
            ColliderCastHit outHit = new ColliderCastHit();

            //是否检测到敌方单位
            if (abox.BoxShape == AttactBoxShape.Box)
                DetectionBox(entity, physicsWorld,ref abox, mylayer, loacltoworld, ref outHitsAll, ref outHit, abox.Is_All);
            else if (abox.BoxShape == AttactBoxShape.Sphere)
                DetectionSphere(physicsWorld, abox, mylayer, loacltoworld, ref outHitsAll, ref outHit, abox.Is_All);
            else if (abox.BoxShape == AttactBoxShape.Capsule)
            {
                return;
            }

            //是否为持续伤害
            if (abox.BoxState == AttactBoxState.OnceDamage)//单次伤害
            {
                //造成伤害
                CalculateHurt(entity, abox, ref outHitsAll, ref outHit, abox.Is_All);
                //后直接删除box
                EntityManager.AddComponentData(entity, new Die());
                m_SX.Update(this);
                m_shibing.Update(this);
            }
            else if(abox.BoxState == AttactBoxState.ManyDamage)//持续伤害
            {
                //持续时间到了后删除box
                if (abox.ExistenceTime <= 0)
                {
                    EntityManager.AddComponentData(entity, new Die());
                    m_SX.Update(this);
                    m_shibing.Update(this);
                }
                //造成伤害
                CalculateHurt(entity, abox, ref outHitsAll, ref outHit, abox.Is_All);

            }


        }).WithoutBurst().WithStructuralChanges().Run();
    }

    bool DetectionBox(Entity entity, PhysicsWorldSingleton physicsworld,ref AttackBox abox, MyLayer mylayer, LocalToWorld localworld,
        ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//矩形检测器
    {
        UpDataComponentLookup();
        bool b = false;
        if(abox.Is_Remote && abox.halfExtents.z < abox.MaxRemote)
        {
            abox.halfExtents.z += abox.MaxRemote * SystemAPI.Time.DeltaTime;
        }
        float maxDistance = abox.halfExtents.z;//math.cmax(abox.halfExtents);

        uint collidesWithMask = 0;
        if ((int)mylayer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)mylayer.CollidesWith_1);
        if ((int)mylayer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)mylayer.CollidesWith_2);
        if ((int)mylayer.BulletCollidesWith != 0)//判断这个单位是否有子弹检测
            collidesWithMask |= (1u << (int)mylayer.BulletCollidesWith);
        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)mylayer.BelongsTo),
            CollidesWith = collidesWithMask,
            GroupIndex = 0
        };
        if (Is_All)//检测碰撞到的全部单位
            b = physicsworld.BoxCastAll(localworld.Position + abox.Offset, m_LocaltoWorld[entity].Rotation, abox.halfExtents, new float3(0, 0, 1), maxDistance, ref outHits, filterDetection);
        else//检测碰到的第一个单位
            b = physicsworld.BoxCast(localworld.Position + abox.Offset, m_LocaltoWorld[entity].Rotation, abox.halfExtents, new float3(0, 0, 1), maxDistance, out outHit, filterDetection);
        return b;
    }

    bool DetectionSphere(PhysicsWorldSingleton physicsworld, AttackBox abox, MyLayer mylayer, LocalToWorld localworld,
        ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//圆形检测器
    {
        bool b = false;

        uint collidesWithMask = 0;
        if ((int)mylayer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)mylayer.CollidesWith_1);
        if ((int)mylayer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)mylayer.CollidesWith_2);
        if ((int)mylayer.BulletCollidesWith != 0)//判断这个单位是否有子弹检测
            collidesWithMask |= (1u << (int)mylayer.BulletCollidesWith);
        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)mylayer.BelongsTo),
            CollidesWith = collidesWithMask,
            GroupIndex = 0
        };
        if (Is_All)//检测碰撞到的全部单位
            b = physicsworld.SphereCastAll(localworld.Position + abox.Offset, abox.R, new float3(0, 1, 0), abox.R * 2, ref outHits, filterDetection);
        else//检测碰到的第一个单位
            b = physicsworld.SphereCast(localworld.Position + abox.Offset, abox.R, new float3(0, 1, 0), abox.R * 2, out outHit, filterDetection);

        return b;
    }

    bool DetectionCapsule()//胶囊检测器
    {
        return false;
    }

    //造成伤害
    void CalculateHurt(Entity entity, AttackBox abox, ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//计算伤害
    {
        if(Is_All)//计算检测到的全部单位
        {
            if (outHits.IsUnityNull())//链表不合法
                return;

            foreach(ColliderCastHit collidHit in outHits)
            {
                var hit = collidHit.Entity;
                Hurt(entity, ref hit, abox.AT);
            }
        }
        else//计算单个单位
        {
            if (outHit.IsUnityNull())//Hit不合法
                return;

            var hit = outHit.Entity;
            Hurt(entity, ref hit, abox.AT);
        }
    }

    void Hurt(Entity entity, ref Entity hit, float hurt)//伤害
    {
        //如果被攻击目标没有SX组件 或者 在护盾里就退出
        if (!EntityManager.HasComponent<SX>(hit))
            return;
        if (EntityManager.HasComponent<InShield>(hit))
        {
            if (EntityManager.IsComponentEnabled<InShield>(hit))
                return;
        }
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        var hit_sx = m_SX[hit];
        //计算这个Box的拥有者对这个敌人是否有对空对地优势
        if (!SystemAPI.HasSingleton<Spawn>()) return;
        var hurtat = SystemAPI.GetSingleton<Spawn>().Is_Advantage(in hurt, entity, in hit_sx);
        //我的攻击力减去被打敌人的格挡率，就是我们的攻击被抵挡的值
        var AT = hurtat - hurtat * hit_sx.DB;
        hit_sx.DP -= AT;
        if (hit_sx.DP < 0)
        {
            float at = hit_sx.DP * -1;
            hit_sx.Cur_HP += hit_sx.DP;
            hit_sx.DP = 0;

            //如果被打的是怪物就记录攻击，用于统计玩家伤害量，获得怪物掉落物
            if (EntityManager.HasComponent<Monster>(hit))
            {   //如果这个攻击的人有EntityOpenID(有玩家的士兵)才记录
                if(EntityManager.HasComponent<OwnerData>(entity))
                {
                    var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;
                    if (EntityManager.HasComponent<EntityOpenID>(shibing))
                    {
                        var EntiOpenID = EntityManager.GetComponentData<EntityOpenID>(shibing);
                        monsterMager.MonsterRecordPlayerAT(EntiOpenID, hit, at, EntityManager);
                    }
                }
            }
        }

        EntityManager.SetComponentData(hit, hit_sx);

        //如果有升级，记录攻击力
        if (EntityManager.HasComponent<UpSkill>(hit))
        {
            UpDataComponentLookup();
            if (m_UpSkill[hit].upSkill_Name == UpSkillName.GangQiu)
            {
                var upSkill = m_UpSkill[hit];
                upSkill.InjuryRecord += AT;
                EntityManager.SetComponentData(hit, upSkill);
            }
        }

        if (hit_sx.Cur_HP <= 0)
        {
            if (!EntityManager.HasComponent<Die>(hit))
            {
                EntityManager.AddComponentData(hit, new Die
                {
                    DeadParticle = m_shibing[hit].DeadParticle,
                    DeadPoint = m_shibing[hit].DeadPoint,
                    DeadLanguage = m_shibing[hit].DeadLanguage,
                    Is_LanguageNoBullet = m_shibing[hit].Is_LanguageNoBullet,
                });
                m_SX.Update(this);
                m_shibing.Update(this);
            }

            //打死目标我获得积分
            if(EntityManager.HasComponent<OwnerData>(entity))
            {
                var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;//获得Box的拥有者(这名士兵)
                if (!EntityManager.HasComponent<Integral>(shibing))
                    return;
                var integralCompent = EntityManager.GetComponentData<Integral>(shibing);//拿到士兵积分
                var HitSXCompent = EntityManager.GetComponentData<SX>(hit);//拿到被我打死的人的最大生命值(生命值=积分)
                integralCompent.ATIntegral += HitSXCompent.HP;
                EntityManager.SetComponentData(shibing, integralCompent);
            }
        }
    }


}
