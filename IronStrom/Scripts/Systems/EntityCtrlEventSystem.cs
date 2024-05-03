using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;


public partial class EntityCtrlEventSystem : SystemBase
{
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<LocalTransform> m_transfrom;
    ComponentLookup<SX> m_SX;
    ComponentLookup<DirectShoot> m_DirectShoot;
    ComponentLookup<DirectBullet> m_DirectBullet;
    ComponentLookup<CorrectPosition> m_CorrPos;
    ComponentLookup<UpSkill> m_UpSkill;
    ComponentLookup<SynchronizeAni> m_SynchronizeAni;
    ComponentLookup<JiDi> m_JiDi;
    ComponentLookup<BaZhu> m_BaZhu;
    BufferLookup<Buff> m_Buffer;

    public Entity CD_ShootEntity = Entity.Null;

    void UpDateComponentLookup()
    {
        m_shibing.Update(this);
        m_LocaltoWorld.Update(this);
        m_transfrom.Update(this);
        m_DirectShoot.Update(this);
        m_DirectBullet.Update(this);
        m_CorrPos.Update(this);
        m_UpSkill.Update(this);
        m_Buffer.Update(this);
        m_SynchronizeAni.Update(this);
        m_JiDi.Update(this);
        m_BaZhu.Update(this);
    }
    protected override void OnCreate()
    {
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_transfrom = GetComponentLookup<LocalTransform>(true);
        m_DirectShoot = GetComponentLookup<DirectShoot>(true);
        m_DirectBullet = GetComponentLookup<DirectBullet>(true);
        m_CorrPos = GetComponentLookup<CorrectPosition>(true);
        m_UpSkill = GetComponentLookup<UpSkill>(true);
        m_SynchronizeAni = GetComponentLookup<SynchronizeAni>(true);
        m_JiDi = GetComponentLookup<JiDi>(true);
        m_BaZhu = GetComponentLookup<BaZhu>(true);
        m_Buffer = GetBufferLookup<Buff>(true);




    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();
        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();

        int random = UnityEngine.Random.Range(0, 100);
        NativeArray<float> randList = new NativeArray<float>(2, Allocator.TempJob);
        for (int i = 0; i < randList.Length; ++i)
        {
            randList[i] = UnityEngine.Random.Range(1.4f, -1.4f); //��С������ŵ��ƫ�ƾ���
            //randList[i] = UnityEngine.Random.Range(10f, -10f); //ԭ��������ŵ��ƫ�ƾ���
        }


        var EntiCtrlEventECB = new EntityCommandBuffer(Allocator.TempJob);
        var entiyCtrlEventjob = new EntiCtrlFireEventJob
        {
            ECB = EntiCtrlEventECB.AsParallelWriter(),
            time = SystemAPI.Time.DeltaTime,
            shibing = m_shibing,
            LocaltoWorld = m_LocaltoWorld,
            transfrom = m_transfrom,
            directShoot = m_DirectShoot,
            directbullet = m_DirectBullet,
            CorrPos = m_CorrPos,
            random = random,
            randoms = randList,
            upSkill = m_UpSkill,
            bufflookup = m_Buffer,
            synchronizeAni = m_SynchronizeAni,
            jidi = m_JiDi,
            bazhu = m_BaZhu,
            spawn = spawn,

            CD_ShootEntity = CD_ShootEntity,
        };
        Dependency = entiyCtrlEventjob.ScheduleParallel(Dependency);
        Dependency.Complete();
        EntiCtrlEventECB.Playback(EntityManager);
        EntiCtrlEventECB.Dispose();
        // �ͷ� NativeArray
        randList.Dispose();



        //�߷����Buff
        Entities.ForEach((Entity entity, EntityCtrl entiCtrl,ref ShiBing shibing,ref ShiBingChange sbChange , ref SX sx,in MyLayer mlayer) =>
        {
            if (sbChange.Act == ActState.Fire && sx.Cur_ShootTime <= 0)
            {
                if (shibing.Name == ShiBingName.HaiKe)
                    HaiKeEvent(entity,ref shibing, ref sx, random);
            }
        }).WithoutBurst().WithStructuralChanges().Run();


    }

    void HaiKeEvent(Entity entity,ref ShiBing shibing, ref SX sx, int random)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        UpDateComponentLookup();
        if (m_DirectShoot[entity].Is_ShootEntityChanges)
        {
            //���ķ�֮һ�ļ��������Ŀ��ʩչһ��buff
            if (random >= 0 && random < 25)
            {
                ADDBuff(ref shibing.ShootEntity, BuffType.buffMutiny, BuffAct.Init, 0, 5f, true, entity, sx.ShootTime);
                //��Ӳ߷�Buffͬʱ����������ʿ���������Ѫ���߷���
                EntityUIManager.Instance.AddMutinySlider(shibing.ShootEntity, EntityManager);

            }
            //else if (random >= 25 && random < 50)
            //    ADDBuff(ref shibing.ShootEntity, BuffType.buffShootTime, BuffAct.Init, 0.2f, 5f, true, entity);
            //else if (random >= 50 && random < 75)
            //    ADDBuff(ref shibing.ShootEntity, BuffType.buffSpeed, BuffAct.Init, 0.2f, 5f, true, entity);
            //else if (random >= 75 && random < 100)
            //    ADDBuff(ref shibing.ShootEntity, BuffType.buffNotMove, BuffAct.Init, 0.2f, 5f, true, entity);
        }
    }
    //���buff
    void ADDBuff(ref Entity shootEntity, BuffType type, BuffAct act, float BuffProportion, float BuffTime,
                 bool Is_deBuff, Entity myAttacker, float intervalTime = 0)
    {
        if (!EntityManager.HasBuffer<Buff>(shootEntity))
            return;
        UpDateComponentLookup();
        var bufflkUP = EntityManager.GetBuffer<Buff>(shootEntity);//m_Buffer[shootEntity];
        for (int i = 0; i < bufflkUP.Length; i++)//��������buff���˳������
        {
            if (bufflkUP[i].buffType == type)
            {
                //var buff = bufflkUP[i];
                //buff.BuffTime = BuffTime;
                //bufflkUP[i] = buff;
                return;
            }
        }
        var buff = new Buff
        {
            buffType = type,
            buffAct = act,
            BuffProportion = BuffProportion,
            BuffTime = BuffTime,
            Is_deBuff = Is_deBuff,
            MyAttacker = myAttacker,
            IntervalTime = intervalTime,
            Cur_IntervalTime = intervalTime,
        };
        EntityManager.AddBuffer<Buff>(shootEntity).Add(buff);
        //ECB.AppendToBuffer(ChunkIndex, shootEntity, buff);//�����buff
        //Debug.Log("    ���buff�ɹ�  " + shootEntity);
    }
}

public partial struct EntiCtrlFireEventJob : IJobEntity//ֱ�ӿ���Entity��FireEvent
{
    public float time;
    public Spawn spawn;
    public Entity CD_ShootEntity;//����ͬһ��Ŀ��ͼ���buff
    public int random;
    public NativeArray<float> randoms;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocaltoWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> transfrom;
    [ReadOnly] public ComponentLookup<DirectShoot> directShoot;
    [ReadOnly] public ComponentLookup<DirectBullet> directbullet;
    [ReadOnly] public ComponentLookup<CorrectPosition> CorrPos;
    [ReadOnly] public ComponentLookup<UpSkill> upSkill;
    [ReadOnly] public ComponentLookup<SynchronizeAni> synchronizeAni;
    [ReadOnly] public ComponentLookup<JiDi> jidi;
    [ReadOnly] public ComponentLookup<BaZhu> bazhu;
    [ReadOnly] public BufferLookup<Buff> bufflookup;


    void Execute(Entity entity, EntityCtrl entiCtrl, ShiBingChange shibingChange,ref SX sx, in MyLayer mlayer, [ChunkIndexInQuery]int ChunkIndex)
    {
        if (shibingChange.Act == ActState.Fire && sx.Cur_ShootTime <= 0)
        {
            switch (shibing[entity].Name)
            {
                case ShiBingName.TieChui: TieChuiEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.YeMa: YeMaEvent(entity, entiCtrl, ref sx, ChunkIndex); break;
                case ShiBingName.GangQiu: GangQiuEvent(entity, entiCtrl, ref sx, ChunkIndex); break;
                case ShiBingName.BaoYu: BaoYuEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.BaZhu: BaZhuEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.FengHuang: FengHuangEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.ZhanZhengGongChang: WarFactoryEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.HuoShen: HuoShenEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.HaiKe:HaiKeEvent(entity, entiCtrl, ref sx, ChunkIndex);break;
                case ShiBingName.RongDian:RongDianEvent(entity, entiCtrl, ref sx, ChunkIndex);break;
                case ShiBingName.XiNiu: XiNiuEvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex); break;
                case ShiBingName.Monster_1: Monster1Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.Monster_3: Monster3Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.Monster_4: Monster4Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.Monster_5: Monster5Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.Monster_6: Monster6Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.Monster_7: Monster7Event(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;
                case ShiBingName.TOWER: TOWEREvent(entity, entiCtrl, ref sx, in mlayer, ChunkIndex);break;

            }
        }
        else if (shibingChange.Act == ActState.Walk)
        {
            switch (shibing[entity].Name)
            {
                case ShiBingName.HuoShen: HuoShenNoFireEvent(entity, ChunkIndex); break;
            }
        }
        else if (shibingChange.Act == ActState.Move)
        {
            switch (shibing[entity].Name)
            {
                case ShiBingName.HuoShen: HuoShenNoFireEvent(entity, ChunkIndex); break;
            }
        }


        //����������
        if (shibing[entity].Name == ShiBingName.BaZhu)
        {
            BaZhuMainGun(entity,ref sx, in mlayer, ChunkIndex);
        }

    }


    void TieChuiEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer,int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_R, ref sx, in mlayer, ChunkIndex);//ʵ�����ӵ�
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_R, ChunkIndex);//ʵ������Ч
        InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_L, ref sx, in mlayer, ChunkIndex);//ʵ�����ӵ�
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_L, ChunkIndex);//ʵ������Ч

        InstanParticle(entity, entiCtrl.Particle_1, shibing[entity].CenterPoint, ChunkIndex);//��Чλ��
    }

    void YeMaEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, int ChunkIndx)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//���û��DirectShoot������˳�
            return;

        if (transfrom.TryGetComponent(directShoot[entity].DirectParticle_Entity,out LocalTransform lt))//�����ֱ�����õĹ�����Ч���˳�
            return;

        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndx);
        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;
        ECB.SetComponent(ChunkIndx, entity, dirshoot);
        ECB.SetComponent(ChunkIndx, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });
    }

    void GangQiuEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, int ChunkIndx)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//���û��DirectShoot������˳�
            return;
        if (transfrom.TryGetComponent(directShoot[entity].DirectParticle_Entity, out LocalTransform lt))//�����ֱ�����õĹ�����Ч���˳�
            return;
        if (!transfrom.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf4))
            return;
        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndx);
        var par2 = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_L, ChunkIndx);
        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;

        ECB.SetComponent(ChunkIndx, entity, dirshoot);
        ECB.SetComponent(ChunkIndx, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });
        ECB.SetComponent(ChunkIndx, par2, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_L,
        });

    }

    void BaoYuEvent(Entity entity,EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var firePoint = TurnFirePoint(entity, ref sx, ChunkIndex, sx.Fire_TakeTurnsIntNum);//���������
        if (firePoint == Entity.Null)
            return;

        var shootenti = shibing[entity].ShootEntity;
        if (!shibing.TryGetComponent(shootenti, out ShiBing sbb))
            return;
        
        if (IS_AirForce(shibing[shootenti].Name))
        {
            //�Կյ�λ��ֱ����������߼�
            if (mlayer.BelongsTo == layer.Team1)
                InstanBullet(entity, spawn.BaoYuBullet_1_Air, firePoint, ref sx, in mlayer, ChunkIndex);
            else if (mlayer.BelongsTo == layer.Team2)
                InstanBullet(entity, spawn.BaoYuBullet_2_Air, firePoint, ref sx, in mlayer, ChunkIndex);
        }
        else
        {
            //�Եص�λ�ı��������߹����߼�
            bool NoShootEntity = false; ;
            float3 enemypoint = RandomImpactPoint(entity, ChunkIndex, ref NoShootEntity);//������ŵ�
            if (shibing[entity].ShootEntity == Entity.Null || NoShootEntity)
                return;
            InstanBezaerBullet(entity, entiCtrl.Bullet, firePoint, in enemypoint, ref sx, in mlayer, false,ChunkIndex);
        }


        InstanParticle(entity, entiCtrl.Muzzle, firePoint, ChunkIndex);
    }

    void BaZhuEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var firePoint = TurnFirePoint_1(entity, ref sx, ChunkIndex, sx.Fire_TakeTurnsIntNum);//���������
        if (firePoint == Entity.Null)
            return;
        bool NoShootEntity = false; ;
        float3 enemypoint = RandomImpactPoint(entity,ChunkIndex,ref NoShootEntity);//������ŵ�

        if (shibing[entity].ShootEntity == Entity.Null || NoShootEntity)
            return;


        InstanBezaerBullet(entity, entiCtrl.Bullet, firePoint, in enemypoint, ref sx, in mlayer, true, ChunkIndex);
        InstanParticle(entity, entiCtrl.Muzzle, firePoint, ChunkIndex);

    }

    void FengHuangEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_R2, ref sx, in mlayer, ChunkIndex);//ʵ�����ӵ�
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_R, ChunkIndex);//ʵ������Ч
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_L, ChunkIndex);//ʵ������Ч
    }

    void WarFactoryEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_R, ref sx, in mlayer, ChunkIndex);//ʵ�����ӵ�
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_R, ChunkIndex);//ʵ������Ч
    }

    void HuoShenEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mylayer, int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        if (transfrom.TryGetComponent(shibing[entity].CorrectPosEntity, out LocalTransform ltf) ||
             shibing[entity].CorrectPosEntity != Entity.Null)//����еĸ����ҵ�������˳�
            return;
        var Attackbox = InstanAttackBox(entity, entiCtrl.AttackBox,shibing[entity].FirePoint_R, mylayer, ChunkIndex);
        if (Attackbox == Entity.Null)
            return;
        ECB.SetComponent(ChunkIndex, Attackbox, new CorrectPosition
        {
            Owner = entity,
        });

        var sb = shibing[entity];
        sb.CorrectPosEntity = Attackbox;
        sb.CorrectPosition_IsDie = false;
        ECB.SetComponent(ChunkIndex, entity, sb);

    }
    void HuoShenNoFireEvent(Entity entity,int ChunkIndex)//����Fire��Ϊ��ɾ��Box(������)
    {
        var sb = shibing[entity];
        sb.CorrectPosition_IsDie = true;
        ECB.SetComponent(ChunkIndex, entity, sb);
    }

    void HaiKeEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, int ChunkIndx)
    {
        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//���û��DirectShoot������˳�
            return;
        if (!transfrom.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf4))
            return;

        //�ۻ�Ŀ���Ƿ�Ϊͬһ��Ŀ�꣬�Ƿ�Ҫ���¼�Buff-------------

        //----------

        if (transfrom.TryGetComponent(directShoot[entity].DirectParticle_Entity, out LocalTransform lt))//�����ֱ�����õĹ�����Ч���˳�
            return;

        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndx);

        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;

        ECB.SetComponent(ChunkIndx, entity, dirshoot);
        ECB.SetComponent(ChunkIndx, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });

    }

    void RongDianEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, int ChunkIndx)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//���û��DirectShoot������˳�
            return;
        if (transfrom.TryGetComponent(directShoot[entity].DirectParticle_Entity, out LocalTransform lt))//�����ֱ�����õĹ�����Ч���˳�
            return;
        if (!transfrom.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf4))
            return;
        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndx);
        var par2 = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_L, ChunkIndx);
        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;

        ECB.SetComponent(ChunkIndx, entity, dirshoot);
        ECB.SetComponent(ChunkIndx, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });
        ECB.SetComponent(ChunkIndx, par2, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_L,
        });
    }

    void XiNiuEvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        if (!directShoot.TryGetComponent(entity, out DirectShoot ds))//���û��DirectShoot������˳�
            return;
        if (transfrom.TryGetComponent(directShoot[entity].DirectParticle_Entity, out LocalTransform lt))//�����ֱ�����õĹ�����Ч���˳�
            return;
        var par = InstanParticle(entity, directShoot[entity].DirectParticle_Parfab, shibing[entity].FirePoint_R, ChunkIndex);
        var dirshoot = directShoot[entity];
        dirshoot.DirectParticle_Entity = par;
        ECB.SetComponent(ChunkIndex, entity, dirshoot);
        ECB.SetComponent(ChunkIndex, par, new DirectBulletChange
        {
            Owner = entity,
            DB_FirePoint = DirectBullet_FirePoint.FirePoint_R,
        });
    }

    void Monster1Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        //if (!synchronizeAni.TryGetComponent(entity, out SynchronizeAni sca))//�������(û��)Obj�����ĵ�λ
        //    return;
        var sy = synchronizeAni[entity];
        ObjAniEventInstanAttackBox(entity, entiCtrl, shibing[entity].FirePoint_R,sy.ObjAniTotalTime_Fire, sy.ObjAniEventTime_Fire, ref sx, mlayer, ChunkIndex);

    }

    void Monster3Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var sy = synchronizeAni[entity];
        if (sy.ShootEnitIs_Air)
            ObjAniEventInstanAttackBox(entity, entiCtrl, shibing[entity].FirePoint_L, sy.ObjAniTotalTime_FireAir, sy.ObjAniEventTime_FireAir, ref sx, mlayer, ChunkIndex);
        else
            ObjAniEventInstanAttackBox(entity, entiCtrl, shibing[entity].FirePoint_R, sy.ObjAniTotalTime_Fire, sy.ObjAniEventTime_Fire, ref sx, mlayer, ChunkIndex);
    }

    void Monster4Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var SynAni = synchronizeAni[entity];
        //Debug.Log("    EventFire : " + SynAni.Is_EventFire);
        if (SynAni.Is_EventFire_1)//����Event�¼�
        {
            if (!transfrom.TryGetComponent(entiCtrl.AttackBox, out LocalTransform ltf))
                return;
            var monstetrBox = ECB.Instantiate(ChunkIndex, entiCtrl.AttackBox);
            var EntiShibingWorld = LocaltoWorld[shibing[entity].FirePoint_R];
            ECB.SetComponent(ChunkIndex, monstetrBox, EntiShibingWorld);
            ECB.SetComponent(ChunkIndex, monstetrBox, new LocalTransform
            {
                Position = EntiShibingWorld.Position,
                Rotation = EntiShibingWorld.Rotation,
                Scale = 1,
            });

            SetBulletMyLayer(mlayer, ref monstetrBox, ChunkIndex);

            SynAni.Is_DotsEventFire = true;//�������(DOTS)�������
            SynAni.Is_EventFire_1 = false;
        }
        else if (SynAni.Is_PlayingAniFire)//��������¼��������
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Is_DotsPlayingAniFire = true;
            SynAni.Is_PlayingAniFire = false;
        }

        ECB.SetComponent(ChunkIndex, entity, SynAni);
    }

    void Monster5Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var SynAni = synchronizeAni[entity];
        if (!transfrom.TryGetComponent(entiCtrl.AttackBox, out LocalTransform ltf) || 
            !transfrom.TryGetComponent(shibing[entity].FirePoint_L, out LocalTransform ltf1)||
            !transfrom.TryGetComponent(shibing[entity].FirePoint_R, out LocalTransform ltf2))
            return;
        if (SynAni.Is_EventFire_1)//����Event�¼�
        {
            var monstetrBox = ECB.Instantiate(ChunkIndex, entiCtrl.AttackBox);
            var EntiShibingWorld = LocaltoWorld[shibing[entity].FirePoint_R];
            ECB.SetComponent(ChunkIndex, monstetrBox, EntiShibingWorld);
            ECB.SetComponent(ChunkIndex, monstetrBox, new LocalTransform
            {
                Position = EntiShibingWorld.Position,
                Rotation = EntiShibingWorld.Rotation,
                Scale = 1,
            });

            SetBulletMyLayer(mlayer, ref monstetrBox, ChunkIndex);

            SynAni.Is_DotsEventFire = false;
            SynAni.Is_EventFire_1 = false;
        }
        if(SynAni.Is_EventFire_2)
        {
            var monstetrBox = ECB.Instantiate(ChunkIndex, entiCtrl.AttackBox);
            var EntiShibingWorld = LocaltoWorld[shibing[entity].FirePoint_L];
            ECB.SetComponent(ChunkIndex, monstetrBox, EntiShibingWorld);
            ECB.SetComponent(ChunkIndex, monstetrBox, new LocalTransform
            {
                Position = EntiShibingWorld.Position,
                Rotation = EntiShibingWorld.Rotation,
                Scale = 1,
            });

            SetBulletMyLayer(mlayer, ref monstetrBox, ChunkIndex);


            SynAni.Is_DotsEventFire = true;//�������(DOTS)�������
            SynAni.Is_EventFire_2 = false;
        }
        if (SynAni.Is_PlayingAniFire)//��������¼��������
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Is_DotsPlayingAniFire = true;
            SynAni.Is_PlayingAniFire = false;
        }


        ECB.SetComponent(ChunkIndex, entity, SynAni);
    }

    void Monster6Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var SynAni = synchronizeAni[entity];
        if (!transfrom.TryGetComponent(entiCtrl.AttackBox, out LocalTransform ltf) ||
            !transfrom.TryGetComponent(shibing[entity].FirePoint_R, out LocalTransform ltf1))
            return;
        if (SynAni.Is_EventFire_1)//����Event�¼�
        {
            var monstetrBox = ECB.Instantiate(ChunkIndex, entiCtrl.AttackBox);
            var EntiShibingWorld = LocaltoWorld[shibing[entity].FirePoint_R];
            ECB.SetComponent(ChunkIndex, monstetrBox, EntiShibingWorld);
            ECB.SetComponent(ChunkIndex, monstetrBox, new LocalTransform
            {
                Position = EntiShibingWorld.Position,
                Rotation = EntiShibingWorld.Rotation,
                Scale = 1,
            });

            SetBulletMyLayer(mlayer, ref monstetrBox, ChunkIndex);

            SynAni.Is_DotsEventFire = true;//�������(DOTS)�������
            SynAni.Is_EventFire_1 = false;
        }
        if (SynAni.Is_PlayingAniFire)//��������¼��������
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Is_DotsPlayingAniFire = true;
            SynAni.Is_PlayingAniFire = false;
        }
        ECB.SetComponent(ChunkIndex, entity, SynAni);
    }

    void Monster7Event(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        var SynAni = synchronizeAni[entity];
        if (!transfrom.TryGetComponent(entiCtrl.Bullet, out LocalTransform ltf) ||
            !transfrom.TryGetComponent(shibing[entity].FirePoint_R, out LocalTransform ltf1) ||
            !transfrom.TryGetComponent(shibing[entity].ShootEntity, out LocalTransform ltf2))
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Is_DotsPlayingAniFire = true;
            SynAni.Is_PlayingAniFire = false;
            return;
        }

        if (SynAni.Is_EventFire_1)//����Event�¼�
        {
            InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_R, ref sx, in mlayer, ChunkIndex);

            SynAni.Is_DotsEventFire = true;//�������(DOTS)�������
            SynAni.Is_EventFire_1 = false;
        }
        if (SynAni.Is_PlayingAniFire)//��������¼��������
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Is_DotsPlayingAniFire = true;
            SynAni.Is_PlayingAniFire = false;
        }
        ECB.SetComponent(ChunkIndex, entity, SynAni);
    }

    void TOWEREvent(Entity entity, EntityCtrl entiCtrl, ref SX sx, in MyLayer mlayer, int ChunkIndex)
    {
        sx.Cur_ShootTime = sx.ShootTime;
        InstanBullet(entity, entiCtrl.Bullet, shibing[entity].FirePoint_R, ref sx, in mlayer, ChunkIndex);//ʵ�����ӵ�
        InstanParticle(entity, entiCtrl.Muzzle, shibing[entity].FirePoint_R, ChunkIndex);//ʵ������Ч
    }

    //ʵ�����ӵ�
    void InstanBullet(Entity entity, Entity bulletP, Entity firepoint, ref SX sx,in MyLayer layer, int ChunkIndex)
    {
        if(!LocaltoWorld.TryGetComponent(entity, out LocalToWorld ltw) ||
           !LocaltoWorld.TryGetComponent(bulletP, out LocalToWorld ltw1)||
           !LocaltoWorld.TryGetComponent(firepoint, out LocalToWorld ltw2) ||
           !shibing.TryGetComponent(shibing[entity].ShootEntity, out ShiBing s))
        {
            return;
        }
        Entity centerPoint = shibing[shibing[entity].ShootEntity].CenterPoint;//��õ��˵�����λ��
        if (jidi.TryGetComponent(shibing[entity].ShootEntity,out JiDi jid))//����ǻ���
        {
            centerPoint = shibing[entity].JidiPoint;
        }
        else if (!LocaltoWorld.TryGetComponent(shibing[shibing[entity].ShootEntity].CenterPoint, out LocalToWorld ltw3))
        {
            centerPoint = shibing[entity].ShootEntity;
        }

        Entity bullet = ECB.Instantiate(ChunkIndex, bulletP);
        var firePoint = LocaltoWorld[firepoint].Position;
        var dir = LocaltoWorld[centerPoint].Position - firePoint;
        dir = math.normalizesafe(dir);

        var localworld = LocaltoWorld[firepoint];
        ECB.SetComponent(ChunkIndex, bullet, localworld);
        ECB.AddComponent(ChunkIndex, bullet, new LocalTransform
        {
            Position = firePoint,
            Rotation = quaternion.LookRotationSafe(dir,new float3(0,1,0)),
            Scale = 1,
        });
        ECB.AddComponent(ChunkIndex, bullet, new BulletChange
        {
            Dir = dir,
            AT = sx.AT,
        });
        SetBulletMyLayer(layer, ref bullet, ChunkIndex);

        //���ӵ������Ϣ
        ECB.AddComponent(ChunkIndex, entity, new OwnerData
        {
            Owner = entity,
        });
    }
    //ʵ�����������ӵ�
    void InstanBezaerBullet(Entity entity, Entity bulletP, Entity firepoint, in float3 enemypoint, ref SX sx, in MyLayer layer, bool Is_Random,int ChunkIndex)
    {
        if (!LocaltoWorld.TryGetComponent(entity, out LocalToWorld ltw) ||
            !LocaltoWorld.TryGetComponent(bulletP, out LocalToWorld ltw1) ||
            !LocaltoWorld.TryGetComponent(firepoint, out LocalToWorld ltw2))
        {
            return;
        }

        Entity bullet = ECB.Instantiate(ChunkIndex, bulletP);
        var localworld = LocaltoWorld[firepoint];
        ECB.SetComponent(ChunkIndex, bullet, localworld);
        ECB.AddComponent(ChunkIndex, bullet, new LocalTransform
        {
            Position = localworld.Position,
            Rotation = localworld.Rotation,
            Scale = 1,
        });
        ECB.AddComponent(ChunkIndex, bullet, new BulletChange
        {
            AT = sx.AT,
        });
        var distance = math.distance(LocaltoWorld[firepoint].Position, enemypoint);
        ECB.AddComponent(ChunkIndex, bullet, new BezierFiringChange
        {
            StartPosition = LocaltoWorld[firepoint].Position,
            EndPosition = enemypoint,
            DirenDistance = distance,
            Is_RandomBezier = Is_Random,
        });
        SetBulletMyLayer(layer, ref bullet, ChunkIndex);

        //���ʿ�����������ܵ�ʿ��,�͸��ⷢ�ӵ�����UpSkill
        if (upSkill.TryGetComponent(entity,out UpSkill ups))
        {
            if (upSkill[entity].Is_UpSkill && upSkill[entity].upSkill_Name == UpSkillName.BaoYu)
            {
                ECB.AddComponent(ChunkIndex, bullet, new UpSkill
                {
                    Is_UpSkill = true,
                    upSkill_Name = UpSkillName.BaoYuBullet,
                });
            }
        }

        //���ӵ������Ϣ
        ECB.AddComponent(ChunkIndex, entity, new OwnerData
        {
            Owner = entity,
        });

    }
    //ʵ������������
    Entity InstanAttackBox(Entity entity, Entity box, Entity Point,in  MyLayer mlayer ,int ChunkIndex)
    {
        if (!transfrom.TryGetComponent(entity, out LocalTransform ltf) ||
            !transfrom.TryGetComponent(box, out LocalTransform ltf1) ||
            !transfrom.TryGetComponent(Point, out LocalTransform ltf2))
        {
            return Entity.Null;
        }

        var instanBox = ECB.Instantiate(ChunkIndex, box);
        //SetBulletMyLayer(mlayer,ref instanBox, ChunkIndex);//����Layer
        ECB.SetComponent(ChunkIndex, instanBox, new LocalTransform
        {
            Position = LocaltoWorld[Point].Position,
            Rotation = LocaltoWorld[Point].Rotation,
            Scale = 1,
        });
        //���ӵ������Ϣ
        ECB.AddComponent(ChunkIndex, entity, new OwnerData
        {
            Owner = entity,
        });


        return instanBox;
    }
    //ʵ������Ч
    Entity InstanParticle(Entity shibing, Entity particle, Entity point,int ChunkIndx)
    {
        if (!LocaltoWorld.TryGetComponent(shibing, out LocalToWorld ltw) ||
            !LocaltoWorld.TryGetComponent(particle, out LocalToWorld ltw1) ||
            !LocaltoWorld.TryGetComponent(point, out LocalToWorld ltw2)) 
        {
            return Entity.Null;
        }

        var par = ECB.Instantiate(ChunkIndx, particle);
        var localworld = LocaltoWorld[point];
        ECB.SetComponent(ChunkIndx, par, localworld);
        ECB.SetComponent(ChunkIndx, par, new LocalTransform
        {
            Position = LocaltoWorld[point].Position,
            Rotation = LocaltoWorld[point].Rotation,
            Scale = 1,
        });
        return par;
    }
    //Obj���������ʵ��������Box
    void ObjAniEventInstanAttackBox(Entity entity, EntityCtrl entiCtrl,Entity Pos,float EventTotalTime,float objEventTime , ref SX sx, MyLayer mlayer, int ChunkIndex)
    {
        var SynAni = synchronizeAni[entity];
        SynAni.Cur_ObjAniTotalTime_Fire += time;
        if (SynAni.EventKey == 0 && SynAni.Cur_ObjAniTotalTime_Fire >= objEventTime)
        {
            //�¼�
            if (!transfrom.TryGetComponent(entiCtrl.AttackBox, out LocalTransform ltf))
                return;
            var monstetrBox = ECB.Instantiate(ChunkIndex, entiCtrl.AttackBox);
            var EntiShibingWorld = LocaltoWorld[Pos];
            ECB.SetComponent(ChunkIndex, monstetrBox, EntiShibingWorld);
            ECB.SetComponent(ChunkIndex, monstetrBox, new LocalTransform
            {
                Position = EntiShibingWorld.Position,
                Rotation = EntiShibingWorld.Rotation,
                Scale = 1,
            });

            SetBulletMyLayer(mlayer, ref monstetrBox, ChunkIndex);
            SynAni.EventKey = 1;
        }
        else if (SynAni.EventKey == 1 && SynAni.Cur_ObjAniTotalTime_Fire >= EventTotalTime)
        {
            sx.Cur_ShootTime = sx.ShootTime;
            SynAni.Cur_ObjAniTotalTime_Fire = 0;
            SynAni.EventKey = 0;
        }


        ECB.SetComponent(ChunkIndex, entity, SynAni);
    }

    //����ͼ��
    void SetBulletMyLayer(in MyLayer shibingLayer, ref Entity Instan, int chunkIndx)
    {
        var InstanLyaer = new MyLayer();
        if (shibingLayer.BelongsTo == layer.Team1 || shibingLayer.ParasiticBelongsTo == layer.Team1)
        {
            InstanLyaer.BelongsTo = layer.Team1Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team2;
            InstanLyaer.CollidesWith_2 = layer.Neutral;
        }
        else if (shibingLayer.BelongsTo == layer.Team2 || shibingLayer.ParasiticBelongsTo == layer.Team2)
        {
            InstanLyaer.BelongsTo = layer.Team2Bullet;
            InstanLyaer.CollidesWith_1 = layer.Team1;
            InstanLyaer.CollidesWith_2 = layer.Neutral;
        }
        else if(shibingLayer.BelongsTo == layer.Neutral)
        {
            InstanLyaer.BelongsTo = layer.Neutral;
            InstanLyaer.CollidesWith_1 = layer.Team1;
            InstanLyaer.CollidesWith_2 = layer.Team2;
        }


        ECB.SetComponent(chunkIndx,Instan, InstanLyaer);
    }
    //���������
    Entity TurnFirePoint(Entity entity,ref SX sx, int ChunkIndex, int Num)
    {
        var firePoint = Entity.Null;
        var shibingComp = shibing[entity];
        if (sx.Cur_ShootTime <= -0.75f && shibingComp.Fire_TakeTurnsInt < 4)
        {
            firePoint = shibing[entity].FirePoint_L2;
            shibingComp.Fire_TakeTurnsInt = 4;
        }
        else if (sx.Cur_ShootTime <= -0.5f && shibingComp.Fire_TakeTurnsInt < 3)
        {
            firePoint = shibing[entity].FirePoint_R2;
            shibingComp.Fire_TakeTurnsInt = 3;
        }
        else if (sx.Cur_ShootTime <= -0.25f && shibingComp.Fire_TakeTurnsInt < 2)
        {
            firePoint = shibing[entity].FirePoint_L;
            shibingComp.Fire_TakeTurnsInt = 2;
        }
        else if (sx.Cur_ShootTime <= 0 && shibingComp.Fire_TakeTurnsInt < 1)
        {
            firePoint = shibing[entity].FirePoint_R;
            shibingComp.Fire_TakeTurnsInt = 1;
        }

        if (shibingComp.Fire_TakeTurnsInt == Num)
        {
            sx.Cur_ShootTime = sx.ShootTime;
            shibingComp.Fire_TakeTurnsInt = 0;
        }
        ECB.SetComponent(ChunkIndex, entity, shibingComp);
        return firePoint;
    }
    //���������
    Entity TurnFirePoint_1(Entity entity, ref SX sx, int ChunkIndex, int Num)
    {
        sx.Cur_AttackNumberTime -= time;
        if (sx.Cur_AttackNumberTime >= 0)
            return Entity.Null;
        else
            sx.Cur_AttackNumberTime = sx.AttackNumberTime;
        var firePoint = Entity.Null;
        var shibingComp = shibing[entity];
        firePoint = sx.Cur_Fire_TakeTurnsIntNum % 2 == 0 ? shibing[entity].FirePoint_R : shibing[entity].FirePoint_L;
        sx.Cur_Fire_TakeTurnsIntNum -= 1;
        if(sx.Cur_Fire_TakeTurnsIntNum <= 0)
        {
            sx.Cur_ShootTime = sx.ShootTime;
            sx.Cur_Fire_TakeTurnsIntNum = sx.Fire_TakeTurnsIntNum;
        }
        return firePoint;
    }
    //������ŵ�
    float3 RandomImpactPoint(Entity entity,int ChunkIndex ,ref bool NoShootEntity,bool Is_Ground = false)
    {
        if (!shibing.TryGetComponent(shibing[entity].ShootEntity, out ShiBing s))
        {
            var shibingEnit = shibing[entity];
            shibingEnit.ShootEntity = Entity.Null;
            ECB.SetComponent(ChunkIndex, entity, shibingEnit);
            NoShootEntity = true;
            return float3.zero;
        }


        Entity centerPoint = shibing[shibing[entity].ShootEntity].CenterPoint;//��õ��˵�����λ��
        if (jidi.TryGetComponent(shibing[entity].ShootEntity, out JiDi jd))//�������Ŀ���ǻ���
        {
            centerPoint = shibing[entity].TarEntity;
        }
        else if (!LocaltoWorld.TryGetComponent(centerPoint, out LocalToWorld ltw3))
        {
            centerPoint = shibing[entity].ShootEntity;
        }

        if(!transfrom.TryGetComponent(centerPoint,out LocalTransform trf1))
        {
            NoShootEntity = true;
            return float3.zero;
        }

        var enemypoint = LocaltoWorld[centerPoint].Position;

        float x = 0;
        float z = 0;
        foreach (float f in randoms)
        {
            if (x == 0)
                x = f;
            else if (z == 0)
                z = f;
        }
        enemypoint.x += x;
        enemypoint.z += z;
        if(Is_Ground)
            enemypoint.y = 0;

        return enemypoint;
    }
    //�Ƿ�Ϊ�վ�
    bool IS_AirForce(ShiBingName name)
    {
        bool b = false;
        if(name == ShiBingName.BaZhu || name == ShiBingName.BingFeng || name == ShiBingName.FengHuang ||
           name == ShiBingName.Monster_7 || name == ShiBingName.FengHuangRebirth)
        {
            b = true;
        }

        return b;
    }



    //����������
    void BaZhuMainGun(Entity entity, ref SX sx,in MyLayer mlayer,int ChunkIndex)
    {
        //����
        if (!sx.Is_UpSkill)
            return;

        var bazhuEnti = bazhu[entity];
        if (sx.Cur_ShootTime < bazhuEnti.MainGunPointOfTime && sx.Cur_ShootTime >= -0.04f)
        {
            InstanBullet(entity, bazhuEnti.MainGunBullet, bazhuEnti.FirePoint_1, ref sx, in mlayer, ChunkIndex);
            InstanParticle(entity, bazhuEnti.MainGunMuzzle, bazhuEnti.FirePoint_1, ChunkIndex);

            InstanBullet(entity, bazhuEnti.MainGunBullet, bazhuEnti.FirePoint_2, ref sx, in mlayer, ChunkIndex);
            InstanParticle(entity, bazhuEnti.MainGunMuzzle, bazhuEnti.FirePoint_2, ChunkIndex);
            
            float segmentation = 3;//�ֶ�
            float PointTime = sx.ShootTime / segmentation;
            bazhuEnti.MainGunPointOfTime = sx.ShootTime - (PointTime * bazhuEnti.MainGunShootNum);
            bazhuEnti.MainGunShootNum += 1;
            if (bazhuEnti.MainGunShootNum >= segmentation)
            {
                bazhuEnti.MainGunShootNum = 1;
                bazhuEnti.MainGunPointOfTime = 0;
            }
        }

        ECB.SetComponent(ChunkIndex, entity, bazhuEnti);
    }

}