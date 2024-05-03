using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using Random = Unity.Mathematics.Random;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Jobs;
using GPUInstancer.CrowdAnimations;
using Games.Characters.EliteUnits;
using System;
using Unity.Physics;
using Games.Characters.UI;
using Games.Characters.SDKLayer;

public partial struct GpuMonsterSystem : ISystem, ISystemStartStop
{
    public float damageDis;
    public float bossDamageDis;
    public float findingDistanceBoss;
    private Random randomnum;
    private int interval;
    /// <summary>
    /// module of delcare
    /// </summary>
    /// <param name="state"></param>

    NativeList<int> iceNum;
    NativeList<int> fireNum;
    NativeList<CrowdInstanceState> crowdInstanceStates;
    public NativeHashMap<BlobAssetReference<PlayerID>, float> entityIntegral;
    //步兵 骑兵列表
    NativeList<Entity> targetDicIce;
    NativeList<Entity> targetDicFire;
    //精英怪
    NativeList<Entity> targetEliteUnit;

    ComponentLookup<LocalTransform> lookforUnit1;
    ComponentLookup<Unit1Component> lookforHealth;
    ComponentLookup<EntityAttack> lookforCollisionHealth;
    // public event EventHandler EmyEvent;

    //public GPUICrowdManager gpuiCrowdManager;
    //private GPUICrowdPrototype _crowdPrototype;
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitSpawner>();

        //targetIce = new NativeList<float3>(Allocator.Persistent);
        //targetFire = new NativeList<float3>(Allocator.Persistent);

        iceNum = new NativeList<int>(Allocator.Persistent);
        fireNum = new NativeList<int>(Allocator.Persistent);
        crowdInstanceStates = new NativeList<CrowdInstanceState>(Allocator.Persistent);
        entityIntegral = new NativeHashMap<BlobAssetReference<PlayerID>, float>(10000, Allocator.Persistent);
        randomnum = new Random(1234);
        targetDicIce = new NativeList<Entity>(Allocator.Persistent);
        //targetEliteUnit = new NativeList<Entity>(Allocator.Persistent);
        targetDicFire = new NativeList<Entity>(Allocator.Persistent);
        lookforUnit1 = state.GetComponentLookup<LocalTransform>(true);
        lookforHealth = state.GetComponentLookup<Unit1Component>(true);
        lookforCollisionHealth = state.GetComponentLookup<EntityAttack>(true);

        // lookforUnit1 = GetComponentLookup<Unit1Component>(isReadOnly: false);

    }
    Entity enityFire;
    Entity enityIce;
    Entity enityHitFire;
    Entity enityHitIce;
    bool isAttackBossIce;
    bool isAttackBossFire;
    float upTargeTimer;
    void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        upTargeTimer += Time.deltaTime;
        #region 生成
        if (Monsterins.instance != null)
        {
            foreach (var entity1 in SystemAPI.Query<RefRW<UnitSpawner>>())
            {
                for (int i = 0; i < Monsterins.instance.waitGenerateList.Count; i++)
                {
                    if (Monsterins.instance.waitGenerateList.Count > 0)
                    {
                        var data = Monsterins.instance.waitGenerateList[i];
                        if (data.team == Team.BLUE)
                            ProcessSpawner(ref state, entity1, data.uid, EliteUnitPortalMan.Team.Human, data.type);
                        else
                            ProcessSpawner(ref state, entity1, data.uid, EliteUnitPortalMan.Team.Org, data.type);
                        Monsterins.instance.waitGenerateList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        #endregion
        #region declare par
        //targetEliteUnit.Clear();
        iceNum.Clear();
        fireNum.Clear();
        crowdInstanceStates.Clear();
        targetDicFire.Clear();
        targetDicIce.Clear();
        lookforUnit1.Update(ref state);
        lookforHealth.Update(ref state);
        lookforCollisionHealth.Update(ref state);
        interval = Mathf.FloorToInt(EntityOfMonitor.Instance.entitiesNum / 200);
        #endregion
        #region module of find target
        //Boss有两套entity
        foreach (var (unit, transform, entity) in SystemAPI.Query<RefRW<unitTagBoss>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (unit.ValueRO.team == EliteUnitPortalMan.Team.Human)
            {
                enityIce = entity;
            }
            else
            {
                enityFire = entity;
            }
        }
        //寻找旧模式下的entity
        var HitGlowComponentRO = SystemAPI.GetComponentLookup<EliteUnitHitGlowComponent>(true);
        foreach (var (unit, entity) in SystemAPI.Query<RefRW<EliteUnitComponent>>().WithEntityAccess())
        {
            if (unit.ValueRO.teamIndex == (int)EliteUnitPortalMan.Team.Org)
                enityHitFire = entity;
            else
                enityHitIce = entity;
        }
        //所有步兵 骑兵 精英怪
        foreach (var (unitdis, transformdis, entity) in SystemAPI.Query<RefRW<Unit1Component>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (unitdis.ValueRO.team== EliteUnitPortalMan.Team.Human)
            {
                targetDicIce.Add(entity);

            }
            else if (unitdis.ValueRO.team == EliteUnitPortalMan.Team.Org)
            {
                targetDicFire.Add(entity);
            }
        } //cache the positon of enemy camp
        //foreach (var (unitdis, transformdis, entity) in SystemAPI.Query<RefRW<UnitSprint>, RefRW<LocalTransform>>().WithEntityAccess())
        //{
        //    targetEliteUnit.Add(entity);

        //}//cache the positon of enemy unit camp
        #endregion
        {
            #region the main logic 步兵骑兵的攻击移动逻辑
            isAttackBossIce = false;
            isAttackBossFire = false;
            foreach (var (unit1, crowdanim, transform, velocity, entity) in SystemAPI.Query<RefRW<Unit1Component>, RefRW<CrowdInstanceState>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>().WithEntityAccess())
            {
                if (unit1.ValueRW.health > 0)
                {
                    //这个不能删除 不然第一次出现的兵会抖动 不知道为啥
                   // float3 scale = transform.ValueRO.Scale;
                    //Debug.LogError(transform.ValueRO.Scale);
                    UpItem(ref state, unit1, crowdanim, transform);

                }
                else
                {
                    if (!unit1.ValueRW.death)
                    {
                        crowdanim.ValueRW.animationIndex = 1;
                        ecb.RemoveComponent<PhysicsCollider>(entity);
                        ecb.SetComponent(entity, new PhysicsVelocity()
                        {
                            Angular = randomnum.NextFloat3Direction() * randomnum.NextFloat(10f, 20f),
                            Linear = new float3(randomnum.NextFloat(-30f,30), randomnum.NextFloat(100f, 200f) , randomnum.NextFloat(-30, 30))
                        });
                        unit1.ValueRW.death = true;
                       //Debug.LogError(state.EntityManager.GetComponentData<Unit1Component>(entity).death);
                        //积分
                        GameManager.instance.KillMonsterAddScore(unit1.ValueRO.monsterType, unit1.ValueRO.lastAttacker.ToString());
                    }
                    else
                    {
                        unit1.ValueRW.parameterTime += SystemAPI.Time.DeltaTime;
                        float pro = 1 - unit1.ValueRW.parameterTime / 2.0f;
                        pro *= 1.5f;
                        if (pro > 1)
                            pro = 1;
                        pro = pro * transform.ValueRW.Scale;
                        transform.ValueRW.Scale = pro;
                        if (unit1.ValueRW.parameterTime > 2f)
                        {
                            ecb.DestroyEntity(entity);
                        }
                    }
                }
            }
            if (isAttackBossFire)
            {
                //Bosss受伤闪光
                if (!HitGlowComponentRO.IsComponentEnabled(enityHitFire) && randomnum.NextFloat(0f, 1f) <= 0.05f)
                {
                    ecb.SetComponentEnabled<EliteUnitHitGlowComponent>(enityHitFire, true);
                    ecb.SetComponent(enityHitFire, new EliteUnitHitGlowComponent()
                    {
                        remainTime = EliteUnitHitGlowComponent.GlowTime
                    });
                }
            }
            if (isAttackBossIce)
            {
                //Bosss受伤闪光
                if (!HitGlowComponentRO.IsComponentEnabled(enityHitIce) && randomnum.NextFloat(0f, 1f) <= 0.05f)
                {
                    ecb.SetComponentEnabled<EliteUnitHitGlowComponent>(enityHitIce, true);
                    ecb.SetComponent(enityHitIce, new EliteUnitHitGlowComponent()
                    {
                        remainTime = EliteUnitHitGlowComponent.GlowTime
                    });
                }
            }
            #endregion
        }
        if (upTargeTimer > 0.5f)
            upTargeTimer = 0;
        #region the module of job
        //var collisionJob = new CollisonEventJob
        //{
        //    unit2 =lookforHealth,
        //    unit1 =lookforCollisionHealth,
        //    CommandBuffer = ecbjob,
        //}.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(),state.Dependency);
        //state.Dependency = collisionJob;
        #endregion
        #region the GPU module and others
        //if (ifjobforGpu)
        //{
        //    //GpuInstancerCrowd.Instance.GpuCrowdIns(entityMatirxArrayReal.AsArray());
        //    //GpuInstancerCrowd.Instance.RunJobGpuAnimator(crowdInstanceStates.AsArray());
        //}
        //else
        //{

        //    GpuInstanceTK.Instance.DrawGpuMonster(entityMatrixArray.AsArray(), entityMatirxArrayReal.AsArray().ToArray());//call the monitor to draw the gpumonster
        //}
        #endregion
        #region test module

        foreach (var entity1 in SystemAPI.Query<RefRW<UnitSpawner>>())
        {
            //if (Input.GetKeyDown(KeyCode.O))
            //{
            //    ProcessSpawner(ref state, entity1, "kaka0", EliteUnitPortalMan.Team.Human, MonsterType.cavalry);
            //}
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    ProcessSpawner(ref state, entity1, "kaka1", EliteUnitPortalMan.Team.Org, MonsterType.cavalry);
            //}
            ////if (Input.GetKeyDown(KeyCode.K))
            ////{
            ////    ProcessSpawner3(ref state, entity1);
            ////    EntityOfMonitor.Instance.ifclone = false;
            ////    Debug.Log("start to generate killer");
            ////}
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    ProcessSpawner(ref state, entity1, "kaka2", EliteUnitPortalMan.Team.Human, MonsterType.infantry);
            //}
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    ProcessSpawner(ref state, entity1, "kaka3", EliteUnitPortalMan.Team.Org, MonsterType.infantry);
            //} 
            if (entity1.ValueRW.nextSpawnerTime < SystemAPI.Time.ElapsedTime)
            {
                entity1.ValueRW.nextSpawnerTime = entity1.ValueRO.lengthRate + (float)SystemAPI.Time.ElapsedTime;

            }

        }
        #endregion
        #region dispose the temp par
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
       
        //if (GpuInstancerCrowd.Instance._runtimeData.dependentJob.IsCompleted)

        //    crowdInstanceStates.Dispose();
        //RTS.Dispose();
        #endregion
    }
    void UpItem(ref SystemState state, RefRW<Unit1Component> unit1, RefRW<CrowdInstanceState> crowdanim, RefRW<LocalTransform> transform) 
    {
        var boss = enityFire;
        var targetDic = targetDicFire;
        if (unit1.ValueRO.team == EliteUnitPortalMan.Team.Org)
        {
            targetDic = targetDicIce;
            boss = enityIce;
        }
        if (unit1.ValueRW.isIntoAttack)
        {
            unit1.ValueRW.tempattackTimer -= SystemAPI.Time.DeltaTime;
            //crowdanim.ValueRW.animationIndex = 2;
            if (unit1.ValueRW.tempattackTimer < 0)
            {
                //攻击动画结束 计算周未敌人 对其产生伤害
                unit1.ValueRW.isIntoAttack = false;
                if (unit1.ValueRW.isAttackBoss)
                {
                    if (unit1.ValueRO.team == EliteUnitPortalMan.Team.Org)
                    {
                        isAttackBossIce = true;
                    }
                    else 
                    {
                        isAttackBossFire = true;
                    }
                    //积分
                    GameManager.instance.AddSelfScoreByNotInfantryAttackBoss(unit1.ValueRW.playerName.ToString(), unit1.ValueRW.damage);
                    //伤害
                    unitTagBoss unitTagBoss = state.EntityManager.GetComponentData<unitTagBoss>(unit1.ValueRW.targetEntity);
                    unitTagBoss.health += unit1.ValueRW.damage;
                    state.EntityManager.SetComponentData<unitTagBoss>(unit1.ValueRW.targetEntity, unitTagBoss);
                }
                else 
                {
                    if (state.EntityManager.Exists(unit1.ValueRW.targetEntity))
                    {

                        var hp= lookforHealth[unit1.ValueRW.targetEntity];
                        hp.health -= unit1.ValueRW.damage;
                        hp.lastAttacker = unit1.ValueRW.playerName;
                        state.EntityManager.SetComponentData(unit1.ValueRW.targetEntity, hp);

                        //Unit1Component aa = state.EntityManager.GetComponentData<Unit1Component>(unit1.ValueRW.targetEntity);
                        //Debug.LogError(aa.health);
                    }
                }
            }
        }
        else
        {
            //crowdanim.ValueRW.animationIndex = 0;
            //红色方
            if (targetDic.Length == 0)
            {
                //没有其他兵 攻击Boss
                float3 dir = lookforUnit1[boss].Position - transform.ValueRW.Position;
                dir.y = 0;
                float3 dirm = math.normalize(dir);
                float dis = math.lengthsq(dir);
                transform.ValueRW.Rotation = Quaternion.Lerp(transform.ValueRW.Rotation, quaternion.LookRotationSafe(dirm, math.up()), 0.1f);
                unit1.ValueRW.targetEntity = boss;
                unit1.ValueRW.isAttackBoss = true;
                if (dis > bossDamageDis)
                {
                    //移动
                    transform.ValueRW.Position += dirm * unit1.ValueRW.speed * SystemAPI.Time.DeltaTime;
                    crowdanim.ValueRW.animationIndex = 0;
                    transform.ValueRW.Position.y = unit1.ValueRO.height;
                }
                else
                {
                   crowdanim.ValueRW.animationIndex = 2;
                    unit1.ValueRW.isIntoAttack = true;
                    unit1.ValueRW.tempattackTimer = unit1.ValueRW.attackTimer;
                }
            }
            else
            {
                if (!state.EntityManager.Exists(unit1.ValueRO.targetEntity))
                {
                    unit1.ValueRW.iftarget = false;
                }

                if (lookforHealth.TryGetComponent(unit1.ValueRO.targetEntity, out var hp))
                {
                    if (hp.health < 0)
                    {
                        unit1.ValueRW.iftarget = false;
                    }
                }
                else 
                {
                    unit1.ValueRW.iftarget = false;
                }

                if (!unit1.ValueRO.iftarget)
                {
                    if (targetDic.Length < 20)
                    {
                        unit1.ValueRW.targetEntity = targetDic[0];
                    }
                    else
                    {
                        if (unit1.ValueRO.random_index == -1)
                        {
                            var randomInt = randomnum.NextInt(0, targetDic.Length);
                            unit1.ValueRW.random_index = randomInt;
                            unit1.ValueRW.targetEntity = targetDic[randomInt];
                        }
                        else if (unit1.ValueRO.random_index >= targetDic.Length)
                        {
                            var randomInt = randomnum.NextInt(0, targetDic.Length);
                            unit1.ValueRW.random_index = randomInt;
                            unit1.ValueRW.targetEntity = targetDic[randomInt];
                        }
                        else 
                        {
                            unit1.ValueRW.targetEntity = targetDic[unit1.ValueRW.random_index];
                        }
                    }
                }
                float3 dir = lookforUnit1[unit1.ValueRO.targetEntity].Position - transform.ValueRW.Position;
                dir.y = 0;
                float3 dirm = math.normalize(dir);
                float dis = math.lengthsq(dir);
                unit1.ValueRW.isAttackBoss = false;
                if (dis < damageDis)
                {
                    unit1.ValueRW.iftarget = true;
                }
                if (upTargeTimer > 0.5f)
                {
                    if (!unit1.ValueRW.iftarget)
                    {
                        for (int i = 0; i < targetDic.Length; i+=interval+1)
                        {
                            dir = lookforUnit1[targetDic[i]].Position - transform.ValueRW.Position;
                            dir.y = 0;
                            if (math.lengthsq(dir) <= damageDis)
                            {
                                unit1.ValueRW.targetEntity = targetDic[i];
                                dir = lookforUnit1[unit1.ValueRO.targetEntity].Position - transform.ValueRW.Position;
                                dirm = math.normalize(dir);
                                dis = math.lengthsq(dir);
                                unit1.ValueRW.iftarget = true;
                                break;
                            }
                        }
                    }
                }//give the check time
                transform.ValueRW.Rotation = Quaternion.Lerp(transform.ValueRW.Rotation, quaternion.LookRotationSafe(dirm, math.up()), 0.1f);
                if (dis > damageDis)
                {
                    transform.ValueRW.Position += dirm * unit1.ValueRW.speed * SystemAPI.Time.DeltaTime;
                    crowdanim.ValueRW.animationIndex = 0;
                    transform.ValueRW.Position.y = unit1.ValueRO.height;
                }
                else
                {
                    //进入攻击流程
                    crowdanim.ValueRW.animationIndex = 2;
                    unit1.ValueRW.isIntoAttack = true;
                    unit1.ValueRW.tempattackTimer = unit1.ValueRW.attackTimer;
                }
            }
        }

        transform.ValueRW.Position.y = unit1.ValueRO.height;
    }
    void OnDestroy(ref SystemState state)
    {
        iceNum.Dispose();
        fireNum.Dispose();
        crowdInstanceStates.Dispose();
        targetDicFire.Dispose();
        targetDicIce.Dispose();
        //targetEliteUnit.Dispose();
        foreach (var item in entityIntegral)//dispose the itemkey
        {
            item.Key.Dispose();
        }
        entityIntegral.Dispose();

    }
    private void ProcessSpawner(ref SystemState state, RefRW<UnitSpawner> spawner0, string uid, EliteUnitPortalMan.Team team, MonsterType monsterType)
    {
        BlobAssetReference<PlayerID> nameBlobAssetReference;
        using (var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var root = ref builder.ConstructRoot<PlayerID>();
            var len = uid.Length;
            var array = builder.Allocate(ref root.Name, len);

            for (int j = 0; j < len; j++)
            {
                array[j] = uid[j];
            }
            nameBlobAssetReference = builder.CreateBlobAssetReference<PlayerID>(Allocator.Persistent);

            foreach (var item in EntityOfMonitor.Instance.entityIntegral)
            {
                if (EntityOfMonitor.Instance.entityIntegral.ContainsKey(uid))
                {
                    nameBlobAssetReference = item.Value;
                    break;
                }
            }
            if (entityIntegral.ContainsKey(nameBlobAssetReference) == false)
            {
                entityIntegral.Add(nameBlobAssetReference, 0);
                EntityOfMonitor.Instance.entityIntegral.Add(uid, nameBlobAssetReference);//the outer intergral system
            }
        }  //--the module of entity intergral for statistics
        int count = 0;

        JoinData playerdata = UIManager.instance._MainPanel.GetPlayer(uid);
        switch (monsterType)
        {
            case MonsterType.infantry:
                count = 10;
                break;
            case MonsterType.cavalry:
                count = 8;
                break;
        }
        int index = -count / 2;
        //随机一个生成点
        int initBirthPoint = 0;
        if (randomnum.NextInt(0, 10) <= 5)
        {
            //直接用第一个点
        }
        else 
        {
            initBirthPoint= randomnum.NextInt(1, EntityOfMonitor.Instance.iceInitList.Count);
        }
        for (int j = 0; j < count; j++)
        {
            index++;
            Entity newEntity;
            float3 initPos;
            if (team == EliteUnitPortalMan.Team.Human)
            {
                initPos = EntityOfMonitor.Instance.iceInitList[initBirthPoint].position;
                initPos += (index) * 20;
                initPos.y = 0.5f;

                newEntity = state.EntityManager.Instantiate(spawner0.ValueRW.GpuMonsterIce);
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPositionRotationScale(initPos, quaternion.EulerXYZ(EntityOfMonitor.Instance.iceInitRotate), 20));
            }
            else
            {
                initPos = EntityOfMonitor.Instance.fireInitList[initBirthPoint].position;
                initPos += (index) * 20;
                initPos.y = 0.5f;
                newEntity = state.EntityManager.Instantiate(spawner0.ValueRW.GpuMonsterFire);
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPositionRotationScale(initPos, quaternion.EulerXYZ(EntityOfMonitor.Instance.fireInitRotate), 20));
            }
            state.EntityManager.SetName(newEntity, $"unit{monsterType}");
            //initialize
            var conf = HttpRquest.instance.GetMonsterConfigs(monsterType);
            var change = state.EntityManager.GetComponentData<Unit1Component>(newEntity);
            change.speed = conf.monsterSpeed;
            change.health = conf.monsterHealth;
            change.damage = conf.damage;
            change.monsterType = monsterType;
            change.team = team;
            change.random_index = -1;
            //动画时间除动画速度
            change.attackTimer = conf.attackTimer/ 1f;
            switch (monsterType)
            {
                case MonsterType.infantry:
                    if (team == EliteUnitPortalMan.Team.Human)
                    {
                        change.order = 61;
                    }
                    else 
                    {
                        change.order = 62;
                    }
                    break;
                case MonsterType.cavalry:
                    if (team == EliteUnitPortalMan.Team.Human)
                    {
                        change.order = 63;
                    }
                    else
                    {
                        change.order = 64;
                    }
                    break;
            }
            change.playerName = playerdata.nickname.ToSTR32();
            change.height = 0;
            change.PlayerName = nameBlobAssetReference;
            //change.deadBody = spawner0.ValueRO.deadBodyfire2;
            change.death = false;
            state.EntityManager.SetComponentData<Unit1Component>(newEntity, change);
            var animator = state.EntityManager.GetComponentData<CrowdInstanceState>(newEntity);
            animator.animationIndex = GpuInstancerCrowd.Instance._crowdPrototype.animationData.crowdAnimatorDefaultClip;
            animator.animationSpeed = 1f;
            animator.initanimationStartTimeMultiplier = randomnum.NextFloat(0, 1.0f);
            animator.animationStartTimeMultiplier = animator.initanimationStartTimeMultiplier;
            animator.modificationType = StateModificationType.All;
            state.EntityManager.SetComponentData<CrowdInstanceState>(newEntity, animator);

            //兵的名字
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
    .CreateCommandBuffer(state.WorldUnmanaged);
            ecb.AddComponent(newEntity, new UIPortalNameComponent()
            {
                playerName = change.playerName,
                ownerEntity = newEntity,
                fontScale = 0.5f,

            }) ;
            //ecb.AddComponent(newEntity, new UIEliteNameDisplay()
            //{
            //    uiIndex = j
            //});
        }
        
    }
    public void OnStartRunning(ref SystemState state)
    {
        damageDis = EntityOfMonitor.Instance.entityDamageDis;
        bossDamageDis = EntityOfMonitor.Instance.entityBossDamageDis;
        findingDistanceBoss = EntityOfMonitor.Instance.findingDistanceBoss;


        Debug.Log("GPUSYSTEM   :   " + damageDis);
    }

    public void OnStopRunning(ref SystemState state)
    {

    }
    public string TransferBlobAsset(BlobAssetReference<PlayerID> nameBlobAssetReference)//the method for covert BlobAssetReference 
    {
        string result = BlobAssetUtilities.ConvertBlobAssetReferenceToString(nameBlobAssetReference);
        return result;
    }
}

#region the following JOB systems
public partial struct GpuInstancerEntity : IJob
{
    public NativeList<float4x4> entityMatrixArray;
    public double ElapsedTime;
    public NativeArray<Matrix4x4> RTS;
    private float4x4 f4x4;
    void Execute()
    {
        //    for (int i = 0; i < entityMatrixArray.Length; i++)
        //{
        //       f4x4 = entityMatrixArray[i];
        //       Matrix4x4 m4x4 = new Matrix4x4(
        //       new float4(f4x4.c0.x, f4x4.c0.y, f4x4.c0.z, f4x4.c0.w),
        //       new float4(f4x4.c1.x, f4x4.c1.y, f4x4.c1.z, f4x4.c1.w),
        //       new float4(f4x4.c2.x, f4x4.c2.y, f4x4.c2.z, f4x4.c2.w),
        //       new float4(f4x4.c3.x, f4x4.c3.y, f4x4.c3.z, f4x4.c3.w)
        //   );
        //        RTS[i] = m4x4;
        //  }      
    }

    void IJob.Execute()
    {
        for (int i = 0; i < entityMatrixArray.Length; i++)
        {
            f4x4 = entityMatrixArray[i];
            Matrix4x4 m4x4 = new Matrix4x4(
            new float4(f4x4.c0.x, f4x4.c0.y, f4x4.c0.z, f4x4.c0.w),
            new float4(f4x4.c1.x, f4x4.c1.y, f4x4.c1.z, f4x4.c1.w),
            new float4(f4x4.c2.x, f4x4.c2.y, f4x4.c2.z, f4x4.c2.w),
            new float4(f4x4.c3.x, f4x4.c3.y, f4x4.c3.z, f4x4.c3.w)
        );
            RTS[i] = m4x4;
        }
    }
}
public struct GpuCrowdInstancerAnimaion : IJobParallelFor
{
    [ReadOnly] public NativeArray<GPUIAnimationClipData> clipDatas;
    [ReadOnly] public NativeArray<CrowdInstanceState> instanceStateArray;
    /// <summary>
    /// <para>index: 0 x -> frameNo1, y -> frameNo2, z -> frameNo3, w -> frameNo4</para> 
    /// <para>index: 1 x -> weight1, y -> weight2, z -> weight3, w -> weight4</para> 
    /// </summary>
    [NativeDisableParallelForRestriction] public NativeArray<Vector4> animationData;
    /// <summary>
    /// 0 to 4: x ->  minFrame, y -> maxFrame (negative if not looping), z -> speed, w -> startTime
    /// </summary>
    [NativeDisableParallelForRestriction] public NativeArray<Vector4> crowdAnimatorControllerData;

    public void Execute(int index)
    {
        CrowdInstanceState state = instanceStateArray[index];
        if (state.modificationType == StateModificationType.None)
            return;

        Vector4 activeClip0 = crowdAnimatorControllerData[index * 4];
        Vector4 clipFrames = animationData[index * 2];
        Vector4 clipWeights = animationData[index * 2 + 1];
        GPUIAnimationClipData clipData = clipDatas[state.animationIndex];

        switch (state.modificationType)
        {
            case StateModificationType.All:
                clipFrames.x = clipData.clipStartFrame;
                activeClip0.x = clipData.clipStartFrame;
                activeClip0.y = clipData.clipStartFrame + clipData.clipFrameCount - 1;
                activeClip0.z = state.animationSpeed;
                activeClip0.w = clipDatas[state.animationIndex].length * state.animationStartTimeMultiplier;
                clipWeights = new Vector4(1f, 0f, 0f, 0f);
                break;
            case StateModificationType.Clip:
                clipFrames.x = clipData.clipStartFrame;
                activeClip0.x = clipData.clipStartFrame;
                activeClip0.y = clipData.clipStartFrame + clipData.clipFrameCount - 1;
                activeClip0.w = 0f;
                clipWeights = new Vector4(1f, 0f, 0f, 0f);
                break;
            case StateModificationType.Speed:
                activeClip0.z = state.animationSpeed;
                break;
            case StateModificationType.StartTime:
                activeClip0.w = clipData.length * state.animationStartTimeMultiplier;
                break;
        }

        animationData[index * 2] = clipFrames;
        animationData[index * 2 + 1] = clipWeights;
        crowdAnimatorControllerData[index * 4] = activeClip0;

    }
}

///the module of collsionjob
public  struct CollisonEventJob : ICollisionEventsJob
{
    [ReadOnly]
    public ComponentLookup<EntityAttack> unit1;
    [ReadOnly]
    public ComponentLookup<Unit1Component> unit2;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;
    public void Execute(CollisionEvent collisionEvent)
    {
        //Entity entityA = collisionEvent.EntityB;
        //Entity entityB = collisionEvent.EntityA;
        //if (unit1.HasComponent(entityA) && unit2.HasComponent(entityB))
        //{

        //    //var healthA =unit1[entityA];
        //    //healthA.collisionHealth -= 3;
        //    //unit1[entityA] = healthA;
        //    var healthB = unit2[entityB];
        //    healthB.collisionHealth -=2;
        //    unit2[entityB] = healthB;

        //    //if (healthA.collisionHealth <= 0)
        //    //{
        //    //    CommandBuffer.DestroyEntity(0, entityA);
        //    //}
        //    if (healthB.collisionHealth <= 0)
        //    {
        //        CommandBuffer.DestroyEntity(0, entityB);
        //    }
        //}
   
    }
}
#endregion
