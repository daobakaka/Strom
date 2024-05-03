using FormationDemo.Scripts;
using Games.Characters.EliteUnits;
using ProjectDawn.Navigation;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ObjTransfer : ISystem,ISystemStartStop
{
   public float damageDis;
    public float eliteDamageDis;
    public float bossDamageDis;
    NativeList<float3> targetIce;
    NativeList<float3> targetFire;
    NativeList<Entity> targetIceEntity;
    NativeList<Entity> targetFireEntity;
    bool iceObj, fireObj;
    ComponentLookup<LocalTransform> lookforUnit1;
    ComponentLookup<Unit1Component> lookforHealth;

    public void OnStopRunning(ref SystemState state)
    {
        Debug.Log("objtransfer has stoped");
    }

    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Unit1Component>();
        // Debug.Log("objTransfer has started");
        targetIce = new NativeList<float3>(Allocator.Persistent);
        targetFire = new NativeList<float3>(Allocator.Persistent);
        targetIceEntity = new NativeList<Entity>(Allocator.Persistent);
        targetFireEntity = new NativeList<Entity>(Allocator.Persistent);
        lookforUnit1 = state.GetComponentLookup<LocalTransform>(true);
        lookforHealth = state.GetComponentLookup<Unit1Component>(true);
    }
   public void OnStartRunning(ref SystemState state)
    {
        //state.Enabled = false;
        // Debug.Log($"targetice's position is  and the dis is:{damageDis}");
        damageDis = EntityOfMonitor.Instance.entityDamageDis;
        eliteDamageDis = EntityOfMonitor.Instance.entityEliteDamageDis;
        bossDamageDis = EntityOfMonitor.Instance.entityBossDamageDis;
       // Debug.Log($"targetice's position is  and the dis is:{damageDis}and the bossdis is{bossDamageDis}");

    }
    void OnDestroy(ref SystemState state)
    {
        targetFire.Dispose();
        targetIce.Dispose();

        targetIceEntity.Dispose();
        targetFireEntity.Dispose();
    }
    void OnUpdate(ref SystemState state) 
    {
        //var ecb = new EntityCommandBuffer(Allocator.Temp);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ecb = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
      .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        #region declare the patameter
        //int findInterval = (int)math.floor(EntityOfMonitor.Instance.entitiesNum / 50);
        var randomnum = new Random(2345);
        targetFire.Clear();
        targetIce.Clear();
        targetIceEntity.Clear();
        targetFireEntity.Clear();
        lookforUnit1.Update(ref state);
        lookforHealth.Update(ref state);
        #endregion
        #region  the module of set foreach
        foreach (var (unitdis, transformdis,entity) in SystemAPI.Query<RefRW<Unit1Component>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (unitdis.ValueRO.monsterType == MonsterType.cavalry || unitdis.ValueRO.monsterType == MonsterType.infantry)
            {
                if (unitdis.ValueRO.team == EliteUnitPortalMan.Team.Human)
                {
                    targetIce.Add(transformdis.ValueRO.Position);
                    targetIceEntity.Add(entity);
                }
                else
                {
                    targetFire.Add(transformdis.ValueRO.Position);
                    targetFireEntity.Add(entity);
                }
            }
        }

        #region region of OBJ  trasfer system
        float3 targetfire = float3.zero;
        float3 targetice = float3.zero;
        foreach (var (unit, transform, entity) in SystemAPI.Query<RefRW<unitTagBoss>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if(unit.ValueRO.team== EliteUnitPortalMan.Team.Human)
                targetice = transform.ValueRO.Position;
            else
                targetfire = transform.ValueRO.Position;
        }
        iceObj = false;
        fireObj = false;
        foreach (var UnitSprint0 in SystemAPI.Query<RefRW<UnitSprint>>())
        {
            //是否有蓝色 精英怪
            if (UnitSprint0.ValueRO.team== EliteUnitPortalMan.Team.Human)
            {
                iceObj = true;
            }
            else if (UnitSprint0.ValueRO.team == EliteUnitPortalMan.Team.Org)
            {
                fireObj = true;
            } 
        }
        if (iceObj == true && fireObj == false && targetFire.Length > 0)
        {
            Monsterins.IceObjWorld = false;
        }
        else
        {
            Monsterins.IceObjWorld = true;
        }
        if (fireObj == true && iceObj == false && targetIce.Length > 0)
        {
            Monsterins.FireObjWorld = false;
        }
        else
        {
            Monsterins.FireObjWorld = true;
        }
        foreach (var (UnitSprint, Unit1, transform, entity) in SystemAPI.Query<RefRW<UnitSprint>, RefRW<Unit1Component>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            //精英怪攻击步兵   骑兵逻辑
            if (Unit1.ValueRO.team == EliteUnitPortalMan.Team.Org && Monsterins.FireObjWorld == false && targetIce.Length > 0)
            {
                UpItem(ref state,UnitSprint, Unit1, transform, entity, targetice);

            }
            else if (Unit1.ValueRO.team == EliteUnitPortalMan.Team.Human && Monsterins.IceObjWorld == false && targetFire.Length > 0)
            {
                UpItem(ref state, UnitSprint, Unit1, transform, entity,targetfire);
            }
            if (UnitSprint.ValueRW.generalattackBack)
            {
                //普功回调 对步兵 骑兵产生伤害
                UnitSprint.ValueRW.generalattackBack = false;
                //if (UnitSprint.ValueRO.monsterType == MonsterType.FemaleHunter || UnitSprint.ValueRO.monsterType == MonsterType.guard)
                //{
                //    #region 只检测一个对象
                //    // 定义射线的起点和方向
                //    float3 rayOrigin = transform.ValueRO.Position + new float3(0, 1, 0);
                //    float3 rayDirection = math.forward(transform.ValueRO.Rotation);

                //    // 射线投射输入
                //    CollisionFilter Filter;
                //    if (UnitSprint.ValueRO.team == EliteUnitPortalMan.Team.Org)
                //    {
                //        //火
                //        Filter = new CollisionFilter()
                //        {
                //            //表示这个过滤器属于哪些层
                //            BelongsTo = (1u << 3),
                //            //表示这个过滤器想要与哪些层碰撞
                //            CollidesWith = (1u << 2),
                //            GroupIndex = 0,
                //        };
                //    }
                //    else
                //    {
                //        Filter = new CollisionFilter()
                //        {
                //            BelongsTo = (1u << 2),
                //            CollidesWith = (1u << 3),
                //            GroupIndex = 0,
                //        };
                //    }
                //    var raycastInput = new RaycastInput
                //    {
                //        Start = rayOrigin,
                //        End = rayOrigin + rayDirection * 600, // 以10个单位长度作为射线的长度
                //        Filter = Filter
                //    };

                //    // 执行射线投射
                //    if (physicsWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
                //    {
                //        if (state.EntityManager.HasComponent<Unit1Component>(hit.Entity))
                //        {
                //            Unit1Component unit1Component = state.EntityManager.GetComponentData<Unit1Component>(hit.Entity);
                //            unit1Component.health -= Unit1.ValueRO.damage;
                //            unit1Component.lastAttacker = Unit1.ValueRO.playerName;
                //            state.EntityManager.SetComponentData<Unit1Component>(hit.Entity, unit1Component);
                //            // Debug.LogError("步兵 骑兵受到了伤害");
                //        }
                //    }
                //    #endregion
                //}
                //else
                {
                    #region 检测多个对象
                    //给Boss找步兵
                    CollisionFilter filter;
                    int maxCount = 5;
                    if (Unit1.ValueRO.team == EliteUnitPortalMan.Team.Org)
                    {
                        //火Boss 
                        filter = new CollisionFilter()
                        {
                            //表示这个过滤器属于哪些层
                            BelongsTo = (1u << 3),
                            //表示这个过滤器想要与哪些层碰撞
                            CollidesWith = (1u << 2),
                            GroupIndex = 0,
                        };
                    }
                    else
                    {
                        filter = new CollisionFilter()
                        {
                            BelongsTo = (1u << 2),
                            CollidesWith = (1u << 3),
                            GroupIndex = 0,
                        };
                    }
                    var newInfantrys = new NativeList<ColliderCastHit>(12, Allocator.Temp);
                    if (physicsWorld.SphereCastAll(transform.ValueRO.Position, 150f, Vector3.forward, 150f, ref newInfantrys, filter))
                    {
                        foreach (var elite in newInfantrys)
                        {
                            if (maxCount == 0) break;
                            if (state.EntityManager.HasComponent<Unit1Component>(elite.Entity))
                            {
                                Unit1Component unit1Component = state.EntityManager.GetComponentData<Unit1Component>(elite.Entity);
                                unit1Component.health -= Unit1.ValueRO.damage;
                                unit1Component.lastAttacker = Unit1.ValueRO.playerName;
                                state.EntityManager.SetComponentData<Unit1Component>(elite.Entity, unit1Component);
                                //Debug.LogError("步兵 骑兵受到了伤害");
                                maxCount--;
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion


        #endregion
        #region 机甲子弹逻辑
        var job = new MechBulletsJob
        {
            ecb = ecb,
            physicsWorld=physicsWorld,
            spatialSingleton = SystemAPI.GetSingleton<AgentSpatialPartitioningSystem.Singleton>(),

            teamLookupRO = SystemAPI.GetComponentLookup<FormationTeamData>(true),
            unit1ComponentList = SystemAPI.GetComponentLookup<Unit1Component>(true),
            teamChildrenLookupRO = SystemAPI.GetBufferLookup<FormationChildUnit>(true),
            formationUnitHealthLookupRW = SystemAPI.GetComponentLookup<FormationUnitHealthComponent>(),
        };

        state.Dependency = job.ScheduleParallelByRef(state.Dependency);

     
        #endregion
        #region dispose

        #endregion
    }
    void UpItem(ref SystemState state, RefRW<UnitSprint> UnitSprint, RefRW<Unit1Component> Unit1, RefRW<LocalTransform> transform,Entity entity,float3 boss)
    {
        var targetList = targetIce;
        if (Unit1.ValueRO.team == EliteUnitPortalMan.Team.Human)
            targetList = targetFire;
        float attackdistance = eliteDamageDis;
        if (Unit1.ValueRO.monsterType == MonsterType.Mecha) 
        {
            attackdistance *= 10;
            if (Unit1.ValueRO.isAttackCD)
            {
                //等待攻击动画执行完
                //处于攻击CD中 虽然达到了远程攻击距离
                if (Unit1.ValueRO.iscontinueMove)
                {
                    //单纯向着目标移动
                }
                else 
                {
                    //进入待机
                    return;
                }
            }
        }
        float3 dir = boss - transform.ValueRW.Position;
        dir.y = 0;
        //判断与Boss的距离
        float dis = math.lengthsq(dir);

        float3 dir1 = float3.zero;
        float dis1 = int.MaxValue;
        float3 dirm;

        for (int i = 0; i < targetList.Length; i ++)
        {
            dir1 = targetList[i] - transform.ValueRW.Position;
            dir1.y = 0;
            dis1 = math.lengthsq(dir1);
            if (dis1 < attackdistance)
            {
                if (Unit1.ValueRO.team == EliteUnitPortalMan.Team.Human)
                    UnitSprint.ValueRW.targetEntity = targetFireEntity[i];
                else
                    UnitSprint.ValueRW.targetEntity = targetIceEntity[i];
                state.EntityManager.SetComponentData<UnitSprint>(entity, UnitSprint.ValueRW);
                break;
                // Debug.Log($"targetice's position is :{targetIce[i]},and the num is:{i},   and the dis is:{dis}");
            }//change the ta
        }
        if (dis < dis1)
        {
            //继续攻击Boss
            dirm = math.normalize(dir);
        }
        else
        {
            //切换目标
            dis = dis1;
            dirm = math.normalize(dir1);
        }
        transform.ValueRW.Rotation = Quaternion.Lerp(transform.ValueRW.Rotation, quaternion.LookRotationSafe(dirm, math.up()), 0.1f);//turn around

        if (Unit1.ValueRO.isAttackCD && Unit1.ValueRO.iscontinueMove)
        {
            //单纯向着目标移动
            transform.ValueRW.Position += dirm * Unit1.ValueRO.speed * SystemAPI.Time.DeltaTime;
            return;
        }
        
        if (dis > attackdistance)
        {
            transform.ValueRW.Position += dirm * Unit1.ValueRO.speed * SystemAPI.Time.DeltaTime;
            UnitSprint.ValueRW.attack = false;
        }
        else
        {
            UnitSprint.ValueRW.attack = true;
        }
        if (Unit1.ValueRO.isAttackCD) 
        {
            UnitSprint.ValueRW.attack = true;
        }
    }

    [BurstCompile]//机甲子弹伤害检测
    private partial struct MechBulletsJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;
        [ReadOnly]
        public ComponentLookup<Unit1Component> unit1ComponentList;
        [ReadOnly]
        public ComponentLookup<FormationTeamData> teamLookupRO;
        [ReadOnly]
        public AgentSpatialPartitioningSystem.Singleton spatialSingleton;

        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<FormationUnitHealthComponent> formationUnitHealthLookupRW;
        [ReadOnly]
        public BufferLookup<FormationChildUnit> teamChildrenLookupRO;

        public EntityCommandBuffer.ParallelWriter ecb;


        public void Execute([ChunkIndexInQuery] int chunkIndex,
            [EntityIndexInChunk] int entityIndex,
            BulletSkill skill, in LocalTransform transform,Entity entity)
        {
            int maxCount = 999;
            //给子弹找点赞兵
            var targetFormations = new NativeList<Entity>(12, Allocator.Temp);
            var action = new FindEnemyDamageTakerAction()
            {
                teamLookupRO = teamLookupRO,
                myTeam = (byte)skill.team,
                targetFormations = targetFormations,
            };
            spatialSingleton.QuerySphere(transform.Position, skill.range, ref action);

            foreach (var formation in targetFormations)
            {
                if (teamChildrenLookupRO.TryGetBuffer(formation, out var children))
                {
                    foreach (var child in children)
                    {
                        if (maxCount == 0)return;

                        var hp = formationUnitHealthLookupRW.GetRefRW(child.unitEntity);
                        hp.ValueRW.curHp -= skill.damage; //角色自行判断死亡与否
                        maxCount--;
                    }
                }
            }

            //给子弹找步兵
            CollisionFilter filter;
            if (skill.team == EliteUnitPortalMan.Team.Org)
            {
                //火Boss 
                filter = new CollisionFilter()
                {
                    //表示这个过滤器属于哪些层
                    BelongsTo = (1u << 3),
                    //表示这个过滤器想要与哪些层碰撞
                    CollidesWith = (1u << 2),
                    GroupIndex = 0,
                };
            }
            else
            {
                filter = new CollisionFilter()
                {
                    BelongsTo = (1u << 2),
                    CollidesWith = (1u << 3),
                    GroupIndex = 0,
                };
            }
            var newInfantrys = new NativeList<ColliderCastHit>(12, Allocator.Temp);
            if (physicsWorld.SphereCastAll(transform.Position, skill.range, Vector3.forward, skill.range, ref newInfantrys, filter))
            {
                foreach (var elite in newInfantrys)
                {
                    if (maxCount == 0) break;
                    if (unit1ComponentList.TryGetComponent(elite.Entity,out var unit1Component))
                    {
                        unit1Component.health -= skill.damage;
                        unit1Component.lastAttacker = skill.playerName;
                        ecb.SetComponent(chunkIndex, elite.Entity, unit1Component);
                        //state.EntityManager.SetComponentData<Unit1Component>(elite.Entity, unit1Component);
                        //Debug.LogError("步兵 骑兵受到了伤害");
                        maxCount--;
                    }
                }
            }
            ecb.DestroyEntity(entityIndex, entity);
        }
    }
    public struct FindEnemyDamageTakerAction : ISpatialQueryEntity
    {
        public NativeList<Entity> targetFormations;
        public byte myTeam;

        [ReadOnly]
        public ComponentLookup<FormationTeamData> teamLookupRO;



        public void Execute(Entity entity, AgentBody body, AgentShape shape, LocalTransform transform)
        {
            var formationComponent = teamLookupRO.GetRefROOptional(entity);
            if (formationComponent.IsValid)
            {
                if (formationComponent.ValueRO.teamIndex != myTeam)
                {
                    targetFormations.Add(entity);
                }
            }
        }
    }
}
