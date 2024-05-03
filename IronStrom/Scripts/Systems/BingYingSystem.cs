using GPUECSAnimationBaker.Engine.AnimatorSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public partial class BingYingSystem : SystemBase
{
    ComponentLookup<LocalToWorld> m_LocalToWorld;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<Idle> m_lookupIdle;
    protected override void OnCreate()
    {
        m_LocalToWorld = GetComponentLookup<LocalToWorld>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_lookupIdle = GetComponentLookup<Idle>(true);
        //RequireForUpdate<Spawn>();
    }
    protected override void OnUpdate()
    {
        m_LocalToWorld.Update(this);
        m_transform.Update(this);
        m_lookupIdle.Update(this);

        //var ecbSingLeton = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = ecbSingLeton.CreateCommandBuffer(EntityManager.WorldUnmanaged);


        var ecb = new EntityCommandBuffer(Allocator.TempJob);



        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        //var bingyingjob = new BingYingJob
        //{
        //    ECB = ecb.AsParallelWriter(),
        //    LocalToWorldEntity = m_LocalToWorld,
        //    transform = m_transform,
        //    tiem = SystemAPI.Time.DeltaTime,
        //    spawn = spawn,
        //};
        //Dependency = bingyingjob.ScheduleParallel(Dependency);
        //Dependency.Complete();//�ȴ���ȷ��ĳ��������ϵ��JobHandle���Ѿ����
        //ecb.Playback(EntityManager);//Ӧ��ʵ����޸�
        //ecb.Dispose();//�ֶ��ͷ�new��Buffer


    }

}

[BurstCompile]
public partial struct BingYingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public float tiem;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldEntity;
    [Unity.Collections.ReadOnly] public ComponentLookup<LocalTransform> transform;
    [Unity.Collections.ReadOnly] public Spawn spawn;

    private void Execute(BingYingAspects bingying, [ChunkIndexInQuery] int chunkIndx)//����ִ����ҵ��ʱ�򣬻�ȥ���²����
    {
        bingying.Cur_CountDownTime -= tiem;
        if (bingying.Cur_CountDownTime <= 0)
            bingying.Cur_CountDownTime = bingying.CountDownTime;
        else return;


        //���ݵ�Ļѡ�������ı���===========================================
        Entity temp = Entity.Null;
        //if (bingying.TeamID == 1)
        //    temp = spawn.Team1_ShiBing;
        //else if (bingying.TeamID == 2)
        //    temp = spawn.Team2_ShiBing;
        //else
        //    return;


        if(temp == Entity.Null)
        {
            return;
        }
        var shibing = ECB.Instantiate(chunkIndx,temp);
        var firePoint = LocalToWorldEntity[bingying.FirePoint];
        //����ʿ���ĸ������,������������е�ĳһ��������ʹû�����õĲ�������
        ECB.SetComponent(chunkIndx,shibing, new LocalTransform
        {
            Position = firePoint.Position,
            Rotation = firePoint.Rotation,
            Scale = transform[temp].Scale
        });
        ECB.SetComponent(chunkIndx,shibing, new ShiBingChange
        {
            Dir = firePoint.Value.Forward(),//ʿ����Ĭ����ǰ�ߵĳ���Ӧ���ǵط����ص�λ��
            Act = ActState.Idle,
        });
        ECB.AddComponent(chunkIndx,shibing, new Idle());
        ECB.SetComponentEnabled<Idle>(chunkIndx, shibing, false);
        ECB.AddComponent(chunkIndx, shibing, new Walk());
        ECB.SetComponentEnabled<Walk>(chunkIndx, shibing, true);
        ECB.AddComponent(chunkIndx, shibing, new Move());
        ECB.SetComponentEnabled<Move>(chunkIndx, shibing, false);
        ECB.AddComponent(chunkIndx, shibing, new Fire());
        ECB.SetComponentEnabled<Fire>(chunkIndx, shibing, false);

    }
}
