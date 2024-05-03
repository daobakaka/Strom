
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class DetectionSystem : SystemBase
{
    ComponentLookup<SX> m_sx;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<Idle> m_idle;
    ComponentLookup<Walk> m_walk;
    ComponentLookup<Move> m_move;
    ComponentLookup<Fire> m_fire;
    ComponentLookup<Attack> m_attack;
    ComponentLookup<JiDiPoint> m_JiDiPoint;
    ComponentLookup<JiDi> m_JiDi;
    ComponentLookup<BarrageCommand> m_BarrageCommand;
    BufferLookup<JiDiPointBuffer> m_JiDiPointBuffer;
    protected override void OnCreate()
    {
        m_sx = GetComponentLookup<SX>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_idle = GetComponentLookup<Idle>(true);
        m_walk = GetComponentLookup<Walk>(true);
        m_move = GetComponentLookup<Move>(true);
        m_fire = GetComponentLookup<Fire>(true);
        m_attack = GetComponentLookup<Attack>(true);
        m_JiDiPoint = GetComponentLookup<JiDiPoint>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
        m_BarrageCommand = GetComponentLookup<BarrageCommand>(true);
        m_JiDiPointBuffer = GetBufferLookup<JiDiPointBuffer>(true);
    }
    protected override void OnUpdate()
    {
        if (TeamManager.teamManager == null)
            return;
        TeamManager.teamManager.Cur_DetectionNum -= 1;
        if (TeamManager.teamManager.Cur_DetectionNum > 0)
            return;
        else
            TeamManager.teamManager.Cur_DetectionNum = TeamManager.teamManager.DetectionNum;

        m_sx.Update(this);
        m_LocaltoWorld.Update(this);
        m_idle.Update(this);
        m_walk.Update(this);
        m_move.Update(this);
        m_fire.Update(this);
        m_attack.Update(this);
        m_JiDiPoint.Update(this);
        m_JiDi.Update(this);
        m_BarrageCommand.Update(this);
        m_JiDiPointBuffer.Update(this);

        var jcECB = new EntityCommandBuffer(Allocator.TempJob);

        //��ⷶΧ ==================================================================
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//����һ����ײ�����Ҫ�ĵ���

        var detectionjob = new DetectionJob
        {
            ECB = jcECB.AsParallelWriter(),
            physicsWorld = physicsWorld,
            LocaltoWorld = m_LocaltoWorld,
            walk = m_walk,
            attack = m_attack,
            sx = m_sx,
            jidiPoint = m_JiDiPoint,
            jidi = m_JiDi,
            jidiPointbuffer = m_JiDiPointBuffer,
            barrageCommand = m_BarrageCommand,
        };
        Dependency = detectionjob.ScheduleParallel(Dependency);

        Dependency.Complete();
        jcECB.Playback(EntityManager);
        jcECB.Dispose();

    }

}

[BurstCompile]
public partial struct DetectionJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public PhysicsWorldSingleton physicsWorld;//���Ǿ�̬������Ҫ���ⲿ������
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld; // ���ڻ�ȡtransform�������
    [ReadOnly] public ComponentLookup<Walk> walk;
    [ReadOnly] public ComponentLookup<Attack> attack;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<JiDiPoint> jidiPoint;
    [ReadOnly] public ComponentLookup<JiDi> jidi;
    [ReadOnly] public ComponentLookup<BarrageCommand> barrageCommand;
    [ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    private void Execute(Detection detec, ShiBingAspects shibing, MyLayerAspects mylayer, [ChunkIndexInQuery] int chunkIndx)
    {
        if(shibing.ACT == ActState.Fire || shibing.ACT == ActState.NotMove || shibing.ACT == ActState.Appear)//��������ڲ��Ź��������Ͳ��ɱ���ϣ��������һ������;����Ƕ���Ҳ�˳�������г���Ҳ�˳�
            return;
        //����й�������Ŀ����˳�
        if(barrageCommand.TryGetComponent(shibing.ShiBing_Entity,out BarrageCommand bc))
        {
            if (barrageCommand[shibing.ShiBing_Entity].command != commandType.NUll)
                return;
        }

        if (!LocaltoWorld.TryGetComponent(shibing.enemyJiDi, out LocalToWorld ltww) && shibing.Name != ShiBingName.TOWER
            && !shibing.Is_Parasitic && mylayer.BelongsTo != layer.Neutral)//û�������ؾ��˳�
            return;

        //ShootEntity�Ƿ�����
        if (!LocaltoWorld.TryGetComponent(shibing.ShootEntity, out LocalToWorld ltw))
        {
            shibing.ShootEntity = Entity.Null;
        }

        if (shibing.ShootEntity != Entity.Null)
        {
            //ShootEntity�Ƿ񳬳�������Χ
            if (math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[shibing.ShootEntity].Position) > sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis * 1.3f)
                shibing.ShootEntity = Entity.Null;
            //ShootEntity�Ƿ�Ϊ���أ�����ǣ����ټ�����ȹ���������󹥻�����
            if (jidi.TryGetComponent(shibing.ShootEntity, out JiDi jidi2))
                shibing.ShootEntity = Entity.Null;
        }
        if (shibing.ShootEntity != Entity.Null)//����й���Ŀ�꣬�ҹ���Ŀ�껹�ھͲ��������Ŀ��
            return;

        //�����Ƿ��ез���λ���� ���� ��Χ====================================================================
        shibing.TarEntity = Entity.Null;
        shibing.ShootEntity = Entity.Null;
        NativeList<ColliderCastHit> outHitsAttack = new NativeList<ColliderCastHit>(Allocator.Temp);
        if (DetectionTar(mylayer, shibing, ref outHitsAttack, sx[shibing.ShiBing_Entity].Tardis))
        {
            //����Ŀ����ڼ��Ŀ�꣬���������Χ��Ŀ��Ͳ�׷֮ǰ��ⷶΧ���Ŀ��
            //GetMaxThreat(outHitsAttack, shibing,ref shibing.TarEntity, shibing.ShootEntity, LocaltoWorld);//��ȡ���ȹ���Ŀ��
            Entity MaxDistance = Entity.Null;
            float maxdis = 0;

            //��ȡ��С�����Entity---------------------------------
            foreach (var item in outHitsAttack)//ɸѡһ���Ѿ���ɾ����ʵ��
            {
                if (LocaltoWorld.TryGetComponent(item.Entity, out LocalToWorld lTW))
                {
                    //�Ƿ�ԿնԵ�
                    if (sx[shibing.ShiBing_Entity].Anti_SX == SkyGround.Anti_Surface && sx[item.Entity].Is_AirForce)
                        continue;

                    if (MaxDistance == Entity.Null)
                    {
                        MaxDistance = item.Entity;
                        maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[MaxDistance].Position);
                        if (sx.TryGetComponent(MaxDistance, out SX sx1))
                        {
                            var sxx = sx[MaxDistance];
                            if (sxx.Is_AirForce)//���Ϊ�վ���������̽�����ͼ�44��44 * 44��
                                maxdis -= 100;//1936;   //��С����е�λYΪ10(10 * 10)
                            else            //ÿ����λ��һ��������룬�ý�ս��λ�Ĺ������루maxdis����ȥ���������룬
                                            //�Ͳ��ý�ս��λ�ܽ������λ��ԭ����
                            {
                                //maxdis - ����Ŀ���������롣���Լ����������
                                maxdis -= sxx.VolumetricDistance * sxx.VolumetricDistance;
                                maxdis -= sx[shibing.ShiBing_Entity].VolumetricDistance * sx[shibing.ShiBing_Entity].VolumetricDistance;
                            }
                        }
                    }
                    else
                    {
                        float team_maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[item.Entity].Position);
                        if (sx.TryGetComponent(item.Entity, out SX sx1))
                        {
                            var sxx = sx[item.Entity];
                            if (sxx.Is_AirForce)//���Ϊ�վ���������̽�����ͼ�44��44 * 44��
                                team_maxdis -= 100;//1936;    //��С����е�λYΪ10(10 * 10)
                            else            //ÿ����λ��һ��������룬�ý�ս��λ�Ĺ������루maxdis����ȥ���������룬
                                            //�Ͳ��ý�ս��λ�ܽ������λ��ԭ����
                            {
                                //maxdis - ����Ŀ���������롣���Լ����������
                                team_maxdis -= sxx.VolumetricDistance * sxx.VolumetricDistance;
                                team_maxdis -= sx[shibing.ShiBing_Entity].VolumetricDistance * sx[shibing.ShiBing_Entity].VolumetricDistance;
                            }
                        }
                        if (team_maxdis < maxdis)
                        {
                            maxdis = team_maxdis;
                            MaxDistance = item.Entity;
                        }
                    }
                    //if (jidi.TryGetComponent(MaxDistance, out JiDi jd))//��������λ�ǻ��أ���ֱ���ƹ�TarEntityΪ����
                    //    Is_jidi = true;
                }
            }


            //if (sx.TryGetComponent(MaxDistance, out SX sx1))
            //{
            //    var sxx = sx[MaxDistance];
            //    if (sxx.Is_AirForce)//���Ϊ�վ���������̽�����ͼ�44��44 * 44��
            //        maxdis -= 1936;
            //    else            //ÿ����λ��һ��������룬�ý�ս��λ�Ĺ������루maxdis����ȥ���������룬
            //                    //�Ͳ��ý�ս��λ�ܽ������λ��ԭ����
            //    {
            //        //maxdis - ����Ŀ����������
            //        maxdis -= sxx.VolumetricDistance * sxx.VolumetricDistance;
            //    }
            //}
            //if(ShiBingEnti != Entity.Null)
            //{
            //    MaxDistance = ShiBingEnti;
            //    maxdis = ShiBingmaxdix;
            //    Debug.Log("       ѡ��ĵ�λ�ǣ�" + MaxDistance + " �����ǣ�" + maxdis);
            //}

            if (maxdis < sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis)
            {
                shibing.ShootEntity = MaxDistance;
            }
            else if (maxdis < sx[shibing.ShiBing_Entity].Tardis * sx[shibing.ShiBing_Entity].Tardis)
            {
                shibing.TarEntity = MaxDistance;
            }

            //���Ϊ���ؾ�ѡ��һ��������
            if (jidi.TryGetComponent(MaxDistance, out JiDi jidi1))
            {
                Entity TarEnti = Entity.Null;
                TarEnti = ChooseJiDiPoint(shibing.ShiBing_Entity, MaxDistance);//ѡ�����Լ�����Ļ��ص�
                if (TarEnti != Entity.Null)
                {
                    shibing.TarEntity = TarEnti;
                    shibing.JidiPoint = TarEnti;
                }
            }

        }
        outHitsAttack.Dispose();

        //�������Ŀ��Ϊ����,�����Ѿ���ȡ�����ص�ĳ���㣬
        //������ֱ�ӱȽ��Լ��������ľ��룬�Ӷ��������Լ������������ص㣬�����ǻ���
        if (jidiPoint.TryGetComponent(shibing.JidiPoint, out JiDiPoint jdp))//����Ҫ��Ϊ�˽�ս��λ�������ص�ĳ���㣩
        {
            float maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[shibing.JidiPoint].Position);
            if (maxdis < sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis)
                shibing.ShootEntity = jidiPoint[shibing.JidiPoint].Jidi;
        }

        //������Ϊ���-----------------------------------------------------
        if (shibing.ShootEntity != Entity.Null)
        {
            ClearAllComponents(shibing.ShiBing_Entity, shibing.ACT, chunkIndx);
            ECB.SetComponentEnabled<Fire>(chunkIndx, shibing.ShiBing_Entity, true);
            return;
        }
        else if (shibing.TarEntity != Entity.Null)
        {
            ClearAllComponents(shibing.ShiBing_Entity, shibing.ACT, chunkIndx);
            ECB.SetComponentEnabled<Move>(chunkIndx, shibing.ShiBing_Entity, true);
            return;
        }
        if (!walk.IsComponentEnabled(shibing.ShiBing_Entity))//û������walk������walk
        {
            ClearAllComponents(shibing.ShiBing_Entity, shibing.ACT, chunkIndx);
            ECB.SetComponentEnabled<Walk>(chunkIndx, shibing.ShiBing_Entity, true);
        }


    }

    //����Ƿ���Ŀ��
    bool DetectionTar(MyLayerAspects layer,ShiBingAspects shibing,ref NativeList<ColliderCastHit> outHits,float radius)
    {
        uint collidesWithMask = 0;
        if ((int)layer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)layer.CollidesWith_1);
        if ((int)layer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)layer.CollidesWith_2);
        if ((int)layer.BulletCollidesWith != 0)//�ж������λ�Ƿ����ӵ����
            collidesWithMask |= (1u << (int)layer.BulletCollidesWith);

        CollisionFilter filterDetection = new CollisionFilter()
        {
            BelongsTo = (1u << (int)layer.BelongsTo),
            CollidesWith = collidesWithMask,//(1u << (int)layer.CollidesWith),
            GroupIndex = 0
        };

        bool b = physicsWorld.SphereCastAll(LocaltoWorld[shibing.ShiBing_Entity].Position, radius, Vector3.up, 10, ref outHits, filterDetection);
        return b;
    }

    //ɸѡ��вֵ����
    void GetMaxThreat(NativeList<ColliderCastHit> hits, ShiBingAspects shibing,ref Entity Tarentity ,  Entity Shootentity, ComponentLookup<LocalToWorld> ltoworld)
    {
        Entity MaxDistance = Entity.Null;
        float maxdis = 0;
        //��ȡ��С�����Entity---------------------------------

        //ɸѡһ���Ѿ���ɾ����ʵ��
        foreach (var item in hits)
        {
            if (LocaltoWorld.TryGetComponent(item.Entity, out LocalToWorld lTW))
            {
                if(MaxDistance == Entity.Null)
                    MaxDistance = item.Entity;
                else
                {
                    maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, ltoworld[MaxDistance].Position);
                    float team_maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, ltoworld[item.Entity].Position);
                    if (team_maxdis < maxdis)
                    {
                        maxdis = team_maxdis;
                        MaxDistance = item.Entity;
                    }
                }
            }
        }
        Shootentity = Entity.Null;
        Tarentity = Entity.Null;
        if (MaxDistance == Entity.Null)
            return;

        if (maxdis < 4 * 4)
            Shootentity = MaxDistance;
        else
            Tarentity = MaxDistance;

    }

    //ɾ��ȫ����Ϊ���
    void ClearAllComponents(Entity entity, ActState act, int chunkIndx)
    {
        if (act == ActState.NULL)
            return;
        //if(act == ActState.Idle)
        //    ECB.SetComponentEnabled<Idle>(chunkIndx, entity, false);
        //else if (act == ActState.Walk)
        //    ECB.SetComponentEnabled<Walk>(chunkIndx, entity, false);
        //else if (act == ActState.Move)
        //    ECB.SetComponentEnabled<Move>(chunkIndx, entity, false);
        //else if (act == ActState.Fire)
        //    ECB.SetComponentEnabled<Fire>(chunkIndx, entity, false);

        ECB.SetComponentEnabled<Idle>(chunkIndx, entity, false);
        ECB.SetComponentEnabled<Walk>(chunkIndx, entity, false);
        ECB.SetComponentEnabled<Move>(chunkIndx, entity, false);
        ECB.SetComponentEnabled<Fire>(chunkIndx, entity, false);
    }

    //ѡ����������Լ�����ĵ�
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)
    {
        Entity selectedPoint = Entity.Null;//ѡ�е����Entity
        float distan = float.MaxValue;
        var EntiWorldPos = LocaltoWorld[entity].Position;
        foreach (JiDiPointBuffer jidiBuffer in jidiPointbuffer[jidiEnti])
        {
            var jidiPointEntiWorldPos = LocaltoWorld[jidiBuffer.PointEntity].Position;
            jidiPointEntiWorldPos.z = LocaltoWorld[jidiEnti].Position.z;
            float dis = math.distance(EntiWorldPos, jidiPointEntiWorldPos);
            if (dis < distan)
            {
                distan = dis;
                selectedPoint = jidiBuffer.PointEntity;
            }

        }
        return selectedPoint;
    }
}

