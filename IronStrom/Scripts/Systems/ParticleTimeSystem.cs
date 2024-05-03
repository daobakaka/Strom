using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
public partial class ParticleTimeSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    protected override void OnCreate()
    {
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
    }

    protected override void OnUpdate()
    {
        m_transform.Update(this);
        m_LocaltoWorld.Update(this);



        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var particlejob = new ParticleJob
        {
            ECB = ecb.AsParallelWriter(),
            time = SystemAPI.Time.DeltaTime,
            transform = m_transform,
            LocaltoWorld = m_LocaltoWorld,
        };
        Dependency = particlejob.ScheduleParallel(Dependency);


        Dependency.Complete();//等待和确认某个依赖关系（JobHandle）已经完成
        ecb.Playback(EntityManager);
        ecb.Dispose();

    }
}

[BurstCompile]
partial struct ParticleJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public float time;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    void Execute(Entity entity, ref ParticleComponent par, [ChunkIndexInQuery] int IndexChunk)
    {
        if(!par.Is_NoDieTime)//是否倒计时，true为不倒计时，false为倒计时
            par.DieTime -= time;
        if (par.DieTime <= 0)
        {
            ECB.DestroyEntity(IndexChunk, entity);
            return;
        }



        //特效是否要跟随目标
        if (par.Is_Follow)
        {
            if (!transform.TryGetComponent(par.FollowPoint, out LocalTransform ltf))//如果跟随的点死亡
            {
                ECB.DestroyEntity(IndexChunk, entity);
                return;
            }
            //修正自己和跟随人的位置
            var FollowPointTransform = LocaltoWorld[par.FollowPoint];
            ECB.SetComponent(IndexChunk, entity, new LocalTransform
            {
                Position = FollowPointTransform.Position,
                Rotation = FollowPointTransform.Rotation,
                Scale = par.ParScale,
            });
        }
        if(par.Is_CasterDead)
        {
            if (!transform.TryGetComponent(par.Caster, out LocalTransform ltf))//如果施法者死亡
            {
                ECB.DestroyEntity(IndexChunk, entity);
                return;
            }
        }

    }
}


