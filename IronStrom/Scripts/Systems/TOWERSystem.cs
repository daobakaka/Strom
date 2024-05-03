using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using TMPro;

[BurstCompile]
[UpdateBefore(typeof(DetectionSystem))]//�ڸ�System֮ǰִ��
public partial class TOWERSystem : SystemBase
{
    ComponentLookup<ShiBingChange> m_ShiBingChange;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;

    protected override void OnCreate()
    {
        m_ShiBingChange = GetComponentLookup<ShiBingChange>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
    }

    protected override void OnUpdate()
    {
        m_ShiBingChange.Update(this);
        m_ShiBing.Update(this);
        m_transform.Update(this);
        m_LocaltoWorld.Update(this);


        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var towerJob = new TOWERJob
        {
            time = SystemAPI.Time.DeltaTime,
            ECB = ecb.AsParallelWriter(),
            shibingChange = m_ShiBingChange,
            shibing = m_ShiBing,
            transform = m_transform,
            LocaltoWorld = m_LocaltoWorld,
        };

        Dependency = towerJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

    }
}

[BurstCompile]
public partial struct TOWERJob : IJobEntity
{
    public float time;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<ShiBingChange> shibingChange;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;

    void Execute(Entity entity, ref TOWER tower, [ChunkIndexInQuery] int ChunkIndex)
    {
        if(!tower.Is_HaveActComponent)
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
            tower.Is_HaveActComponent = true;
        }

        // ��������̧�����µĽǶ�
        var shootEnti = shibing[entity].ShootEntity;

        if (!transform.TryGetComponent(tower.GunBarrel, out LocalTransform ltf) ||
            !transform.TryGetComponent(shootEnti, out LocalTransform ltf1))
            return;

        // ��ȡ�ڹܺ�Ŀ���λ��
        var gunBarrelPosition = LocaltoWorld[tower.GunBarrel].Position;
        var targetPosition = LocaltoWorld[shootEnti].Position;
 
        // ���㷽������
        var direction = targetPosition - gunBarrelPosition;

        // ����ˮƽ�ʹ�ֱ����
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
        float verticalDistance = direction.y;

        // ����Ƕ�
        float angle = -Mathf.Atan2(verticalDistance, horizontalDistance) * Mathf.Rad2Deg;

        // ����Ŀ����ת
        quaternion targetRotation = quaternion.Euler(math.radians(angle), 0, 0);

        // Ӧ����ת
        var gunBarrelTranf = transform[tower.GunBarrel];
        gunBarrelTranf.Rotation = math.slerp(gunBarrelTranf.Rotation, targetRotation, 1f);
        
        ECB.SetComponent(ChunkIndex, tower.GunBarrel, gunBarrelTranf);
    }
}