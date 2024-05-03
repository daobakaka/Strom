using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(EntityCtrlEventSystem))] //在个System之前执行
public partial class DirectShootSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<ShiBingChange> m_shibingCh;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<EntityCtrl> m_entiCtrl;
    ComponentLookup<Die> m_Die;
    ComponentLookup<SX> m_SX;
    ComponentLookup<UpSkill> m_UpSkill;

    void UpDateComponentLookup()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_entiCtrl.Update(this);
        m_Die.Update(this);
        m_SX.Update(this);
        m_UpSkill.Update(this);
        m_shibing.Update(this);
    }
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_Die = GetComponentLookup<Die>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);

    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        if (!SystemAPI.HasSingleton<Spawn>()) return;// 检查是否存在 Spawn 类型的实体
        Spawn spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        //var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        //var directshootjob = new DirectShootJob
        //{
        //    ECB = ecb,
        //    transfrom = m_transfrom,
        //    localtoWorld = m_LocaltoWorld,
        //    entiCtrl = m_entiCtrl,
        //    die = m_Die,
        //};
        //Dependency = directshootjob.ScheduleParallel(Dependency);
        //Dependency.Complete();
        //ecb.Playback(EntityManager);
        //ecb.Dispose();
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        //单线程中计算直接攻击伤害
        Entities.ForEach((Entity entity, ShiBing shibing,ref DirectShoot dirShoot) =>
        {
            UpDateComponentLookup();
            if (m_SX[entity].Cur_ShootTime > 0)//如果攻击时间没到
                return;
            var ShootEntity = shibing.ShootEntity;
            //如果没有攻击目标 或者 在护盾内
            if (!EntityManager.Exists(ShootEntity))
                return;
            if (EntityManager.HasComponent<InShield>(ShootEntity))
            {
                if (EntityManager.IsComponentEnabled<InShield>(ShootEntity))
                    return;
            }
            var shootSX = m_SX[ShootEntity];
            var entitySX = m_SX[entity];
            //判断是否对空对地敌人有攻击优势
            var entiAT = spawn.Is_Advantage(in entitySX.AT, in entitySX, in shootSX);
            //我的攻击力减去被打敌人的格挡率，就是我们的攻击被抵挡的值
            var AT = entiAT - entiAT * shootSX.DB;
            shootSX.DP -= AT;
            if(shootSX.DP < 0)
            {
                float at = shootSX.DP * -1;
                shootSX.Cur_HP += shootSX.DP;//先减完防御值再减生命值
                shootSX.DP = 0;
                //如果被打的是怪物就记录攻击，用于统计玩家伤害量，获得怪物掉落物
                if (EntityManager.HasComponent<Monster>(ShootEntity))
                {   //如果这个攻击的人有EntityOpenID(有玩家的士兵)才记录
                    if (EntityManager.HasComponent<EntityOpenID>(entity))
                    {
                        var EntiOpenID = EntityManager.GetComponentData<EntityOpenID>(entity);
                        monsterMager.MonsterRecordPlayerAT(EntiOpenID, ShootEntity, at, EntityManager);
                    }
                }
            }

            EntityManager.SetComponentData(ShootEntity, shootSX);

            //打敌人告诉他是我在打他，他死的时候好把积分加给我
            if (EntityManager.HasComponent<Integral>(ShootEntity))
            {
                var shootInteg = EntityManager.GetComponentData<Integral>(ShootEntity);
                shootInteg.AttackMeEntity = entity;
                EntityManager.SetComponentData(ShootEntity, shootInteg);
            }

            //如果有升级，记录挨打伤害
            if (EntityManager.HasComponent<UpSkill>(shibing.ShootEntity))
            {
                if(m_UpSkill[shibing.ShootEntity].upSkill_Name == UpSkillName.GangQiu)
                {
                    var upSkill = m_UpSkill[shibing.ShootEntity];
                    upSkill.InjuryRecord += AT;
                    EntityManager.SetComponentData(shibing.ShootEntity, upSkill);
                }
            }

            //累积目标是否为同一个目标，是否要重新累积--------------
            if (dirShoot.CD_ShootEntity == Entity.Null)
            {
                dirShoot.CD_ShootEntity = shibing.ShootEntity;
                dirShoot.Is_ShootEntityChanges = true;
            }
            else if (dirShoot.CD_ShootEntity != shibing.ShootEntity)
            {
                entitySX.AT = dirShoot.AT_Min;
                dirShoot.CD_ShootEntity = shibing.ShootEntity;
                dirShoot.Is_ShootEntityChanges = true;
            }
            else
                dirShoot.Is_ShootEntityChanges = false;
            //----------

            //如果为累积伤害----------------
            dirShoot.Cur_IntervalTime -= SystemAPI.Time.DeltaTime;
            if(dirShoot.Is_CumulativeDamage && entitySX.AT < dirShoot.AT_Max &&dirShoot.Cur_IntervalTime <= 0)
            {
                dirShoot.Cur_IntervalTime = dirShoot.IntervalTime;
                //累积公式
                entitySX.AT += 1;
                EntityManager.SetComponentData(entity, entitySX);
            }

        }).WithoutBurst().WithStructuralChanges().Run();

    }
}

[BurstCompile]
public partial struct DirectShootJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transfrom;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> localtoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<Die> die;
    void Execute(Entity entity,ShiBingAspects shibingAsp, DirectShoot dshoot)
    {

    }
}
