using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


public partial class PutUpShiBingSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<ShiBing> m_shibing;
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
    }
    protected override void OnUpdate()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_UpSkill.Update(this);
        m_shibing.Update(this);


        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        uint seed = (uint)Random.Range(1, int.MaxValue);
        var Mathrand = new Unity.Mathematics.Random(seed);

        //var putupshibingjob = new PutUpShiBingJob
        //{
        //    time = SystemAPI.Time.DeltaTime,
        //    rand = rand,
        //    ECB = ecb.AsParallelWriter(),
        //    spawn = spawn,
        //    transform = m_transfrom,
        //    LocaltoWorld = m_LocaltoWorld,
        //    upSkill = m_UpSkill,
        //    shibing = m_shibing,
        //};
        //Dependency = putupshibingjob.ScheduleParallel(Dependency);
        //Dependency.Complete();
        //ecb.Playback(EntityManager);
        //ecb.Dispose();

        Entities.ForEach((Entity enti, MyLayer myLayer, ref PutUpShiBing PUshibing) =>
        {
            PUshibing.Cur_PutUpTime -= SystemAPI.Time.DeltaTime;
            if (PUshibing.Cur_PutUpTime > 0) return;
            PUshibing.Cur_PutUpTime = PUshibing.PutUpTime;

            var teamMager = TeamManager.teamManager;
            if (teamMager == null) return;
            //获得这个士兵的玩家
            if (!EntityManager.HasComponent<EntityOpenID>(enti)) return;
            var PlayerEntiID = EntityManager.GetComponentData<EntityOpenID>(enti).PlayerEntiyID;
            var Player = teamMager.EntityIDByPlayerData(PlayerEntiID);
            if (!EntityManager.Exists(PUshibing.GroundSpawnPoint) || !EntityManager.Exists(PUshibing.AirSpawnPoint) ||
                Player == null)
                return;
            Entity InstanShiBing = Entity.Null;
            int InstanShiBingNum = 0;
            Entity enemyjidi = Entity.Null;

            int rand = Mathrand.NextInt(0, 100);
            Mathrand = new Unity.Mathematics.Random(Mathrand.state);//跟新随机数状态
            //如果有指定制造的士兵
            if (PUshibing.SpecifyShiBing != Entity.Null && EntityManager.HasComponent<ShiBing>(PUshibing.SpecifyShiBing))
                InstanShiBing = PUshibing.SpecifyShiBing;
            else
            {
                InstanShiBing = RandShiBing(rand, in spawn, myLayer, ref InstanShiBingNum);
                PUshibing.PutUpNum = InstanShiBingNum;
            }
            if (InstanShiBing == Entity.Null) return;

            for (int i = 0; i < PUshibing.PutUpNum; ++i)
            {
                if (myLayer.BelongsTo == layer.Team1)
                    enemyjidi = spawn.JiDi_Team2;
                else if (myLayer.BelongsTo == layer.Team2)
                    enemyjidi = spawn.JiDi_Team1;
                Entity spawnPoint = PUshibing.GroundSpawnPoint;//出生在地面还是空中
                if (rand < 10)//小于10的单位是两个飞行单位
                    spawnPoint = PUshibing.AirSpawnPoint;
                teamMager.InstantiateShiBing(in Player, InstanShiBing, spawnPoint, enemyjidi, EntityManager);
                
            }

        }).WithBurst().WithStructuralChanges().Run();


    }

    Entity RandShiBing(int rand, in Spawn spawn, MyLayer layer, ref int InstanShiBingNum)
    {
        Entity randEntity = Entity.Null;

        if (layer.BelongsTo == global::layer.Team1)
        {
            if (rand >= 0 && rand < 5)
            {
                randEntity = spawn.Team1_BingFeng;
                InstanShiBingNum = 5;
            }
            else if (rand >= 5 && rand < 10)
            {
                randEntity = spawn.Team1_FengHuang;
                InstanShiBingNum = 2;
            }
            else if (rand >= 10 && rand < 15)
            {
                randEntity = spawn.Team1_HaiKe;
                InstanShiBingNum = 1;
            }
            else if (rand >= 20 && rand < 25)
            {
                randEntity = spawn.Team1_PaChong;
                InstanShiBingNum = 10;
            }
            else if (rand >= 25 && rand < 30)
            {
                randEntity = spawn.Team1_HuGuang;
                InstanShiBingNum = 5;
            }
            else if (rand >= 30 && rand < 35)
            {
                randEntity = spawn.Team1_YeMa;
                InstanShiBingNum = 10;
            }
            else if (rand >= 35 && rand < 40)
            {
                randEntity = spawn.Team1_GangQiu;
                InstanShiBingNum = 3;
            }
            else if (rand >= 40 && rand < 45)
            {
                InstanShiBingNum = 5;
                randEntity = spawn.Team1_BaoYu;
            }
            else if (rand >= 45 && rand < 50)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_BaZhu;
            }
            else if (rand >= 50 && rand < 55)
            {
                randEntity = spawn.Team1_TieChui;
                InstanShiBingNum = 2;
            }
            else if (rand >= 55 && rand < 65)
            {
                randEntity = spawn.Team1_BaoLei;
                InstanShiBingNum = 1;
            }
            else if (rand >= 65 && rand < 67)//最后三个单位为obj的身体
            {
                randEntity = spawn.Team1_XiNiu;
                InstanShiBingNum = 1;
            }
            else if (rand >= 67 && rand < 69)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_HuoShen;
            }
            else if (rand >= 69 && rand < 71)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_RongDian;
            }
            else if (rand >= 71 && rand < 80)
            {
                randEntity = spawn.Team1_JianYa;
                InstanShiBingNum = 10;
            }
            else if (rand <= 80 && rand < 85)
            {
                randEntity = spawn.Team1_ChangGong;
                InstanShiBingNum = 5;
            }
        }
        else if (layer.BelongsTo == global::layer.Team2)
        {

            if (rand >= 0 && rand < 5)
            {
                randEntity = spawn.Team2_BingFeng;
                InstanShiBingNum = 5;
            }
            else if (rand >= 5 && rand < 10)
            {
                randEntity = spawn.Team2_FengHuang;
                InstanShiBingNum = 2;
            }
            else if (rand >= 10 && rand < 15)
            {
                randEntity = spawn.Team2_HaiKe;
                InstanShiBingNum = 1;
            }
            else if (rand >= 20 && rand < 25)
            {
                InstanShiBingNum = 10;
                randEntity = spawn.Team2_PaChong;
            }
            else if (rand >= 25 && rand < 30)
            {
                randEntity = spawn.Team2_HuGuang;
                InstanShiBingNum = 5;
            }
            else if (rand >= 30 && rand < 35)
            {
                randEntity = spawn.Team2_YeMa;
                InstanShiBingNum = 10;
            }
            else if (rand >= 35 && rand < 40)
            {
                randEntity = spawn.Team2_GangQiu;
                InstanShiBingNum = 3;
            }
            else if (rand >= 40 && rand < 45)
            {
                randEntity = spawn.Team2_BaoYu;
                InstanShiBingNum = 5;
            }
            else if (rand >= 45 && rand < 50)
            {
                randEntity = spawn.Team2_BaZhu;
                InstanShiBingNum = 1;
            }
            else if (rand >= 50 && rand < 55)
            {
                randEntity = spawn.Team2_TieChui;
                InstanShiBingNum = 2;
            }
            else if (rand >= 55 && rand < 65)
            {
                randEntity = spawn.Team2_BaoLei;
                InstanShiBingNum = 1;
            }
            else if (rand >= 65 && rand < 67)//最后三个单位为obj的身体
            {
                randEntity = spawn.Team2_XiNiu;
                InstanShiBingNum = 1;
            }
            else if (rand >= 67 && rand < 69)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team2_HuoShen;
            }
            else if (rand >= 71 && rand < 75)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team2_RongDian;
            }
            else if (rand >= 70 && rand < 80)
            {
                InstanShiBingNum = 10;
                randEntity = spawn.Team2_JianYa;
            }
            else if (rand <= 80 && rand < 85)
            {
                randEntity = spawn.Team2_ChangGong;
                InstanShiBingNum = 5;
            }
        }
        return randEntity;
    }

}
[BurstCompile]
public partial struct PutUpShiBingJob : IJobEntity
{
    public float time;
    public int rand;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public Spawn spawn;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<UpSkill> upSkill;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    void Execute(Entity entity,ref PutUpShiBing PUshiBing, MyLayerAspects layerAsp, [ChunkIndexInQuery] int ChunkIndex)
    {
        PUshiBing.Cur_PutUpTime -= time;
        if (PUshiBing.Cur_PutUpTime > 0)
            return;

        PUshiBing.Cur_PutUpTime = PUshiBing.PutUpTime;

        if (!transform.TryGetComponent(PUshiBing.GroundSpawnPoint, out LocalTransform ltf) ||
           !transform.TryGetComponent(PUshiBing.AirSpawnPoint, out LocalTransform ltf1))
            return;

        Entity InstanShiBing = Entity.Null;
        int InstanShiBingNum = 0;
        Entity enemyjidi = Entity.Null;

        //如果有指定制造的士兵
        if (PUshiBing.SpecifyShiBing != Entity.Null && shibing.TryGetComponent(PUshiBing.SpecifyShiBing, out ShiBing sb))
            InstanShiBing = PUshiBing.SpecifyShiBing;
        else
        {
            InstanShiBing = RandShiBing(rand, in spawn, layerAsp, ref InstanShiBingNum);
            PUshiBing.PutUpNum = InstanShiBingNum;
        }



        if (InstanShiBing == Entity.Null)
            return;

        for (int i = 0; i < PUshiBing.PutUpNum; ++i)
        {
            var InsShiBing = ECB.Instantiate(ChunkIndex, InstanShiBing);
            if (layerAsp.BelongsTo == layer.Team1)
                enemyjidi = spawn.JiDi_Team2;
            else if (layerAsp.BelongsTo == layer.Team2)
                enemyjidi = spawn.JiDi_Team1;
            Entity spawnPoint = PUshiBing.GroundSpawnPoint;//出生在地面还是空中
            if (rand < 10)//小于10的单位是两个飞行单位
                spawnPoint = PUshiBing.AirSpawnPoint;
            ECB.AddComponent(ChunkIndex, InsShiBing, new LocalTransform
            {
                Position = LocaltoWorld[spawnPoint].Position,
                Rotation = LocaltoWorld[spawnPoint].Rotation,
                Scale = 1,//transform[InstanShiBing].Scale,
            });
            ECB.AddComponent(ChunkIndex, InsShiBing, new ShiBingChange
            {
                Act = ActState.Idle,
                enemyJiDi = enemyjidi,
            });
            ECB.AddComponent(ChunkIndex, InsShiBing, new Idle());
            ECB.SetComponentEnabled<Idle>(ChunkIndex, InsShiBing, false);
            ECB.AddComponent(ChunkIndex, InsShiBing, new Walk());
            ECB.SetComponentEnabled<Walk>(ChunkIndex, InsShiBing, true);
            ECB.AddComponent(ChunkIndex, InsShiBing, new Move());
            ECB.SetComponentEnabled<Move>(ChunkIndex, InsShiBing, false);
            ECB.AddComponent(ChunkIndex, InsShiBing, new Fire());
            ECB.SetComponentEnabled<Fire>(ChunkIndex, InsShiBing, false);

        }

    }

    Entity RandShiBing(int rand,in Spawn spawn, MyLayerAspects layerAsp, ref int InstanShiBingNum)
    {
        Entity randEntity = Entity.Null;

        if(layerAsp.BelongsTo == layer.Team1)
        {
            if (rand >= 0 && rand < 5)
            {
                randEntity = spawn.Team1_BingFeng;
                InstanShiBingNum = 5;
            }
            else if (rand >= 5 && rand < 10)
            {
                randEntity = spawn.Team1_FengHuang;
                InstanShiBingNum = 2;
            }
            else if (rand >= 10 && rand < 15)
            {
                randEntity = spawn.Team1_HaiKe;
                InstanShiBingNum = 1;
            }
            else if (rand >= 20 && rand < 25)
            {
                randEntity = spawn.Team1_PaChong;
                InstanShiBingNum = 10;
            }
            else if (rand >= 25 && rand < 30)
            {
                randEntity = spawn.Team1_HuGuang;
                InstanShiBingNum = 5;
            }
            else if (rand >= 30 && rand < 35)
            {
                randEntity = spawn.Team1_YeMa;
                InstanShiBingNum = 10;
            }
            else if (rand >= 35 && rand < 40)
            {
                randEntity = spawn.Team1_GangQiu;
                InstanShiBingNum = 5;
            }
            else if (rand >= 40 && rand < 45)
            {
                InstanShiBingNum = 3;
                randEntity = spawn.Team1_BaoYu;
            }
            else if (rand >= 45 && rand < 50)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_BaZhu;
            }
            else if (rand >= 50 && rand < 55)
            {
                randEntity = spawn.Team1_TieChui;
                InstanShiBingNum = 5;
            }
            else if (rand >= 55 && rand < 60)
            {
                randEntity = spawn.Team1_BaoLei;
                InstanShiBingNum = 1;
            }
            else if (rand >= 60 && rand < 65)//最后三个单位为obj的身体
            {
                randEntity = spawn.Team1_XiNiu;
                InstanShiBingNum = 3;
            }
            else if (rand >= 65 && rand < 70)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_HuoShen;
            }
            else if (rand >= 70 && rand < 75)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team1_RongDian;
            }
            else if (rand >= 75 && rand < 80)
            {
                randEntity = spawn.Team1_JianYa;
                InstanShiBingNum = 10;
            }
            else if (rand <= 80 && rand < 85)
            {
                randEntity = spawn.Team1_ChangGong;
                InstanShiBingNum = 5;
            }
        }
        else if(layerAsp.BelongsTo == layer.Team2)
        {

            if (rand >= 0 && rand < 5)
            {
                randEntity = spawn.Team2_BingFeng;
                InstanShiBingNum = 5;
            }
            else if (rand >= 5 && rand < 10)
            {
                randEntity = spawn.Team2_FengHuang;
                InstanShiBingNum = 2;
            }
            else if (rand >= 10 && rand < 15)
            {
                randEntity = spawn.Team2_HaiKe;
                InstanShiBingNum = 1;
            }
            else if (rand >= 20 && rand < 25)
            {
                InstanShiBingNum = 10;
                randEntity = spawn.Team2_PaChong;
            }
            else if (rand >= 25 && rand < 30)
            {
                randEntity = spawn.Team2_HuGuang;
                InstanShiBingNum = 5;
            }
            else if (rand >= 30 && rand < 35)
            {
                randEntity = spawn.Team2_YeMa;
                InstanShiBingNum = 10;
            }
            else if (rand >= 35 && rand < 40)
            {
                randEntity = spawn.Team2_GangQiu;
                InstanShiBingNum = 5;
            }
            else if (rand >= 40 && rand < 45)
            {
                randEntity = spawn.Team2_BaoYu;
                InstanShiBingNum = 3;
            }
            else if (rand >= 45 && rand < 50)
            {
                randEntity = spawn.Team2_BaZhu;
                InstanShiBingNum = 1;
            }
            else if (rand >= 50 && rand < 55)
            {
                randEntity = spawn.Team2_TieChui;
                InstanShiBingNum = 5;
            }
            else if (rand >= 55 && rand < 60)
            {
                randEntity = spawn.Team2_BaoLei;
                InstanShiBingNum = 1;
            }
            else if (rand >= 60 && rand < 65)//最后三个单位为obj的身体
            {
                randEntity = spawn.Team2_XiNiu;
                InstanShiBingNum = 3;
            }
            else if (rand >= 65 && rand < 70)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team2_HuoShen;
            }
            else if (rand >= 70 && rand < 75)
            {
                InstanShiBingNum = 1;
                randEntity = spawn.Team2_RongDian;
            }
            else if (rand >= 75 && rand < 80)
            {
                InstanShiBingNum = 10;
                randEntity = spawn.Team2_JianYa;
            }
            else if (rand <= 80 && rand < 85)
            {
                randEntity = spawn.Team2_ChangGong;
                InstanShiBingNum = 5;
            }
        }
        return randEntity;
    }


}
