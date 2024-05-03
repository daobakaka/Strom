using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(EntityCtrlEventSystem))] //�ڸ�System֮ǰִ��
public partial class DirectShootSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<ShiBingChange> m_shibingCh;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<EntityCtrl> m_entiCtrl;
    ComponentLookup<Die> m_Die;
    ComponentLookup<SX> m_SX;
    ComponentLookup<UpSkill> m_UpSkill;

    void UpDateComponentLookup()
    {
        m_transfrom.Update(this);
        m_LocaltoWorld.Update(this);
        m_entiCtrl.Update(this);
        m_Die.Update(this);
        m_SX.Update(this);
        m_UpSkill.Update(this);
        m_shibing.Update(this);
    }
    protected override void OnCreate()
    {
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_Die = GetComponentLookup<Die>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);

    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        if (!SystemAPI.HasSingleton<Spawn>()) return;// ����Ƿ���� Spawn ���͵�ʵ��
        Spawn spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        //var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        //var directshootjob = new DirectShootJob
        //{
        //    ECB = ecb,
        //    transfrom = m_transfrom,
        //    localtoWorld = m_LocaltoWorld,
        //    entiCtrl = m_entiCtrl,
        //    die = m_Die,
        //};
        //Dependency = directshootjob.ScheduleParallel(Dependency);
        //Dependency.Complete();
        //ecb.Playback(EntityManager);
        //ecb.Dispose();
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        //���߳��м���ֱ�ӹ����˺�
        Entities.ForEach((Entity entity, ShiBing shibing,ref DirectShoot dirShoot) =>
        {
            UpDateComponentLookup();
            if (m_SX[entity].Cur_ShootTime > 0)//�������ʱ��û��
                return;
            var ShootEntity = shibing.ShootEntity;
            //���û�й���Ŀ�� ���� �ڻ�����
            if (!EntityManager.Exists(ShootEntity))
                return;
            if (EntityManager.HasComponent<InShield>(ShootEntity))
            {
                if (EntityManager.IsComponentEnabled<InShield>(ShootEntity))
                    return;
            }
            var shootSX = m_SX[ShootEntity];
            var entitySX = m_SX[entity];
            //�ж��Ƿ�ԿնԵص����й�������
            var entiAT = spawn.Is_Advantage(in entitySX.AT, in entitySX, in shootSX);
            //�ҵĹ�������ȥ������˵ĸ��ʣ��������ǵĹ������ֵ���ֵ
            var AT = entiAT - entiAT * shootSX.DB;
            shootSX.DP -= AT;
            if(shootSX.DP < 0)
            {
                float at = shootSX.DP * -1;
                shootSX.Cur_HP += shootSX.DP;//�ȼ������ֵ�ټ�����ֵ
                shootSX.DP = 0;
                //���������ǹ���ͼ�¼����������ͳ������˺�������ù��������
                if (EntityManager.HasComponent<Monster>(ShootEntity))
                {   //����������������EntityOpenID(����ҵ�ʿ��)�ż�¼
                    if (EntityManager.HasComponent<EntityOpenID>(entity))
                    {
                        var EntiOpenID = EntityManager.GetComponentData<EntityOpenID>(entity);
                        monsterMager.MonsterRecordPlayerAT(EntiOpenID, ShootEntity, at, EntityManager);
                    }
                }
            }

            EntityManager.SetComponentData(ShootEntity, shootSX);

            //����˸����������ڴ�����������ʱ��ðѻ��ּӸ���
            if (EntityManager.HasComponent<Integral>(ShootEntity))
            {
                var shootInteg = EntityManager.GetComponentData<Integral>(ShootEntity);
                shootInteg.AttackMeEntity = entity;
                EntityManager.SetComponentData(ShootEntity, shootInteg);
            }

            //�������������¼�����˺�
            if (EntityManager.HasComponent<UpSkill>(shibing.ShootEntity))
            {
                if(m_UpSkill[shibing.ShootEntity].upSkill_Name == UpSkillName.GangQiu)
                {
                    var upSkill = m_UpSkill[shibing.ShootEntity];
                    upSkill.InjuryRecord += AT;
                    EntityManager.SetComponentData(shibing.ShootEntity, upSkill);
                }
            }

            //�ۻ�Ŀ���Ƿ�Ϊͬһ��Ŀ�꣬�Ƿ�Ҫ�����ۻ�--------------
            if (dirShoot.CD_ShootEntity == Entity.Null)
            {
                dirShoot.CD_ShootEntity = shibing.ShootEntity;
                dirShoot.Is_ShootEntityChanges = true;
            }
            else if (dirShoot.CD_ShootEntity != shibing.ShootEntity)
            {
                entitySX.AT = dirShoot.AT_Min;
                dirShoot.CD_ShootEntity = shibing.ShootEntity;
                dirShoot.Is_ShootEntityChanges = true;
            }
            else
                dirShoot.Is_ShootEntityChanges = false;
            //----------

            //���Ϊ�ۻ��˺�----------------
            dirShoot.Cur_IntervalTime -= SystemAPI.Time.DeltaTime;
            if(dirShoot.Is_CumulativeDamage && entitySX.AT < dirShoot.AT_Max &&dirShoot.Cur_IntervalTime <= 0)
            {
                dirShoot.Cur_IntervalTime = dirShoot.IntervalTime;
                //�ۻ���ʽ
                entitySX.AT += 1;
                EntityManager.SetComponentData(entity, entitySX);
            }

        }).WithoutBurst().WithStructuralChanges().Run();

    }
}

[BurstCompile]
public partial struct DirectShootJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transfrom;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> localtoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<Die> die;
    void Execute(Entity entity,ShiBingAspects shibingAsp, DirectShoot dshoot)
    {

    }
}
