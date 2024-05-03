using System.Collections;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using System;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.VisualScripting;
[BurstCompile]
public partial class SceneBombSystem : SystemBase//音量达到后的场景炸弹
{
    ComponentLookup<LocalToWorld> m_LocaltoWorld;
    ComponentLookup<LocalTransform> m_transform;
    void UpDateComponentLookup()
    {
        m_LocaltoWorld.Update(this);
        m_transform.Update(this);
    }

    protected override void OnCreate()
    {
        m_LocaltoWorld = GetComponentLookup<LocalToWorld>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// 检查是否存在 Spawn 类型的实体
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//获取Spawn单例

        //键盘按键直接出音浪武器
        TeamSceneBombKeys(spawn);
        //玩家音浪值达到了后释放音浪技能
        PlayerVoiceWaveBySkill(spawn);
        //各个音浪武器Init时间到了后，的主要执行逻辑
        RunSceneBomb(spawn);




        //var ecb = new EntityCommandBuffer(Allocator.TempJob);
        //var sceneBombJob = new SceneBombJob
        //{
        //    ECB = ecb.AsParallelWriter(),
        //};
        //Dependency = sceneBombJob.ScheduleParallel(Dependency);
        //ecb.Playback(EntityManager);
        //ecb.Dispose();
    }

    void Init_SceneBomb_1(Spawn spawn, layer TeamID)//原子弹准备
    {
        var comdSkill_nucleaBomb = spawn.comdSkill_NucleaBomb;
        if (!EntityManager.Exists(comdSkill_nucleaBomb) && 
            !EntityManager.HasComponent<CommanderSkill>(comdSkill_nucleaBomb))
            return;
        int rangeX = UnityEngine.Random.Range(-7, 7);//int rangeX = UnityEngine.Random.Range(-60, 60);
        int rangeZ = UnityEngine.Random.Range(-9, 11);//int rangeZ = UnityEngine.Random.Range(-80, 100);
        var Instan = EntityManager.Instantiate(comdSkill_nucleaBomb);
        var ComderSkill = EntityManager.GetComponentData<CommanderSkill>(Instan);
        ComderSkill.TeamID = TeamID;
        EntityManager.SetComponentData(Instan, ComderSkill);

        var InstanTransf = EntityManager.GetComponentData<LocalTransform>(Instan);
        var firePointPos = m_LocaltoWorld[spawn.SceneBombFirePoint].Position;
        InstanTransf.Position = new float3(firePointPos.x + rangeX, 1f, firePointPos.z + rangeZ);
        EntityManager.SetComponentData(Instan, InstanTransf);

        //InstanBullet(spawn.SceneBomb_NucleaBomb, spawn.SceneBombFirePoint,TeamID , rangeX, rangeZ);
    }
    void SceneBomb_1(Entity entity, Spawn spawn, CommanderSkill comdSkill)//原子弹
    {
        UpDateComponentLookup();
        var firePoint = m_LocaltoWorld[entity].Position;
        firePoint.y = m_LocaltoWorld[spawn.SceneBombFirePoint].Position.y;

        InstanBullet(spawn.SceneBomb_NucleaBomb, entity, comdSkill.TeamID,spawn);
        EntityManager.DestroyEntity(entity);
    }
    void Init_SceneBomb_2(Spawn spawn, layer TeamID)//导弹空袭准备
    {
        var comdSkill_missile = spawn.comdSkill_Missile;
        if (!EntityManager.Exists(comdSkill_missile) &&
            !EntityManager.HasComponent<CommanderSkill>(comdSkill_missile))
            return;

        var Instan = EntityManager.Instantiate(comdSkill_missile);
        var ComderSkill = EntityManager.GetComponentData<CommanderSkill>(Instan);
        ComderSkill.TeamID = TeamID;
        EntityManager.SetComponentData(Instan, ComderSkill);

    }
    void SceneBomb_2(Entity entity, Spawn spawn,ref CommanderSkill comdSkill)//导弹空袭
    {
        UpDateComponentLookup();
        var missile = spawn.SceneBomb_Missile;

        if (!EntityManager.Exists(missile))
            return;

        comdSkill.Cur_IntervalTime -= SystemAPI.Time.DeltaTime;
        if (comdSkill.Cur_IntervalTime > 0)
            return;

        comdSkill.Cur_IntervalTime = comdSkill.IntervalTime;

        var firepointTransf = m_LocaltoWorld[spawn.SceneBombFirePoint];
        int rangeX = UnityEngine.Random.Range(-67, 67);//int rangeX = UnityEngine.Random.Range(-590, 590);
        int rangeZ = UnityEngine.Random.Range(-28, 28);//int rangeZ = UnityEngine.Random.Range(-250, 250);

        EntityManager.AddComponentData(entity, new LocalTransform
        {
            Position = new float3(firepointTransf.Position.x + rangeX,firepointTransf.Position.y,firepointTransf.Position.z + rangeZ),
            Rotation = firepointTransf.Rotation,
            Scale = 1,
        });
        InstanBullet(missile, entity, comdSkill.TeamID, spawn);
        comdSkill.MissileNum -= 1;
        if (comdSkill.MissileNum <= 0)
            EntityManager.DestroyEntity(entity);

    }
    void Init_SceneBomb_3(Spawn spawn, layer TeamID, int rangeX)//激光准备
    {
        var comdSkill_Laser = spawn.comdSkill_Laser;
        if (!EntityManager.Exists(comdSkill_Laser) &&
            !EntityManager.HasComponent<CommanderSkill>(comdSkill_Laser))
            return;

        //int rangeX = UnityEngine.Random.Range(-10, 10);//int rangeX = UnityEngine.Random.Range(-60, 60);
        int rangeZ = UnityEngine.Random.Range(-10, 10);//int rangeZ = UnityEngine.Random.Range(-80, 100);
        var Instan = EntityManager.Instantiate(comdSkill_Laser);

        var InstanTransf = EntityManager.GetComponentData<LocalTransform>(Instan);
        //var firePointPos = m_LocaltoWorld[spawn.SceneBombFirePoint].Position;
        //InstanTransf.Position = new float3(firePointPos.x + rangeX, firePointPos.y, firePointPos.z + rangeZ);
        Entity jidi = Entity.Null;
        switch (TeamID)
        {
            case layer.Team1:jidi = spawn.JiDi_Team1;break;
            case layer.Team2:jidi = spawn.JiDi_Team2;break;
        }
        if (jidi == null) return;
        var firePointPos = m_LocaltoWorld[jidi].Position;
        InstanTransf.Position = new float3(firePointPos.x + rangeX, firePointPos.y, firePointPos.z + rangeZ);
        EntityManager.SetComponentData(Instan, InstanTransf);

        var ComderSkill = EntityManager.GetComponentData<CommanderSkill>(Instan);
        ComderSkill.TeamID = TeamID;
        ComderSkill.Laser = InstanAttackBox(spawn.SceneBomb_Laser, InstanTransf.Position,TeamID);//拿到实例化的激光
        EntityManager.SetComponentData(Instan, ComderSkill);

        

    }
    void SceneBomb_3(Entity entity,ref CommanderSkill comdSkill)//激光
    {
        UpDateComponentLookup();
        if (!EntityManager.Exists(comdSkill.Laser) || !EntityManager.HasComponent<LocalTransform>(comdSkill.Laser))
            return;
        if (comdSkill.Cur_SustainTime <= 0)
        {
            EntityManager.DestroyEntity(comdSkill.Laser);
            EntityManager.DestroyEntity(entity);
            return;
        }

        var laserTransfrom = m_transform[comdSkill.Laser];
        if(laserTransfrom.Position.y > 1)
        {
            laserTransfrom.Position.y -= comdSkill.Speed * 10 * SystemAPI.Time.DeltaTime;
        }
        else
        {
            comdSkill.Cur_SustainTime -= SystemAPI.Time.DeltaTime;
            float3 dir = float3.zero;
            if (comdSkill.TeamID == layer.Team1)
                dir = new float3(0, 0, -1);
            else if (comdSkill.TeamID == layer.Team2)
                dir = new float3(0, 0, 1);
            laserTransfrom.Position += dir * comdSkill.Speed * SystemAPI.Time.DeltaTime;
        }
        EntityManager.SetComponentData(comdSkill.Laser, laserTransfrom);
    }
    void ScencSkill_Shile(Spawn spawn, PlayerData player)//给基地护盾
    {
        var sceneBombMager = SceneBombManager.instance;
        Entity jidi = Entity.Null;
        if (player.m_Team == layer.Team1)
            jidi = spawn.JiDi_Team1;
        else if (player.m_Team == layer.Team2)
            jidi = spawn.JiDi_Team2;
        if (jidi == Entity.Null || sceneBombMager == null || 
            !EntityManager.HasComponent<JiDi>(jidi)) return;
        var jidiCompt = EntityManager.GetComponentData<JiDi>(jidi);
        var ShieldPoint = jidiCompt.shieldPoint;
        //如果基地护盾没有了就给这个基地添加护盾
        if (!EntityManager.Exists(jidiCompt.JidiShield))
        {
            jidiCompt.JidiShield = Entity.Null;
            EntityManager.AddComponentData(jidi, new AddShield
            {
                ShieldHP = sceneBombMager.ShieldValue,
                ShieldScale = 40f,
                ShieldParent = ShieldPoint,
                ShieldExpandSpeed = 5,
            });
        }
        else//如果这个基地有护盾就增加护盾值
        {
            if (!EntityManager.HasComponent<SX>(jidiCompt.JidiShield))
                return;
            var jidiShieldSX = EntityManager.GetComponentData<SX>(jidiCompt.JidiShield);
            jidiShieldSX.HP += sceneBombMager.ShieldValue;
            jidiShieldSX.Cur_HP += sceneBombMager.ShieldValue;
            EntityManager.SetComponentData(jidiCompt.JidiShield, jidiShieldSX);
        }

    }


    void InstanBullet(Entity bullet, Entity firepoint, layer TeamID, Spawn spawn)//实例化子弹
    {
        UpDateComponentLookup();
        var BUllet = EntityManager.Instantiate(bullet);
        var bulletTransf = EntityManager.GetComponentData<LocalTransform>(BUllet);
        bulletTransf.Position = m_LocaltoWorld[firepoint].Position;
        bulletTransf.Position.y = m_LocaltoWorld[spawn.SceneBombFirePoint].Position.y;
        bulletTransf.Rotation = m_LocaltoWorld[spawn.SceneBombFirePoint].Rotation;
        EntityManager.SetComponentData(BUllet, bulletTransf);

        EntityManager.AddComponentData(BUllet, new BulletChange
        {
            Dir = new float3(0, -1, 0),
            AT = 0,
        });

        var layerr = EntityManager.GetComponentData<MyLayer>(BUllet);
        if (TeamID == layer.Team1)
        {
            layerr.BelongsTo = layer.Team1;
            layerr.CollidesWith_1 = layer.Team2;
        }
        else if(TeamID == layer.Team2)
        {
            layerr.BelongsTo = layer.Team2;
            layerr.CollidesWith_1 = layer.Team1;
        }
        EntityManager.SetComponentData(BUllet, layerr);

    }
    Entity InstanAttackBox(Entity Box, float3 firepoint,layer TeamID)//实例化攻击盒子
    {

        if (!EntityManager.Exists(Box))
            return Entity.Null;
        var instanBox = EntityManager.Instantiate(Box);
        var BoxTransform = m_transform[instanBox];
        BoxTransform.Position = firepoint;
        EntityManager.SetComponentData(instanBox, BoxTransform);

        var layerr = EntityManager.GetComponentData<MyLayer>(instanBox);
        if (TeamID == layer.Team1)
        {
            layerr.BelongsTo = layer.Team1;
            layerr.CollidesWith_1 = layer.Team2;
        }
        else if (TeamID == layer.Team2)
        {
            layerr.BelongsTo = layer.Team2;
            layerr.CollidesWith_1 = layer.Team1;
        }
        EntityManager.SetComponentData(instanBox, layerr);
        return instanBox;
    }

    //键盘按键直接出音浪武器
    void TeamSceneBombKeys(Spawn spawn)
    {
        int rangeX = UnityEngine.Random.Range(-10, 10);
        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.B))//原子弹
            Init_SceneBomb_1(spawn, layer.Team1);
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.B))
            Init_SceneBomb_1(spawn, layer.Team2);
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.N))//导弹空袭
            Init_SceneBomb_2(spawn, layer.Team1);
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.N))//导弹空袭
            Init_SceneBomb_2(spawn, layer.Team2);
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < 3; ++i)
            {
                Init_SceneBomb_3(spawn, layer.Team1, rangeX);
                rangeX += 10;
            }
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < 3; ++i)
            {
                Init_SceneBomb_3(spawn, layer.Team2, rangeX);
                rangeX += 10;
            }
        }
    }
    //玩家音浪值达到了后释放音浪技能
    void PlayerVoiceWaveBySkill(Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        var sceneBombMager = SceneBombManager.instance;
        if (teamMager == null || sceneBombMager == null) return;

        var playerList = teamMager.GetAllPlayer();//拿到全部玩家
        if (playerList.Count <= 0) return;

        foreach(var player in playerList)
        {
            var voiceWave = player.m_TotalVoiceWave;
            if (voiceWave < sceneBombMager.VoiceWave_Shield)
                continue;

            if(voiceWave >= sceneBombMager.VoiceWave_Shield && player.m_sceneBombType == SceneBombType.Shile)
            {
                //通知那名玩家发布了音浪技能
                sceneBombMager.AddVoicWaveNotice(in player);

                ScencSkill_Shile(spawn, player);//给基地加护盾
                player.m_sceneBombType = SceneBombType.MissileAirStrike;
            }
            else if(voiceWave >= sceneBombMager.VoiceWave_Missile && player.m_sceneBombType == SceneBombType.MissileAirStrike)
            {
                sceneBombMager.AddVoicWaveNotice(in player);
                Init_SceneBomb_2(spawn, player.m_Team);//导弹空袭
                player.m_sceneBombType = SceneBombType.Laser;
            }
            else if (voiceWave >= sceneBombMager.VoiceWave_Laser && player.m_sceneBombType == SceneBombType.Laser)
            {
                sceneBombMager.AddVoicWaveNotice(in player);
                int rangeX = UnityEngine.Random.Range(-10, 10);
                for (int i = 0; i < 3; ++i)
                {
                    Init_SceneBomb_3(spawn, player.m_Team, rangeX);//激光
                    rangeX += 10;
                }


                player.m_sceneBombType = SceneBombType.NucleaBomb;
            }
            else if (voiceWave >= sceneBombMager.VoiceWave_NucleaBomb && player.m_sceneBombType == SceneBombType.NucleaBomb)
            {
                sceneBombMager.AddVoicWaveNotice(in player);
                Init_SceneBomb_1(spawn, player.m_Team);//原子弹
                player.m_sceneBombType = SceneBombType.Shile;
                player.m_TotalVoiceWave -= sceneBombMager.VoiceWave_NucleaBomb;//如果玩家一次性送了很多音浪，多余最大值了音浪保留
            }
        }

    }
    //各个音浪武器Init时间到了后的主要执行逻辑
    void RunSceneBomb(Spawn spawn)
    {
        Entities.ForEach((Entity entity, ref CommanderSkill comdSkill) =>
        {
            comdSkill.Cur_InitTime -= SystemAPI.Time.DeltaTime;
            if (comdSkill.Cur_InitTime > 0)
                return;

            switch (comdSkill.SceneBombtype)
            {
                case SceneBombType.NucleaBomb: SceneBomb_1(entity, spawn, comdSkill); break;
                case SceneBombType.MissileAirStrike: SceneBomb_2(entity, spawn, ref comdSkill); break;
                case SceneBombType.Laser: SceneBomb_3(entity, ref comdSkill); break;
            }

        }).WithoutBurst().WithStructuralChanges().Run();
    }

}


//[BurstCompile]
//public partial struct SceneBombJob : IJobEntity
//{
//    public EntityCommandBuffer.ParallelWriter ECB;
//    void Execute(Entity entity)
//    {
//    }
//}
