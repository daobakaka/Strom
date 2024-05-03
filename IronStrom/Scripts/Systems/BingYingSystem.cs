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
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        //var bingyingjob = new BingYingJob
        //{
        //    ECB = ecb.AsParallelWriter(),
        //    LocalToWorldEntity = m_LocalToWorld,
        //    transform = m_transform,
        //    tiem = SystemAPI.Time.DeltaTime,
        //    spawn = spawn,
        //};
        //Dependency = bingyingjob.ScheduleParallel(Dependency);
        //Dependency.Complete();//等待和确认某个依赖关系（JobHandle）已经完成
        //ecb.Playback(EntityManager);//应用实体的修改
        //ecb.Dispose();//手动释放new的Buffer


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

    private void Execute(BingYingAspects bingying, [ChunkIndexInQuery] int chunkIndx)//这里执行作业的时候，会去找新参里的
    {
        bingying.Cur_CountDownTime -= tiem;
        if (bingying.Cur_CountDownTime <= 0)
            bingying.Cur_CountDownTime = bingying.CountDownTime;
        else return;


        //根据弹幕选择所出的兵种===========================================
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
        //设置士兵的各个组件,这里设置组件中的某一个参数会使没有设置的参数重制
        ECB.SetComponent(chunkIndx,shibing, new LocalTransform
        {
            Position = firePoint.Position,
            Rotation = firePoint.Rotation,
            Scale = transform[temp].Scale
        });
        ECB.SetComponent(chunkIndx,shibing, new ShiBingChange
        {
            Dir = firePoint.Value.Forward(),//士兵的默认往前走的朝向应该是地方基地的位置
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
