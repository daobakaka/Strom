using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using System.Net.NetworkInformation;

[BurstCompile]
public partial class RebirthSystem : SystemBase
{
    ComponentLookup<FengHuang> m_FengHuang;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<SX> m_SX;
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<RebirthChange> m_RebirthChange;
    ComponentLookup<UpSkill> m_UpSkill;

    void UpDateComponentLookup()
    {
        m_FengHuang.Update(this);
        m_LocaltoWorld.Update(this);
        m_transform.Update(this);
        m_SX.Update(this);
        m_MyLayer.Update(this);
        m_RebirthChange.Update(this);
        m_UpSkill.Update(this);
    }

    protected override void OnCreate()
    {
        m_FengHuang = GetComponentLookup<FengHuang>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_RebirthChange = GetComponentLookup<RebirthChange>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
    }

    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var rebirthJob = new RebirthJob
        {
            ECB = ecb.AsParallelWriter(),
            time = SystemAPI.Time.DeltaTime,
            fenghuang = m_FengHuang,
            LocaltoWorld = m_LocaltoWorld,
            transform = m_transform,
            sx = m_SX,
            myLayer = m_MyLayer,
            spawn = spawn,
            rebirthChange = m_RebirthChange,
            Upskill = m_UpSkill,
        };
        Dependency = rebirthJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        //获得全部已方凤凰
        List<Entity> FengHuangEntityTeam1 = new List<Entity>();
        List<Entity> FengHuangEntityTeam2 = new List<Entity>();
        Entities.ForEach((Entity entity, FengHuang fenghuang, MyLayer mylayer) =>
        {
            if (EntityManager.HasComponent<Die>(entity))
                return;

            if(mylayer.BelongsTo == layer.Team1)
                FengHuangEntityTeam1.Add(entity);
            else if(mylayer.BelongsTo == layer.Team2)
                FengHuangEntityTeam2.Add(entity);

        }).WithBurst().WithStructuralChanges().Run();
        //跟随离自己最近的凤凰
        Entities.ForEach((Entity entity, MyLayer mylayer, ref Rebirth rebirth) =>
        {
            if (EntityManager.Exists(rebirth.FollowEntity))//有跟随的目标就不要再算了
                return;

            UpDateComponentLookup();
            if (mylayer.BelongsTo == layer.Team1)
                CountDistance(entity, ref FengHuangEntityTeam1, ref rebirth);
            else if (mylayer.BelongsTo == layer.Team2)
                CountDistance(entity, ref FengHuangEntityTeam2, ref rebirth);

        }).WithBurst().WithStructuralChanges().Run();

    }
    //计算距离
    void CountDistance(Entity entity, ref List<Entity> FHEntity, ref Rebirth rebirth)
    {
        if (FHEntity.Count <= 0)
            return;
        Entity closestEntity = Entity.Null;
        float minDistance = float.MaxValue;
        
        foreach (Entity enti in FHEntity)
        {
            float distance = math.distance(m_LocaltoWorld[enti].Position, m_LocaltoWorld[entity].Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEntity = enti;
            }
        }
        rebirth.FollowEntity = m_FengHuang[closestEntity].FollowPoint;
        float offx = UnityEngine.Random.Range(5f, -5f);
        float offz = UnityEngine.Random.Range(5f, -5f);

        rebirth.FollowOffDistance = new float3(offx, 0, offz);
    }

}

public partial struct RebirthJob : IJobEntity
{
    public float time;
    [ReadOnly] public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<FengHuang> fenghuang;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public ComponentLookup<RebirthChange> rebirthChange;
    [ReadOnly] public ComponentLookup<UpSkill> Upskill;
    void Execute(Entity entity,ref Rebirth rebirth, ShiBing shibing, [ChunkIndexInQuery] int ChunkIndex)
    {
        if (transform.TryGetComponent(rebirth.FollowEntity, out LocalTransform ltf))//没有跟随点就退出
        {
            if (math.distance(LocaltoWorld[entity].Position, LocaltoWorld[rebirth.FollowEntity].Position + rebirth.FollowOffDistance) > 5 &&
                rebirth.RebirthTime > 0)
            {
                //修正自己和跟随点的坐标
                float3 vdir = (LocaltoWorld[rebirth.FollowEntity].Position + rebirth.FollowOffDistance) - LocaltoWorld[entity].Position;
                float3 CheShendir = math.normalize(vdir);
                //CheShendir.y = 0;
                var worldPos = transform[entity];
                quaternion targetRotation = quaternion.LookRotationSafe(CheShendir, new float3(0, 1, 0));
                worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转
                                                                                             //车身移动
                worldPos.Position += sx[entity].Speed * CheShendir * time;
                //worldPos.Position.y = 0;

                ECB.SetComponent(ChunkIndex, entity, worldPos);
            }
            else if (rebirth.RebirthTime > 0)
            {
                ECB.SetComponent(ChunkIndex, entity, new LocalTransform
                {
                    Position = LocaltoWorld[rebirth.FollowEntity].Position + rebirth.FollowOffDistance,
                    Rotation = LocaltoWorld[rebirth.FollowEntity].Rotation,
                    Scale = 1,
                });
            }
        }

        //倒计时复活时间
        rebirth.RebirthTime -= time;
        if(rebirth.RebirthTime <= -3)
        {
            if (!transform.TryGetComponent(rebirth.RebirthEntity, out LocalTransform ltf2))
                return;

            var rebirthEnti = ECB.Instantiate(ChunkIndex, rebirth.RebirthEntity);
            ECB.SetComponent(ChunkIndex, rebirthEnti, new LocalTransform
            {
                Position = transform[entity].Position,
                Rotation = transform[entity].Rotation,
                Scale = 1,
            });

            var enemyjidi = Entity.Null;
            if (myLayer[entity].BelongsTo == layer.Team1)
                enemyjidi = spawn.JiDi_Team2;
            else if (myLayer[entity].BelongsTo == layer.Team2)
                enemyjidi = spawn.JiDi_Team1;

            ECB.AddComponent(ChunkIndex, rebirthEnti, new ShiBingChange
            {
                Act = ActState.Idle,
                enemyJiDi = enemyjidi,
            });
            //添加指挥官命令组件
            ECB.AddComponent(ChunkIndex, rebirthEnti, new BarrageCommand
            {
                command = commandType.NUll,
                Comd_ShootEntity = Entity.Null,
            });
            //添加积分组件
            ECB.AddComponent(ChunkIndex, rebirthEnti, new Integral
            {
                ATIntegral = 0,
                AttackMeEntity = Entity.Null,
            });
            //让士兵获得玩家的EntityID
            ECB.AddComponent(ChunkIndex, rebirthEnti, new EntityOpenID
            {
                PlayerEntiyID = rebirthChange[entity].PlayerEntityID,
            });
            ECB.AddComponent(ChunkIndex, rebirthEnti, new Idle());
            ECB.SetComponentEnabled<Idle>(ChunkIndex, rebirthEnti, false);
            ECB.AddComponent(ChunkIndex, rebirthEnti, new Walk());
            ECB.SetComponentEnabled<Walk>(ChunkIndex, rebirthEnti, true);
            ECB.AddComponent(ChunkIndex, rebirthEnti, new Move());
            ECB.SetComponentEnabled<Move>(ChunkIndex, rebirthEnti, false);
            ECB.AddComponent(ChunkIndex, rebirthEnti, new Fire());
            ECB.SetComponentEnabled<Fire>(ChunkIndex, rebirthEnti, false);

            //重生前是否已升级
            if (rebirthChange[entity].Is_UpSkill)
            {
                ECB.SetComponent(ChunkIndex, rebirthEnti, new UpSkill
                {
                    Is_UpSkill = rebirthChange[entity].Is_UpSkill,
                    upSkill_Name = rebirth.upSkill_Name,
                });
            }

            ECB.DestroyEntity(ChunkIndex, entity);
        }
        else if (rebirth.RebirthTime <= 0 && rebirth.Is_HaveRebirthParticle == false)
        {
            //重生特效
            rebirth.Is_HaveRebirthParticle = true;
            var rebirthPar = ECB.Instantiate(ChunkIndex, rebirth.RebirthParticle);
            ECB.SetComponent(ChunkIndex, rebirthPar, new LocalTransform
            {
                Position = transform[entity].Position,
                Rotation = transform[entity].Rotation,
                Scale = 1,
            });
        }

    }

}