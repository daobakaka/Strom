using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BarrageCommandSystem : SystemBase
{
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<BarrageCommand> m_BarrageCommand;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocalToWorld;
    ComponentLookup<SX> m_SX;


    void UpDataComponentLookup()
    {
        m_ShiBing.Update(this);
        m_BarrageCommand.Update(this);
        m_transform.Update(this);
        m_LocalToWorld.Update(this);
        m_SX.Update(this);
    }
    protected override void OnCreate()
    {
        m_ShiBing = GetComponentLookup<ShiBing>(true);
        m_BarrageCommand = GetComponentLookup<BarrageCommand>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocalToWorld = GetComponentLookup<LocalToWorld>(true);
        m_SX = GetComponentLookup<SX>(true);
    }
    protected override void OnUpdate()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        UpDataComponentLookup();


        string shootPlayer = "";
        commandType comd = commandType.NUll;
        InputCommand(ref shootPlayer, ref comd);


        //如果玩家有命令，就执行命令
        foreach (KeyValuePair<string, PlayerData> player in teamMager._Dic_Team1)
        {
            //每位玩家是什么命令就执行什么命令
            if (player.Value.m_comdType == commandType.NUll)
                continue;
            PlayerData pl = player.Value;
            if (player.Value.m_comdType == commandType.Attack)//命令是攻击玩家
                SelectAttackPlayer(in player.Value.m_shootTarget, ref pl);
            else if (player.Value.m_comdType == commandType.Jungle)//命令是打野
                SelectAttackMonster(ref pl);
        }
        foreach (KeyValuePair<string, PlayerData> player in teamMager._Dic_Team2)
        {
            //每位玩家是什么命令就执行什么命令
            if (player.Value.m_comdType == commandType.NUll)
                continue;
            PlayerData pl = player.Value;
            if (player.Value.m_comdType == commandType.Attack)//命令是攻击玩家
                SelectAttackPlayer(in player.Value.m_shootTarget, ref pl);
            else if (player.Value.m_comdType == commandType.Jungle)//命令是打野
                SelectAttackMonster(ref pl);
        }


    }

    //按键输入命令和攻击对象
    void InputCommand(ref string shootPlayer, ref commandType comd)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        if (Input.GetKey(KeyCode.J))
        {
            comd = commandType.Attack;
            ReleaseCommand(in comd);//发布命令
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (teamMager.SelectedPlayer.m_Team == layer.Team1)
                    shootPlayer = "OpenID_2_1";//"双面龟";
                else if (teamMager.SelectedPlayer.m_Team == layer.Team2)
                    shootPlayer = "OpenID_1_1";// "汤姆";
            }
            else if(Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (teamMager.SelectedPlayer.m_Team == layer.Team1)
                    shootPlayer = "OpenID_2_2";// "黑小虎";
                else if (teamMager.SelectedPlayer.m_Team == layer.Team2)
                    shootPlayer = "OpenID_1_2";// "沸洋洋";
            }
            if(!string.IsNullOrEmpty(shootPlayer))
            {
                teamMager.SelectedPlayer.m_shootTarget = shootPlayer;
                teamMager.SelectedPlayer.m_comdType = commandType.Attack;
            }
            else
            {
                //comd = commandType.NUll;
                //ReleaseCommand(in comd);//发布命令
            }
        }
        else if(Input.GetKey(KeyCode.K))
        {
            comd = commandType.Jungle;//打野
            teamMager.SelectedPlayer.m_comdType = comd;
            ReleaseCommand(in comd);//发布命令
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            comd = commandType.NUll;//取消攻击
            teamMager.SelectedPlayer.m_comdType = comd;
            ReleaseCommand(in comd);//发布命令
        }

    }

    //发布命令
    void ReleaseCommand(in commandType comd)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        //拿到选中玩家旗下的所有单位，并给他命令
        foreach (Entity enti in teamMager.SelectedPlayer.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                return;
            var bargeComd = m_BarrageCommand[enti];
            bargeComd.command = comd;
            EntityManager.SetComponentData(enti, bargeComd);
        }
    }

    //攻击玩家命令，选着离自己最近的攻击对象
    void SelectAttackPlayer(in string shootPlayer, ref PlayerData player)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        //拿到攻击的对象是谁
        PlayerData shootTar = null;
        if(player.m_Team == layer.Team1)
        {
            if (!teamMager._Dic_Team2.ContainsKey(shootPlayer))
                return;
            shootTar = teamMager._Dic_Team2[shootPlayer];
        }
        else if (player.m_Team == layer.Team2)
        {
            if (!teamMager._Dic_Team1.ContainsKey(shootPlayer))
                return;
            shootTar = teamMager._Dic_Team1[shootPlayer];
        }
        if (shootTar == null)
        {
            Debug.Log($" 没有{shootPlayer}这名玩家");
            player.m_comdType = commandType.NUll;
            return;
        }
            


        //选着离自己最近的目标
        foreach (Entity enti in player.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                continue;
            //指定敌人全灭后玩家麾下所有士兵命令归NUll
            if (shootTar.m_GiftShiBingList.Count <= 0)
            {
                var baracomd = m_BarrageCommand[enti];
                baracomd.Comd_ShootEntity = Entity.Null;
                baracomd.command = commandType.NUll;
                EntityManager.SetComponentData(enti, baracomd);

                EntityManager.SetComponentEnabled<Idle>(enti, false);
                EntityManager.SetComponentEnabled<Walk>(enti, true);
                EntityManager.SetComponentEnabled<Move>(enti, false);
                EntityManager.SetComponentEnabled<Fire>(enti, false);
                continue;
            }

            //如果这个士兵有攻击目标，那就计算和目标之间的距离，是否转换为攻击
            if (m_transform.TryGetComponent(m_BarrageCommand[enti].Comd_ShootEntity, out LocalTransform ltf))
            {
                CountTaDistance(enti);
                continue;
            }

            //如果这个士兵没有命令攻击目标或命令攻击目标死亡，就选一个
            Entity entiTar = Entity.Null;
            float distanMin = float.MaxValue;
            foreach (Entity shootenti in shootTar.m_GiftShiBingList)
            {
                if (!EntityManager.Exists(shootenti))
                    continue;
                if (entiTar == Entity.Null)
                    entiTar = shootenti;
                else
                {
                    var distanTemp = math.distance(m_LocalToWorld[enti].Position, m_LocalToWorld[shootenti].Position);
                    if(distanTemp < distanMin)
                    {
                        distanMin = distanTemp;
                        entiTar = shootenti;
                    }
                }
            }

            //拿到最近的指定敌人攻击目标
            var barcomd = m_BarrageCommand[enti];
            barcomd.Comd_ShootEntity = entiTar;
            EntityManager.SetComponentData(enti, barcomd);

            var shibing = m_ShiBing[enti];
            shibing.TarEntity = entiTar;
            shibing.ShootEntity = Entity.Null;
            EntityManager.SetComponentData(enti, shibing);
            EntityManager.SetComponentEnabled<Idle>(enti, false);
            EntityManager.SetComponentEnabled<Walk>(enti, false);
            EntityManager.SetComponentEnabled<Move>(enti, true);
            EntityManager.SetComponentEnabled<Fire>(enti, false);
        }
        //指定敌人全灭后玩家命令归NUll
        if (shootTar.m_GiftShiBingList.Count <= 0)
        {
            player.m_shootTarget = "";
            player.m_comdType = commandType.NUll;
        }


    }
    //攻击野怪命令，选择离自己最近的野怪
    void SelectAttackMonster(ref PlayerData player)
    {
        var teamMager = TeamManager.teamManager;
        var monsterMager = MonsterManager.instance;
        if (teamMager == null || monsterMager == null) return;

        //选着离自己最近的目标
        foreach (Entity enti in player.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                continue;

            //野怪全灭后玩家麾下所有士兵命令归NUll
            if (monsterMager.MonsterDic.Count <= 0)
            {
                var baracomd = m_BarrageCommand[enti];
                baracomd.Comd_ShootEntity = Entity.Null;
                baracomd.command = commandType.NUll;
                EntityManager.SetComponentData(enti, baracomd);

                EntityManager.SetComponentEnabled<Idle>(enti, false);
                EntityManager.SetComponentEnabled<Walk>(enti, true);
                EntityManager.SetComponentEnabled<Move>(enti, false);
                EntityManager.SetComponentEnabled<Fire>(enti, false);
                continue;
            }

            //如果这个士兵有攻击目标，那就计算和目标之间的距离，是否转换为攻击
            if (m_transform.TryGetComponent(m_BarrageCommand[enti].Comd_ShootEntity, out LocalTransform ltf))
            {
                CountTaDistance(enti);
                continue;
            }

            //如果这个士兵没有命令攻击目标或命令攻击目标死亡，就选一个
            Entity entiTar = Entity.Null;
            float distanMin = float.MaxValue;
            foreach (var pair in monsterMager.MonsterDic)
            {
                if (!EntityManager.Exists(pair.Key))
                    continue;
                if (entiTar == Entity.Null)
                    entiTar = pair.Key;
                else
                {
                    var distanTemp = math.distance(m_LocalToWorld[enti].Position, m_LocalToWorld[pair.Key].Position);
                    if (distanTemp < distanMin)
                    {
                        distanMin = distanTemp;
                        entiTar = pair.Key;
                    }
                }
            }

            //拿到最近的指定敌人攻击目标
            var barcomd = m_BarrageCommand[enti];
            barcomd.Comd_ShootEntity = entiTar;
            EntityManager.SetComponentData(enti, barcomd);

            var shibing = m_ShiBing[enti];
            shibing.TarEntity = entiTar;
            shibing.ShootEntity = Entity.Null;
            EntityManager.SetComponentData(enti, shibing);
            EntityManager.SetComponentEnabled<Idle>(enti, false);
            EntityManager.SetComponentEnabled<Walk>(enti, false);
            EntityManager.SetComponentEnabled<Move>(enti, true);
            EntityManager.SetComponentEnabled<Fire>(enti, false);
        }
        //指定敌人全灭后玩家命令归NUll
        if (monsterMager.MonsterDic.Count <= 0)
        {
            player.m_shootTarget = "";
            player.m_comdType = commandType.NUll;
        }

    }


    //计算和目标之间的距离，是否转换为攻击
    void CountTaDistance(Entity enti)
    {
        var Comd_shootEnti = m_BarrageCommand[enti].Comd_ShootEntity;
        if (!EntityManager.HasComponent<SX>(Comd_shootEnti))
            return;

        float dis = math.distance(m_LocalToWorld[enti].Position, m_LocalToWorld[Comd_shootEnti].Position);
        var sxx = m_SX[Comd_shootEnti];
        if (sxx.Is_AirForce)//如果为空军攻击距离探测距离就加44
            dis -= 44;
        else
            dis -= sxx.VolumetricDistance;
        //目标进入攻击范围里，转换为攻击
        if (dis <= m_SX[enti].Shootdis)
        {
            var shibing = m_ShiBing[enti];
            shibing.ShootEntity = Comd_shootEnti;
            EntityManager.SetComponentData(enti, shibing);
            EntityManager.SetComponentEnabled<Idle>(enti, false);
            EntityManager.SetComponentEnabled<Walk>(enti, false);
            EntityManager.SetComponentEnabled<Move>(enti, false);
            EntityManager.SetComponentEnabled<Fire>(enti, true);
        }

    }


}
