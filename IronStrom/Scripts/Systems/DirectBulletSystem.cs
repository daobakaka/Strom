using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public partial class DirectBulletSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<ShiBingChange> m_shibingCh;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<EntityCtrl> m_entiCtrl;
    ComponentLookup<Die> m_Die;
    ComponentLookup<DirectBullet> m_DirBullet;
    ComponentLookup<Bullet> m_Bullet;
    ComponentLookup<JiDi> m_JiDi;
    ComponentLookup<SX> m_SX;

    void UpDateComponentLookup()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_entiCtrl.Update(this);
        m_Die.Update(this);
        m_shibing.Update(this);
        m_shibingCh.Update(this);
        m_DirBullet.Update(this);
        m_Bullet.Update(this);
        m_JiDi.Update(this);
        m_SX.Update(this);
    }
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_shibingCh = GetComponentLookup<ShiBingChange>(true);
        m_Die = GetComponentLookup<Die>(true);
        m_DirBullet = GetComponentLookup<DirectBullet>(true);
        m_Bullet = GetComponentLookup<Bullet>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
        m_SX = GetComponentLookup<SX>(true);
    }

    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var dirbulletjob = new DirectBulletJob
        {
            ECB = ecb.AsParallelWriter(),
            transfrom = m_transfrom,
            localtoWorld = m_LocaltoWorld,
            entiCtrl = m_entiCtrl,
            die = m_Die,
            shibing = m_shibing,
            shibingCh = m_shibingCh,
            tiem = SystemAPI.Time.DeltaTime,
            bullet = m_Bullet,
            jidi = m_JiDi,
            sx = m_SX,
        };
        Dependency = dirbulletjob.ScheduleParallel(Dependency);

        Dependency.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        //修正直接子弹的特效===========================
        Entities.ForEach((Entity entity, DirectBulletSub dbs) =>
        {
            if (!EntityManager.Exists(dbs.parBullet))
                return;
            UpDateComponentLookup();
            var particleSystem = EntityManager.GetComponentObject<ParticleSystem>(entity);
            var main = particleSystem.main;
            
            //设置激光长度
            main.startLifetime = m_DirBullet[dbs.parBullet].StartLifetime;
            //设置激光粗细
            float SizeX = main.startSizeX.constant;
            float totalIncrease = 1f * 3f;//2秒增加的粗细
            float increaseThisFrame = 1f * SystemAPI.Time.DeltaTime;
            SizeX += increaseThisFrame;
            if (SizeX < totalIncrease)
                main.startSizeX = SizeX;

        }).WithoutBurst().WithStructuralChanges().Run();

    }


}
[BurstCompile]
public partial struct DirectBulletJob : IJobEntity
{
    public float tiem;
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transfrom;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> localtoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<ShiBing> shibing;
    [Unity.Collections.ReadOnly] public ComponentLookup<ShiBingChange> shibingCh;
    [Unity.Collections.ReadOnly] public ComponentLookup<Die> die;
    [Unity.Collections.ReadOnly] public ComponentLookup<Bullet> bullet;
    [Unity.Collections.ReadOnly] public ComponentLookup<JiDi> jidi;
    [Unity.Collections.ReadOnly] public ComponentLookup<SX> sx;

    void Execute(Entity entity, DirectBulletAspects dirBulletAsp, [ChunkIndexInQuery] int ChunkIndex)
    {
        if (!transfrom.TryGetComponent(dirBulletAsp.Owner, out LocalTransform ltf))//判断子弹拥有者是否还在
        {
            ECB.AddComponent(ChunkIndex, entity, new Die());//打子弹的士兵都没了，子弹特效也没了
            return;
        }
        var direnEntity = shibing[dirBulletAsp.Owner].ShootEntity;
        if (!transfrom.TryGetComponent(direnEntity, out LocalTransform ltf2))
        {
            ECB.AddComponent(ChunkIndex, entity, new Die());
            return;
        }

        if ((shibingCh[dirBulletAsp.Owner].Act != ActState.Fire && shibingCh[dirBulletAsp.Owner].Act != ActState.Ready) || 
            direnEntity == Entity.Null || (!shibing.TryGetComponent(direnEntity, out ShiBing sb) && !bullet.TryGetComponent(direnEntity, out Bullet bull)))//如果不是攻击状态就不要有子弹特效
        {
            ECB.AddComponent(ChunkIndex, entity, new Die());
            return;
        }

        //更具敌人的位置修正自己的面朝向
        float3 direnPos = float3.zero;
        if (bullet.TryGetComponent(direnEntity, out Bullet b))//如果敌人是一发子弹
        {
            direnPos = localtoWorld[bullet[direnEntity].CenterPoint].Position;
        }
        else if (jidi.TryGetComponent(direnEntity, out JiDi ji))//如果敌人是基地
        {
            if (!transfrom.TryGetComponent(shibing[dirBulletAsp.Owner].JidiPoint, out LocalTransform ltf1))
                return;
            direnPos = localtoWorld[shibing[dirBulletAsp.Owner].JidiPoint].Position;
        }
        else//如果敌人是一个士兵
        {
            if (transfrom.TryGetComponent(shibing[direnEntity].CenterPoint, out LocalTransform ltf1))
                direnPos = localtoWorld[shibing[direnEntity].CenterPoint].Position;
            else
                direnPos = localtoWorld[direnEntity].Position;
        }


        float3 shibingPos = localtoWorld[entity].Position;
        var vdir = direnPos - shibingPos;
        vdir = math.normalize(vdir);
        var dirBulletTransf = transfrom[entity];
        dirBulletTransf.Rotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));

        //更具士兵的发射口位置修正自己的位置
        Entity firepoint = Entity.Null;
        switch (dirBulletAsp.DB_FirePoint)
        {
            case  DirectBullet_FirePoint.FirePoint_R : firepoint = shibing[dirBulletAsp.Owner].FirePoint_R; break;
            case  DirectBullet_FirePoint.FirePoint_L : firepoint = shibing[dirBulletAsp.Owner].FirePoint_L; break;
            case  DirectBullet_FirePoint.FirePoint_R2 : firepoint = shibing[dirBulletAsp.Owner].FirePoint_R2; break;
            case  DirectBullet_FirePoint.FirePoint_L2 : firepoint = shibing[dirBulletAsp.Owner].FirePoint_L2; break;
        }
        if (firepoint == Entity.Null)
            return;
        dirBulletTransf.Position = localtoWorld[firepoint].Position;


        //计算我和敌人之间的位置
        var distan = math.distance(direnPos, shibingPos);
        //更具敌人的位置设置特效粒子的长度
        distan -= sx[direnEntity].VolumetricDistance;
        dirBulletAsp.StartLifetime = distan * dirBulletAsp.StartOffset / dirBulletAsp.StartSpeed;

        //实例化子弹击中特效,并实时修正击中特效的位置
        if (transfrom.TryGetComponent(dirBulletAsp.BulletHit, out LocalTransform ltf3))
        {
            var hitPos = transfrom[dirBulletAsp.BulletHit];
            hitPos.Position.z = distan + 2f;
            ECB.SetComponent(ChunkIndex, dirBulletAsp.BulletHit, hitPos);
        }
        ECB.SetComponent(ChunkIndex, entity, dirBulletTransf);
    }

}
