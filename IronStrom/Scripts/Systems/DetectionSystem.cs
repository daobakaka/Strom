
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

        //检测范围 ==================================================================
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//创建一个碰撞检测需要的单例

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
    [ReadOnly] public PhysicsWorldSingleton physicsWorld;//不是静态方法需要从外部传进来
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld; // 用于获取transform组件数据
    [ReadOnly] public ComponentLookup<Walk> walk;
    [ReadOnly] public ComponentLookup<Attack> attack;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<JiDiPoint> jidiPoint;
    [ReadOnly] public ComponentLookup<JiDi> jidi;
    [ReadOnly] public ComponentLookup<BarrageCommand> barrageCommand;
    [ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    private void Execute(Detection detec, ShiBingAspects shibing, MyLayerAspects mylayer, [ChunkIndexInQuery] int chunkIndx)
    {
        if(shibing.ACT == ActState.Fire || shibing.ACT == ActState.NotMove || shibing.ACT == ActState.Appear)//如果是正在播放攻击动画就不可被打断，必须把这一发打完;如果是定身也退出，如果有出场也退出
            return;
        //如果有攻击命令目标就退出
        if(barrageCommand.TryGetComponent(shibing.ShiBing_Entity,out BarrageCommand bc))
        {
            if (barrageCommand[shibing.ShiBing_Entity].command != commandType.NUll)
                return;
        }

        if (!LocaltoWorld.TryGetComponent(shibing.enemyJiDi, out LocalToWorld ltww) && shibing.Name != ShiBingName.TOWER
            && !shibing.Is_Parasitic && mylayer.BelongsTo != layer.Neutral)//没攻击基地就退出
            return;

        //ShootEntity是否死亡
        if (!LocaltoWorld.TryGetComponent(shibing.ShootEntity, out LocalToWorld ltw))
        {
            shibing.ShootEntity = Entity.Null;
        }

        if (shibing.ShootEntity != Entity.Null)
        {
            //ShootEntity是否超出攻击范围
            if (math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[shibing.ShootEntity].Position) > sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis * 1.3f)
                shibing.ShootEntity = Entity.Null;
            //ShootEntity是否为基地，如何是，就再检测优先攻击敌人最后攻击基地
            if (jidi.TryGetComponent(shibing.ShootEntity, out JiDi jidi2))
                shibing.ShootEntity = Entity.Null;
        }
        if (shibing.ShootEntity != Entity.Null)//如果有攻击目标，且攻击目标还在就不检测其他目标
            return;

        //附件是否有敌方单位进入 攻击 范围====================================================================
        shibing.TarEntity = Entity.Null;
        shibing.ShootEntity = Entity.Null;
        NativeList<ColliderCastHit> outHitsAttack = new NativeList<ColliderCastHit>(Allocator.Temp);
        if (DetectionTar(mylayer, shibing, ref outHitsAttack, sx[shibing.ShiBing_Entity].Tardis))
        {
            //攻击目标大于检测目标，如果攻击范围有目标就不追之前检测范围里的目标
            //GetMaxThreat(outHitsAttack, shibing,ref shibing.TarEntity, shibing.ShootEntity, LocaltoWorld);//获取优先攻击目标
            Entity MaxDistance = Entity.Null;
            float maxdis = 0;

            //获取最小距离的Entity---------------------------------
            foreach (var item in outHitsAttack)//筛选一遍已经被删除的实体
            {
                if (LocaltoWorld.TryGetComponent(item.Entity, out LocalToWorld lTW))
                {
                    //是否对空对地
                    if (sx[shibing.ShiBing_Entity].Anti_SX == SkyGround.Anti_Surface && sx[item.Entity].Is_AirForce)
                        continue;

                    if (MaxDistance == Entity.Null)
                    {
                        MaxDistance = item.Entity;
                        maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[MaxDistance].Position);
                        if (sx.TryGetComponent(MaxDistance, out SX sx1))
                        {
                            var sxx = sx[MaxDistance];
                            if (sxx.Is_AirForce)//如果为空军攻击距离探测距离就加44（44 * 44）
                                maxdis -= 100;//1936;   //缩小后空中单位Y为10(10 * 10)
                            else            //每个单位有一个体积距离，让近战单位的攻击距离（maxdis）减去这个体积距离，
                                            //就不用近战单位跑进这个单位的原点了
                            {
                                //maxdis - 攻击目标的体积距离。和自己的体积距离
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
                            if (sxx.Is_AirForce)//如果为空军攻击距离探测距离就加44（44 * 44）
                                team_maxdis -= 100;//1936;    //缩小后空中单位Y为10(10 * 10)
                            else            //每个单位有一个体积距离，让近战单位的攻击距离（maxdis）减去这个体积距离，
                                            //就不用近战单位跑进这个单位的原点了
                            {
                                //maxdis - 攻击目标的体积距离。和自己的体积距离
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
                    //if (jidi.TryGetComponent(MaxDistance, out JiDi jd))//如果这个单位是基地，就直接绕过TarEntity为基地
                    //    Is_jidi = true;
                }
            }


            //if (sx.TryGetComponent(MaxDistance, out SX sx1))
            //{
            //    var sxx = sx[MaxDistance];
            //    if (sxx.Is_AirForce)//如果为空军攻击距离探测距离就加44（44 * 44）
            //        maxdis -= 1936;
            //    else            //每个单位有一个体积距离，让近战单位的攻击距离（maxdis）减去这个体积距离，
            //                    //就不用近战单位跑进这个单位的原点了
            //    {
            //        //maxdis - 攻击目标的体积距离
            //        maxdis -= sxx.VolumetricDistance * sxx.VolumetricDistance;
            //    }
            //}
            //if(ShiBingEnti != Entity.Null)
            //{
            //    MaxDistance = ShiBingEnti;
            //    maxdis = ShiBingmaxdix;
            //    Debug.Log("       选择的单位是：" + MaxDistance + " 距离是：" + maxdis);
            //}

            if (maxdis < sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis)
            {
                shibing.ShootEntity = MaxDistance;
            }
            else if (maxdis < sx[shibing.ShiBing_Entity].Tardis * sx[shibing.ShiBing_Entity].Tardis)
            {
                shibing.TarEntity = MaxDistance;
            }

            //如果为基地就选择一个攻击点
            if (jidi.TryGetComponent(MaxDistance, out JiDi jidi1))
            {
                Entity TarEnti = Entity.Null;
                TarEnti = ChooseJiDiPoint(shibing.ShiBing_Entity, MaxDistance);//选择离自己最近的基地点
                if (TarEnti != Entity.Null)
                {
                    shibing.TarEntity = TarEnti;
                    shibing.JidiPoint = TarEnti;
                }
            }

        }
        outHitsAttack.Dispose();

        //如果检测的目标为基地,并且已经获取到基地的某个点，
        //在这里直接比较自己和这个点的距离，从而攻击离自己最近的这个基地点，而不是基地
        if (jidiPoint.TryGetComponent(shibing.JidiPoint, out JiDiPoint jdp))//（主要是为了近战单位攻击基地的某个点）
        {
            float maxdis = math.distancesq(LocaltoWorld[shibing.ShiBing_Entity].Position, LocaltoWorld[shibing.JidiPoint].Position);
            if (maxdis < sx[shibing.ShiBing_Entity].Shootdis * sx[shibing.ShiBing_Entity].Shootdis)
                shibing.ShootEntity = jidiPoint[shibing.JidiPoint].Jidi;
        }

        //分配行为组件-----------------------------------------------------
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
        if (!walk.IsComponentEnabled(shibing.ShiBing_Entity))//没有启用walk就启用walk
        {
            ClearAllComponents(shibing.ShiBing_Entity, shibing.ACT, chunkIndx);
            ECB.SetComponentEnabled<Walk>(chunkIndx, shibing.ShiBing_Entity, true);
        }


    }

    //检测是否有目标
    bool DetectionTar(MyLayerAspects layer,ShiBingAspects shibing,ref NativeList<ColliderCastHit> outHits,float radius)
    {
        uint collidesWithMask = 0;
        if ((int)layer.CollidesWith_1 != 0)
            collidesWithMask = (1u << (int)layer.CollidesWith_1);
        if ((int)layer.CollidesWith_2 != 0)
            collidesWithMask |= (1u << (int)layer.CollidesWith_2);
        if ((int)layer.BulletCollidesWith != 0)//判断这个单位是否有子弹检测
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

    //筛选威胁值最大的
    void GetMaxThreat(NativeList<ColliderCastHit> hits, ShiBingAspects shibing,ref Entity Tarentity ,  Entity Shootentity, ComponentLookup<LocalToWorld> ltoworld)
    {
        Entity MaxDistance = Entity.Null;
        float maxdis = 0;
        //获取最小距离的Entity---------------------------------

        //筛选一遍已经被删除的实体
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

    //删除全部行为组件
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

    //选择基地中离自己最近的点
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)
    {
        Entity selectedPoint = Entity.Null;//选中的最近Entity
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

