using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using GPUECSAnimationBaker.Engine.AnimatorSystem;


//[UpdateAfter()]��ĳ��System֮��ִ��
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
        if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����


        
        bool enter = false;
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            enter = true;

        //GpuECSAnimation
        //ʿ��idle��job��Ϊ==================================================
        var shibingidlejob = new ShiBingIdleJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            LocaltoWorld = m_localtoWorld,
            transform = m_transform,
            gpuCtrl = m_GpuCtrl,
        };
        Dependency = shibingidlejob.ScheduleParallel(Dependency);

        //ʿ��walk��job��Ϊ==================================================
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

        //ʿ��move��job��Ϊ======================================================
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

        //ʿ��fire��job��Ϊ========================================================
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

        //ʿ������Լ��Ƿ�����=====================================================
        var shibingIsDiejob = new ShiBingIsDieJob
        {
            ECB = ecbParallel,
            die = m_Die,
            integral = m_Integral,
        };
        Dependency = shibingIsDiejob.ScheduleParallel(Dependency);



        //EntityCtrl
        //�޶���ʿ����idle��Ϊ==========================================================
        var entiCtrlIdlejob = new EntityCtrlIdleJob
        {
            ECB = ecbParallel,
            gpuCtrl = m_GpuCtrl,
            transform = m_transform,
            sx = m_SX,
        };
        Dependency = entiCtrlIdlejob.ScheduleParallel(Dependency);

        //�޶���ʿ����Walk��Ϊ==========================================================
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

        //�޶���ʿ����Move��Ϊ=========================================================
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

        //�޶���ʿ����Fire��Ϊ=========================================================
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

        //�ǳ���Ϊ=====================================================================
        var entiCtrlAppearJob = new EntityCtrlAppearJob
        {
            ECB = ecbParallel,
            time = SystemAPI.Time.DeltaTime,
            transform = m_transform,
            LocaltoWorld = m_localtoWorld,
            
        };
        Dependency = entiCtrlAppearJob.ScheduleParallel(Dependency);





        //�޶���ʿ����ˢ����̨����=====================================================
        //var paotaiUpjob = new PaoTaiUpJob
        //{
        //    ECB = ecbParallel,
        //    tansform = m_transform,
        //    entiCtrl = m_entiCtrl,
        //};
        //Dependency = paotaiUpjob.ScheduleParallel(Dependency);






        Dependency.Complete();
        ecb.Playback(EntityManager);//Ӧ��ʵ����޸�
        ecb.Dispose();//�ֶ��ͷ�new��Buffer
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

//GpuECSAnimation����ECSAnimation���ƶ����Ľ�ɫjob=============
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
        //�ж��Ƿ������entity,���û��entity�����������ͻ᷵��false
        //if (!LocaltoWorld.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalToWorld ltw))
        //{
        //    Debug.Log("                         ShiBing_EntityΪ��");
        //    return;
        //}
        //var pos = transform[shibingAsp.ShiBing_Entity];
        //pos.Position += shibingAsp.Speed * shibingAsp.ShiBingInitDir * time;
        //ECB.SetComponent(chunkIndx,shibingAsp.ShiBing_Entity, pos);
        if(shibingAsp.ACT == ActState.NotMove)
        {
            int index = shibingAsp.GetAnimIndex(ActState.Idle);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
            ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, animCtrl);
            return;
        }

        if(shibingAsp.ACT != ActState.Idle)
        {
            int index = shibingAsp.GetAnimIndex(ActState.Idle);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
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
        //����û�˾�idle
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ktf) && !shibingAsp.Is_Parasitic)
        {
            ECB.SetComponentEnabled<Idle>(chunkIndx, shibingAsp.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(chunkIndx, shibingAsp.ShiBing_Entity, false);
            return;
        }
        //�ж��Ƿ������entity,���û��entity�����������ͻ᷵��false
        if (!LocaltoWorld.TryGetComponent(shibingAsp.ShiBing_Entity, out LocalToWorld ltw))
        {
            return;
        }

        if(shibingAsp.ACT != ActState.Walk || sx.Is_ChangedAinWalkSpeed || enter)//�����ƶ��Ͳ����ƶ�����
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//ȴ������
            ECB.SetComponent(chunkIndx, shibingAsp.ShiBing_Entity, animCtrl);
            shibingAsp.ACT = ActState.Walk;
            sx.Is_ChangedAinWalkSpeed = false;
        }



        //�����泯�з����صķ���
        float3 direnjidiPos = LocaltoWorld[JIDI[shibingAsp.enemyJiDi].WalkPoint].Position;
        float3 shibingPos = LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        var vdir = direnjidiPos - shibingPos;
        vdir.y = 0;
        shibingAsp.ShiBingInitDir = math.normalize(vdir);
        
        var pos = transform[shibingAsp.ShiBing_Entity];

        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//���ϣ�����泯��
        pos.Rotation = math.slerp(pos.Rotation, targetRotation, 5f * time);// ��ֵ��ת

        pos.Position += sx.Speed * shibingAsp.ShiBingInitDir * time;//���ط������ƶ�
        pos.Position.y = 0;//����ʿ�������ڿ���

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
        ////�����⵽��Ŀ��Ϊ���أ���ѡ���Լ�����Ļ��ص���й���
        //if (JIDI.TryGetComponent(shibingAsp.TarEntity, out JiDi jd))
        //{
        //    var jidiPointEnti = ChooseJiDiPoint(shibingAsp.ShiBing_Entity, shibingAsp.TarEntity);
        //    if (jidiPointEnti == Entity.Null)
        //        return;
        //    shibingAsp.TarEntity = jidiPointEnti;
        //}
        //����û�˾�idle
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
            //Debug.Log("                        " + shibingAsp.ShiBing_Entity +"δ��⵽Ŀ�� �� ��⵽��Ŀ���Ѳ�����");
            return;
        }
 
        float3 vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];

        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//���ϣ�����泯��
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת

        worldPos.Position += sx.Speed * vdir * time;
        ECB.SetComponent(chunkIndex, shibingAsp.ShiBing_Entity, worldPos);


        if (shibingAsp.ACT != ActState.Move || sx.Is_ChangedAinWalkSpeed)//�����ƶ��Ͳ����ƶ�����
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index, sx.Cur_AinWalkSpeed);//ȴ������
            ECB.SetComponent(chunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Move;
        }
    }

    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//ѡ����������Լ�����ĵ�
    {
        Entity selectedPoint = Entity.Null;//ѡ�е����Entity
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
        //����û�˾�idle
        if (!transform.TryGetComponent(shibing.enemyJiDi, out LocalTransform ktf) && !shibing.Is_Parasitic)
        {
            ECB.SetComponentEnabled<Idle>(chunkIndex, shibing.ShiBing_Entity, true);
            ECB.SetComponentEnabled<Move>(chunkIndex, shibing.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Fire>(chunkIndex, shibing.ShiBing_Entity, false);
            ECB.SetComponentEnabled<Walk>(chunkIndex, shibing.ShiBing_Entity, false);
            return;
        }
        //�ж��Ƿ������entity,���û��entity�����������ͻ᷵��false
        if (!transform.TryGetComponent(shibing.ShootEntity, out LocalTransform ltf))
        {
            shibing.ShootEntity = Entity.Null;
            //sx.Cur_ShootTime = sx.ShootTime;
            if (sx.Cur_ShootTime <= 0)
                sx.Cur_ShootTime = sx.ShootTime;
            shibing.ACT = ActState.Ready;
            return;
        }
        //����Ѿ�������״̬��
        if (sx.Is_Die)
        {
            shibing.ShootEntity = Entity.Null;
            sx.Cur_ShootTime = sx.ShootTime;
            shibing.ACT = ActState.Ready;
            return;
        }

        var shootEnti = shibing.ShootEntity;
        //�����⵽��Ŀ��Ϊ���أ���ѡ���Լ�����Ļ��ص���й���
        if (JIDI.TryGetComponent(shibing.ShootEntity, out JiDi jd))
        {
            if(transform.TryGetComponent(shibing.JidiPoint, out LocalTransform ltf2))
                shootEnti = shibing.JidiPoint;
        }

        //�����泯���˵ķ���
        float3 vdir = LocaltoWorld[shootEnti].Position - LocaltoWorld[shibing.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibing.ShiBing_Entity];
        worldPos.Position.y = 0;//����Y�ᣬ��Ҫ�������˵�ͷ��
        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//���ϣ�����泯��
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת
        ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, worldPos);

        //���㹥�����
        sx.Cur_ShootTime -= time;
        if (sx.Cur_ShootTime <= 0)//���Ź�������
        {
            if (gpuState[shibing.ShiBing_Entity].currentNormalizedTime == 1f)
                sx.Cur_ShootTime = sx.ShootTime;
            if (shibing.ACT != ActState.Fire)
            {
                int index = shibing.GetAnimIndex(ActState.Fire);//Get ����ID
                var animCtrl = gpuCtrl[shibing.ShiBing_Entity];
                shibing.RunAnimation(ref animCtrl, index);//ȴ������
                ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, animCtrl);

                shibing.ACT = ActState.Fire;
            }
        }
        else//������׼����
        {
            if (shibing.ACT != ActState.Ready)//���Ŷ���
            {
                int index = shibing.GetAnimIndex(ActState.Ready);//Get Ready�Ķ���ID
                var animCtrl = gpuCtrl[shibing.ShiBing_Entity];
                shibing.RunAnimation(ref animCtrl, index);//ȴ������
                ECB.SetComponent(chunkIndex, shibing.ShiBing_Entity, animCtrl);

                shibing.ACT = ActState.Ready;
            }
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//ѡ����������Լ�����ĵ�
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


//GpuECSAnimation����ECSAnimation���ƶ����Ľ�ɫjob=============

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
                shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
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
        if (shibingAsp.Is_Parasitic)//���Ϊ������λ�Ͳ���Ҫ�ƶ�
            return;
        //����û�˾�idle
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
        if (!transform.TryGetComponent(shibingAsp.enemyJiDi, out LocalTransform ltf1))//û��Ŀ�����
        {
            shibingAsp.ACT = ActState.Idle;
            return;
        }

        //�����泯�з����صķ���
        float3 direnjidiPos = LocaltoWorld[JIDI[shibingAsp.enemyJiDi].WalkPoint].Position;
        float3 shibingPos = LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        var vdir = direnjidiPos - shibingPos;
        vdir.y = 0;
        shibingAsp.ShiBingInitDir = math.normalize(vdir);

        //ʿ���ƶ�����������shibingEntity�������泯��
        var pos = transform[shibingAsp.ShiBing_Entity];
        quaternion targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));//���ϣ�����泯��
        pos.Rotation = math.slerp(pos.Rotation, targetRotation, 5f * time);// ��ֵ��ת

        pos.Position += sx.Speed * shibingAsp.ShiBingInitDir * time;//���ط������ƶ�
        pos.Position.y = 0;//����ʿ�������ڿ���

        if ((entiCtrl.EntityCtrlAni_Walk && shibingAsp.ACT != ActState.Walk) || sx.Is_ChangedAinWalkSpeed || enter)//�������Ҫ���ŵĶ����Ͳ���
        {
            int index = shibingAsp.GetAnimIndex(ActState.Walk);//Get ����ID
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//ȴ������
            ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Walk;
        }
        else if(entiCtrl.Is_TraditionalAnimation)
            shibingAsp.ACT = ActState.Walk;

        //========�Ƿ���������ת�Ķ���===
        //if (shibingAsp.Is_Ani_MoveRL)
        //    PlayAniMoveRL(ref pos, shibingAsp, in targetRotation, ChunkIndex);

        //========����ǿվ���λ=========
        if (sx.Is_AirForce)
            AirForce(ref pos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, pos);
    }
    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // ��ȡ��ǰǰ��������Ŀ��ǰ������
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // ������������֮��ĽǶ�
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // ʹ�ò���ж���ת����
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// ������ת
                zAngle = -40f;
            else
                zAngle = 40f;// ������ת
            // Ӧ��Z����ת
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // ��ֵ��ת
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }

    void PlayAniMoveRL(ref LocalTransform pos,ShiBingAspects shibingAsp, in quaternion targetRotation,int ChunkIndex)
    {
        // ��ȡ��ǰǰ��������Ŀ��ǰ������
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // ������������֮��ĽǶ�
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y < 0)// ������ת
            {
                int index = shibingAsp.GetAnimIndex(ActState.Walk_L);//Get ����ID
                var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
                ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            }
            else
            {
                int index = shibingAsp.GetAnimIndex(ActState.Walk_R);//Get ����ID
                var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
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
        ////�����⵽��Ŀ��Ϊ���أ���ѡ���Լ�����Ļ��ص���й���
        //if (JIDI.TryGetComponent(shibingAsp.TarEntity, out JiDi jd))
        //{
        //    //var jidiPointEnti = ChooseJiDiPoint(shibingAsp.ShiBing_Entity, shibingAsp.TarEntity);
        //    //if (jidiPointEnti == Entity.Null)
        //    //    return;
        //    //shibingAsp.TarEntity = jidiPointEnti;
        //}
        if (shibingAsp.Is_Parasitic)//���Ϊ������λ�Ͳ���Ҫ�ƶ�
            return;
        //����û�˾�idle
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

        //���������泯���˵ķ���
        float3 vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        float3 CheShendir = math.normalize(vdir);
        CheShendir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];
        quaternion targetRotation = quaternion.LookRotationSafe(CheShendir, new float3(0, 1, 0));
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת

        //�����ƶ�
        worldPos.Position += sx.Speed * CheShendir * time;
        worldPos.Position.y = 0;

        if ((entiCtrl.EntityCtrlAni_Walk && shibingAsp.ACT != ActState.Move) || sx.Is_ChangedAinWalkSpeed)//�������Ҫ���ŵĶ����Ͳ���
        {
            int index = shibingAsp.GetAnimIndex(ActState.Move);//Get ����ID
            var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
            shibingAsp.RunAnimation(ref animCtrl, index,sx.Cur_AinWalkSpeed);//ȴ������
            ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
            sx.Is_ChangedAinWalkSpeed = false;
            shibingAsp.ACT = ActState.Move;
        }
        else if (entiCtrl.Is_TraditionalAnimation)
            shibingAsp.ACT = ActState.Move;

        //========����ǿվ���λ=========
        if (sx.Is_AirForce)
            AirForce(ref worldPos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, worldPos);

        ////��̨�����泯���˵ķ���
        //vdir = LocaltoWorld[shibingAsp.TarEntity].Position - LocaltoWorld[entiCtrl.PaoTai].Position;
        //float3 PaoTaidir = math.normalize(vdir);
        //worldPos = transform[entiCtrl.PaoTai];
        //targetRotation = quaternion.LookRotationSafe(PaoTaidir, new float3(0, 1, 0));
        //worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת
        //entiCtrl.PaoTaiRotation = worldPos.Rotation;//������õ���̨��ת�洢����������̨ˢ�µ�ʱ���Լ���ת
        //ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, entiCtrl);

    }

    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // ��ȡ��ǰǰ��������Ŀ��ǰ������
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // ������������֮��ĽǶ�
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // ʹ�ò���ж���ת����
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// ������ת
                zAngle = -40f;
            else
                zAngle = 40f;// ������ת
                             // Ӧ��Z����ת
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // ��ֵ��ת
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//ѡ����������Լ�����ĵ�
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
        //����û�˾�idle
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
            //Obj�����Ĳ��Ź��������ǿ�ShootTime����ֵ�ģ����Ծ��㹥����Ŀ���������ҵ�ShootTimeҲ�������ϻָ��������ø���������������
            if (!SynAni.TryGetComponent(shibingAsp.ShiBing_Entity, out SynchronizeAni sa))
            {
                //if (sx.Cur_ShootTime <= 0)
                //    sx.Cur_ShootTime = sx.ShootTime;
                shibingAsp.ACT = ActState.Ready;
                return;
            }
        }
        //����Ѿ�������״̬��
        if (sx.Is_Die)
        {
            shibingAsp.ShootEntity = Entity.Null;
            shibingAsp.ACT = ActState.Ready;
            sx.Cur_ShootTime = sx.ShootTime;
            return;
        }

        //���㹥�����
        sx.Cur_ShootTime -= time;
        if (sx.Cur_ShootTime <= 0)//�������Ҫ����GpuECSAin�����ľͲ��Ź�������
        {
            if (entiCtrl.EntityCtrlAni_Fire)//������ж�����Fire�Ͷ�������������ƹ���ʱ��
            {
                if (gpuState[shibingAsp.ShiBing_Entity].currentNormalizedTime == 1f)
                    sx.Cur_ShootTime = sx.ShootTime;
            }
            if (shibingAsp.ACT != ActState.Fire)
            {
                shibingAsp.ACT = ActState.Fire;
                if (entiCtrl.EntityCtrlAni_Fire)//�������Ҫ���ŵĶ����Ͳ���
                {
                    int index = shibingAsp.GetAnimIndex(ActState.Fire);//Get ����ID
                    var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                    shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
                    ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
                }
            }

        }
        else
        {
            if (shibingAsp.ACT != ActState.Ready)
            {
                shibingAsp.ACT = ActState.Ready;
                if (entiCtrl.EntityCtrlAni_Ready)//�������Ҫ���ŵĶ����Ͳ���
                {
                    int index = shibingAsp.GetAnimIndex(ActState.Ready);//Get ����ID
                    var animCtrl = gpuCtrl[shibingAsp.ShiBing_Entity];
                    shibingAsp.RunAnimation(ref animCtrl, index);//ȴ������
                    ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, animCtrl);
                }
            }

        }
        if (!transform.TryGetComponent(shibingAsp.ShootEntity, out LocalTransform lt))
            return;


        var shootEnti = shibingAsp.ShootEntity;
        //�����⵽��Ŀ��Ϊ���أ���ѡ���Լ�����Ļ��ص���й���
        if (JIDI.TryGetComponent(shibingAsp.ShootEntity, out JiDi jd))
        {
            if(transform.TryGetComponent(shibingAsp.JidiPoint, out LocalTransform ltf4))
                shootEnti = shibingAsp.JidiPoint;
        }

        //�����泯���˵ķ���
        float3 vdir = LocaltoWorld[shootEnti].Position - LocaltoWorld[shibingAsp.ShiBing_Entity].Position;
        vdir = math.normalize(vdir);
        vdir.y = 0;
        var worldPos = transform[shibingAsp.ShiBing_Entity];
        if (!shibingAsp.Is_Parasitic)//���Ǽ�����λ����
            worldPos.Position.y = 0;//����Y�ᣬ��Ҫ�������˵�ͷ��
        var targetRotation = quaternion.LookRotationSafe(vdir, new float3(0, 1, 0));
        worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת

        //========����ǿվ���λ=========
        if (sx.Is_AirForce)
            AirForce(ref worldPos, in targetRotation);

        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, worldPos);

        ////��̨�����泯���˵ķ���
        //var vdir = LocaltoWorld[shibingAsp.ShootEntity].Position - LocaltoWorld[entiCtrl.PaoTai].Position;
        //float3 PaoTaidir = math.normalize(vdir);
        //var worldPos = transform[entiCtrl.PaoTai];
        //var targetRotation = quaternion.LookRotationSafe(PaoTaidir, new float3(0, 1, 0));
        //worldPos.Rotation = math.slerp(worldPos.Rotation, targetRotation, 5f * time);// ��ֵ��ת
        //entiCtrl.PaoTaiRotation = worldPos.Rotation;//������õ���̨��ת�洢����������̨ˢ�µ�ʱ���Լ���ת

    }
    void AirForce(ref LocalTransform pos, in quaternion targetRotation)
    {
        float zAngle = 0;
        pos.Position.y = 10;//50;
        // ��ȡ��ǰǰ��������Ŀ��ǰ������
        float3 currentForward = math.mul(pos.Rotation, new float3(0, 0, 1));
        float3 targetForward = math.mul(targetRotation, new float3(0, 0, 1));
        // ������������֮��ĽǶ�
        float angle = Vector3.Angle(currentForward, targetForward);
        if (angle >= 5f)
        {
            // ʹ�ò���ж���ת����
            float3 crossProduct = math.cross(currentForward, targetForward);
            if (crossProduct.y > 0)// ������ת
                zAngle = -40f;
            else
                zAngle = 40f;// ������ת
                             // Ӧ��Z����ת
            quaternion zRotation = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(zAngle));
            quaternion targetRotationZ = math.mul(targetRotation, zRotation);
            // ��ֵ��ת
            pos.Rotation = math.slerp(pos.Rotation, targetRotationZ, 5f * time);
        }
    }
    Entity ChooseJiDiPoint(Entity entity, Entity jidiEnti)//ѡ����������Լ�����ĵ�
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


    void QuitAppearStatic(ref ShiBingAspects shibingAsp, int ChunkIndex)//�˳�������Ϊ״̬
    {
        ECB.RemoveComponent<Appear>(ChunkIndex, shibingAsp.ShiBing_Entity);
        ECB.SetComponentEnabled<Idle>(ChunkIndex, shibingAsp.ShiBing_Entity, true);
        shibingAsp.ACT = ActState.Idle;
    }
    void MonsterAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear,int ChunkIndex)//����ǳ�
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
    void MonsterAirForceAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//���й���ǳ�
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
    void BaZhuAppear(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//�����ǳ�
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
    void BaZhuAppear2(ShiBingAspects shibingAsp, SX sx, ref Appear appear, int ChunkIndex)//�����ڶ��ֳ�����ʽ
    {
        //���������û�в�������˳�
        if (!appear.IsOver_AppearAnimPlay) return;

        //����������󽫲�������Entityʿ���Żض���������λ��
        var entiTransform = transform[shibingAsp.ShiBing_Entity];
        entiTransform.Position = appear.AppearPos;
        entiTransform.Rotation = appear.AppearRot;
        ECB.SetComponent(ChunkIndex, shibingAsp.ShiBing_Entity, entiTransform);
        QuitAppearStatic(ref shibingAsp, ChunkIndex);
    }

}










//Ĭ�Ϸ���Ϊ������˻���(Walk)
//��⵽Ŀ�����̨�ͳ���ͬʱ����(Move)
//���빥����Χ�󣬳�������ֻ������̨������˵ķ���(Fire)
//public partial struct PaoTaiUpJob : IJobEntity//���ݳ���ˢ����̨��״̬
//{
//    public EntityCommandBuffer.ParallelWriter ECB;
//    [Unity.Collections.ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
//    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> tansform;
//    void Execute(Entity entity , [ChunkIndexInQuery]int ChunkIndex)
//    {
//    }
//}


//����ʿ�����õ�Diejob
[BurstCompile]
public partial struct ShiBingIsDieJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<Die> die;
    [ReadOnly] public ComponentLookup<Integral> integral;

    void Execute(Entity entity ,ShiBing shibing, SX sx, [ChunkIndexInQuery] int chunkindex)
    {
        //�����Die���˳���Ҫ��Die����ˣ�����Ǽ�����λҲ�˳�
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

            //����û��ص���������
            if (shibing.Name == ShiBingName.JiDi)
                return;
            //�����ˣ��������ҵ��˼ӻ���
            if(integral.TryGetComponent(entity,out Integral integ))
            {
                var intel = integral[entity];
                if (!integral.TryGetComponent(intel.AttackMeEntity, out Integral integ1))
                    return;
                //����ֵ�Ļ���(�������ֵ = ����)�������ҵ���
                var AttackMeintel = integral[intel.AttackMeEntity];
                AttackMeintel.ATIntegral += sx.HP;
                ECB.SetComponent(chunkindex, intel.AttackMeEntity, AttackMeintel);
            }
        }

    }
}
