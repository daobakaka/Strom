using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using System.Buffers;
using Unity.Mathematics;

[BurstCompile]
public partial class CorrectPositionSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<ShiBingChange> m_shibingChange;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<JiDi> m_JiDi;
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_shibingChange = GetComponentLookup<ShiBingChange>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
    }

    protected override void OnUpdate()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_shibingChange.Update(this);
        m_shibing.Update(this);
        m_JiDi.Update(this);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var corrPosjob = new CorrectPositionJob
        {
            ECB = ecb.AsParallelWriter(),
            transform = m_transfrom,
            LocalwoWorld = m_LocaltoWorld,
            shibingChange = m_shibingChange,
            shibing = m_shibing,
            time = SystemAPI.Time.DeltaTime,
            jidi = m_JiDi,
        };
        Dependency = corrPosjob.ScheduleParallel(Dependency);

        Dependency.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

}

public partial struct CorrectPositionJob : IJobEntity
{
    public float time;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocalwoWorld;
    [ReadOnly] public ComponentLookup<ShiBingChange> shibingChange;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<JiDi> jidi;
    void Execute(Entity entity, CorrectPosition CorrPos, [ChunkIndexInQuery] int ChunkIndex)
    {
        //拥有者死亡，或者拥有者发来死亡通讯 都删除自己
        if (!transform.TryGetComponent(CorrPos.Owner, out LocalTransform ltf) ||
            !transform.TryGetComponent(shibing[CorrPos.Owner].FirePoint_R, out LocalTransform ltf1))
        {
            ECB.AddComponent(ChunkIndex, entity, new Die());
            return;
        }
        if (shibing[CorrPos.Owner].CorrectPosition_IsDie)
        {
            var shib = shibing[CorrPos.Owner];
            shib.CorrectPosEntity = Entity.Null;
            ECB.SetComponent(ChunkIndex, CorrPos.Owner, shib);
            ECB.AddComponent(ChunkIndex, entity, new Die());
            return;
        }
        var ShootEnti = shibing[CorrPos.Owner].ShootEntity;
        if (!transform.TryGetComponent(ShootEnti, out LocalTransform ltf2))
            return;

        if (jidi.TryGetComponent(ShootEnti,out JiDi jd))//如果是基地，目标就是选择后的基地点
        {
            ShootEnti = shibing[CorrPos.Owner].JidiPoint;
        }
        else//如果是士兵
        {
            ShootEnti = shibing[ShootEnti].CenterPoint;
        }
        if (!transform.TryGetComponent(ShootEnti, out LocalTransform ltf11))
            return;
        float3 direnPos = LocalwoWorld[ShootEnti].Position;
        float3 Pos = LocalwoWorld[entity].Position;
        var vdir = direnPos - Pos;
        vdir.y = vdir.y <= 0 ? 0 : vdir.y;
        var pos = transform[entity];
        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//获得希望的面朝向
        pos.Rotation = math.slerp(pos.Rotation, targetRotation, 5f * time);// 插值旋转
        pos.Position = LocalwoWorld[shibing[CorrPos.Owner].FirePoint_R].Position;
        pos.Scale = 1;

        ECB.SetComponent(ChunkIndex, entity, pos);
    }
}