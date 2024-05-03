using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
[UpdateAfter(typeof(DirectShootSystem))]//�ڸ�System֮��ִ��
[UpdateAfter(typeof(bulletSystem))]//�ڸ�System֮��ִ��
[UpdateAfter(typeof(AttackBoxSystem))]//�ڸ�System֮��ִ��
public partial class ShieldSystem : SystemBase
{
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<InShield> m_InShield;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<JiDi> m_JiDi;

    void UpdateComponentLookup()
    {
        m_LocaltoWorld.Update(this);
        m_transform.Update(this);
        m_InShield.Update(this);
        m_ShiBing.Update(this);
        m_JiDi.Update(this);
    }

    protected override void OnCreate()
    {
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_InShield = GetComponentLookup<InShield>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);



    }
    protected override void OnUpdate()
    {

        //Entities.ForEach((Entity entity, Shield shield) =>
        //{
        //    if(EntityManager.HasComponent<Parent>(entity))
        //    {
        //        if (EntityManager.GetComponentData<Parent>(entity).Value == shield.ShieldParent)
        //            return;
        //    }
        //    EntityManager.AddComponentData(entity, new Parent { Value = shield.ShieldParent });
        //}).WithBurst().WithStructuralChanges().Run();

        UpdateComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        //ÿ�����ܵ����ܱ�����ȫ���رջ��ܱ�������ShieldJob�ڿ�Ҫ��˭
        Entities.ForEach((Entity entity, InShield inshield) =>
        {
            EntityManager.SetComponentEnabled<InShield>(entity, false);
        }).WithBurst().WithStructuralChanges().Run();


        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//����һ����ײ�����Ҫ�ĵ���
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbparallel = ecb.AsParallelWriter();

        var addshieldJob = new AddShieldJob
        {
            ECB = ecbparallel,
            spawn = spawn,
            LocaltoWorld = m_LocaltoWorld,
            transform = m_transform,
            time = SystemAPI.Time.DeltaTime,
            shibing = m_ShiBing,
            jidi = m_JiDi,
        };
        Dependency = addshieldJob.ScheduleParallel(Dependency);

        //var closeshieldJob = new CloseShieldJob
        //{
        //    ECB = ecbparallel,
        //};
        //Dependency = closeshieldJob.ScheduleParallel(Dependency);

        var shieldJob = new ShieldJob
        {
            spawn = spawn,
            ECB = ecbparallel,
            physicsWorld = physicsWorld,
            LocaltoWorld = m_LocaltoWorld,
            transform  = m_transform,
            inshield = m_InShield,
            time = SystemAPI.Time.DeltaTime,
        };
        Dependency = shieldJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();


        var entiUIMager = EntityUIManager.Instance;
        if (entiUIMager == null) return;
        //Ϊӵ��AddShieldSlider��Entity��ӻ���ֵUI
        Entities.ForEach((Entity entity, AddShieldSlider addshieldslider, Shield shield) =>
        {
            entiUIMager.AddShieldSlider(entity, shield, EntityManager);

            EntityManager.RemoveComponent<AddShieldSlider>(entity);
        }).WithBurst().WithStructuralChanges().Run();

    }
}
[BurstCompile]
public partial struct AddShieldJob : IJobEntity//������Ҫ��ӻ��ܵĵ�λ��ӻ���
{
    public float time;
    public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<JiDi> jidi;
    void Execute(Entity entity, AddShield addshield,in MyLayer mylayer, [ChunkIndexInQuery] int ChunkIdex)
    {
        Entity InstanEntity = Entity.Null;
        if (mylayer.BelongsTo == layer.Team1)
        {
            switch (shibing[entity].Name)
            {
                case ShiBingName.HaiKe: InstanEntity = spawn.HaiKe_Shield_1; break;
                case ShiBingName.JiDi: InstanEntity = spawn.JiDi_Shield_1; break;
            }
        }
        else if (mylayer.BelongsTo == layer.Team2)
        {
            switch (shibing[entity].Name)
            {
                case ShiBingName.HaiKe: InstanEntity = spawn.HaiKe_Shield_2; break;
                case ShiBingName.JiDi: InstanEntity = spawn.JiDi_Shield_2; break;
            }
        }

        if (!transform.TryGetComponent(InstanEntity, out LocalTransform ltf) ||
            !transform.TryGetComponent(addshield.ShieldParent, out LocalTransform ltf1))
            return;

        var shield = ECB.Instantiate(ChunkIdex, InstanEntity);
        ECB.SetComponent(ChunkIdex, shield, new LocalTransform
        {
            Position = LocaltoWorld[addshield.ShieldParent].Position,
            Rotation = LocaltoWorld[addshield.ShieldParent].Rotation,
            Scale = 1,
        });
        ECB.SetComponent(ChunkIdex, shield, new Shield
        {
            ShieldParent = addshield.ShieldParent,
            ShieldExpandSpeed = addshield.ShieldExpandSpeed,
            ShieldScale = addshield.ShieldScale,
        });
        ECB.SetComponent(ChunkIdex, shield, new SX
        {
            HP = addshield.ShieldHP,
            Cur_HP = addshield.ShieldHP,
            VolumetricDistance = (addshield.ShieldScale + addshield.ShieldScale * 0.1f)/2,
        });

        ECB.RemoveComponent<AddShield>(ChunkIdex, entity);

        //���Ҫ��ӻ��ܵ��ǻ��صĻ����û����õ��Լ����ܵ�Entity
        if(jidi.TryGetComponent(entity,out JiDi jd))
        {
            var jidiCompt = jidi[entity];
            jidiCompt.JidiShield = shield;
            ECB.SetComponent(ChunkIdex, entity, jidiCompt);
            //�ǻ��صĻ���Ϊ������ػ�����ӻ�����
            ECB.AddComponent(ChunkIdex, shield, new AddShieldSlider());
        }

    }
}

[BurstCompile]
public partial struct CloseShieldJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    void Execute(Entity entity, InShield inshield, [ChunkIndexInQuery] int ChunkIdex)
    {
        ECB.SetComponentEnabled<InShield>(ChunkIdex, entity, false);
    }
}

//[BurstCompile]
public partial struct ShieldJob : IJobEntity
{
    public float time;
    public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public PhysicsWorldSingleton physicsWorld;//���Ǿ�̬������Ҫ���ⲿ������
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<InShield> inshield;
    void Execute(Entity entity, Shield shield, in MyLayer mylayer, [ChunkIndexInQuery]int ChunkIndex)
    {
        if (!LocaltoWorld.TryGetComponent(shield.ShieldParent, out LocalToWorld ltw))
            return;

        var shieldTransform = transform[entity];
        shieldTransform.Position = LocaltoWorld[shield.ShieldParent].Position;
        shieldTransform.Rotation = LocaltoWorld[shield.ShieldParent].Rotation;
        if(shieldTransform.Scale < shield.ShieldScale)//���ܱ��
            shieldTransform.Scale += shield.ShieldExpandSpeed * time;
        //������ͬ���Լ���λ��
        ECB.SetComponent(ChunkIndex, entity, shieldTransform);

        //����Լ����ֵ��ѷ���λ��Ϊ���ṩ�ӻ�
        NativeList<ColliderCastHit> outHitsAttack = new NativeList<ColliderCastHit>(Allocator.Temp);
        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)mylayer.BelongsTo),
            CollidesWith = (1u << (int)mylayer.CollidesWith_1),
            GroupIndex = 0
        };
        bool b_SphereCastAll = physicsWorld.SphereCastAll(LocaltoWorld[entity].Position, transform[entity].Scale/2 , Vector3.up, 1, ref outHitsAttack, filterDetection);
        
        if(b_SphereCastAll)//�п��ܱ���
        {
            foreach(var Hit in outHitsAttack)
            {
                if (!transform.TryGetComponent(Hit.Entity, out LocalTransform ltf))
                    return;
                if (!inshield.TryGetComponent(Hit.Entity,out InShield inshie))
                    ECB.AddComponent(ChunkIndex, Hit.Entity, new InShield());

                ECB.SetComponentEnabled<InShield>(ChunkIndex, Hit.Entity, true);
            }
        }

        //Debug.Log("  ���ܼ�ⷶΧ��" + transform[entity].Scale);
        //var fanwei = ECB.Instantiate(ChunkIndex, spawn.TieChui_Muzzle_1);
        //ECB.SetComponent(ChunkIndex, fanwei, new LocalTransform
        //{
        //    Position = transform[entity].Position,
        //    Rotation = transform[entity].Rotation,
        //    Scale = transform[entity].Scale,
        //});

        outHitsAttack.Dispose();
    }
}
