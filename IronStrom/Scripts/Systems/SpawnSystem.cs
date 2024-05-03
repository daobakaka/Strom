using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class SpawnSystem : SystemBase
{
    int PlayerID = 0;

    [BurstCompile]
    protected override void OnCreate()
    {

    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        ////����һ��Buffer��ʵ����
        //var ecbSingleton = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        //EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(EntityManager.WorldUnmanaged);

        //Spawn spawn;
        //if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
        //    return;
        //else
        //    spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        //Entity Temp = Entity.Null;
        //int id = 0;
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Temp = spawn.Team1_BingYing;
        //    id = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    Temp = spawn.Team2_BingYing;
        //    id = 2;
        //}
        //else
        //    return;

        //if (Temp == Entity.Null)
        //{
        //    //Debug.Log("                    Ԥ����û��ֵ������  SpawnSysytem");
        //    return;
        //}


        //Entity bingyingEntity = EntityManager.Instantiate(Temp);//ʵ������Ӫ
        //BingYing bingying = EntityManager.GetComponentData<BingYing>(bingyingEntity);//��ȡ��Ӫ�������,job�����޷���ȡ�������
        //bingying.TeamID = id;
        //PlayerID += 1;
        //bingying.PlayerID = PlayerID;
        //LocalTransform bingyinglocalWorld = EntityManager.GetComponentData<LocalTransform>(bingyingEntity);



        //if (id == 1)
        //{
        //    bingyinglocalWorld.Position = CountBingYingInitPos(id);
        //    bingyinglocalWorld.Scale = 1;
        //}
        //else if (id == 2)
        //{
        //    bingyinglocalWorld.Position = CountBingYingInitPos(id);
        //    bingyinglocalWorld.Rotation = quaternion.Euler(0, math.PI, 0);
        //    bingyinglocalWorld.Scale = 1;
        //}
        //EntityManager.SetComponentData(bingyingEntity, bingying);//��ʵ������
        //EntityManager.SetComponentData(bingyingEntity, bingyinglocalWorld);//��ʵ������

        //TeamManager.teamManager.AddTeam(bingying.TeamID, bingying.PlayerID, bingyingEntity);

    }

    float3 CountBingYingInitPos(int ID)//����ID���ñ�Ӫ����λ��
    {
        float3 Pos = 0;
        Pos.x = UnityEngine.Random.Range(20f, -20f);
        if (ID == 1)
            Pos.z = UnityEngine.Random.Range(-30f, -35f);
        else if (ID == 2)
            Pos.z = UnityEngine.Random.Range(30f, 35f);
        return Pos;
    }
}
