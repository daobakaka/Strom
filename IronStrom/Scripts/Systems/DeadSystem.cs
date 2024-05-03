using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ShiBingSystem))]
[UpdateAfter(typeof(bulletSystem))]
[UpdateAfter(typeof(AttackBoxSystem))]
public partial class DeadSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<RebirthChange> m_RebirthChange;
    ComponentLookup<SX> m_SX;
    ComponentLookup<FengHuang> m_FengHuang;
    ComponentLookup<Monster> m_Monster;
    ComponentLookup<Bullet> m_Bullet;
    ComponentLookup<OwnerData> m_OwnerData;
    ComponentLookup<BarrageCommand> m_BarrageCommand;
    ComponentLookup<EntityOpenID> m_EntityOpenID;

    void UpDataComponentLookup()
    {
        m_transform.Update(this);
        m_LocaltoWorld.Update(this);
        m_MyLayer.Update(this);
        m_UpSkill.Update(this);
        m_ShiBing.Update(this);
        m_RebirthChange.Update(this);
        m_SX.Update(this);
        m_FengHuang.Update(this);
        m_Monster.Update(this);
        m_Bullet.Update(this);
        m_OwnerData.Update(this);
        m_BarrageCommand.Update(this);
        m_EntityOpenID.Update(this);

    }

    protected override void OnCreate()
    {
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_RebirthChange = GetComponentLookup<RebirthChange>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_FengHuang = GetComponentLookup<FengHuang>(true);
        m_Monster = GetComponentLookup<Monster>(true);
        m_Bullet = GetComponentLookup<Bullet>(true);
        m_OwnerData = GetComponentLookup<OwnerData>(true);
        m_BarrageCommand = GetComponentLookup<BarrageCommand>(true);
        m_EntityOpenID = GetComponentLookup<EntityOpenID>(true);

    }
    protected override void OnUpdate()
    {
        UpDataComponentLookup();

        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);

        int random = UnityEngine.Random.Range(0, 100);
        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();
        //EndFixedStepSimulationEntityCommandBufferSystem.Singleton
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        var deadjob = new DeadJob
        {
            random = random,
            spawn = spawn,
            ECB = ecbParallel,
            transform = m_transform,
            LocaltoWorld = m_LocaltoWorld,
            m_mylayer = m_MyLayer,
            upSkill = m_UpSkill,
            shibing = m_ShiBing,
            rebirthChange = m_RebirthChange,
            sx = m_SX,
            fenghuang = m_FengHuang,
            monster = m_Monster,
            bullet = m_Bullet,
            ownerData = m_OwnerData,
            entityOpenID = m_EntityOpenID,
        };
        Dependency = deadjob.ScheduleParallel(Dependency);

        //Entity有对应Obj删除对应的Obj
        Entities.ForEach((Entity entity, SynchronizeAni synAni, Die die) =>
        {
            if (SynchronizeManager.synchronizeManager.SynAniDic.TryGetValue(entity, out GameObject obj))
                SynchronizeManager.synchronizeManager.DestroyObj(obj, entity);

        }).WithBurst().WithStructuralChanges().Run();

        //怪物的死亡单独处理
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;
        Entities.ForEach((Entity enti, Monster monster, Die dead) =>
        {
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
            // 生成一个1到100的随机整数（包含1和100）
            int random1 = random.NextInt(1, 101);
            //这个玩家(PlayerEntiID)获得这个怪物的掉落物品
            var PlayerEntiID = monsterMager.MonsterDeadCountPlayerAT(enti, EntityManager);
            MonsterItemDrops(PlayerEntiID, random1, dead.DeadPoint, spawn);
            EntityManager.DestroyEntity(enti);
        }).WithBurst().WithStructuralChanges().Run();



        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

    //怪兽掉落物品
    void MonsterItemDrops(Entity PlayerEntiID, int random, Entity Pos, Spawn spawn)
    {
        if (!EntityManager.Exists(Pos)) return;
        var teamMager = TeamManager.teamManager;
        var EntiUIMager = EntityUIManager.Instance;
        var gameRoot = GameRoot.Instance;
        if (teamMager == null || EntiUIMager == null || gameRoot == null) return;
        //拿到PlayerEntiID的玩家
        var player = teamMager.EntityIDByPlayerData(PlayerEntiID);
        if (string.IsNullOrWhiteSpace(player.m_Open_ID)) return;

        Entity dropsEnti = Entity.Null;
        var enemyjidi = Entity.Null;
        if (player.m_Team == layer.Team1)
            enemyjidi = spawn.JiDi_Team2;
        else if (player.m_Team == layer.Team2)
            enemyjidi = spawn.JiDi_Team1;

        int count = gameRoot.AwardShiBingNumDic.Count;
        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue); // 在主线程生成种子
        Unity.Mathematics.Random random_ = new Unity.Mathematics.Random(seed);
        int random2 = random_.NextInt(0, count); // 生成0到count - 1之间的随机整数
        //int random2 = UnityEngine.Random.Range(0, gameRoot.AwardShiBingNumDic.Count - 1);
        int len = 0;
        ShiBingName sbName = ShiBingName.Null;
        int sbNum = 0;
        foreach(var pair in gameRoot.AwardShiBingNumDic)
        {
            if(len >= random2)
            {
                sbName = pair.Key;
                sbNum = pair.Value;
                break;
            }
            len += 1;
        }
        dropsEnti = spawn.ShiBingNameEntity(in sbName, in player.m_Team, in spawn, false);
        if (dropsEnti == Entity.Null) return;
        for (int i = 0; i < sbNum; ++i)
            teamMager.InstantiateShiBing(in player, dropsEnti, Pos, enemyjidi, EntityManager);

        Debug.Log($"  玩家{player.m_Nick}获得怪物掉落");
    }

}
[BurstCompile]
partial struct DeadJob : IJobEntity
{
    public int random;
    public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<MyLayer> m_mylayer;
    [ReadOnly] public ComponentLookup<UpSkill> upSkill;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<RebirthChange> rebirthChange;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<FengHuang> fenghuang;
    [ReadOnly] public ComponentLookup<Monster> monster;
    [ReadOnly] public ComponentLookup<Bullet> bullet;
    [ReadOnly] public ComponentLookup<OwnerData> ownerData;
    [ReadOnly] public ComponentLookup<EntityOpenID> entityOpenID;
    void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndex, DeadAspects dead)
    {
        //基地死亡不销毁
        if(shibing.TryGetComponent(entity,out ShiBing sb))
        {
            if (shibing[entity].Name == ShiBingName.JiDi)
                return;
        }
        //如果是怪兽,就退出（因为要计算玩家攻击力）
        if (monster.TryGetComponent(entity, out Monster mons))
        {
            return;
            //测试写的队伍1，之后应该是根据击杀怪兽的人来决定的
            //MonsterItemDrops(random, dead.DeadPoint, layer.Team1, chunkIndex);
        }
        if (dead.Is_DieDirectly)//直接死亡单位
        {
            ECB.DestroyEntity(chunkIndex, dead.Die_Entity);
            return;
        }


        //有死亡特效，触发死亡特效
        if (transform.TryGetComponent(dead.DeadParticle, out LocalTransform ltf))
        {
            var deadPar = ECB.Instantiate(chunkIndex, dead.DeadParticle);
            var localworld = LocaltoWorld[dead.DeadPoint];
            ECB.SetComponent(chunkIndex, deadPar, localworld);
            ECB.SetComponent(chunkIndex, deadPar, new LocalTransform
            {
                Position = LocaltoWorld[dead.DeadPoint].Position,
                Rotation = LocaltoWorld[dead.DeadPoint].Rotation,
                Scale = 1,
            });
        }

        //有亡语效果，触发亡语效果
        if (transform.TryGetComponent(dead.DeadLanguage, out LocalTransform localtransform))
            DeadLanguag(ref entity, dead, dead.DeadLanguage, chunkIndex);
        if (transform.TryGetComponent(dead.DeadLanguage2, out LocalTransform localtransform2))
            DeadLanguag(ref entity, dead, dead.DeadLanguage2, chunkIndex);





        ECB.DestroyEntity(chunkIndex, dead.Die_Entity);
    }

    //亡语效果
    void DeadLanguag(ref Entity entity, DeadAspects dead,Entity deadLanguag, int chunkIndex)
    {
        var deadLanguage = ECB.Instantiate(chunkIndex, deadLanguag);
        var localworld = LocaltoWorld[dead.DeadPoint];
        ECB.SetComponent(chunkIndex, deadLanguage, localworld);
        LocalTransform deadtransform;
        deadtransform.Position = LocaltoWorld[dead.DeadPoint].Position;
        deadtransform.Rotation = LocaltoWorld[dead.DeadPoint].Rotation;
        //如果是不攻击单位，只和地方发射碰撞引爆的子弹
        //确保子弹爆炸实例化出来的火焰是正确的朝向
        if (bullet.TryGetComponent(entity,out Bullet bullt))
        {
            if (bullet[entity].Is_NOAttack)
            {
                deadtransform.Rotation = quaternion.identity;
                deadtransform.Position.y = 0;
            }
        }
        ECB.SetComponent(chunkIndex, deadLanguage, new LocalTransform
        {
            Position = deadtransform.Position,
            Rotation = deadtransform.Rotation,
            Scale = 1,//transform[deadLanguag].Scale,//1,
        });

        //为亡语效果添加拥有者数据
        AddOwnerData(entity, ref deadLanguage, chunkIndex);


        MyLayer InstanLyaer = new MyLayer();
        if (m_mylayer[entity].BelongsTo == layer.Team1 ||
            m_mylayer[entity].BelongsTo == layer.Team1Bullet)
        {
            if(dead.Is_LanguageNoBullet)
                InstanLyaer.BelongsTo = layer.Team1;
            else
                InstanLyaer.BelongsTo = layer.Team1Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team2;
            InstanLyaer.CollidesWith_2 = layer.Neutral;
        }
        else if (m_mylayer[entity].BelongsTo == layer.Team2 ||
            m_mylayer[entity].BelongsTo == layer.Team2Bullet)
        {
            if (dead.Is_LanguageNoBullet)
                InstanLyaer.BelongsTo = layer.Team2;
            else
                InstanLyaer.BelongsTo = layer.Team2Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team1;
            InstanLyaer.CollidesWith_2 = layer.Neutral;
        }
        else if (m_mylayer[entity].BelongsTo == layer.Neutral)
        {
            InstanLyaer.BelongsTo = layer.Neutral;
            InstanLyaer.CollidesWith_1 = layer.Team1;
            InstanLyaer.CollidesWith_2 = layer.Team2;
        }

        ECB.SetComponent(chunkIndex, deadLanguage, new MyLayer
        {
            BelongsTo = InstanLyaer.BelongsTo,
            CollidesWith_1 = InstanLyaer.CollidesWith_1,
            CollidesWith_2 = InstanLyaer.CollidesWith_2,
        });

        //如果为重生单位
        if (fenghuang.TryGetComponent(entity, out FengHuang rb) &&
            entityOpenID.TryGetComponent(entity, out EntityOpenID entiID))
        {
            ECB.SetComponent(chunkIndex, deadLanguage, new RebirthChange
            {
                Is_UpSkill = sx[entity].Is_UpSkill,
                PlayerEntityID = entityOpenID[entity].PlayerEntiyID,
            });

        }

    }

    //添加拥有者数据
    void AddOwnerData(Entity entity, ref Entity deadLanguage, int chunkIndex)
    {
        if (shibing.TryGetComponent(entity, out ShiBing sb))//如果是士兵，拥有者信息就是我
        {
            ECB.AddComponent(chunkIndex, deadLanguage, new OwnerData//拥有者信息
            {
                Owner = entity,
            });
        }
        else if (bullet.TryGetComponent(entity, out Bullet bult))//如果是子弹，拥有者信息就是子弹的拥有者
        {
            if (ownerData.TryGetComponent(entity, out OwnerData owdata))
            {
                ECB.AddComponent(chunkIndex, deadLanguage, new OwnerData
                {
                    Owner = ownerData[entity].Owner,
                });
            }
        }
    }
}
