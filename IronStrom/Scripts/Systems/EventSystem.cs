using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial class EventSystem : SystemBase
{
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<SX> m_SX;
    ComponentLookup<MyLayer> m_mylayer;
    ComponentLookup<JiDi> m_JiDi;
    ComponentLookup<DirectShoot> m_DirectShoot;

    void UpDataComponentLookup(SystemBase system)
    {
        m_shibing.Update(system);
        m_transform.Update(system);
        m_LocaltoWorld.Update(system);
        m_SX.Update(system);
        m_mylayer.Update(system);
        m_JiDi.Update(system);
        m_DirectShoot.Update(system);
    }
    protected override void OnCreate()
    {
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_mylayer = GetComponentLookup<MyLayer>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
        m_DirectShoot = GetComponentLookup<DirectShoot>(true);
    }
    protected override void OnUpdate()
    {
        UpDataComponentLookup(this);

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        var Eventjob = new EventJob
        {
            ECB = ecb.AsParallelWriter(),
            spawn = spawn,
            time = SystemAPI.Time.DeltaTime,
            transform = m_transform,
            LocaltoWorld = m_LocaltoWorld,
            shibing = m_shibing,
            mylayer = m_mylayer,
            sx = m_SX,
            JIDI = m_JiDi,
            directShoot = m_DirectShoot,
        };
        Dependency = Eventjob.Schedule(Dependency);

        Dependency.Complete();
        ecb.Playback(EntityManager);//应用实体的修改
        ecb.Dispose();//手动释放new的Buffer


        //Entities.ForEach((Entity entity, LocalTransform transform, in DynamicBuffer <GpuEcsAnimatorEventBufferElement> gpuEcsEventBuffer) =>
        //{
        //    if(EntityManager.Exists(entity))//检测这个Entity是否被删除
        //    {
        //        foreach (GpuEcsAnimatorEventBufferElement Event in gpuEcsEventBuffer)
        //        {
        //            //测试-----
        //            string entityName = m_shibing[entity].Name.ToString();
        //            string animationID = ((AnimationIdsBaolei)Event.animationId).ToString();
        //            string eventID = ((AnimationEventIdsBaolei)Event.eventId).ToString();
        //            string time = UnityEngine.Time.time.ToString();
        //            //---------
        //            TriggerEvent(entity, spawn, Event.animationId, Event.eventId);
        //        }
        //    }
        //}).WithoutBurst().WithStructuralChanges().Run();
        ////WithoutBurst() :
        ////这是一个链式调用，用于指示系统在执行时不使用 Burst 编译器。
        ////Burst 是Unity的一种编译器优化技术，可以提高代码的执行效率。然而，某些情况下可能会有不兼容的问题，
        ////因此可能需要使用 WithoutBurst 来禁用 Burst 编译。

    }

    //public void TriggerEvent(Entity entity, Spawn spawn, int animationID, int eventID)
    //{
    //    ShiBingName name = m_shibing[entity].Name;
    //    switch (name)
    //    {
    //        case ShiBingName.BaoLei: EventBaoLei(entity, spawn, (AnimationEventIdsBaolei)eventID); break;
    //        case ShiBingName.ChangGong: EventChangGong(entity, spawn, (AnimationEventIdsChangGong)eventID); break;
    //        case ShiBingName.JianYa: EventJianYa(entity, spawn, (AnimationEventIdsJianYa)eventID); break;
    //        case ShiBingName.HuGuang: EventHuGuang(entity, spawn, (AnimationEventIdsHuGuang)eventID); break;
    //        case ShiBingName.PaChong: EventPaChong(entity, spawn, (AnimationEventIdsPaChang)eventID); break;
    //        case ShiBingName.BingFeng: EventBingFeng(entity, spawn, (AnimationEventIdsBingFeng)eventID); break;
    //        case ShiBingName.HaiKe: EventHaiKe(entity, spawn, (AnimationEventIdsHaiKe)eventID); break;
    //    }
    //}


    //void EventBaoLei(Entity entity, Spawn spawn, AnimationEventIdsBaolei eventID)//BaoLei的动画事件
    //{
    //    switch (eventID)
    //    {
    //        case AnimationEventIdsBaolei.Fire_R_EventFire_R : InstanFire(entity, spawn.BaoLeiBullet,spawn.BaoLei_Muzzle_1 , m_shibing[entity].FirePoint_R);break;
    //        case AnimationEventIdsBaolei.Fire_L_EventFire_L: InstanFire(entity, spawn.BaoLeiBullet, spawn.BaoLei_Muzzle_1, m_shibing[entity].FirePoint_L);break;
    //        case AnimationEventIdsBaolei.Walk_Foot_R: InstanParticle(entity, spawn.BaoLei_Foot, m_shibing[entity].Foot_R); break;
    //        case AnimationEventIdsBaolei.Walk_Foot_L: InstanParticle(entity, spawn.BaoLei_Foot, m_shibing[entity].Foot_L); break;
    //    }
    //}




    //void EventChangGong(Entity entity, Spawn spawn, AnimationEventIdsChangGong eventID)//ChangGong的动画事件
    //{
    //    if(eventID == AnimationEventIdsChangGong.Attack_EventFire)
    //    {
    //        InstanFire(entity, spawn.ChangGongBullet, spawn.ChangGong_Muzzle_1, m_shibing[entity].FirePoint_R);
    //        InstanParticle(entity, spawn.ChangGong_Muzzle_2, m_shibing[entity].FirePoint_R);
    //    }
    //}

    //void EventJianYa(Entity entity,Spawn spawn, AnimationEventIdsJianYa eventID)
    //{
    //    if(eventID == AnimationEventIdsJianYa.Attack_Attack)
    //    {
    //        InstanFire(entity, spawn.JianYaBullet, spawn.JianYa_Muzzle_1, m_shibing[entity].FirePoint_R);
    //    }

    //}

    //void EventHuGuang(Entity entity, Spawn spawn, AnimationEventIdsHuGuang eventID)
    //{

    //    switch (eventID)
    //    {
    //        case AnimationEventIdsHuGuang.Attack_Fire: InstanFire(entity, spawn.HuGuangBullet, spawn.HuGuang_Muzzle_1, m_shibing[entity].FirePoint_R); break;
    //        case AnimationEventIdsHuGuang.Attack2_Charged: InstanParticle(entity, spawn.HuGuang_Muzzle_2, m_shibing[entity].FirePoint_R, true);break;
    //        case AnimationEventIdsHuGuang.Attack2_Fire2: InstanFire(entity, spawn.HuGuangBullet2, spawn.HuGuang_Muzzle_1, m_shibing[entity].FirePoint_R); break;
    //    }

    //}

    //void EventPaChong(Entity shibing, Spawn spawn, AnimationEventIdsPaChang eventID)
    //{
    //    if(eventID == AnimationEventIdsPaChang.Attack_Fire)
    //    {
    //        InstanAttackBox(shibing, spawn.PaChong_AttackBox, m_shibing[shibing].FirePoint_R);
    //        InstanParticle(shibing, spawn.PaChong_Muzzle_1, m_shibing[shibing].FirePoint_R);
    //    }
    //}

    //void EventBingFeng(Entity shibing, Spawn spawn, AnimationEventIdsBingFeng eventID)
    //{
    //    if(eventID == AnimationEventIdsBingFeng.Fire_Fire)
    //    {
    //        bool Is_GroundShiBing = false;//是否为地面单位
    //        float ATChangeValue = 0;
    //        var shootEnti = m_shibing[shibing].ShootEntity;
    //        if (EntityManager.Exists(shootEnti))
    //        {
    //            if (!EntityManager.HasComponent<SX>(shootEnti))
    //                return;

    //            if(!m_SX[shootEnti].Is_AirForce)//兵锋对地攻击力加50%
    //            {
    //                var entiSX = m_SX[shibing];
    //                ATChangeValue = entiSX.AT * 0.5f;
    //                entiSX.AT += ATChangeValue;
    //                EntityManager.SetComponentData(shibing, entiSX);
    //                Is_GroundShiBing = true;
    //            }
    //        }
    //        InstanFire(shibing, spawn.BingFengBullet, spawn.BingFeng_Muzzle_1, m_shibing[shibing].FirePoint_R);
    //        if(Is_GroundShiBing)
    //        {
    //            var entiSX = m_SX[shibing];
    //            entiSX.AT -= ATChangeValue;
    //            EntityManager.SetComponentData(shibing, entiSX);
    //        }
    //    }
    //}

    //void EventHaiKe(Entity shibing,Spawn spawn, AnimationEventIdsHaiKe eventID)
    //{
    //    switch (eventID)
    //    {
    //        case AnimationEventIdsHaiKe.Walk_FootR: InstanParticle(shibing, spawn.HaiKe_Muzzle_1, m_shibing[shibing].Foot_R); break;
    //        case AnimationEventIdsHaiKe.Walk_FootL: InstanParticle(shibing, spawn.HaiKe_Muzzle_1, m_shibing[shibing].Foot_L); break;
    //        case AnimationEventIdsHaiKe.Walk_FootMid: InstanParticle(shibing, spawn.HaiKe_Muzzle_1, m_shibing[shibing].Particle_1); break;
    //    }
    //}





    //void InstanFire(Entity entity, Entity bulletP , Entity muzzle, Entity firepoint)//实例化炮弹
    //{
    //    if (!EntityManager.Exists(bulletP))
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(muzzle))
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(m_shibing[entity].FirePoint_R))
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(m_shibing[entity].ShootEntity) || m_SX[entity].Is_Die)
    //    {
    //        return;
    //    }
    //    if (!EntityManager.HasComponent<LocalTransform>(m_shibing[entity].ShootEntity))
    //    {
    //        return;
    //    }
    //    Entity centerPoint = Entity.Null;
    //    if (EntityManager.Exists(m_shibing[entity].JidiPoint))
    //        centerPoint = m_shibing[entity].JidiPoint;
    //    else
    //    {
    //        centerPoint = m_shibing[m_shibing[entity].ShootEntity].CenterPoint;//获得敌人的中心位置
    //        if (!EntityManager.Exists(centerPoint))
    //        {
    //            centerPoint = m_shibing[entity].ShootEntity;
    //        }
    //    }

        
    //    Entity bullet = EntityManager.Instantiate(bulletP);
    //    Entity par = EntityManager.Instantiate(muzzle);

    //    float3 firePoint = m_LocaltoWorld[firepoint].Position;
    //    float3 dir = m_LocaltoWorld[centerPoint].Position - firePoint;//m_transform[entity].Position;
    //    dir = math.normalizesafe(dir);

    //    var parLocalTransfrom = EntityManager.GetComponentData<LocalTransform>(par);//枪口特效位置
    //    parLocalTransfrom.Position = firePoint;
    //    parLocalTransfrom.Rotation = quaternion.LookRotationSafe(dir, new float3(0, 1, 0));

    //    var bulletLocalTransfrom = EntityManager.GetComponentData<LocalTransform>(bullet);//子弹初始位置
    //    bulletLocalTransfrom.Position = firePoint;
    //    bulletLocalTransfrom.Rotation = quaternion.LookRotationSafe(dir, new float3(0, 1, 0));

    //    //var bull = EntityManager.GetComponentData<Bullet>(bullet);
    //    //bull.Dir = dir;
    //    //bull.AT = m_SX[entity].AT;
    //    EntityManager.SetComponentData(par, parLocalTransfrom);
    //    EntityManager.SetComponentData(bullet, bulletLocalTransfrom);
    //    EntityManager.AddComponentData(bullet, new BulletChange
    //    {
    //        Dir = dir,
    //        AT = m_SX[entity].AT,
    //    });
    //    UpDataComponentLookup(this);
    //    SetBulletMyLayer(in entity,ref bullet);

    //}

    //void SetBulletMyLayer(in Entity shibing,ref Entity Instan)//设置子弹的层级
    //{
    //    var InstanLyaer = m_mylayer[Instan];
    //    if (m_mylayer[shibing].BelongsTo == layer.Team1)
    //    {
    //        InstanLyaer.BelongsTo = layer.Team1Bullet;
    //        InstanLyaer.CollidesWith_1 = layer.Team2;
    //    }
    //    else if (m_mylayer[shibing].BelongsTo == layer.Team2)
    //    {
    //        InstanLyaer.BelongsTo = layer.Team2Bullet;
    //        InstanLyaer.CollidesWith_1 = layer.Team1;
    //    }
    //    InstanLyaer.CollidesWith_2 = layer.Neutral;
    //    EntityManager.SetComponentData(Instan, InstanLyaer);
    //}

    //void InstanParticle(Entity shibing, Entity particle, Entity point, bool Is_Follow = false)//实例化特效
    //{
    //    if (!EntityManager.Exists(particle))
    //    {
    //        return;
    //    }
    //    if (m_SX[shibing].Is_Die)
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(point))
    //    {
    //        return;
    //    }
    //    Entity par = EntityManager.Instantiate(particle);
    //    EntityManager.SetComponentData(par, new LocalTransform
    //    {
    //        Position = m_LocaltoWorld[point].Position,
    //        Rotation = m_LocaltoWorld[point].Rotation,
    //        Scale = 1,
    //    });
    //    if (Is_Follow)//HuGuang的蓄力攻击
    //    {
    //        EntityManager.SetComponentData(par, new ParticleComponent
    //        {
    //            Is_Follow = true,
    //            FollowPoint = point,
    //            DieTime = 1f,
    //            ParScale = 1f,
    //        });
    //    }
    //    //var parPos = EntityManager.GetComponentData<LocalTransform>(par);
    //    //parPos.Position = m_LocaltoWorld[point].Position;
    //    //parPos.Rotation = m_LocaltoWorld[point].Rotation;
    //    //EntityManager.SetComponentData(particle, parPos);
    //}
    //void InstanAttackBox(Entity shibing, Entity box, Entity point)//实例化攻击矩形
    //{

    //    if (!EntityManager.Exists(box))
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(m_shibing[shibing].FirePoint_R))
    //    {
    //        return;
    //    }
    //    if (!EntityManager.Exists(m_shibing[shibing].ShootEntity) || m_SX[shibing].Is_Die)
    //    {
    //        //Debug.Log("           shibing的攻击目标为空/或者已死亡     " + " 士兵为：" + shibing + "   shoot目标为：" + m_shibing[shibing].ShootEntity);
    //        return;
    //    }

    //    var instanBox = EntityManager.Instantiate(box);//实例化攻击矩形
    //    SetBulletMyLayer(in shibing, ref instanBox);//设置Layer
    //    var BoxWorld = m_LocaltoWorld[point];
    //    EntityManager.SetComponentData(instanBox, BoxWorld);
    //    EntityManager.SetComponentData(instanBox, new LocalTransform
    //    {
    //        Position = m_LocaltoWorld[point].Position,
    //        Rotation = m_LocaltoWorld[point].Rotation,
    //        Scale = m_transform[box].Scale,
    //    });
    //}

}

[BurstCompile]
partial struct EventJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public Spawn spawn;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<MyLayer> mylayer;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<JiDi> JIDI;
    [ReadOnly] public ComponentLookup<DirectShoot> directShoot;
    public float time;
    void Execute(Entity entity, DynamicBuffer<GpuEcsAnimatorEventBufferElement> gpuEcsEventBuffer, [ChunkIndexInQuery] int ChunkIndex)
    {
        foreach (GpuEcsAnimatorEventBufferElement Event in gpuEcsEventBuffer)
        {
            //string entityName = shibing[entity].Name.ToString();
            //string animationID = Event.animationId.ToString();
            //string eventID = Event.eventId.ToString();
            //Debug.Log("    Name:" + entityName + "  动画ID:" + animationID + " 事件ID:" + eventID + " 时间:" + time);

            TriggerEvent(entity, spawn, Event.animationId, Event.eventId, ChunkIndex);
        }
    }

    public void TriggerEvent(Entity entity, Spawn spawn, int animationID, int eventID, int ChunkIndex)
    {
        ShiBingName name = shibing[entity].Name;
        switch (name)
        {
            case ShiBingName.BaoLei: EventBaoLei(entity, spawn, (AnimationEventIdsBaolei)eventID,ChunkIndex); break;
            case ShiBingName.ChangGong: EventChangGong(entity, spawn, (AnimationEventIdsChangGong)eventID, ChunkIndex); break;
            case ShiBingName.JianYa: EventJianYa(entity, spawn, (AnimationEventIdsJianYa)eventID, ChunkIndex); break;
            case ShiBingName.HuGuang: EventHuGuang(entity, spawn, (AnimationEventIdsHuGuang)eventID, ChunkIndex); break;
            case ShiBingName.PaChong: EventPaChong(entity, spawn, (AnimationEventIdsPaChang)eventID, ChunkIndex); break;
            case ShiBingName.BingFeng: EventBingFeng(entity, spawn, (AnimationEventIdsBingFeng)eventID, ChunkIndex); break;
            case ShiBingName.HaiKe: EventHaiKe(entity, spawn, (AnimationEventIdsHaiKe)eventID, ChunkIndex); break;
            //case ShiBingName.YeMa: YeMaEvent(entity, ChunkIndex); break;
        }
    }
    void EventBaoLei(Entity entity, Spawn spawn, AnimationEventIdsBaolei eventID, int ChunkIndex)//BaoLei的动画事件
    {
        switch (eventID)
        {
            case AnimationEventIdsBaolei.Fire_R_EventFire_R: InstanFire(entity, spawn.BaoLeiBullet, spawn.BaoLei_Muzzle_1, shibing[entity].FirePoint_R,ChunkIndex); break;
            case AnimationEventIdsBaolei.Fire_L_EventFire_L: InstanFire(entity, spawn.BaoLeiBullet, spawn.BaoLei_Muzzle_1, shibing[entity].FirePoint_L,ChunkIndex); break;
            case AnimationEventIdsBaolei.Walk_Foot_R: InstanParticle(entity, spawn.BaoLei_Foot, shibing[entity].Foot_R,ChunkIndex); break;
            case AnimationEventIdsBaolei.Walk_Foot_L: InstanParticle(entity, spawn.BaoLei_Foot, shibing[entity].Foot_L,ChunkIndex); break;
        }
    }

    void EventChangGong(Entity entity, Spawn spawn, AnimationEventIdsChangGong eventID, int ChunkIndex)//ChangGong的动画事件
    {
        if (eventID == AnimationEventIdsChangGong.Attack_EventFire)
        {
            InstanFire(entity, spawn.ChangGongBullet, spawn.ChangGong_Muzzle_1, shibing[entity].FirePoint_R, ChunkIndex);
            InstanParticle(entity, spawn.ChangGong_Muzzle_2, shibing[entity].FirePoint_R, ChunkIndex);
        }
    }

    void EventJianYa(Entity entity, Spawn spawn, AnimationEventIdsJianYa eventID, int ChunkIndex)
    {
        if (eventID == AnimationEventIdsJianYa.Attack_Attack)
        {
            InstanFire(entity, spawn.JianYaBullet, spawn.JianYa_Muzzle_1, shibing[entity].FirePoint_R, ChunkIndex);
        }
    }

    void EventHuGuang(Entity entity, Spawn spawn, AnimationEventIdsHuGuang eventID, int ChunkIndex)
    {
        switch (eventID)
        {
            case AnimationEventIdsHuGuang.Attack_Fire: InstanFire(entity, spawn.HuGuangBullet, spawn.HuGuang_Muzzle_1, shibing[entity].FirePoint_R,ChunkIndex); break;
            case AnimationEventIdsHuGuang.Attack2_Charged: InstanParticle(entity, spawn.HuGuang_Muzzle_2, shibing[entity].FirePoint_R,ChunkIndex , true); break;
            case AnimationEventIdsHuGuang.Attack2_Fire2: InstanFire(entity, spawn.HuGuangBullet2, spawn.HuGuang_Muzzle_1, shibing[entity].FirePoint_R,ChunkIndex); break;
        }
    }

    void EventPaChong(Entity entity, Spawn spawn, AnimationEventIdsPaChang eventID, int ChunkIndex)
    {
        if (eventID == AnimationEventIdsPaChang.Attack_Fire)
        {
            InstanAttackBox(entity, spawn.PaChong_AttackBox, shibing[entity].FirePoint_R, ChunkIndex);
            InstanParticle(entity, spawn.PaChong_Muzzle_1, shibing[entity].FirePoint_R, ChunkIndex);
        }
    }

    void EventBingFeng(Entity entity, Spawn spawn, AnimationEventIdsBingFeng eventID, int ChunkIndex)
    {
        if (eventID == AnimationEventIdsBingFeng.Fire_Fire)
        {
            bool Is_GroundShiBing = false;//是否为地面单位
            float ATChangeValue = 0;
            var shootEnti = shibing[entity].ShootEntity;
            if (transform.TryGetComponent(shootEnti,out LocalTransform ltf))
            {
                if (!sx.TryGetComponent(shootEnti, out SX ssx))
                    return;

                if (!sx[shootEnti].Is_AirForce)//兵锋对地攻击力加50%
                {
                    var entiSX = sx[entity];
                    ATChangeValue = entiSX.AT * 0.5f;
                    entiSX.AT += ATChangeValue;
                    ECB.SetComponent(ChunkIndex, entity, entiSX);
                    Is_GroundShiBing = true;
                }
            }
            InstanFire(entity, spawn.BingFengBullet, spawn.BingFeng_Muzzle_1, shibing[entity].FirePoint_R, ChunkIndex);
            if (Is_GroundShiBing)
            {
                var entiSX = sx[entity];
                entiSX.AT -= ATChangeValue;
                ECB.SetComponent(ChunkIndex,entity, entiSX);
            }
        }
    }

    void EventHaiKe(Entity entity, Spawn spawn, AnimationEventIdsHaiKe eventID, int ChunkIndex)
    {
        switch (eventID)
        {
            case AnimationEventIdsHaiKe.Walk_FootR: InstanParticle(entity, spawn.HaiKe_Muzzle_1, shibing[entity].Foot_R, ChunkIndex); break;
            case AnimationEventIdsHaiKe.Walk_FootL: InstanParticle(entity, spawn.HaiKe_Muzzle_1, shibing[entity].Foot_L, ChunkIndex); break;
            case AnimationEventIdsHaiKe.Walk_FootMid: InstanParticle(entity, spawn.HaiKe_Muzzle_1, shibing[entity].Particle_1,ChunkIndex); break;
        }
    }
    
    void YeMaEvent(Entity entity, int ChunkIndx)
    {
        if (!sx.TryGetComponent(entity, out SX s))
            return;
        var sxx = sx[entity];
        sxx.Cur_ShootTime = sxx.ShootTime;
        ECB.SetComponent(ChunkIndx, entity, sxx);

        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//如果没有DirectShoot组件就退出
            return;

        if (transform.TryGetComponent(directShoot[entity].DirectParticle_Entity, out LocalTransform lt))//如果有直接作用的攻击特效就退出
            return;

        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndx);
        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;
        ECB.SetComponent(ChunkIndx, entity, dirshoot);
        ECB.SetComponent(ChunkIndx, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });

    }

    void InstanFire(Entity entity, Entity bulletP, Entity muzzle, Entity firepoint, int ChunkIndex)//实例化炮弹
    {
        if (!transform.TryGetComponent(bulletP, out LocalTransform ltf))
        {
            return;
        }
        if (!transform.TryGetComponent(muzzle, out LocalTransform ltf1))
        {
            return;
        }
        if (!transform.TryGetComponent(shibing[entity].FirePoint_R,out LocalTransform ltf2))
        {
            return;
        }
        if (!transform.TryGetComponent(shibing[entity].ShootEntity,out LocalTransform ltf3) || sx[entity].Is_Die)
        {
            return;
        }
        if (!transform.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf4))
        {
            return;
        }
        Entity centerPoint = Entity.Null;
        if (JIDI.TryGetComponent(shibing[entity].ShootEntity, out JiDi jd))
        {
            centerPoint = shibing[entity].JidiPoint;
        }
        else
        {
            centerPoint = shibing[shibing[entity].ShootEntity].CenterPoint;//获得敌人的中心位置
            if (!transform.TryGetComponent(centerPoint, out LocalTransform ltf6))
            {
                centerPoint = shibing[entity].ShootEntity;
            }
        }


        Entity bullet = ECB.Instantiate(ChunkIndex, bulletP);
        Entity par = ECB.Instantiate(ChunkIndex, muzzle);

        float3 firePoint = LocaltoWorld[firepoint].Position;
        float3 dir = LocaltoWorld[centerPoint].Position - firePoint;//m_transform[entity].Position;
        dir = math.normalizesafe(dir);

        //枪口特效位置
        ECB.SetComponent(ChunkIndex, par, new LocalTransform
        {
            Position = LocaltoWorld[firepoint].Position,
            Rotation = LocaltoWorld[firepoint].Rotation,
            Scale = 1,
        });

        //子弹初始位置
        ECB.SetComponent(ChunkIndex, bullet, new LocalTransform
        {
            Position = LocaltoWorld[firepoint].Position,
            Rotation = quaternion.LookRotationSafe(dir, new float3(0, 1, 0)),
            Scale = 1,
        });

        ECB.AddComponent(ChunkIndex, bullet, new BulletChange
        {
            Dir = dir,
            AT = sx[entity].AT,
        });

        //子弹拥有者
        ECB.AddComponent(ChunkIndex, bullet, new OwnerData()
        {
            Owner = entity,
        });

        SetBulletMyLayer(in entity, ref bullet , ChunkIndex);
    }
    
    void SetBulletMyLayer(in Entity shibing, ref Entity Instan, int ChunkIndex)//设置子弹的层级
    {
        var InstanLyaer = new MyLayer();
        if (mylayer[shibing].BelongsTo == layer.Team1)
        {
            InstanLyaer.BelongsTo = layer.Team1Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team2;
        }
        else if (mylayer[shibing].BelongsTo == layer.Team2)
        {
            InstanLyaer.BelongsTo = layer.Team2Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team1;
        }
        InstanLyaer.CollidesWith_2 = layer.Neutral;
        ECB.SetComponent(ChunkIndex, Instan, InstanLyaer);
    }

    Entity InstanParticle(Entity shibing, Entity particle, Entity point, int ChunkIndex, bool Is_Follow = false)//实例化特效
    {
        if (!transform.TryGetComponent(particle, out LocalTransform ltf))
        {
            return Entity.Null;
        }
        if (sx[shibing].Is_Die)
        {
            return Entity.Null;
        }
        if (!transform.TryGetComponent(point, out LocalTransform ltf1))
        {
            return Entity.Null;
        }
        Entity par = ECB.Instantiate(ChunkIndex, particle);
        ECB.SetComponent(ChunkIndex, par, new LocalTransform
        {
            Position = LocaltoWorld[point].Position,
            Rotation = LocaltoWorld[point].Rotation,
            Scale = 1,
        });
        if (Is_Follow)//HuGuang的蓄力攻击
        {
            ECB.SetComponent(ChunkIndex, par, new ParticleComponent
            {
                Is_Follow = true,
                FollowPoint = point,
                DieTime = 1f,
                ParScale = 1f,
            });
        }
        return par;
    }

    void InstanAttackBox(Entity entity, Entity box, Entity point, int ChunkIndex)//实例化攻击矩形
    {

        if (!transform.TryGetComponent(box, out LocalTransform ltf))
        {
            return;
        }
        if (!transform.TryGetComponent(shibing[entity].FirePoint_R, out LocalTransform ltf1))
        {
            return;
        }
        if (!transform.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf2) || sx[entity].Is_Die)
        {
            //Debug.Log("           shibing的攻击目标为空/或者已死亡     " + " 士兵为：" + shibing + "   shoot目标为：" + m_shibing[shibing].ShootEntity);
            return;
        }

        var instanBox = ECB.Instantiate(ChunkIndex, box);//实例化攻击矩形
        SetBulletMyLayer(in entity, ref instanBox, ChunkIndex);//设置Layer
        var BoxWorld = LocaltoWorld[point];
        ECB.SetComponent(ChunkIndex, instanBox, BoxWorld);
        ECB.SetComponent(ChunkIndex, instanBox, new LocalTransform
        {
            Position = LocaltoWorld[point].Position,
            Rotation = LocaltoWorld[point].Rotation,
            Scale = transform[box].Scale,
        });

        //添加拥有者信息
        ECB.AddComponent(ChunkIndex, entity, new OwnerData
        {
            Owner = entity,
        });
    }
}
