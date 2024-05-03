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


        //�������������ִ������
        foreach (KeyValuePair<string, PlayerData> player in teamMager._Dic_Team1)
        {
            //ÿλ�����ʲô�����ִ��ʲô����
            if (player.Value.m_comdType == commandType.NUll)
                continue;
            PlayerData pl = player.Value;
            if (player.Value.m_comdType == commandType.Attack)//�����ǹ������
                SelectAttackPlayer(in player.Value.m_shootTarget, ref pl);
            else if (player.Value.m_comdType == commandType.Jungle)//�����Ǵ�Ұ
                SelectAttackMonster(ref pl);
        }
        foreach (KeyValuePair<string, PlayerData> player in teamMager._Dic_Team2)
        {
            //ÿλ�����ʲô�����ִ��ʲô����
            if (player.Value.m_comdType == commandType.NUll)
                continue;
            PlayerData pl = player.Value;
            if (player.Value.m_comdType == commandType.Attack)//�����ǹ������
                SelectAttackPlayer(in player.Value.m_shootTarget, ref pl);
            else if (player.Value.m_comdType == commandType.Jungle)//�����Ǵ�Ұ
                SelectAttackMonster(ref pl);
        }


    }

    //������������͹�������
    void InputCommand(ref string shootPlayer, ref commandType comd)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        if (Input.GetKey(KeyCode.J))
        {
            comd = commandType.Attack;
            ReleaseCommand(in comd);//��������
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (teamMager.SelectedPlayer.m_Team == layer.Team1)
                    shootPlayer = "OpenID_2_1";//"˫���";
                else if (teamMager.SelectedPlayer.m_Team == layer.Team2)
                    shootPlayer = "OpenID_1_1";// "��ķ";
            }
            else if(Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (teamMager.SelectedPlayer.m_Team == layer.Team1)
                    shootPlayer = "OpenID_2_2";// "��С��";
                else if (teamMager.SelectedPlayer.m_Team == layer.Team2)
                    shootPlayer = "OpenID_1_2";// "������";
            }
            if(!string.IsNullOrEmpty(shootPlayer))
            {
                teamMager.SelectedPlayer.m_shootTarget = shootPlayer;
                teamMager.SelectedPlayer.m_comdType = commandType.Attack;
            }
            else
            {
                //comd = commandType.NUll;
                //ReleaseCommand(in comd);//��������
            }
        }
        else if(Input.GetKey(KeyCode.K))
        {
            comd = commandType.Jungle;//��Ұ
            teamMager.SelectedPlayer.m_comdType = comd;
            ReleaseCommand(in comd);//��������
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            comd = commandType.NUll;//ȡ������
            teamMager.SelectedPlayer.m_comdType = comd;
            ReleaseCommand(in comd);//��������
        }

    }

    //��������
    void ReleaseCommand(in commandType comd)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        //�õ�ѡ��������µ����е�λ������������
        foreach (Entity enti in teamMager.SelectedPlayer.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                return;
            var bargeComd = m_BarrageCommand[enti];
            bargeComd.command = comd;
            EntityManager.SetComponentData(enti, bargeComd);
        }
    }

    //����������ѡ�����Լ�����Ĺ�������
    void SelectAttackPlayer(in string shootPlayer, ref PlayerData player)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;

        //�õ������Ķ�����˭
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
            Debug.Log($" û��{shootPlayer}�������");
            player.m_comdType = commandType.NUll;
            return;
        }
            


        //ѡ�����Լ������Ŀ��
        foreach (Entity enti in player.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                continue;
            //ָ������ȫ��������������ʿ�������NUll
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

            //������ʿ���й���Ŀ�꣬�Ǿͼ����Ŀ��֮��ľ��룬�Ƿ�ת��Ϊ����
            if (m_transform.TryGetComponent(m_BarrageCommand[enti].Comd_ShootEntity, out LocalTransform ltf))
            {
                CountTaDistance(enti);
                continue;
            }

            //������ʿ��û�������Ŀ��������Ŀ����������ѡһ��
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

            //�õ������ָ�����˹���Ŀ��
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
        //ָ������ȫ�����������NUll
        if (shootTar.m_GiftShiBingList.Count <= 0)
        {
            player.m_shootTarget = "";
            player.m_comdType = commandType.NUll;
        }


    }
    //����Ұ�����ѡ�����Լ������Ұ��
    void SelectAttackMonster(ref PlayerData player)
    {
        var teamMager = TeamManager.teamManager;
        var monsterMager = MonsterManager.instance;
        if (teamMager == null || monsterMager == null) return;

        //ѡ�����Լ������Ŀ��
        foreach (Entity enti in player.m_GiftShiBingList)
        {
            if (!EntityManager.Exists(enti))
                continue;

            //Ұ��ȫ��������������ʿ�������NUll
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

            //������ʿ���й���Ŀ�꣬�Ǿͼ����Ŀ��֮��ľ��룬�Ƿ�ת��Ϊ����
            if (m_transform.TryGetComponent(m_BarrageCommand[enti].Comd_ShootEntity, out LocalTransform ltf))
            {
                CountTaDistance(enti);
                continue;
            }

            //������ʿ��û�������Ŀ��������Ŀ����������ѡһ��
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

            //�õ������ָ�����˹���Ŀ��
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
        //ָ������ȫ�����������NUll
        if (monsterMager.MonsterDic.Count <= 0)
        {
            player.m_shootTarget = "";
            player.m_comdType = commandType.NUll;
        }

    }


    //�����Ŀ��֮��ľ��룬�Ƿ�ת��Ϊ����
    void CountTaDistance(Entity enti)
    {
        var Comd_shootEnti = m_BarrageCommand[enti].Comd_ShootEntity;
        if (!EntityManager.HasComponent<SX>(Comd_shootEnti))
            return;

        float dis = math.distance(m_LocalToWorld[enti].Position, m_LocalToWorld[Comd_shootEnti].Position);
        var sxx = m_SX[Comd_shootEnti];
        if (sxx.Is_AirForce)//���Ϊ�վ���������̽�����ͼ�44
            dis -= 44;
        else
            dis -= sxx.VolumetricDistance;
        //Ŀ����빥����Χ�ת��Ϊ����
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
