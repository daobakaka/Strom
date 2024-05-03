using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using GPUECSAnimationBaker.Engine.AnimatorSystem;


//[UpdateAfter()]在某个System之后执行
[BurstCompile]
public partial class ShiBingSystem : SystemBase
{
    ComponentLookup<LocalToWorld> m_localtoWorld;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<GpuEcsAnimatorControlComponent> m_GpuCtrl;
    ComponentLookup<GpuEcsAnimatorStateComponent> m_GpuState;
    ComponentLookup<SX> m_SX;
    ComponentLookup<Die> m_Die;
    ComponentLookup<EntityCtrl> m_entiCtrl;
    ComponentLookup<SynchronizeAni> m_SynchronizeAni;
    ComponentLookup<Monster> m_Monster;
    ComponentLookup<JiDi> m_JiDi;
    ComponentLookup<MyLayer> m_Mylayer;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<Integral> m_Integral;
    BufferLookup<JiDiPointBuffer> m_JiDiPointBuffer;


    void UpdataComponentLookup()
    {
        m_localtoWorld.Update(this);
        m_transform.Update(this);
        m_GpuCtrl.Update(this);
        m_GpuState.Update(this);
        m_SX.Update(this);
        m_Die.Update(this);
        m_entiCtrl.Update(this);
        m_SynchronizeAni.Update(this);
        m_JiDi.Update(this);
        m_Mylayer.Update(this);
        m_ShiBing.Update(this);
        m_Integral.Update(this);
        m_JiDiPointBuffer.Update(this);
    }

    protected override void OnCreate()
    {
        m_localtoWorld = GetComponentLookup<LocalToWorld>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_GpuCtrl = GetComponentLookup<GpuEcsAnimatorControlComponent>(true);
        m_GpuState = GetComponentLookup<GpuEcsAnimatorStateComponent>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_Die = GetComponentLookup<Die>(true);
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_SynchronizeAni = GetComponentLookup<SynchronizeAni>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
        m_Mylayer = GetComponentLookup<MyLayer>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_Integral = GetComponentLookup<Integral>(true);
        m_JiDiPointBuffer = GetBufferLookup<JiDiPointBuffer>(true);

    }
    protected override void OnUpdate()
    {
        UpdataComponentLookup();

        //var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        //var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例


        
        bool enter = false;
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            enter = true;

        //GpuECSAnimation
        //士兵idle的job行为==================================================
        var shibingidlejob = new ShiBingIdleJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
        };
        Dependency = shibingidlejob.ScheduleParallel(Dependency);

        //士兵walk的job行为==================================================
        var shibingwalkjob = new ShiBingWalkJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
            gpuState = m_GpuState,
            JIDI = m_JiDi,
            enter = enter,
        };
        Dependency = shibingwalkjob.ScheduleParallel(Dependency);

        //士兵move的job行为======================================================
        var shibingmovejob = new ShiBingMoveJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
            JIDI = m_JiDi,
            jidiPointbuffer = m_JiDiPointBuffer,
        };
        Dependency = shibingmovejob.ScheduleParallel(Dependency);

        //士兵fire的job行为========================================================
        var shibingfirejob = new ShiBingFireJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
            spawn = spawn,
            gpuState = m_GpuState,
            B_Fire = false,
            LocaltoWorld = m_localtoWorld,
            jidiPointbuffer = m_JiDiPointBuffer,
            JIDI = m_JiDi,
        };
        Dependency = shibingfirejob.ScheduleParallel(Dependency);

        //士兵检测自己是否死亡=====================================================
        var shibingIsDiejob = new ShiBingIsDieJob
        {
            ECB = ecbParallel,
            die = m_Die,
            integral = m_Integral,
        };
        Dependency = shibingIsDiejob.ScheduleParallel(Dependency);



        //EntityCtrl
        //无动画士兵的idle行为==========================================================
        var entiCtrlIdlejob = new EntityCtrlIdleJob
        {
            ECB = ecbParallel,
            gpuCtrl = m_GpuCtrl,
            transform = m_transform,
            sx = m_SX,
        };
        Dependency = entiCtrlIdlejob.ScheduleParallel(Dependency);

        //无动画士兵的Walk行为==========================================================
        var entiCtrlWalkjob = new EntityCtrlWalkJob
        {
            ECB = ecbParallel,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
            time = SystemAPI.Time.DeltaTime,
            JIDI = m_JiDi,
            enter = enter,
            myLayer = m_Mylayer,
        };
        Dependency = entiCtrlWalkjob.ScheduleParallel(Dependency);

        //无动画士兵的Move行为=========================================================
        var entiCtrlMovejob = new EntityCtrlMoveJob
        {
            ECB = ecbParallel,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            time = SystemAPI.Time.DeltaTime,
            gpuCtrl = m_GpuCtrl,
            JIDI = m_JiDi,
            myLayer = m_Mylayer,
            jidiPointbuffer = m_JiDiPointBuffer,
        };
        Dependency = entiCtrlMovejob.ScheduleParallel(Dependency);

        //无动画士兵的Fire行为=========================================================
        var entiCtrlFirejob = new EntityCtrlFireJob
        {
            ECB = ecbParallel,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            time = SystemAPI.Time.DeltaTime,
            spawn = spawn,
            gpuCtrl = m_GpuCtrl,
            gpuState = m_GpuState,
            SynAni = m_SynchronizeAni,
            JIDI = m_JiDi,
            myLayer = m_Mylayer,
            jidiPointbuffer = m_JiDiPointBuffer,
        };
        Dependency = entiCtrlFirejob.ScheduleParallel(Dependency);

        //登场行为=====================================================================
        var entiCtrlAppearJob = new EntityCtrlAppearJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            transform = m_transform,
            LocaltoWorld = m_localtoWorld,
            
        };
        Dependency = entiCtrlAppearJob.ScheduleParallel(Dependency);





        //无动画士兵的刷新炮台数据=====================================================
        //var paotaiUpjob = new PaoTaiUpJob
        //{
        //    ECB = ecbParallel,
        //    tansform = m_transform,
        //    entiCtrl = m_entiCtrl,
        //};
        //Dependency = paotaiUpjob.ScheduleParallel(Dependency);






        Dependency.Complete();
        ecb.Playback(EntityManager);//应用实体的修改
        ecb.Dispose();//手动释放new的Buffer
        var teamMager = TeamManager.teamManager;
        teamMager.OnGpuEntityNum = 0;
        teamMager.NoGpuEntityNum = 0;
        teamMager.UnityAniEntityNum = 0;
        teamMager.TotalEntityNum = 0;
        teamMager.BulletNum = 0;
        teamMager.AttackBoxNum = 0;
        teamMager.Tema1_LikeSoldierAllNum = 0;
        teamMager.Tema2_LikeSoldierAllNum = 0;

        UpdataComponentLookup();
        Entities.ForEach((Entity entity, GpuECSAni OnGpu) =>
        {
            teamMager.OnGpuEntityNum += 1;
            if (m_ShiBing[entity].Name == ShiBingName.JianYa)
            {
                if (m_Mylayer[entity].BelongsTo == layer.Team1)
                    teamMager.Tema1_LikeSoldierAllNum += 1;
                else if (m_Mylayer[entity].BelongsTo == layer.Team2)
                    teamMager.Tema2_LikeSoldierAllNum += 1;
            }

        }).WithBurst().WithStructuralChanges().Run();

        Entities.ForEach((Entity entity, EntityCtrl EntiCtrl) =>
        {
            if (EntityManager.HasComponent<GpuECSAni>(entity)||
                EntityManager.HasComponent<SynchronizeAni>(entity))
                return;
            teamMager.NoGpuEntityNum += 1;

        }).WithBurst().WithStructuralChanges().Run();

        Entities.ForEach((Entity entity, EntityCtrl entiCtrl, SynchronizeAni SyAni) =>
        {
            teamMager.UnityAniEntityNum += 1;

        }).WithBurst().WithStructuralChanges().Run();

        Entities.ForEach((Entity entity, ShiBing shibing) =>
        {
            teamMager.TotalEntityNum += 1;

        }).WithBurst().WithStructuralChanges().Run();

        Entities.ForEach((Entity entity, Bullet bullet) =>
        {
            teamMager.BulletNum += 1;

        }).WithBurst().WithStructuralChanges().Run();

        Entities.ForEach((Entity entity, AttackBox attackBox) =>
        {
            teamMager.AttackBoxNum += 1;

        }).WithBurst().WithStructuralChanges().Run();

    }
}

//GpuECSAnimation，由ECSAnimation控制动画的角色job=============
[BurstCompile]
partial struct ShiBingIdleJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;

    public float time;
    void Execute(GpuECSAni gpuAni, Idle act, ShiBingAspects shibingAsp, [ChunkIndexInQuery] int chunkIndx)
    {
        //判断是否有这个entity,如果没有entity和这个组件，就会返回false
        //if (!LocaltoWorld.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalToWorld ltw))
        //{
        //    Debug.Log("                         ShiBing_Entity为空");
        //    return;
        //}
        //var pos = transform[shibingAsp.ShiBing_Entity];
        //pos.Position += shibingAsp.Speed * shibingAsp.ShiBingInitDir * time;
        //ECB.SetComponent(chunkIndx,shibingAsp.ShiBing_Entity, pos);
        if(shibingAsp.ACT == ActState.NotMove)
        {
            int index = shibingAsp.GetAnimIndex(ActState.Idle);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
            ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, animCtrl);
            return;
        }

        if(shibingAsp.ACT != ActState.Idle)
        {
            int index = shibingAsp.GetAnimIndex(ActState.Idle);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
            ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, animCtrl);
            shibingAsp.ACT = ActState.Idle;
        }


    }
}
[BurstCompile]
partial struct ShiBingWalkJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<GpuEcsAnimatorStateComponent> gpuState;  //Current Normalized Time
    [Unity.Collections.ReadOnly] public ComponentLookup<JiDi> JIDI;

    public bool enter;
    public float time;
    void Execute(GpuECSAni gpuAni, Walk act, ShiBingAspects shibingAsp,SXAspects sx ,[ChunkIndexInQuery] int chunkIndx)
    {
        //基地没了就idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) && !shibingAsp.Is_Parasitic)
        {
            ECB.SetComponentEnabled<Idle>(chunkIndx, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            return;
        }
        //判断是否有这个entity,如果没有entity和这个组件，就会返回false
        if (!LocaltoWorld.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalToWorld ltw))
        {
            return;
        }

        if(shibingAsp.ACT != ActState.Walk || sx.Is_ChangedAinWalkSpeed || enter)//不是移动就播放移动动画
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//却换动画
            ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, animCtrl);
            shibingAsp.ACT = ActState.Walk;
            sx.Is_ChangedAinWalkSpeed = false;
        }



        //修正面朝敌方基地的方向
        float3 direnjidiPos = LocaltoWorld[JIDI[shibingAsp.enemyJiDi].WalkPoint].Position;
        float3 shibingPos = LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        var vdir = direnjidiPos - shibingPos;
        vdir.y = 0;
        shibingAsp.ShiBingInitDir = math.normalize(vdir);
        
        var pos = transform[shibingAsp.ShiBing_Entity];

        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//获得希望的面朝向
        pos.Rotation = math.slerp(pos.Rotation, targetRotation, 5f * time);// 插值旋转

        pos.Position += sx.Speed * shibingAsp.ShiBingInitDir * time;//朝地方基地移动
        pos.Position.y = 0;//限制士兵不能在空中

        ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, pos);




    }
}

[BurstCompile]
public partial struct ShiBingMoveJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<JiDi> JIDI;
    [Unity.Collections.ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    public float time;

    void Execute(GpuECSAni gpuAni, Move act, ShiBingAspects shibingAsp,SXAspects sx, [ChunkIndexInQuery] int chunkIndex)
    {
        ////如果检测到的目标为基地，就选离自己最近的基地点进行攻击
        //if (JIDI.TryGetComponent(shibingAsp.TarEntity, out JiDi jd))
        //{
        //    var jidiPointEnti = ChooseJiDiPoint(shibingAsp.ShiBing_Entity, shibingAsp.TarEntity);
        //    if (jidiPointEnti == Entity.Null)
        //        return;
        //    shibingAsp.TarEntity = jidiPointEnti;
        //}
        //基地没了就idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) && !shibingAsp.Is_Parasitic)
        {
            ECB.SetComponentEnabled<Idle>(chunkIndex, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(chunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(chunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(chunkIndex, shibingAsp.ShiBing_Entity, false);
            return;
        }
        if (!LocaltoWorld.TryGetComponent(shibingAsp.TarEntity,out LocalToWorld ltw))
        {
            shibingAsp.TarEntity = Entity.Null;
            //Debug.Log("                        " + shibingAsp.ShiBing_Entity +"未检测到目标 或 检测到的目标已不存在");
            return;
        }
 
        float3 vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];

        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//获得希望的面朝向
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转

        worldPos.Position += sx.Speed * vdir * time;
        ECB.SetComponent(chunkIndex, shibingAsp.ShiBing_Entity, worldPos);


        if (shibingAsp.ACT != ActState.Move || sx.Is_ChangedAinWalkSpeed)//不是移动就播放移动动画
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index, sx.Cur_AinWalkSpeed);//却换动画
            ECB.SetComponent(chunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Move;
        }
    }

    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//选择基地中离自己最近的点
    {
        Entity selectedPoint = Entity.Null;//选中的最近Entity
        float distan = float.MaxValue;
        var EntiWorldPos = LocaltoWorld[entity].Position;
        foreach(JiDiPointBuffer jidiBuffer in jidiPointbuffer[jidiEnti])
        {
            var jidiPointEntiWorldPos = LocaltoWorld[jidiBuffer.PointEntity].Position;
            jidiPointEntiWorldPos.z = LocaltoWorld[jidiEnti].Position.z;
            float dis = math.distance(EntiWorldPos, jidiPointEntiWorldPos);
            if(dis < distan)
            {
                distan = dis;
                selectedPoint = jidiBuffer.PointEntity;
            }

        }
        return selectedPoint;
    }


}
[BurstCompile]
public partial struct ShiBingFireJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorStateComponent> gpuState;  //Current Normalized Time
    [ReadOnly] public Spawn spawn;
    [ReadOnly] public ComponentLookup<JiDi> JIDI;

    public float time;
    public bool B_Fire;
    void Execute(GpuECSAni gpuAni, Fire act, ShiBingAspects shibing,SXAspects sx ,[ChunkIndexInQuery]int chunkIndex)
    {
        //基地没了就idle
        if (!transform.TryGetComponent(shibing.enemyJiDi, out LocalTransform ktf) && !shibing.Is_Parasitic)
        {
            ECB.SetComponentEnabled<Idle>(chunkIndex, shibing.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(chunkIndex, shibing.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(chunkIndex, shibing.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(chunkIndex, shibing.ShiBing_Entity, false);
            return;
        }
        //判断是否有这个entity,如果没有entity和这个组件，就会返回false
        if (!transform.TryGetComponent(shibing.ShootEntity, out LocalTransform ltf))
        {
            shibing.ShootEntity = Entity.Null;
            //sx.Cur_ShootTime = sx.ShootTime;
            if (sx.Cur_ShootTime <= 0)
                sx.Cur_ShootTime = sx.ShootTime;
            shibing.ACT = ActState.Ready;
            return;
        }
        //如果已经是死亡状态了
        if (sx.Is_Die)
        {
            shibing.ShootEntity = Entity.Null;
            sx.Cur_ShootTime = sx.ShootTime;
            shibing.ACT = ActState.Ready;
            return;
        }

        var shootEnti = shibing.ShootEntity;
        //如果检测到的目标为基地，就选离自己最近的基地点进行攻击
        if (JIDI.TryGetComponent(shibing.ShootEntity, out JiDi jd))
        {
            if(transform.TryGetComponent(shibing.JidiPoint, out LocalTransform ltf2))
                shootEnti = shibing.JidiPoint;
        }

        //修正面朝敌人的方向
        float3 vdir = LocaltoWorld[shootEnti].Position - LocaltoWorld[shibing.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibing.ShiBing_Entity];
        worldPos.Position.y = 0;//限制Y轴，不要挤到别人的头上
        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//获得希望的面朝向
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转
        ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, worldPos);

        //计算攻击间隔
        sx.Cur_ShootTime -= time;
        if (sx.Cur_ShootTime <= 0)//播放攻击动画
        {
            if (gpuState[shibing.ShiBing_Entity].currentNormalizedTime == 1f)
                sx.Cur_ShootTime = sx.ShootTime;
            if (shibing.ACT != ActState.Fire)
            {
                int index = shibing.GetAnimIndex(ActState.Fire);//Get 动画ID
                var animCtrl = gpuCtrl[shibing.ShiBing_Entity];
                shibing.RunAnimation(ref animCtrl, index);//却换动画
                ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, animCtrl);

                shibing.ACT = ActState.Fire;
            }
        }
        else//播放瞄准动画
        {
            if (shibing.ACT != ActState.Ready)//播放动画
            {
                int index = shibing.GetAnimIndex(ActState.Ready);//Get Ready的动画ID
                var animCtrl = gpuCtrl[shibing.ShiBing_Entity];
                shibing.RunAnimation(ref animCtrl, index);//却换动画
                ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, animCtrl);

                shibing.ACT = ActState.Ready;
            }
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//选择基地中离自己最近的点
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


//GpuECSAnimation，由ECSAnimation控制动画的角色job=============

[BurstCompile]
public partial struct EntityCtrlIdleJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<SX> sx;
    void Execute(ShiBingAspects shibingAsp, EntityCtrl entiCtrl, Idle idle, [ChunkIndexInQuery] int Chunkindex)
    {
        if (shibingAsp.ACT == ActState.NotMove)
        {
            if(entiCtrl.EntityCtrlAni_Idle)
            {
                int index = shibingAsp.GetAnimIndex(ActState.Idle);
                var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
                ECB.SetComponent(Chunkindex, shibingAsp.ShiBing_Entity, animCtrl);
            }
            return;
        }
        if (sx[shibingAsp.ShiBing_Entity].Is_AirForce)
        {
            var Pos = transform[shibingAsp.ShiBing_Entity];
            Pos.Position.y = 10;//50;
            ECB.SetComponent(Chunkindex, shibingAsp.ShiBing_Entity, Pos);
        }


        shibingAsp.ACT = ActState.Idle;
    }
}

[BurstCompile]
public partial struct EntityCtrlWalkJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [Unity.Collections.ReadOnly] public ComponentLookup<JiDi> JIDI;
    [Unity.Collections.ReadOnly] public ComponentLookup<MyLayer> myLayer;
    public float time;

    public bool enter;
    void Execute(ShiBingAspects shibingAsp, EntityCtrl entiCtrl,ref SX sx, Walk walk, [ChunkIndexInQuery] int ChunkIndex)
    {
        if (shibingAsp.Is_Parasitic)//如果为寄生单位就不需要移动
            return;
        //基地没了就idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) &&
            shibingAsp.Name != ShiBingName.TOWER && myLayer[shibingAsp.ShiBing_Entity].BelongsTo != layer.Neutral)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            return;
        }

        if (!transform.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalTransform ltf))
        {
            return;
        }
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ltf1))//没有目标基地
        {
            shibingAsp.ACT = ActState.Idle;
            return;
        }

        //修正面朝敌方基地的方向
        float3 direnjidiPos = LocaltoWorld[JIDI[shibingAsp.enemyJiDi].WalkPoint].Position;
        float3 shibingPos = LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        var vdir = direnjidiPos - shibingPos;
        vdir.y = 0;
        shibingAsp.ShiBingInitDir = math.normalize(vdir);

        //士兵移动修正车身用shibingEntity，修正面朝向
        var pos = transform[shibingAsp.ShiBing_Entity];
        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//获得希望的面朝向
        pos.Rotation = math.slerp(pos.Rotation, targetRotation, 5f * time);// 插值旋转

        pos.Position += sx.Speed * shibingAsp.ShiBingInitDir * time;//朝地方基地移动
        pos.Position.y = 0;//限制士兵不能在空中

        if ((entiCtrl.EntityCtrlAni_Walk && shibingAsp.ACT != ActState.Walk) || sx.Is_ChangedAinWalkSpeed || enter)//如果有需要播放的动画就播放
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);//Get 动画ID
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//却换动画
            ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Walk;
        }
        else if(entiCtrl.Is_TraditionalAnimation)
            shibingAsp.ACT = ActState.Walk;

        //========是否有左右旋转的动画===
        //if (shibingAsp.Is_Ani_MoveRL)
        //    PlayAniMoveRL(ref pos, shibingAsp, in targetRotation, ChunkIndex);

        //========如果是空军单位=========
        if (sx.Is_AirForce)
            AirForce(ref pos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, pos);
    }
    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // 获取当前前向向量和目标前向向量
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // 计算两个向量之间的角度
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // 使用叉积判断旋转方向
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// 向左旋转
                zAngle = -40f;
            else
                zAngle = 40f;// 向右旋转
            // 应用Z轴旋转
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // 插值旋转
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }

    void PlayAniMoveRL(ref LocalTransform pos,ShiBingAspects shibingAsp, in quaternion targetRotation,int ChunkIndex)
    {
        // 获取当前前向向量和目标前向向量
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // 计算两个向量之间的角度
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y < 0)// 向左旋转
            {
                int index = shibingAsp.GetAnimIndex(ActState.Walk_L);//Get 动画ID
                var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
                ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            }
            else
            {
                int index = shibingAsp.GetAnimIndex(ActState.Walk_R);//Get 动画ID
                var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
                ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            }
        }
    }
}

[BurstCompile]
public partial struct EntityCtrlMoveJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [ReadOnly] public ComponentLookup<JiDi> JIDI;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    public float time;
    void Execute(ShiBingAspects shibingAsp,ref EntityCtrl entiCtrl,ref SX sx, Move move, [ChunkIndexInQuery] int ChunkIndex)
    {
        ////如果检测到的目标为基地，就选离自己最近的基地点进行攻击
        //if (JIDI.TryGetComponent(shibingAsp.TarEntity, out JiDi jd))
        //{
        //    //var jidiPointEnti = ChooseJiDiPoint(shibingAsp.ShiBing_Entity, shibingAsp.TarEntity);
        //    //if (jidiPointEnti == Entity.Null)
        //    //    return;
        //    //shibingAsp.TarEntity = jidiPointEnti;
        //}
        if (shibingAsp.Is_Parasitic)//如果为寄生单位就不需要移动
            return;
        //基地没了就idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) &&
            shibingAsp.Name != ShiBingName.TOWER && myLayer[shibingAsp.ShiBing_Entity].BelongsTo != layer.Neutral)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            return;
        }


        if (!LocaltoWorld.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalToWorld ltf) ||
            !LocaltoWorld.TryGetComponent(shibingAsp.TarEntity, out LocalToWorld ltf1))
        {
            return;
        }

        //车身修正面朝敌人的方向
        float3 vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        float3 CheShendir = math.normalize(vdir);
        CheShendir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];
        quaternion targetRotation = quaternion.LookRotationSafe(CheShendir, new float3(0, 1, 0));
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转

        //车身移动
        worldPos.Position += sx.Speed * CheShendir * time;
        worldPos.Position.y = 0;

        if ((entiCtrl.EntityCtrlAni_Walk && shibingAsp.ACT != ActState.Move) || sx.Is_ChangedAinWalkSpeed)//如果有需要播放的动画就播放
        {
            int index = shibingAsp.GetAnimIndex(ActState.Move);//Get 动画ID
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//却换动画
            ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Move;
        }
        else if (entiCtrl.Is_TraditionalAnimation)
            shibingAsp.ACT = ActState.Move;

        //========如果是空军单位=========
        if (sx.Is_AirForce)
            AirForce(ref worldPos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, worldPos);

        ////炮台修正面朝敌人的方向
        //vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[entiCtrl.PaoTai].Position;
        //float3 PaoTaidir = math.normalize(vdir);
        //worldPos = transform[entiCtrl.PaoTai];
        //targetRotation = quaternion.LookRotationSafe(PaoTaidir, new float3(0, 1, 0));
        //worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转
        //entiCtrl.PaoTaiRotation = worldPos.Rotation;//将计算好的炮台旋转存储起来等着炮台刷新的时候自己旋转
        //ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, entiCtrl);

    }

    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // 获取当前前向向量和目标前向向量
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // 计算两个向量之间的角度
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // 使用叉积判断旋转方向
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// 向左旋转
                zAngle = -40f;
            else
                zAngle = 40f;// 向右旋转
                             // 应用Z轴旋转
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // 插值旋转
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//选择基地中离自己最近的点
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

public partial struct EntityCtrlFireJob : IJobEntity
{
    public float time;
    public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorControlComponent> gpuCtrl;
    [ReadOnly] public ComponentLookup<GpuEcsAnimatorStateComponent> gpuState;
    [ReadOnly] public ComponentLookup<SynchronizeAni> SynAni;
    [ReadOnly] public ComponentLookup<JiDi> JIDI;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public BufferLookup<JiDiPointBuffer> jidiPointbuffer;
    void Execute(ShiBingAspects shibingAsp,ref EntityCtrl entiCtrl,ref SX sx, Fire fire, [ChunkIndexInQuery] int ChunkIndex)
    {
        //基地没了就idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) && 
            shibingAsp.Name != ShiBingName.TOWER && !shibingAsp.Is_Parasitic && myLayer[shibingAsp.ShiBing_Entity].BelongsTo != layer.Neutral)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, shibingAsp.ShiBing_Entity, false);
            return;
        }
        if (!transform.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalTransform ltf) ||
            !transform.TryGetComponent(shibingAsp.ShootEntity, out LocalTransform ltf2))
        {
            shibingAsp.ShootEntity = Entity.Null;
            //Obj动画的播放攻击动画是靠ShootTime的数值的，所以就算攻击的目标死亡，我的ShootTime也不能马上恢复，必须让个攻击动画播放完
            if (!SynAni.TryGetComponent(shibingAsp.ShiBing_Entity, out SynchronizeAni sa))
            {
                //if (sx.Cur_ShootTime <= 0)
                //    sx.Cur_ShootTime = sx.ShootTime;
                shibingAsp.ACT = ActState.Ready;
                return;
            }
        }
        //如果已经是死亡状态了
        if (sx.Is_Die)
        {
            shibingAsp.ShootEntity = Entity.Null;
            shibingAsp.ACT = ActState.Ready;
            sx.Cur_ShootTime = sx.ShootTime;
            return;
        }

        //计算攻击间隔
        sx.Cur_ShootTime -= time;
        if (sx.Cur_ShootTime <= 0)//如果有需要播放GpuECSAin动画的就播放攻击动画
        {
            if (entiCtrl.EntityCtrlAni_Fire)//如果是有动画的Fire就动画播放完后重制攻击时间
            {
                if (gpuState[shibingAsp.ShiBing_Entity].currentNormalizedTime == 1f)
                    sx.Cur_ShootTime = sx.ShootTime;
            }
            if (shibingAsp.ACT != ActState.Fire)
            {
                shibingAsp.ACT = ActState.Fire;
                if (entiCtrl.EntityCtrlAni_Fire)//如果有需要播放的动画就播放
                {
                    int index = shibingAsp.GetAnimIndex(ActState.Fire);//Get 动画ID
                    var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                    shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
                    ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
                }
            }

        }
        else
        {
            if (shibingAsp.ACT != ActState.Ready)
            {
                shibingAsp.ACT = ActState.Ready;
                if (entiCtrl.EntityCtrlAni_Ready)//如果有需要播放的动画就播放
                {
                    int index = shibingAsp.GetAnimIndex(ActState.Ready);//Get 动画ID
                    var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                    shibingAsp.RunAnimation(ref animCtrl, index);//却换动画
                    ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
                }
            }

        }
        if (!transform.TryGetComponent(shibingAsp.ShootEntity, out LocalTransform lt))
            return;


        var shootEnti = shibingAsp.ShootEntity;
        //如果检测到的目标为基地，就选离自己最近的基地点进行攻击
        if (JIDI.TryGetComponent(shibingAsp.ShootEntity, out JiDi jd))
        {
            if(transform.TryGetComponent(shibingAsp.JidiPoint, out LocalTransform ltf4))
                shootEnti = shibingAsp.JidiPoint;
        }

        //修正面朝敌人的方向
        float3 vdir = LocaltoWorld[shootEnti].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];
        if (!shibingAsp.Is_Parasitic)//但是寄生单位除外
            worldPos.Position.y = 0;//限制Y轴，不要挤到别人的头上
        var targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转

        //========如果是空军单位=========
        if (sx.Is_AirForce)
            AirForce(ref worldPos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, worldPos);

        ////炮台修正面朝敌人的方向
        //var vdir = LocaltoWorld[shibingAsp.ShootEntity].Position - LocaltoWorld[entiCtrl.PaoTai].Position;
        //float3 PaoTaidir = math.normalize(vdir);
        //var worldPos = transform[entiCtrl.PaoTai];
        //var targetRotation = quaternion.LookRotationSafe(PaoTaidir, new float3(0, 1, 0));
        //worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// 插值旋转
        //entiCtrl.PaoTaiRotation = worldPos.Rotation;//将计算好的炮台旋转存储起来等着炮台刷新的时候自己旋转

    }
    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // 获取当前前向向量和目标前向向量
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // 计算两个向量之间的角度
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // 使用叉积判断旋转方向
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// 向左旋转
                zAngle = -40f;
            else
                zAngle = 40f;// 向右旋转
                             // 应用Z轴旋转
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // 插值旋转
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//选择基地中离自己最近的点
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

public partial struct EntityCtrlAppearJob : IJobEntity
{
    public float time;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    void Execute(ShiBingAspects shibingAsp, ref EntityCtrl entiCtrl,SX sx,ref Appear appear, [ChunkIndexInQuery] int ChunkIndex)
    {
        if (!transform.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalTransform ltf))
            return;

        switch(appear.appearName)
        {
            case AppearName.NUll : QuitAppearStatic(ref shibingAsp, ChunkIndex); break;
            case AppearName.MonsterAppear : MonsterAppear(shibingAsp, sx,ref appear, ChunkIndex); break;
            case AppearName.MonsterAirForceAppear : MonsterAirForceAppear(shibingAsp, sx,ref appear, ChunkIndex); break;
            case AppearName.BaZhuApper : BaZhuAppear(shibingAsp, sx,ref appear, ChunkIndex); break;
            case AppearName.BaZhuApper2 : BaZhuAppear2(shibingAsp, sx,ref appear, ChunkIndex); break;
        }





    }


    void QuitAppearStatic(ref ShiBingAspects shibingAsp, int ChunkIndex)//退出出场行为状态
    {
        ECB.RemoveComponent<Appear>(ChunkIndex, shibingAsp.ShiBing_Entity);
        ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
        shibingAsp.ACT = ActState.Idle;
    }
    void MonsterAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear,int ChunkIndex)//怪物登场
    {
        var Pos = transform[shibingAsp.ShiBing_Entity];
        var forwardMovement = sx.Speed * time * Pos.Forward();
        var distance = math.distance(Pos.Position, Pos.Position + forwardMovement);
        Pos.Position += forwardMovement;
        appear.AppearDistance -= distance;
        Pos.Position.y = 0;
        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, Pos);

        if (appear.AppearDistance <= 0)
        {
            QuitAppearStatic(ref shibingAsp, ChunkIndex);
            //ECB.RemoveComponent<Appear>(ChunkIndex, shibingAsp.ShiBing_Entity);
            //ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            //shibingAsp.ACT = ActState.Idle;
        }
    }
    void MonsterAirForceAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//飞行怪物登场
    {
        var Pos = transform[shibingAsp.ShiBing_Entity];
        var dir = Pos.Forward() + Pos.Up() * 0.4f;
        dir = math.normalize(dir);
        Pos.Position += appear.AppearSpeed * time * dir;
        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, Pos);
        if (Pos.Position.y >= 10)//50)
        {
            QuitAppearStatic(ref shibingAsp, ChunkIndex);
            //ECB.RemoveComponent<Appear>(ChunkIndex, shibingAsp.ShiBing_Entity);
            //ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            //shibingAsp.ACT = ActState.Idle;
        }
    }
    void BaZhuAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//霸主登场
    {
        var Pos = transform[shibingAsp.ShiBing_Entity];
        var DownMovement = (appear.AppearSpeed * time * Pos.Up()) * -1;
        var distance = math.distance(Pos.Position, Pos.Position + DownMovement);
        Pos.Position += DownMovement;
        appear.AppearDistance -= distance;
        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, Pos);

        if (appear.AppearDistance <= 0 || Pos.Position.y <= 10)//50)
        {
            QuitAppearStatic(ref shibingAsp, ChunkIndex);
            //ECB.RemoveComponent<Appear>(ChunkIndex, shibingAsp.ShiBing_Entity);
            //ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
            //shibingAsp.ACT = ActState.Idle;
        }
    }
    void BaZhuAppear2(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//霸主第二种出场方式
    {
        //如果动画还没有播放完就退出
        if (!appear.IsOver_AppearAnimPlay) return;

        //动画播放完后将藏起来的Entity士兵放回动画结束的位置
        var entiTransform = transform[shibingAsp.ShiBing_Entity];
        entiTransform.Position = appear.AppearPos;
        entiTransform.Rotation = appear.AppearRot;
        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, entiTransform);
        QuitAppearStatic(ref shibingAsp, ChunkIndex);
    }

}










//默认方向为朝向敌人基地(Walk)
//检测到目标后，炮台和车身同时修正(Move)
//进入攻击范围后，车身不动，只修正炮台面向敌人的方向(Fire)
//public partial struct PaoTaiUpJob : IJobEntity//根据车身刷新炮台的状态
//{
//    public EntityCommandBuffer.ParallelWriter ECB;
//    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
//    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> tansform;
//    void Execute(Entity entity , [ChunkIndexInQuery]int ChunkIndex)
//    {
//    }
//}


//所有士兵公用的Diejob
[BurstCompile]
public partial struct ShiBingIsDieJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<Die> die;
    [ReadOnly] public ComponentLookup<Integral> integral;

    void Execute(Entity entity ,ShiBing shibing, SX sx, [ChunkIndexInQuery] int chunkindex)
    {
        //如果有Die就退出不要加Die组件了；如果是寄生单位也退出
        if (die.TryGetComponent(entity, out Die d) || shibing.Is_Parasitic)
            return;

        if(sx.Cur_HP <= 0)
        {
            ECB.AddComponent(chunkindex, entity, new Die
            {
                DeadParticle = shibing.DeadParticle,
                DeadPoint = shibing.DeadPoint,
                DeadLanguage = shibing.DeadLanguage,
                Is_LanguageNoBullet = shibing.Is_LanguageNoBullet,
            });

            //不获得基地的生命积分
            if (shibing.Name == ShiBingName.JiDi)
                return;
            //我死了，给攻击我的人加积分
            if(integral.TryGetComponent(entity,out Integral integ))
            {
                var intel = integral[entity];
                if (!integral.TryGetComponent(intel.AttackMeEntity, out Integral integ1))
                    return;
                //把我值的积分(最大生命值 = 积分)给攻击我的人
                var AttackMeintel = integral[intel.AttackMeEntity];
                AttackMeintel.ATIntegral += sx.HP;
                ECB.SetComponent(chunkindex, intel.AttackMeEntity, AttackMeintel);
            }
        }

    }
}
