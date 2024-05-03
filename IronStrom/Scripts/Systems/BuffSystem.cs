using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using System;

[BurstCompile]
public partial class BuffSystem : SystemBase
{
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<ShiBingChange> m_ShiBingChange;
    ComponentLookup<SX> m_SX;
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocalToWorld;
    ComponentLookup<EntityOpenID> m_EntityOpenID;
    ComponentLookup<Parasitic> m_Parasitic;

    void UpDataComponentLookup()
    {
        m_ShiBing.Update(this);
        m_SX.Update(this);
        m_ShiBingChange.Update(this);
        m_MyLayer.Update(this);
        m_transform.Update(this);
        m_LocalToWorld.Update(this);
        m_EntityOpenID.Update(this);
        m_Parasitic.Update(this);
    }

    protected override void OnCreate()
    {
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_ShiBingChange = GetComponentLookup<ShiBingChange>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocalToWorld = GetComponentLookup<LocalToWorld>(true);
        m_EntityOpenID = GetComponentLookup<EntityOpenID>(true);
        m_Parasitic = GetComponentLookup<Parasitic>(true);
    }
    protected override void OnUpdate()
    {
        UpDataComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var buffJob = new BuffJob
        {
            ECB = ecb.AsParallelWriter(),
            time = SystemAPI.Time.DeltaTime,
            shibing = m_ShiBing,
            shibingChange = m_ShiBingChange,
            sx = m_SX,
            myLayer = m_MyLayer,
            spawn = spawn,
            transform = m_transform,
            LocaltoWorld = m_LocalToWorld,
            entityOpenID = m_EntityOpenID,
            parasitic = m_Parasitic,
        };
        Dependency = buffJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Entities.ForEach((Entity entity,ref DynamicBuffer<Buff> buffcontainer) =>
            {
                buffcontainer.Add(new Buff
                {
                    buffType = BuffType.buffRampage,
                    buffAct = BuffAct.Init,
                    BuffProportion = 0.9f,
                    BuffTime = 10f,
                    Is_deBuff = false,
                });
            }).WithoutBurst().WithStructuralChanges().Run();
        }


        //���߷����ˣ��Լ�Ϊ�Լ�������
        Entities.ForEach((Entity entity, ShiBing shibing , InstantiateFlesh instanFlesh) =>
        {
            var obj = SynchronizeManager.synchronizeManager.InstanCorrespondObj(shibing.Name, instanFlesh.TeamID);
            if (obj != null)
                SynchronizeManager.synchronizeManager.SynAniDic[entity] = obj;

            EntityManager.RemoveComponent<InstantiateFlesh>(entity);

        }).WithBurst().WithStructuralChanges().Run();

        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        //���߷��ĵ������ͷ�񣬲���ӵ�����б�
        Entities.ForEach((Entity entity, AddAvaandPlayerList AddAvaPlayerList) =>
        {
            //���EntityID,ͨ��EntityID������
            if (!EntityManager.Exists(entity)) return;
            if (!EntityManager.HasComponent<EntityOpenID>(entity)) return;
            var EntiID = EntityManager.GetComponentData<EntityOpenID>(entity);
            var player = teamMager.EntityIDByPlayerData(EntiID.PlayerEntiyID);
            if (!EntityManager.HasComponent<ShiBing>(entity)) return;
            var entiShibing = EntityManager.GetComponentData<ShiBing>(entity);
            if(entiShibing.Name == ShiBingName.JianYa)
            {
                if (!player.m_LikeShiBingList.Contains(entity))
                    player.m_LikeShiBingList.Add(entity);
            }
            else
            {
                if (!player.m_GiftShiBingList.Contains(entity))
                    player.m_GiftShiBingList.Add(entity);
            }

            //���ͷ��
            var EntiUIMager = EntityUIManager.Instance;
            if (EntiUIMager == null) return;
            Debug.Log("  5");
            //ByEntityAddAvatar(entity, EntityManager);
            EntityManager.RemoveComponent<AddAvaandPlayerList>(entity);

        }).WithBurst().WithStructuralChanges().Run();


        ////�߷�����
        //Entities.ForEach((Entity entity, Mutiny mutiny,ShiBing shibing , ref MyLayer mylayer) =>
        //{
        //    //uint collidesWithMask = 0;
        //    //if ((int)mylayer.BelongsTo == (int)layer.Team1)
        //    //{
        //    //    collidesWithMask = 1u << (int)layer.Team2;
        //    //    mylayer.BelongsTo = layer.Team2;
        //    //    mylayer.CollidesWith = layer.Team1;
        //    //    if (mylayer.BulletCollidesWith == layer.Team1Bullet)
        //    //        mylayer.BulletCollidesWith = layer.Team2Bullet;
        //    //}
        //    //else if ((int)mylayer.BelongsTo == (int)layer.Team2)
        //    //{
        //    //    collidesWithMask = 1u << (int)layer.Team1;
        //    //    mylayer.BelongsTo = layer.Team1;
        //    //    mylayer.CollidesWith = layer.Team2;
        //    //    if (mylayer.BulletCollidesWith == layer.Team2Bullet)
        //    //        mylayer.BulletCollidesWith = layer.Team1Bullet;
        //    //}
        //    //var physics = EntityManager.GetComponentData<PhysicsCollider>(entity);
        //    //var collisFilter = physics.Value.Value.GetCollisionFilter();
        //    //collisFilter.BelongsTo = collidesWithMask;
        //    //physics.Value.Value.SetCollisionFilter(collisFilter);
        //    //EntityManager.SetComponentData(entity, physics);
        //    //EntityManager.RemoveComponent<Mutiny>(entity);
        //    //Debug.Log("        ���߷���Entity�ǣ� " + entity);

        //}).WithoutBurst().WithStructuralChanges().Run();



    }



}
[BurstCompile]
public partial struct BuffJob : IJobEntity
{
    public float time;
    public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<ShiBingChange> shibingChange;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public ComponentLookup<LocalTransform> transform;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<EntityOpenID> entityOpenID;
    [ReadOnly] public ComponentLookup<Parasitic> parasitic;

    void Execute(Entity entity, ref DynamicBuffer<Buff> buffcontainer, [ChunkIndexInQuery]int ChunkIndex)
    {
        for (int i = buffcontainer.Length - 1; i >= 0; i--)
        {
            var buff = buffcontainer[i];
            switch (buff.buffType)
            {
                case BuffType.buffAT:       ATBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffDP:       DPBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffDB:       DBBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffHP:       HPAddBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffRampage:  RampageBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffSpeed:    SpeedBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffShootTime:ShootTimeBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffNotMove:  NotMoveBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.buffMutiny:   MutinyBuff(entity,ref buff, buffcontainer, ChunkIndex);break;
                case BuffType.AddHPbuff:    AddHPBuff(entity,ref buff, ChunkIndex);break;
                case BuffType.RecoverHPbuff:RecoverHPBuff(entity,ref buff, ChunkIndex);break;
            }
            if(buff.buffAct == BuffAct.Delete)
                buffcontainer.RemoveAt(i);
            else
                buffcontainer[i] = buff;
        }
    }

    //������buff
    void ATBuff(Entity entity,ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)//�������Ե�����
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = entiSX.AT * buff.BuffProportion;
            if (buff.Is_deBuff)
                entiSX.AT -= buff.BuffChangeValue;
            else
                entiSX.AT += buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            InstanBuffParticle(entity, buff, spawn.buffAT,false,Entity.Null,ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)//����buff���õ����ԸĻ�buff�޸�ǰ������
        {
            var entiSX = sx[entity];
            if(buff.Is_deBuff)
                entiSX.AT += buff.BuffChangeValue;
            else
                entiSX.AT -= buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //������buff,����ֵ
    void DPBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if (buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = entiSX.HP * buff.BuffProportion;
            if (buff.Is_deBuff)
                entiSX.DP -= buff.BuffChangeValue;
            else
                entiSX.DP += buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            InstanBuffParticle(entity, buff, spawn.buffDP,false,Entity.Null, ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            if(buff.Is_deBuff)
                entiSX.DP += buff.BuffChangeValue;
            else
                entiSX.DP -= buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //Ѫ������buff
    void HPAddBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            entiSX.Cur_HP += entiSX.Cur_HP * buff.BuffProportion;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            InstanBuffParticle(entity, buff, spawn.buffHP, false, Entity.Null,ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            entiSX.Cur_HP -= entiSX.Cur_HP * buff.BuffProportion;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }
    
    //��buff
    void DBBuff(Entity entity ,ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = buff.BuffProportion;
            if (buff.Is_deBuff)
                entiSX.DB -= buff.BuffChangeValue;
            else
                entiSX.DB += buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            InstanBuffParticle(entity, buff, spawn.buffDB, false, Entity.Null, ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            if (buff.Is_deBuff)
                entiSX.DB += buff.BuffChangeValue;
            else
                entiSX.DB -= buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //����Buff,���� ����20% 10s
    void RampageBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = entiSX.Speed * buff.BuffProportion;
            buff.BuffChangeValue1 = entiSX.ShootTime * buff.BuffProportion;
            if (buff.Is_deBuff)
            {
                entiSX.Speed -= buff.BuffChangeValue;
                entiSX.ShootTime += buff.BuffChangeValue1;
            }
            else
            {
                entiSX.Speed += buff.BuffChangeValue;
                entiSX.ShootTime -= buff.BuffChangeValue1;
            }
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            InstanBuffParticle(entity, buff, spawn.buffRampage, false, Entity.Null, ChunkIndex);
            //buff.BuffParticle = buffPar;

            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            if (buff.Is_deBuff)
            {
                entiSX.Speed += buff.BuffChangeValue;
                entiSX.ShootTime -= buff.BuffChangeValue1;
            }
            else
            {
                entiSX.Speed -= buff.BuffChangeValue;
                entiSX.ShootTime += buff.BuffChangeValue1;
            }
            ECB.SetComponent(ChunkIndex, entity, entiSX);

            buff.buffAct = BuffAct.Delete;
        }
    }

    //�ƶ�Buff
    void SpeedBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = entiSX.Speed * buff.BuffProportion;
            if (buff.Is_deBuff)
                entiSX.Speed -= buff.BuffChangeValue;
            else
                entiSX.Speed += buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            if (buff.Is_deBuff)
                InstanBuffParticle(entity, buff, spawn.debuffSpeed, false, Entity.Null, ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            if (buff.Is_deBuff)
                entiSX.Speed += buff.BuffChangeValue;
            else
                entiSX.Speed -= buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //����buff
    void ShootTimeBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            buff.BuffChangeValue = entiSX.ShootTime * buff.BuffProportion;
            if (buff.Is_deBuff)
                entiSX.ShootTime += buff.BuffChangeValue;
            else
                entiSX.ShootTime -= buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            if(buff.Is_deBuff)
                InstanBuffParticle(entity, buff, spawn.debuffShootTime, false, Entity.Null, ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            var entiSX = sx[entity];
            if (buff.Is_deBuff)
                entiSX.ShootTime -= buff.BuffChangeValue;
            else
                entiSX.ShootTime += buff.BuffChangeValue;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //����buff
    void NotMoveBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, entity, true);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Move>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, entity, false);

            var shibingEnti = shibing[entity];
            shibingEnti.TarEntity = Entity.Null;
            shibingEnti.ShootEntity = Entity.Null;
            ECB.SetComponent(ChunkIndex, entity, shibingEnti);

            var sbChange = shibingChange[entity];
            sbChange.Act = ActState.NotMove;
            ECB.SetComponent(ChunkIndex, entity, sbChange);
            if (buff.Is_deBuff)
                InstanBuffParticle(entity, buff, spawn.debuffNotMove, false, Entity.Null, ChunkIndex);
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {

            buff.BuffTime -= time;
            if (buff.BuffTime <= 0)
                buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {
            ECB.SetComponentEnabled<Idle>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Walk>(ChunkIndex, entity, true);
            ECB.SetComponentEnabled<Move>(ChunkIndex, entity, false);
            ECB.SetComponentEnabled<Fire>(ChunkIndex, entity, false);

            var sbChange = shibingChange[entity];
            sbChange.Act = ActState.Idle;
            ECB.SetComponent(ChunkIndex, entity, sbChange);
            buff.buffAct = BuffAct.Delete;
        }
    }

    //�߷�buff
    void MutinyBuff(Entity entity, ref Buff buff, DynamicBuffer<Buff> buffcontainer, int ChunkIndex)
    {
        //��������ҵ�����������򹥻��ߵ�Ŀ�겻���ң��ͽ������buff
        if (!shibing.TryGetComponent(buff.MyAttacker, out ShiBing shib))
        {
            var entiSX = sx[entity];
            entiSX.MutinyValue = 0;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
            return;
        }
        if (shibing[buff.MyAttacker].ShootEntity != entity)
        {
            var entiSX = sx[entity];
            entiSX.MutinyValue = 0;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Delete;
            return;
        }

        if (buff.buffAct == BuffAct.Init)
        {
            //buff.BuffTime = sx[entity].Cur_HP * 0.02f;
            InstanBuffParticle(entity, buff, spawn.debuffMutiny, true, buff.MyAttacker, ChunkIndex,true );
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.Cur_IntervalTime -= time;
            if (buff.Cur_IntervalTime > 0)
                return;
            buff.Cur_IntervalTime = buff.IntervalTime;

            var entiSX = sx[entity];
            entiSX.MutinyValue += sx[buff.MyAttacker].AT;

            if (entiSX.MutinyValue >= entiSX.Cur_HP)
            {
                entiSX.MutinyValue = 0;
                buff.buffAct = BuffAct.End;
            }
            ECB.SetComponent(ChunkIndex, entity, entiSX);


        }
        else if(buff.buffAct == BuffAct.End)
        {
            InstanMutinyEntity(entity, ref buff, myLayer[buff.MyAttacker], buffcontainer, ChunkIndex);

            //ECB.AddComponent(ChunkIndex, entity, new Mutiny());
            buff.buffAct = BuffAct.Delete;
        }
    }

    //����Ѫ��buff
    void AddHPBuff(Entity entity, ref Buff buff, int ChunkIndex)
    {
        if (buff.buffAct == BuffAct.Init)
        {
            var entiSX = sx[entity];
            entiSX.HP = entiSX.HP + entiSX.HP * buff.BuffProportion;
            ECB.SetComponent(ChunkIndex, entity, entiSX);
            buff.buffAct = BuffAct.Run;
        }
        else if (buff.buffAct == BuffAct.Run)
        {


            //buff.buffAct = BuffAct.End;
        }
        else if (buff.buffAct == BuffAct.End)
        {

            buff.buffAct = BuffAct.Delete;
        }
    }

    //���û�Ѫbuff
    void RecoverHPBuff(Entity entity , ref Buff buff, int ChunkIndex)
    {
        if(buff.buffAct == BuffAct.Init)
        {
            buff.buffAct = BuffAct.Run;
        }
        else if(buff.buffAct == BuffAct.Run)
        {
            buff.Cur_IntervalTime -= time;//��������룬�ָ�һ��HP
            if(buff.Cur_IntervalTime <= 0)
            {
                buff.Cur_IntervalTime = buff.IntervalTime;
                var entiSX = sx[entity];
                entiSX.Cur_HP += entiSX.HP * buff.BuffProportion;
                if (entiSX.Cur_HP > entiSX.HP)
                    entiSX.Cur_HP = entiSX.HP;

                ECB.SetComponent(ChunkIndex, entity, entiSX);
            }



            //buff.buffAct = BuffAct.End;
        }
        else if(buff.buffAct == BuffAct.End)
        {

            buff.buffAct = BuffAct.Delete;
        }

    }


    //ʵ�����߷�ʿ��
    void InstanMutinyEntity(Entity entity, ref Buff buff, MyLayer MyAttackerlayer, DynamicBuffer<Buff> buffcontainer, int ChunkIndex)
    {
        Entity Myattacker = buff.MyAttacker;
        //�����������ҵ����Ǽ�����λ���Ǿ��õ�����Owner
        if(shibing.TryGetComponent(buff.MyAttacker, out ShiBing sbb))
        {
            if (shibing[buff.MyAttacker].Is_Parasitic)
            {
                Myattacker = parasitic[buff.MyAttacker].Owner;
                Debug.Log($"  ���ҵ������Ϊ������λ OwnerΪ:{parasitic[buff.MyAttacker].Owner}");
            }
        }
        var instanName = shibing[entity].Name;
        var mutinyShiBing = ECB.Instantiate(ChunkIndex, InstanWho(instanName, myLayer[entity]));

        var newlayer = new MyLayer();
        var shibingChange = new ShiBingChange();
        int teamID = 0;
        if (MyAttackerlayer.BelongsTo == layer.Team1 || MyAttackerlayer.CollidesWith_1 == layer.Team2)
        {
            teamID = 1;
            newlayer.BelongsTo = layer.Team1;
            newlayer.CollidesWith_1 = layer.Team2;
            shibingChange.enemyJiDi = spawn.JiDi_Team2;
            if (myLayer[entity].BulletCollidesWith != layer.NUll)
                newlayer.BulletCollidesWith = layer.Team2Bullet;
        }
        else if(MyAttackerlayer.BelongsTo == layer.Team2 || MyAttackerlayer.CollidesWith_1 == layer.Team1)
        {
            teamID = 2;
            newlayer.BelongsTo = layer.Team2;
            newlayer.CollidesWith_1 = layer.Team1;
            shibingChange.enemyJiDi = spawn.JiDi_Team1;
            if (myLayer[entity].BulletCollidesWith != layer.NUll)
                newlayer.BulletCollidesWith = layer.Team1Bullet;
        }
        //�޸Ĳ㼶���
        ECB.AddComponent(ChunkIndex, mutinyShiBing, newlayer);

        //�޸�ShiBingChange���
        shibingChange.Act = ActState.Idle;
        ECB.AddComponent(ChunkIndex, mutinyShiBing, shibingChange);

        //�����������
        var entityTransform = transform[entity];
        ECB.AddComponent(ChunkIndex, mutinyShiBing, entityTransform);

        //�����������
        var entitySX = sx[entity];
        ECB.AddComponent(ChunkIndex, mutinyShiBing, entitySX);

        //���ָ�ӹ��������
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new BarrageCommand
        {
            command = commandType.NUll,
            Comd_ShootEntity = Entity.Null,
        });
        //��ӻ������
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new Integral
        {
            ATIntegral = 0,
            AttackMeEntity = Entity.Null,
        });
        //��ʿ����ù����ҵ������ҵ�EntityID�����߷�
        if (entityOpenID.TryGetComponent(Myattacker, out EntityOpenID eoid))
        {
            ECB.AddComponent(ChunkIndex, mutinyShiBing, new EntityOpenID
            {
                PlayerEntiyID = entityOpenID[Myattacker].PlayerEntiyID,
            });
            Debug.Log($" �߷����Ϊ{entityOpenID[Myattacker].PlayerEntiyID}.");
        }
        else
        {
            Debug.Log($" �����ҵ������{Myattacker}.û��EntityOpenID���");
        }
        //�����ʿ����ӣ������ͷ��
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new AddAvaandPlayerList());

        ECB.AddComponent(ChunkIndex, mutinyShiBing, new Idle());
        ECB.SetComponentEnabled<Idle>(ChunkIndex, mutinyShiBing, false);
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new Walk());
        ECB.SetComponentEnabled<Walk>(ChunkIndex, mutinyShiBing, true);
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new Move());
        ECB.SetComponentEnabled<Move>(ChunkIndex, mutinyShiBing, false);
        ECB.AddComponent(ChunkIndex, mutinyShiBing, new Fire());
        ECB.SetComponentEnabled<Fire>(ChunkIndex, mutinyShiBing, false);

        ECB.AddComponent(ChunkIndex, entity, new Die { Is_DieDirectly = true });

        foreach(Buff b in buffcontainer)
        {
            ECB.AppendToBuffer(ChunkIndex, mutinyShiBing, b);
        }

        //ʵ��������
        if (instanName == ShiBingName.RongDian || instanName == ShiBingName.XiNiu || instanName == ShiBingName.HuoShen)
        {
            ECB.AddComponent(ChunkIndex, mutinyShiBing, new InstantiateFlesh
            {
                TeamID = teamID,
            });
        }


    }
    //ʵ����˭
    Entity InstanWho(ShiBingName name, MyLayer mylayer)
    {
        if(mylayer.BelongsTo == layer.Team2)
        {
            switch(name)
            {
                case ShiBingName.BaoLei: return             spawn.Team1_BaoLei;
                case ShiBingName.ChangGong: return          spawn.Team1_ChangGong;
                case ShiBingName.JianYa: return             spawn.Team1_JianYa;
                case ShiBingName.HuGuang: return            spawn.Team1_HuGuang;
                case ShiBingName.PaChong: return            spawn.Team1_PaChong;
                case ShiBingName.TieChui: return            spawn.Team1_TieChui;
                case ShiBingName.YeMa: return               spawn.Team1_YeMa;
                case ShiBingName.BaoYu: return              spawn.Team1_BaoYu;
                case ShiBingName.BingFeng: return           spawn.Team1_BingFeng;
                case ShiBingName.BaZhu: return              spawn.Team1_BaZhu;
                case ShiBingName.GangQiu: return            spawn.Team1_GangQiu;
                case ShiBingName.FengHuang: return          spawn.Team1_FengHuang;
                case ShiBingName.ZhanZhengGongChang: return spawn.Team1_ZhanZhengGongChang;
                case ShiBingName.HuoShen: return            spawn.Team1_HuoShen;
                case ShiBingName.HaiKe: return              spawn.Team1_HaiKe;
                case ShiBingName.RongDian: return           spawn.Team1_RongDian;
                case ShiBingName.XiNiu: return              spawn.Team1_XiNiu;
            }
        }
        else if (mylayer.BelongsTo == layer.Team1)
        {
            switch (name)
            {
                case ShiBingName.BaoLei: return             spawn.Team2_BaoLei;
                case ShiBingName.ChangGong: return          spawn.Team2_ChangGong;
                case ShiBingName.JianYa: return             spawn.Team2_JianYa;
                case ShiBingName.HuGuang: return            spawn.Team2_HuGuang;
                case ShiBingName.PaChong: return            spawn.Team2_PaChong;
                case ShiBingName.TieChui: return            spawn.Team2_TieChui;
                case ShiBingName.YeMa: return               spawn.Team2_YeMa;
                case ShiBingName.BaoYu: return              spawn.Team2_BaoYu;
                case ShiBingName.BingFeng: return           spawn.Team2_BingFeng;
                case ShiBingName.BaZhu: return              spawn.Team2_BaZhu;
                case ShiBingName.GangQiu: return            spawn.Team2_GangQiu;
                case ShiBingName.FengHuang: return          spawn.Team2_FengHuang;
                case ShiBingName.ZhanZhengGongChang: return spawn.Team2_ZhanZhengGongChang;
                case ShiBingName.HuoShen: return            spawn.Team2_HuoShen;
                case ShiBingName.HaiKe: return              spawn.Team2_HaiKe;
                case ShiBingName.RongDian: return           spawn.Team2_RongDian;
                case ShiBingName.XiNiu: return              spawn.Team2_XiNiu;
            }
        }

        return Entity.Null;
    }
    //ʵ����buff��Ч
    void InstanBuffParticle(Entity entity, Buff buff, Entity buffParticle,bool Is_CasterDead, Entity MyAttackerEnti, int ChunkIndex ,bool Is_NoDieTime = false)
    {
        var buffPar = ECB.Instantiate(ChunkIndex, buffParticle);
        var centPoint = shibing[entity].CenterPoint;
        if (!transform.TryGetComponent(centPoint, out LocalTransform ltf))
            centPoint = entity;
        ECB.SetComponent(ChunkIndex, buffPar, new LocalTransform
        {
            Position = LocaltoWorld[shibing[entity].CenterPoint].Position,
            Rotation = LocaltoWorld[entity].Rotation,
            Scale = sx[entity].BuffParticleScale,
        });
        ECB.AddComponent(ChunkIndex, buffPar, new ParticleComponent
        {
            DieTime = buff.BuffTime,
            Is_Follow = true,
            FollowPoint = centPoint,
            ParScale = sx[entity].BuffParticleScale,
            Is_CasterDead = Is_CasterDead,
            Caster = MyAttackerEnti,
            Is_NoDieTime = Is_NoDieTime,
        });
    }
}
