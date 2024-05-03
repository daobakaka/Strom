using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using UnityEditor;
using Unity.Transforms;
[BurstCompile]
[UpdateAfter(typeof(DirectShootSystem))]//在个System之后执行
[UpdateAfter(typeof(bulletSystem))]//在个System之后执行
[UpdateAfter(typeof(AttackBoxSystem))]//在个System之后执行
public partial class UpSkillSystem : SystemBase
{
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<Bullet> m_Bullet;
    ComponentLookup<EntityCtrl> m_EntityCtrl;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<SX> m_SX;
    ComponentLookup<PutUpShiBing> m_PutUpShiBing;
    BufferLookup<Buff> m_Buffer;

    void UpDateComponentLookup()
    {
        m_ShiBing.Update(this);
        m_MyLayer.Update(this);
        m_Bullet.Update(this);
        m_EntityCtrl.Update(this);
        m_transform.Update(this);
        m_SX.Update(this);
        m_PutUpShiBing.Update(this);
        m_Buffer.Update(this);
    }

    protected override void OnCreate()
    {
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_Bullet = GetComponentLookup<Bullet>(true);
        m_EntityCtrl = GetComponentLookup<EntityCtrl>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_SX = GetComponentLookup<SX>(true);
        m_PutUpShiBing = GetComponentLookup<PutUpShiBing>(true);
        m_Buffer = GetBufferLookup<Buff>(true);
    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var upskillJob = new UpSkillJob
        {
            ECB = ecb.AsParallelWriter(),
            spawn = spawn,
            shibing = m_ShiBing,
            myLayer = m_MyLayer,
            bullet = m_Bullet,
            entiCtrl = m_EntityCtrl,
            sx = m_SX,
            putUpShiBing = m_PutUpShiBing,
            buffer = m_Buffer,
        };
        Dependency = upskillJob.ScheduleParallel(Dependency);
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();


        //计算共同抗伤害----------------------------------
        float Team1TotalDamage = 0;//队伍1伤害总和
        float Team2TotalDamage = 0;//队伍2伤害总和
        float Team1ShibingNum = 0;//队伍1士兵人数
        float Team2ShibingNum = 0;//队伍2士兵人数
        Entities.ForEach((Entity entity, MyLayer mylayer, ShiBing shibing, ref UpSkill upskill, ref SX sx) =>
        {
            if (upskill.upSkill_Name != UpSkillName.GangQiu || !upskill.Is_UpSkill)
                return;

            if (mylayer.BelongsTo == layer.Team1)
                Team1ShibingNum += 1;
            else if (mylayer.BelongsTo == layer.Team2)
                Team2ShibingNum += 1;

            if (upskill.InjuryRecord <= 0)
                return;

            if (mylayer.BelongsTo == layer.Team1)
                Team1TotalDamage += upskill.InjuryRecord;
            else if (mylayer.BelongsTo == layer.Team2)
                Team2TotalDamage += upskill.InjuryRecord;

            sx.Cur_HP += upskill.InjuryRecord;
            upskill.InjuryRecord = 0;
        }).WithoutBurst().WithStructuralChanges().Run();
        float Team1EveryonesHurt = 0;
        float Team2EveryonesHurt = 0;
        if (Team1ShibingNum > 0)
            Team1EveryonesHurt = Team1TotalDamage / Team1ShibingNum;
        else if(Team2ShibingNum > 0)
            Team2EveryonesHurt = Team2TotalDamage / Team2ShibingNum;

        if (Team1EveryonesHurt > 0 || Team2EveryonesHurt > 0)
        {
            Entities.ForEach((Entity entity, MyLayer mylayer ,ShiBing shibing, ref UpSkill upskill, ref SX sx) =>
            {
                if (upskill.upSkill_Name != UpSkillName.GangQiu || !upskill.Is_UpSkill)
                    return;
                if(mylayer.BelongsTo == layer.Team1)
                    sx.DP -= Team1EveryonesHurt;
                else if (mylayer.BelongsTo == layer.Team2)
                    sx.DP -= Team2EveryonesHurt;

                if (sx.DP < 0)
                {
                    sx.Cur_HP += sx.DP;
                    sx.DP = 0;
                }

                //实例化特效
                var damageflat = EntityManager.Instantiate(spawn.DamageFlatteningEffect);
                UpDateComponentLookup();
                EntityManager.SetComponentData(damageflat, new LocalTransform
                {
                    Position = m_transform[entity].Position,
                    Rotation = m_transform[entity].Rotation,
                    Scale = m_transform[damageflat].Scale,
                });
            }).WithoutBurst().WithStructuralChanges().Run();
        }
        //--------------------------------



    }
}
[BurstCompile]
public partial struct UpSkillJob : IJobEntity
{
    [ReadOnly] public Spawn spawn;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<ShiBing> shibing;
    [ReadOnly] public ComponentLookup<MyLayer> myLayer;
    [ReadOnly] public ComponentLookup<Bullet> bullet;
    [ReadOnly] public ComponentLookup<EntityCtrl> entiCtrl;
    [ReadOnly] public ComponentLookup<SX> sx;
    [ReadOnly] public ComponentLookup<PutUpShiBing> putUpShiBing;
    [ReadOnly] public BufferLookup<Buff> buffer;
    void Execute(Entity entity, UpSkill upskill, [ChunkIndexInQuery]int ChunkIndex)
    {
        //属性里的是否升级为true也能升级
        if(sx.TryGetComponent(entity,out SX s))
        {
            if (sx[entity].Is_UpSkill)
                upskill.Is_UpSkill = true;
        }
        if (!upskill.Is_UpSkill)
            return;

        switch (upskill.upSkill_Name)
        {
            case UpSkillName.PaChong:     UpSkillPaChong(entity, ChunkIndex); break;
            case UpSkillName.YeMa:        UpSkillYeMa(entity, ChunkIndex); break;
            case UpSkillName.BaoYuBullet: UpSkillBaoYuBullet(entity, ChunkIndex); break;
            case UpSkillName.BaZhu:       UpSkillBaZhu(entity, ChunkIndex); break;
            case UpSkillName.WarFactory:  UpSkillWarFactory(entity, ChunkIndex); break;
            case UpSkillName.HuGuang:     UpSkillHuGuang(entity, ChunkIndex); break;
            case UpSkillName.FengHuang:   UpSkillFengHuang(entity, ChunkIndex); break;

        }

    }
    //爬虫升级
    void UpSkillPaChong(Entity entity, int ChunkIndex)
    {
        var shibingComp = shibing[entity];
        shibingComp.DeadLanguage = spawn.LiuSuan_AttackBox;
        ECB.SetComponent(ChunkIndex, entity, shibingComp);
        var shibingSX = sx[entity];
        shibingSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, shibingSX);
        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //野马升级
    void UpSkillYeMa(Entity entity, int ChunkIndex)
    {
        var yemaLayer = myLayer[entity];
        if (yemaLayer.BelongsTo == layer.Team1)
            yemaLayer.BulletCollidesWith = layer.Team2Bullet;
        else if (yemaLayer.BelongsTo == layer.Team2)
            yemaLayer.BulletCollidesWith = layer.Team1Bullet;
        ECB.SetComponent(ChunkIndex, entity, yemaLayer);
        var shibingSX = sx[entity];
        shibingSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, shibingSX);
        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //暴雨的子弹升级
    void UpSkillBaoYuBullet(Entity entity,int ChunkIndex)
    {
        var BaoYuBullet = bullet[entity];
        BaoYuBullet.DeadLanguage2 = spawn.DiHuo_AttackBox;
        ECB.SetComponent(ChunkIndex, entity, BaoYuBullet);
        var shibingSX = sx[entity];
        shibingSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, shibingSX);
        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //霸主升级
    void UpSkillBaZhu(Entity entity, int ChunkIndex)
    {
        Entity specify = Entity.Null;
        if (myLayer[entity].BelongsTo == layer.Team1)
            specify = spawn.Team1_BingFeng;
        else if(myLayer[entity].BelongsTo == layer.Team2)
            specify = spawn.Team2_BingFeng;
        ECB.AddComponent(ChunkIndex, entity, new PutUpShiBing
        {
            GroundSpawnPoint = shibing[entity].Foot_R,
            AirSpawnPoint = shibing[entity].Foot_L,
            SpecifyShiBing = specify,
            PutUpTime = 10f,
            Cur_PutUpTime = 10f,
            PutUpNum = 4,
        });
        bool addHPbuff = true;
        bool recoverHPbuff = true;
        foreach (Buff b in buffer[entity])//如果有这个buff就退出不添加
        {
            if (b.buffType == BuffType.AddHPbuff)
                addHPbuff = false;
            else if (b.buffType == BuffType.RecoverHPbuff)
                recoverHPbuff = false;
        }
        if(addHPbuff)
        {
            var buff = new Buff//增加血量buff
            {
                buffType = BuffType.AddHPbuff,
                buffAct = BuffAct.Init,
                BuffProportion = 0.3f,
                BuffTime = 1f,
                Is_deBuff = false,
                MyAttacker = Entity.Null,
            };
            ECB.AppendToBuffer(ChunkIndex, entity, buff);
        }
        if(recoverHPbuff)
        {
            var buff = new Buff//永久回血buff
            {
                buffType = BuffType.RecoverHPbuff,
                buffAct = BuffAct.Init,
                BuffProportion = 0.045f,
                BuffTime = 1f,
                Is_deBuff = false,
                MyAttacker = Entity.Null,
                IntervalTime = 1f,
                Cur_IntervalTime = 1f,
            };
            ECB.AppendToBuffer(ChunkIndex, entity, buff);
        }

        var bazhuSX = sx[entity];
        bazhuSX.Fire_TakeTurnsIntNum = 12;
        bazhuSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, bazhuSX);



        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //战争工厂升级
    void UpSkillWarFactory(Entity entity, int ChunkIndex)
    {
        var layer = myLayer[shibing[entity].Foot_R];
        SetLayer(myLayer[entity], ref layer);
        ECB.SetComponent(ChunkIndex, shibing[entity].Foot_R, layer);
        var layer2 = myLayer[shibing[entity].Foot_L];
        SetLayer(myLayer[entity], ref layer2);
        ECB.SetComponent(ChunkIndex, shibing[entity].Foot_L, layer2);

        var putUp = putUpShiBing[entity];
        putUp.PutUpTime -= 3;
        ECB.SetComponent(ChunkIndex, entity, putUp);

        var shibingSX = sx[entity];
        shibingSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, shibingSX);

        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //弧光升级
    void UpSkillHuGuang(Entity entity, int ChunkIndex)
    {
        var entiShibing = shibing[entity];
        entiShibing.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, entiShibing);
        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }
    //凤凰升级
    void UpSkillFengHuang(Entity entity, int ChunkIndex)
    {
        var shibingComp = shibing[entity];
        if (myLayer[entity].BelongsTo == layer.Team1)
            shibingComp.DeadLanguage = spawn.FengHuangRebirth_1;
        else if (myLayer[entity].BelongsTo == layer.Team2)
            shibingComp.DeadLanguage = spawn.FengHuangRebirth_2;
        ECB.SetComponent(ChunkIndex, entity, shibingComp);
        var shibingSX = sx[entity];
        shibingSX.Is_UpSkill = true;
        ECB.SetComponent(ChunkIndex, entity, shibingSX);
        ECB.RemoveComponent<UpSkill>(ChunkIndex, entity);
    }



    //设置寄生士兵的图层
    void SetLayer(in MyLayer mylayer,ref MyLayer shibinglayer)
    {
        shibinglayer.BelongsTo = layer.Parasitic;
        if (mylayer.BelongsTo == layer.Team1)
        {
            shibinglayer.CollidesWith_1 = layer.Team2;
            shibinglayer.ParasiticBelongsTo = layer.Team1;
        }
        else if(mylayer.BelongsTo == layer.Team2)
        {
            shibinglayer.CollidesWith_1 = layer.Team1;
            shibinglayer.ParasiticBelongsTo = layer.Team2;
        }
        shibinglayer.CollidesWith_2 = layer.Neutral;
    }

}