using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class bulletSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_localtoWorld;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<Die> m_die;
    ComponentLookup<SX> m_SX;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<PhysicsVelocity> m_PhysicsVelocity;
    ComponentLookup<Shield> m_Shield;

    void UpDataComponentLookup(SystemBase system)
    {
        m_transform.Update(system);
        m_localtoWorld.Update(system);
        m_shibing.Update(system);
        m_die.Update(system);
        m_SX.Update(system);
        m_UpSkill.Update(system);
        m_PhysicsVelocity.Update(system);
        m_Shield.Update(system);
    }

    protected override void OnCreate()
    {
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_localtoWorld = GetComponentLookup<LocalToWorld>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_die = GetComponentLookup<Die>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_PhysicsVelocity = GetComponentLookup<PhysicsVelocity>(true);
        m_Shield = GetComponentLookup<Shield>(true);

    }

    protected override void OnUpdate()
    {
        if (TeamManager.teamManager == null)
            return;

        UpDataComponentLookup(this);

        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        //var ecbSingLeton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        //var bulletECB = ecbSingLeton.CreateCommandBuffer(EntityManager.WorldUnmanaged);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ECB = new EntityCommandBuffer(Allocator.TempJob);
        var bulletECB = ECB.AsParallelWriter();
        var bulletJob = new DirectFiringJob
        {
            transform = m_transform,
            time = SystemAPI.Time.DeltaTime,
            ECB = bulletECB,
            physicsVelocity = m_PhysicsVelocity,
        };
        Dependency = bulletJob.ScheduleParallel(Dependency);

        var bulletbezaerJob = new BulletBezierFiringJob
        {
            ECB = bulletECB,
            time = SystemAPI.Time.DeltaTime,
            localtoWorld = m_localtoWorld,
            transform = m_transform,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue)),
        };
        Dependency = bulletbezaerJob.ScheduleParallel(Dependency);

        TeamManager.teamManager.Cur_BulletDetectionTime -= SystemAPI.Time.DeltaTime;
        if(TeamManager.teamManager.Cur_BulletDetectionTime < 0)
        {
            var bulletInjuryJob = new BulletjuryJob
            {
                ECB = bulletECB,
                transform = m_transform,
                physicsWorld = physicsWorld,
                shibing = m_shibing,
                die = m_die,
                shield = m_Shield,
            };
            Dependency = bulletInjuryJob.ScheduleParallel(Dependency);

            TeamManager.teamManager.Cur_BulletDetectionTime = TeamManager.teamManager.BulletDetectionTime;
        }
        


        var bulletisDieJob = new BulletIsDieJob
        {
            ECB = bulletECB,
            die = m_die,
        };
        Dependency = bulletisDieJob.ScheduleParallel(Dependency);

        Dependency.Complete();
        ECB.Playback(EntityManager);
        ECB.Dispose();


        if (!SystemAPI.HasSingleton<Spawn>()) return;
        var spawn = SystemAPI.GetSingleton<Spawn>();

        Entities.ForEach((Entity entity, Bullet bullet,BulletChange bulletchange) =>
        {
            if (!EntityManager.Exists(bullet.TarGet))
                return;
            //�ҹ����ĵ�λû��SX��������������λ�ڻ������棬���˳�
            if (!EntityManager.HasComponent<SX>(bullet.TarGet))
                return;
            if (EntityManager.HasComponent<InShield>(bullet.TarGet))
            {
                if (EntityManager.IsComponentEnabled<InShield>(bullet.TarGet))
                    return;
            }

            if (bullet.DeadLanguage == Entity.Null && bullet.DeadLanguage2 == Entity.Null)//����ӵ�����Box�˺�����ֱ������
            {
                UpDataComponentLookup(this);
                var enemysx = m_SX[bullet.TarGet];
                //�ⷢ�ӵ���ӵ�����Ƿ����������жԿնԵ�����
                var bulletat = spawn.Is_Advantage(in bulletchange.AT, entity, in enemysx);
                //�ҵĹ�������ȥ������˵ĸ��ʣ��������ǵĹ������ֵ���ֵ
                var AT = bulletat - bulletat * enemysx.DB;
                enemysx.DP -= AT;
                if(enemysx.DP < 0)
                {
                    float at = enemysx.DP * -1;
                    enemysx.Cur_HP += enemysx.DP;
                    enemysx.DP = 0;
                    //���������ǹ���ͼ�¼����������ͳ������˺�������ù��������
                    if (EntityManager.HasComponent<Monster>(bullet.TarGet))
                    {   //����������������EntityOpenID(����ҵ�ʿ��)�ż�¼
                        if (EntityManager.HasComponent<OwnerData>(entity))
                        {
                            var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;
                            if (EntityManager.HasComponent<EntityOpenID>(shibing))
                            {
                                var EntiOpenID = EntityManager.GetComponentData<EntityOpenID>(shibing);
                                monsterMager.MonsterRecordPlayerAT(EntiOpenID, bullet.TarGet, at, EntityManager);
                            }
                        }
                    }
                }

                EntityManager.SetComponentData(bullet.TarGet, enemysx);

                //����˸����������ڴ�����������ʱ��ðѻ��ּӸ���
                if (EntityManager.HasComponent<Integral>(bullet.TarGet))
                {
                    var shootInteg = EntityManager.GetComponentData<Integral>(bullet.TarGet);
                    if (EntityManager.HasComponent<OwnerData>(entity))
                    {
                        var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;
                        shootInteg.AttackMeEntity = shibing;
                        EntityManager.SetComponentData(bullet.TarGet, shootInteg);
                    }
                }
                //�������������¼������
                if (EntityManager.HasComponent<UpSkill>(bullet.TarGet))
                {
                    if(m_UpSkill[bullet.TarGet].upSkill_Name == UpSkillName.GangQiu)
                    {
                        var upSkill = m_UpSkill[bullet.TarGet];
                        upSkill.InjuryRecord += AT;
                        EntityManager.SetComponentData(bullet.TarGet, upSkill);
                    }
                }
            }
            EntityManager.AddComponentData(entity, new Die 
            {
                DeadParticle = bullet.CannonHit,
                DeadPoint = entity,
                DeadLanguage = bullet.DeadLanguage,
                DeadLanguage2 = bullet.DeadLanguage2,
            });

        }).WithoutBurst().WithStructuralChanges().Run();

    }

}
[BurstCompile]
public partial struct DirectFiringJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public ComponentLookup<PhysicsVelocity> physicsVelocity;


    public float time;
    void Execute(BulletAspects bullet, DirectFiring dirFiring, [ChunkIndexInQuery] int chunkIndex)
    {
        if (!physicsVelocity.TryGetComponent(bullet.Bullet_Entity, out PhysicsVelocity pv))
            return;


        var BulletTransfrom = transform[bullet.Bullet_Entity];
        var BulletPos = BulletTransfrom.Position;
        if (BulletPos.x > 1000 || BulletPos.x < -1000 || BulletPos.y <= 0.5 || BulletPos.z > 1500 || BulletPos.z < -1500)
        {
            BulletTransfrom.Position.y = 1;
            ECB.SetComponent(chunkIndex, bullet.Bullet_Entity, BulletTransfrom);
            ECB.AddComponent(chunkIndex, bullet.Bullet_Entity, new Die
            {
                DeadParticle = bullet.CannonHit,
                DeadPoint = bullet.Bullet_Entity,
                DeadLanguage = bullet.DeadLanguage,
                DeadLanguage2 = bullet.DeadLanguage2,
            });
            return;
        }



        var bulletPhVe = physicsVelocity[bullet.Bullet_Entity];

        bulletPhVe.Linear = bullet.BulletDir * bullet.Speed; //bullet.Speed * bullet.BulletDir * time;

        //var pos = transform[bullet.Bullet_Entity];
        //pos.Position += bullet.Speed * bullet.BulletDir * time;
        ECB.SetComponent(chunkIndex, bullet.Bullet_Entity, bulletPhVe);


    }
}

public partial struct BulletBezierFiringJob : IJobEntity
{
    public float time;
    public Unity.Mathematics.Random random;
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> localtoWorld;
    void Execute(BulletAspects bulletAsp, BezierFiringAspects bezierFiringAsp, [ChunkIndexInQuery] int ChunkIndex)
    {
        bezierFiringAsp.ElapsedTime += time;
        float FlightDuration = bezierFiringAsp.DirenDistance / bulletAsp.Speed;
        float t = math.clamp(bezierFiringAsp.ElapsedTime / FlightDuration, 0, 1);
        //FlightDuration += 1f;
        //������Ƶ��λ��
        float3 controlPoint = bezierFiringAsp.ControlPoint;
        if (math.all(controlPoint == float3.zero))
        {
            switch (bezierFiringAsp.Is_Random)//�Ƿ�Ϊ����˷��������������
            {
                case true: bezierFiringAsp.ControlPoint = RandomCalculateControlPoint(bezierFiringAsp.StartPosition, bezierFiringAsp.EndPosition, bezierFiringAsp.LerpFactor, bezierFiringAsp.heightFactor, random); break;
                case false: bezierFiringAsp.ControlPoint = CalculateControlPoint(bezierFiringAsp.StartPosition, bezierFiringAsp.EndPosition, bezierFiringAsp.LerpFactor, bezierFiringAsp.heightFactor); break;
            }
        }
        //���㱴�������ߵ�λ��
        var bulletTransf = transform[bulletAsp.Bullet_Entity];
        bulletTransf.Position = CalculateBezierPoint(t, bezierFiringAsp.StartPosition,controlPoint, bezierFiringAsp.EndPosition);
        //�����������õ�ͷ��ת
        var tangent = CalculateBezierTangent(t, bezierFiringAsp.StartPosition, controlPoint, bezierFiringAsp.EndPosition);
        var rotaiton = quaternion.LookRotationSafe(tangent, math.up());
        bulletTransf.Rotation = rotaiton;
        ECB.SetComponent(ChunkIndex, bulletAsp.Bullet_Entity, bulletTransf);

        //���ִ��ʱ�������ʱ�����е��������
        if(bezierFiringAsp.ElapsedTime >= FlightDuration || bulletTransf.Position.y <= 0)
        {
            ECB.SetComponent(ChunkIndex, bulletAsp.Bullet_Entity, new LocalTransform
            {
                Position = transform[bulletAsp.Bullet_Entity].Position,
                Rotation = quaternion.identity,
                Scale = 1,
            });
            ECB.AddComponent(ChunkIndex, bulletAsp.Bullet_Entity, new Die
            {
                DeadParticle = bulletAsp.CannonHit,
                DeadPoint = bulletAsp.Bullet_Entity,
                DeadLanguage = bulletAsp.DeadLanguage,
                DeadLanguage2 = bulletAsp.DeadLanguage2,
            });
        }
    }
    //���㱴��������
    private float3 CalculateBezierPoint(float t, float3 p0, float3 p1, float3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        float3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
    //������Ƶ�controlPoint
    float3 CalculateControlPoint(float3 startPosition, float3 endPosition, float LerpFactor, float heightFactor)
    {
        // LerpFactor = 0.2 ��ʾ�����յ�֮��� 20%
        float3 pointOnLine = startPosition + LerpFactor * (endPosition - startPosition);
        float distance = math.distance(startPosition, endPosition);
        // �����߶�ƫ�ƻ��ھ����һ���Զ���ĸ߶�����
        float heightOffset = distance * heightFactor;
        // ��Ӹ߶�ƫ�Ƶ��е��Yֵ
        pointOnLine.y += heightOffset;
        return pointOnLine;
    }
    //����������Ƶ�controlPoint
    float3 RandomCalculateControlPoint(float3 startPosition, float3 endPosition, float lerpFactor, float heightFactor, Unity.Mathematics.Random random)
    {
        float3 midPoint = startPosition + lerpFactor * (endPosition - startPosition);
        float3 direction = math.normalize(endPosition - startPosition);
        float3 randomDirection = math.normalize(random.NextFloat3Direction());

        float3 upVector = math.cross(direction, randomDirection);

        // Ϊ�˷�ֹ��������������غϵ��²��Ϊ����������Ҫ����У��
        if (math.lengthsq(upVector) < 0.0001f)
        {
            upVector = new float3(0, 1, 0); // Ĭ�����Ϸ���
        }
        else
        {
            upVector = math.normalize(upVector);
        }

        float distance = math.distance(startPosition, endPosition);
        float3 controlPoint = midPoint + upVector * heightFactor * distance;

        return controlPoint;
    }
    //�������ߣ����Ƶ�ͷ����ת
    private float3 CalculateBezierTangent(float t, float3 p0, float3 p1, float3 p2)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }
}



[BurstCompile]
public partial struct BulletjuryJob : IJobEntity//�ӵ�����Ƿ���ײ����job
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public PhysicsWorldSingleton physicsWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<ShiBing> shibing;
    [Unity.Collections.ReadOnly] public ComponentLookup<Die> die;
    [Unity.Collections.ReadOnly] public ComponentLookup<Shield> shield;
    void Execute(BulletAspects bullet, MyLayerAspects layer, [ChunkIndexInQuery]int chunkIndex)
    {
        if (die.TryGetComponent(bullet.Bullet_Entity, out Die d))//����Ѿ�����������ˣ��Ͳ���ȥ��������ܻ���
            return;
        //if (bullet.Is_NOAttack)
        //    return;

        ColliderCastHit hit = new ColliderCastHit();

        uint collidesWithMask = 0;
        if ((int)layer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)layer.CollidesWith_1);
        if ((int)layer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)layer.CollidesWith_2);
        if ((int)layer.BulletCollidesWith != 0)//�ж������λ�Ƿ����ӵ����
            collidesWithMask |= (1u << (int)layer.BulletCollidesWith);

        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = (1u << (int)layer.BelongsTo),
            CollidesWith = collidesWithMask,
            GroupIndex = 0,
        };
        float h = bullet.Height / 2;
        float3 point1 = transform[bullet.Bullet_Entity].Position - new float3(0,1,0) * h; // ��ʵ����·�ƫ��һЩ����
        float3 point2 = transform[bullet.Bullet_Entity].Position + new float3(0,1,0) * h; // ��ʵ����Ϸ�ƫ��һЩ����

        if (physicsWorld.CapsuleCast(point1, point2, bullet.Radius, Vector3.forward, 0.3f, out hit, filter))//��������
        {
            //if (!shibing.TryGetComponent(hit.Entity, out ShiBing shib))
            //    return;
            //var hitshibing = shibing[hit.Entity];
            //hitshibing.Injuries += bullet.BulletAT;
            //ECB.SetComponent(chunkIndex, hit.Entity, hitshibing);

            if (bullet.DeadLanguage == Entity.Null)//����ⷢ�ӵ�û������Ч����ֱ�Ӵ���Ŀ��
                bullet.TarGet = hit.Entity;//�ӵ����Ҫ����Ŀ���Entity

            if (bullet.Is_NOAttack)
            {
                if(!shield.TryGetComponent(hit.Entity, out Shield shis))
                    return;
                else
                {
                    ECB.AddComponent(chunkIndex, bullet.Bullet_Entity, new Die
                    {
                        DeadParticle = bullet.CannonHit,
                        DeadPoint = bullet.Bullet_Entity,
                        DeadLanguage = bullet.DeadLanguage,
                        DeadLanguage2 = Entity.Null,
                    });
                    return;
                }
            }

            ECB.AddComponent(chunkIndex, bullet.Bullet_Entity, new Die
            {
                DeadParticle = bullet.CannonHit,
                DeadPoint = bullet.Bullet_Entity,
                DeadLanguage = bullet.DeadLanguage,
                DeadLanguage2 = bullet.DeadLanguage2,
            });
            return;
        }

    }
}

[BurstCompile]
public partial struct BulletIsDieJob : IJobEntity//����ӵ��Ƿ�����job
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<Die> die;
    void Execute(Entity entity, Bullet bullet, SX sx, [ChunkIndexInQuery]int ChunkInde)
    {
        if (die.TryGetComponent(entity, out Die d))//�����Die���˳���Ҫ��Die�����
            return;

        if (sx.Cur_HP <= 0)
        {
            ECB.AddComponent(ChunkInde, entity, new Die
            {
                DeadParticle = bullet.DeadParticle,
                DeadPoint = bullet.CenterPoint,
                DeadLanguage = Entity.Null,
                DeadLanguage2 = Entity.Null,
            });
        }
    }
}
