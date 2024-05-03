using GPUECSAnimationBaker.Engine.AnimatorSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(GpuEcsAnimatorSystem))]
public partial class TransitionSystem : SystemBase
{
    int Index1;
    protected override void OnCreate()
    {

    }

    protected override void OnUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{

        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //    AniChangGongId = AnimationIdsBaoLei.IdletoBattle;
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    Index1 = (int)AnimationIdsBaolei.Idel;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    Index1 = (int)AnimationIdsBaolei.Walk;
        //    Debug.Log("                      按下了4 ");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    Index1 = (int)AnimationIdsCHangGong.PreAttack;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    Index1 = (int)AnimationIdsCHangGong.Ready_1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    Index1 = (int)AnimationIdsCHangGong.Attack_1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha8))
        //    Index1 = (int)AnimationIdsCHangGong.Attack_2;




        //EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        /*EntityQuery entityQuery = EntityManager.CreateEntityQuery(typeof(GpuEcsAnimatorControlComponent));
        NativeArray<Entity> entitiesArr = entityQuery.ToEntityArray(Allocator.Temp);
        foreach(Entity entity in entitiesArr)
        {
            //entityManager.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation((int)AniChangGongId);

            GpuEcsAnimatorAspect AniAs = EntityManager.GetAspect<GpuEcsAnimatorAspect>(entity);
            //Debug.Log("                        找到的Entity为：" + entity + "  Aspect为：" + AniAs);
            AniAs.RunAnimation((int)AniChangGongId);
        }
        entitiesArr.Dispose();*/

        //Entities.WithStructuralChanges().ForEach((/*int entityInQueryIndex, */Entity entity, in GpuEcsAnimatorControlComponent gpuAnim/*, in LocalTransform localTrans*/) =>
        //{
        //    EntityManager.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(
        //        Index1,
        //        0.0f, 1f, 0.0f, 0.1f);
        //}).Run();




    }

}
