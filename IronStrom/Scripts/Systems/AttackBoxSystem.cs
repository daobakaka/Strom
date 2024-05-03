using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;

public partial class AttackBoxSystem : SystemBase
{
    ComponentLookup<SX> m_SX;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;

    void UpDataComponentLookup()
    {
        m_SX.Update(this);
        m_shibing.Update(this);
        m_UpSkill.Update(this);
        m_LocaltoWorld.Update(this);
    }
    protected override void OnCreate()
    {
        m_SX = GetComponentLookup<SX>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
    }

    protected override void OnUpdate()
    {
        UpDataComponentLookup();



        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//����һ����ײ�����Ҫ�ĵ���

        Entities.ForEach((Entity entity,LocalToWorld loacltoworld ,MyLayer mylayer,ref AttackBox abox) =>
        {
            if (abox.BoxState == AttactBoxState.NUll)
                return;
            var time = SystemAPI.Time.DeltaTime;
            if(!abox.Is_NoTime)
                abox.ExistenceTime -= time;//box����ʱ��
            abox.Cur_SustainTime -= time;//box���ʱ��
            if (abox.Cur_SustainTime > 0)
                return;
            else
                abox.Cur_SustainTime = abox.SustainTime;

            NativeList<ColliderCastHit> outHitsAll = new NativeList<ColliderCastHit>(Allocator.Temp);
            ColliderCastHit outHit = new ColliderCastHit();

            //�Ƿ��⵽�з���λ
            if (abox.BoxShape == AttactBoxShape.Box)
                DetectionBox(entity, physicsWorld,ref abox, mylayer, loacltoworld, ref outHitsAll, ref outHit, abox.Is_All);
            else if (abox.BoxShape == AttactBoxShape.Sphere)
                DetectionSphere(physicsWorld, abox, mylayer, loacltoworld, ref outHitsAll, ref outHit, abox.Is_All);
            else if (abox.BoxShape == AttactBoxShape.Capsule)
            {
                return;
            }

            //�Ƿ�Ϊ�����˺�
            if (abox.BoxState == AttactBoxState.OnceDamage)//�����˺�
            {
                //����˺�
                CalculateHurt(entity, abox, ref outHitsAll, ref outHit, abox.Is_All);
                //��ֱ��ɾ��box
                EntityManager.AddComponentData(entity, new Die());
                m_SX.Update(this);
                m_shibing.Update(this);
            }
            else if(abox.BoxState == AttactBoxState.ManyDamage)//�����˺�
            {
                //����ʱ�䵽�˺�ɾ��box
                if (abox.ExistenceTime <= 0)
                {
                    EntityManager.AddComponentData(entity, new Die());
                    m_SX.Update(this);
                    m_shibing.Update(this);
                }
                //����˺�
                CalculateHurt(entity, abox, ref outHitsAll, ref outHit, abox.Is_All);

            }


        }).WithoutBurst().WithStructuralChanges().Run();
    }

    bool DetectionBox(Entity entity, PhysicsWorldSingleton physicsworld,ref AttackBox abox, MyLayer mylayer, LocalToWorld localworld,
        ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//���μ����
    {
        UpDataComponentLookup();
        bool b = false;
        if(abox.Is_Remote && abox.halfExtents.z < abox.MaxRemote)
        {
            abox.halfExtents.z += abox.MaxRemote * SystemAPI.Time.DeltaTime;
        }
        float maxDistance = abox.halfExtents.z;//math.cmax(abox.halfExtents);

        uint collidesWithMask = 0;
        if ((int)mylayer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)mylayer.CollidesWith_1);
        if ((int)mylayer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)mylayer.CollidesWith_2);
        if ((int)mylayer.BulletCollidesWith != 0)//�ж������λ�Ƿ����ӵ����
            collidesWithMask |= (1u << (int)mylayer.BulletCollidesWith);
        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)mylayer.BelongsTo),
            CollidesWith = collidesWithMask,
            GroupIndex = 0
        };
        if (Is_All)//�����ײ����ȫ����λ
            b = physicsworld.BoxCastAll(localworld.Position + abox.Offset, m_LocaltoWorld[entity].Rotation, abox.halfExtents, new float3(0, 0, 1), maxDistance, ref outHits, filterDetection);
        else//��������ĵ�һ����λ
            b = physicsworld.BoxCast(localworld.Position + abox.Offset, m_LocaltoWorld[entity].Rotation, abox.halfExtents, new float3(0, 0, 1), maxDistance, out outHit, filterDetection);
        return b;
    }

    bool DetectionSphere(PhysicsWorldSingleton physicsworld, AttackBox abox, MyLayer mylayer, LocalToWorld localworld,
        ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//Բ�μ����
    {
        bool b = false;

        uint collidesWithMask = 0;
        if ((int)mylayer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)mylayer.CollidesWith_1);
        if ((int)mylayer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)mylayer.CollidesWith_2);
        if ((int)mylayer.BulletCollidesWith != 0)//�ж������λ�Ƿ����ӵ����
            collidesWithMask |= (1u << (int)mylayer.BulletCollidesWith);
        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)mylayer.BelongsTo),
            CollidesWith = collidesWithMask,
            GroupIndex = 0
        };
        if (Is_All)//�����ײ����ȫ����λ
            b = physicsworld.SphereCastAll(localworld.Position + abox.Offset, abox.R, new float3(0, 1, 0), abox.R * 2, ref outHits, filterDetection);
        else//��������ĵ�һ����λ
            b = physicsworld.SphereCast(localworld.Position + abox.Offset, abox.R, new float3(0, 1, 0), abox.R * 2, out outHit, filterDetection);

        return b;
    }

    bool DetectionCapsule()//���Ҽ����
    {
        return false;
    }

    //����˺�
    void CalculateHurt(Entity entity, AttackBox abox, ref NativeList<ColliderCastHit> outHits, ref ColliderCastHit outHit, bool Is_All)//�����˺�
    {
        if(Is_All)//�����⵽��ȫ����λ
        {
            if (outHits.IsUnityNull())//�����Ϸ�
                return;

            foreach(ColliderCastHit collidHit in outHits)
            {
                var hit = collidHit.Entity;
                Hurt(entity, ref hit, abox.AT);
            }
        }
        else//���㵥����λ
        {
            if (outHit.IsUnityNull())//Hit���Ϸ�
                return;

            var hit = outHit.Entity;
            Hurt(entity, ref hit, abox.AT);
        }
    }

    void Hurt(Entity entity, ref Entity hit, float hurt)//�˺�
    {
        //���������Ŀ��û��SX��� ���� �ڻ�������˳�
        if (!EntityManager.HasComponent<SX>(hit))
            return;
        if (EntityManager.HasComponent<InShield>(hit))
        {
            if (EntityManager.IsComponentEnabled<InShield>(hit))
                return;
        }
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        var hit_sx = m_SX[hit];
        //�������Box��ӵ���߶���������Ƿ��жԿնԵ�����
        if (!SystemAPI.HasSingleton<Spawn>()) return;
        var hurtat = SystemAPI.GetSingleton<Spawn>().Is_Advantage(in hurt, entity, in hit_sx);
        //�ҵĹ�������ȥ������˵ĸ��ʣ��������ǵĹ������ֵ���ֵ
        var AT = hurtat - hurtat * hit_sx.DB;
        hit_sx.DP -= AT;
        if (hit_sx.DP < 0)
        {
            float at = hit_sx.DP * -1;
            hit_sx.Cur_HP += hit_sx.DP;
            hit_sx.DP = 0;

            //���������ǹ���ͼ�¼����������ͳ������˺�������ù��������
            if (EntityManager.HasComponent<Monster>(hit))
            {   //����������������EntityOpenID(����ҵ�ʿ��)�ż�¼
                if(EntityManager.HasComponent<OwnerData>(entity))
                {
                    var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;
                    if (EntityManager.HasComponent<EntityOpenID>(shibing))
                    {
                        var EntiOpenID = EntityManager.GetComponentData<EntityOpenID>(shibing);
                        monsterMager.MonsterRecordPlayerAT(EntiOpenID, hit, at, EntityManager);
                    }
                }
            }
        }

        EntityManager.SetComponentData(hit, hit_sx);

        //�������������¼������
        if (EntityManager.HasComponent<UpSkill>(hit))
        {
            UpDataComponentLookup();
            if (m_UpSkill[hit].upSkill_Name == UpSkillName.GangQiu)
            {
                var upSkill = m_UpSkill[hit];
                upSkill.InjuryRecord += AT;
                EntityManager.SetComponentData(hit, upSkill);
            }
        }

        if (hit_sx.Cur_HP <= 0)
        {
            if (!EntityManager.HasComponent<Die>(hit))
            {
                EntityManager.AddComponentData(hit, new Die
                {
                    DeadParticle = m_shibing[hit].DeadParticle,
                    DeadPoint = m_shibing[hit].DeadPoint,
                    DeadLanguage = m_shibing[hit].DeadLanguage,
                    Is_LanguageNoBullet = m_shibing[hit].Is_LanguageNoBullet,
                });
                m_SX.Update(this);
                m_shibing.Update(this);
            }

            //����Ŀ���һ�û���
            if(EntityManager.HasComponent<OwnerData>(entity))
            {
                var shibing = EntityManager.GetComponentData<OwnerData>(entity).Owner;//���Box��ӵ����(����ʿ��)
                if (!EntityManager.HasComponent<Integral>(shibing))
                    return;
                var integralCompent = EntityManager.GetComponentData<Integral>(shibing);//�õ�ʿ������
                var HitSXCompent = EntityManager.GetComponentData<SX>(hit);//�õ����Ҵ������˵��������ֵ(����ֵ=����)
                integralCompent.ATIntegral += HitSXCompent.HP;
                EntityManager.SetComponentData(shibing, integralCompent);
            }
        }
    }


}
