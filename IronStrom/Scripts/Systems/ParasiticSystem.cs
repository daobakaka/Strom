using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.VisualScripting;
[BurstCompile]
[UpdateBefore(typeof(DetectionSystem))] //在个System之前执行
public partial class ParasiticSystem : SystemBase
{
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<ShiBingChange> m_ShiBingChange;
    ComponentLookup<SX> m_SX;

    protected override void OnCreate()
    {
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_ShiBingChange = GetComponentLookup<ShiBingChange>(true);
        m_SX = GetComponentLookup<SX>(true);

    }
    protected override void OnUpdate()
    {
        m_MyLayer.Update(this);
        m_ShiBing.Update(this);
        m_ShiBingChange.Update(this);
        m_SX.Update(this);


        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parasJob = new ParasiticJob
        {
            ECB = ecb.AsParallelWriter(),
            myLayer = m_MyLayer,
            shibing = m_ShiBing,
            shibingChange = m_ShiBingChange,
            sx = m_SX,
        };
        Dependency = parasJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

}

[BurstCompile]
public partial struct ParasiticJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<ShiBingChange> shibingChange;
    [ReadOnly] public ComponentLookup<SX> sx;

    void Execute(Entity entity,ref Parasitic paras, [ChunkIndexInQuery] int ChunkIndex)
    {
        if(!paras.Is_HaveActComponent)
        {
            ECB.AddComponent(ChunkIndex, entity, new Idle());
            ECB.SetComponentEnabled<Idle>(ChunkIndex, entity, false);
            ECB.AddComponent(ChunkIndex, entity, new Walk());
            ECB.SetComponentEnabled<Walk>(ChunkIndex, entity, false);
            ECB.AddComponent(ChunkIndex, entity, new Move());
            ECB.SetComponentEnabled<Move>(ChunkIndex, entity, false);
            ECB.AddComponent(ChunkIndex, entity, new Fire());
            ECB.SetComponentEnabled<Fire>(ChunkIndex, entity, false);
            var sbChange = shibingChange[entity];
            sbChange.Act = ActState.Idle;
            ECB.SetComponent(ChunkIndex, entity, sbChange);
            paras.Is_HaveActComponent = true;
        }

        if (shibingChange[paras.Owner].Act == ActState.NotMove && shibingChange[entity].Act != ActState.NotMove)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, entity, true);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Move>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, entity, false);

            var shibingEnti = shibing[entity];
            shibingEnti.TarEntity = Entity.Null;
            shibingEnti.ShootEntity = Entity.Null;
            ECB.SetComponent(ChunkIndex, entity, shibingEnti);

            var sbChange = shibingChange[entity];
            sbChange.Act = ActState.NotMove;
            ECB.SetComponent(ChunkIndex, entity, sbChange);
        }
        else if (shibingChange[paras.Owner].Act != ActState.NotMove && shibingChange[entity].Act == ActState.NotMove)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, entity, true);
            ECB.SetComponentEnabled<Move>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, entity, false);

            var sbChange = shibingChange[entity];
            sbChange.Act = ActState.Idle;
            ECB.SetComponent(ChunkIndex, entity, sbChange);
        }

        var SXEnti = sx[entity];
        SXEnti.AT = sx[paras.Owner].AT;
        SXEnti.Tardis = sx[paras.Owner].Tardis;
        SXEnti.Shootdis = sx[paras.Owner].Shootdis;
        SXEnti.ShootTime = sx[paras.Owner].ShootTime;
        ECB.SetComponent(ChunkIndex, entity, SXEnti);
        //ECB.RemoveComponent<Parasitic>(ChunkIndex, entity);
    }

}