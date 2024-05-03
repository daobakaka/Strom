using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
public partial class RotateTiresSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<ShiBingChange> m_shibingCh;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<EntityCtrl> m_entiCtrl;
    ComponentLookup<Die> m_Die;
    ComponentLookup<SX> m_SX;

    void UpDateComponentLookup()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_entiCtrl.Update(this);
        m_Die.Update(this);
        m_SX.Update(this);
        m_shibing.Update(this);
        m_shibingCh.Update(this);
    }
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_Die = GetComponentLookup<Die>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_shibingCh = GetComponentLookup<ShiBingChange>(true);
    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        var rotateTiresjob = new RotateTiresJob
        {
            ECB = ecb,
            transfrom = m_transfrom,
            localtoWorld = m_LocaltoWorld,
            entiCtrl = m_entiCtrl,
            shibing = m_shibing,
            shibingCha = m_shibingCh,
            sx = m_SX,
            time = SystemAPI.Time.DeltaTime,
        };
        Dependency = rotateTiresjob.Schedule(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

[BurstCompile]
public partial struct RotateTiresJob : IJobEntity
{
    public float time;
    public EntityCommandBuffer ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transfrom;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> localtoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<ShiBing> shibing;
    [Unity.Collections.ReadOnly] public ComponentLookup<ShiBingChange> shibingCha;
    [Unity.Collections.ReadOnly] public ComponentLookup<SX> sx;

    void Execute(Entity entity, RotateTires rotate)
    {
        if (!shibing.TryGetComponent(rotate.Owner, out ShiBing sb) ||
            !sx.TryGetComponent(rotate.Owner, out SX s))
            return;
        //如果不是移动状态就退出
        if (shibingCha[rotate.Owner].Act == ActState.Fire || shibingCha[rotate.Owner].Act == ActState.Ready)
            return;

        // 获取当前旋转
        LocalTransform localTransform = transfrom[entity];
        // 计算增量旋转
        quaternion incrementalRotation = quaternion.AxisAngle(new float3(1, 0, 0), sx[rotate.Owner].Speed * 0.1f * time);
        // 应用旋转
        localTransform.Rotation = math.mul(localTransform.Rotation, incrementalRotation);
        ECB.SetComponent(entity, localTransform);

    }
}