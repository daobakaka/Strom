using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class JiDiSystem : SystemBase
{
    ComponentLookup<JiDi> m_jidi;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocalToWorld;
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<EntityCtrl> m_EntiCtrl;
    ComponentLookup<BarrageCommand> m_BarrageCommand;
    ComponentLookup<SX> m_SX;
    //ComponentLookup<PlayerDataComponent> m_PlayerDataComponent;
    float AutoGiftTime = 2f;
    void UpDataComponentLookup()
    {
        m_jidi.Update(this);
        m_transform.Update(this);
        m_LocalToWorld.Update(this);
        m_shibing.Update(this);
        m_EntiCtrl.Update(this);
        m_BarrageCommand.Update(this);
        m_SX.Update(this);
        //m_PlayerDataComponent.Update(this);
    }

    protected override void OnCreate()
    {
        m_jidi = GetComponentLookup<JiDi>(true);
        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocalToWorld = GetComponentLookup<LocalToWorld>(true);
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_EntiCtrl = GetComponentLookup<EntityCtrl>(true);
        m_BarrageCommand = GetComponentLookup<BarrageCommand>(true);
        m_SX = GetComponentLookup<SX>(true);
        //m_PlayerDataComponent = GetComponentLookup<PlayerDataComponent>(true);

    }

    protected override void OnUpdate()
    {
        UpDataComponentLookup();

        Spawn spawn;
        if (!SystemAPI.HasSingleton<Spawn>())// ����Ƿ���� Spawn ���͵�ʵ��
            return;
        else
            spawn = SystemAPI.GetSingleton<Spawn>();//��ȡSpawn����

        var TemaManger = TeamManager.teamManager;
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;

        //var myCustomSystemhandle = World.DefaultGameObjectInjectionWorld.GetExistingSystem<ShiBingSystem>();
        //myCustomSystemhandle.


        InitGetJidiEntity();//��TeamManager��û��ص�Entity
        GetJiDiEnti();//��û���HP
        //û�л��ز���ʵ����
        if (!EntityManager.Exists(spawn.JiDi_Team1) ||
            !EntityManager.Exists(spawn.JiDi_Team2))
            return;

        //����ʿ��ʵ����
        InstanLikeShiBing(ref TemaManger, in spawn, layer.Team1);
        InstanLikeShiBing(ref TemaManger, in spawn, layer.Team2);

        //ʿ������
        //ShiBingInput_1(in spawn, ref TemaManger);
        if(!gameRoot.GiftOrShiBing) ShiBingInput_2(in spawn);
        //��Ұ���
        PlayerInput(in spawn, ref TemaManger);
        //���ﰴ��
        MonsterInput(in spawn);
        //���ޱ�����
        InstanceList(ref TemaManger, in spawn);
        //����ѡ���ʿ��
        InstanCardShiBing(in spawn);
        //�����ﰴ��
        if (gameRoot.GiftOrShiBing) GiftShiBingInput(in spawn);
        //����˳����������������������
        UpdataGiftQueueList(in spawn);
        //��������ȥ�������˵ģ����߲��Ϸ��Ĺ���
        RemoveEntityMonster();
        //���Լ���ӵ�������ҵ�ʿ������
        PlayerEntiIDByPlayerShibingList();
        //���޴ﵽ������ʵ������Ϸ�ѶȵĹ���
        Like_Monster(in spawn);


        //�Զ�����-------------------------------------
        AutoGivingGifts(ref TemaManger);
        //---------------------------------------------

    }

    //�Զ�����----------------------------
    void InitAutoGift(ref TeamManager teamMager)//��ʼ���Զ�����
    {
        if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad1)))
            teamMager.SelectedGift.m_GiftName = "��Ů��";
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad2)))
            teamMager.SelectedGift.m_GiftName = "����Ȧ";
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad3)))
            teamMager.SelectedGift.m_GiftName = "�������";
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad4)))
            teamMager.SelectedGift.m_GiftName = "��ħը��";
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad5)))
            teamMager.SelectedGift.m_GiftName = "���ؿ�Ͷ";
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.Keypad6)))
            teamMager.SelectedGift.m_GiftName = "��������";
    }
    void AutoGift(ref TeamManager teamMager, string OpenID)//�Զ����������
    {
        GiftManager giftMager = GiftManager.Instance;
        if (giftMager == null) return;
        TKMessageData TKdata = new TKMessageData();
        TKdata.OpenID = OpenID;
        if (string.IsNullOrWhiteSpace(teamMager.SelectedGift.m_GiftName))
            return;
        TKdata.GiftType = teamMager.SelectedGift.m_GiftName;
        TKdata.GiftNum = teamMager.SelectedGiftNum.ToString();
        if (!string.IsNullOrWhiteSpace(TKdata.OpenID))
            giftMager.TKTOGiftType(in TKdata);
    }
    void AutoGivingGifts(ref TeamManager TemaManger)
    {
        InitAutoGift(ref TemaManger);
        if (Input.GetKeyDown(KeyCode.F11))
            TemaManger.Is_SelectedAutoGift = TemaManger.Is_SelectedAutoGift == false ? true : false;
        if (Input.GetKeyDown(KeyCode.Equals))
            TemaManger.SelectedGiftNum += 1;
        else if (Input.GetKeyDown(KeyCode.Minus))
            TemaManger.SelectedGiftNum -= 1;
        if (TemaManger.Is_SelectedAutoGift)
        {
            AutoGiftTime -= SystemAPI.Time.DeltaTime;
            {
                if (AutoGiftTime <= 0)
                {
                    AutoGiftTime = 2f;
                    foreach (var palyer in TemaManger.GetAllPlayer())
                    {
                        AutoGift(ref TemaManger, palyer.m_Open_ID);
                    }
                }
            }
        }
        else
        {
            TemaManger.SelectedGift.m_GiftName = "";
            TemaManger.SelectedGiftNum = 1;
        }
    }
    //---------------------------------------



    //��û���Entity
    void InitGetJidiEntity()
    {
        var teamMager = TeamManager.teamManager;

        if (teamMager == null || teamMager.InitGetJiDiEntity == false) return;
        //��û���Entity
        Entities.ForEach((Entity enti, JiDi jidi, in SX sx, in MyLayer mylayer) =>
        {
            switch (mylayer.BelongsTo)
            {
                case layer.Team1: teamMager.Team1_JiDiEntity = enti;break;
                case layer.Team2: teamMager.Team2_JiDiEntity = enti;break;
            }

        }).WithBurst().WithStructuralChanges().Run();
        teamMager.InitGetJiDiEntity = false;


        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;
        //���ò�ͬ�ѶȵĻ���HP
        var jidiSX = EntityManager.GetComponentData<SX>(teamMager.Team1_JiDiEntity);
        jidiSX.HP = gameRoot.JiDiHP;
        jidiSX.Cur_HP = gameRoot.JiDiHP;
        EntityManager.SetComponentData(teamMager.Team1_JiDiEntity, jidiSX);
        jidiSX = EntityManager.GetComponentData<SX>(teamMager.Team2_JiDiEntity);
        jidiSX.HP = gameRoot.JiDiHP;
        jidiSX.Cur_HP = gameRoot.JiDiHP;
        EntityManager.SetComponentData(teamMager.Team2_JiDiEntity, jidiSX);

    }
    //��û���HP
    void GetJiDiEnti()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null || teamMager.Is_GameOver) return;

        if (EntityManager.HasComponent<SX>(teamMager.Team1_JiDiEntity))
            teamMager.Team1_JiDiHP = m_SX[teamMager.Team1_JiDiEntity].Cur_HP;
        if (EntityManager.HasComponent<SX>(teamMager.Team2_JiDiEntity))
            teamMager.Team2_JiDiHP = m_SX[teamMager.Team2_JiDiEntity].Cur_HP;

        if (teamMager.Team1_JiDiHPSlider == null || teamMager.Team2_JiDiHPSlider == null)
        {
            Debug.Log($" δ��ȡ������Slider");
            return;
        }

        //Team1�Ļ���Entityû����,Team2�ͻ�ʤ
        if (teamMager.Team1_JiDiHP <= 0/*!EntityManager.Exists(teamMager.Team1_JiDiEntity)*/)
        {
            teamMager.Team1_JiDiHP = 0;
            teamMager.m_WinTeam = WinTeam.Team2;
            teamMager.Team1_JiDiHPSlider.value = teamMager.Team1_JiDiHP;
            teamMager.AddWinPanel();
            return;
        }
        //Team2�Ļ���Entityû����,Team1�ͻ�ʤ
        if (teamMager.Team2_JiDiHP <= 0/*!EntityManager.Exists(teamMager.Team2_JiDiEntity)*/)
        {
            teamMager.Team2_JiDiHP = 0;
            teamMager.m_WinTeam = WinTeam.Team1;
            teamMager.Team2_JiDiHPSlider.value = teamMager.Team2_JiDiHP;
            teamMager.AddWinPanel();
            return;
        }



        teamMager.Team1_JiDiHPSlider.value = teamMager.Team1_JiDiHP;
        teamMager.Team2_JiDiHPSlider.value = teamMager.Team2_JiDiHP;
        //Debug.Log($"  ����1 HPΪ{teamMager.Team1_JiDiHP}��  ����2 HPΪ{teamMager.Team2_JiDiHP}" +
        //          $"  ����1 SliderValue{teamMager.Team1_JiDiHPSlider.value}�� ����2 SliderValue{teamMager.Team2_JiDiHPSlider.value}" +
        //          $"  ����1 SliderMaxValueΪ{teamMager.Team1_JiDiHPSlider.maxValue}��");

    }
    //ʿ������1
    void ShiBingInput_1(in Spawn spawn, ref TeamManager TemaManger)
    {
        Entity tmepInstan = Entity.Null;
        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad1))//��F1��С����Ϊ����1
            tmepInstan = spawn.Team1_BaoLei;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad2))
            tmepInstan = spawn.Team1_ChangGong;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad3))
            tmepInstan = spawn.Team1_JianYa;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad4))
            tmepInstan = spawn.Team1_HuGuang;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad5))
            tmepInstan = spawn.Team1_PaChong;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad6))
            tmepInstan = spawn.Team1_TieChui;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad7))
            tmepInstan = spawn.Team1_YeMa;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad8))
            tmepInstan = spawn.Team1_BaoYu;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad9))
            tmepInstan = spawn.Team1_BingFeng;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha1))
            tmepInstan = spawn.Team1_BaZhu;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha2))
            tmepInstan = spawn.Team1_GangQiu;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha3))
            tmepInstan = spawn.Team1_FengHuang;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha4))
            tmepInstan = spawn.Team1_ZhanZhengGongChang;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha5))
            tmepInstan = spawn.Team1_HuoShen;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha6))
            tmepInstan = spawn.Team1_HaiKe;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha7))
            tmepInstan = spawn.Team1_RongDian;
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha8))
            tmepInstan = spawn.Team1_XiNiu;



        if (tmepInstan != Entity.Null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                for (int i = 0; i < 50; ++i)
                {
                    if (tmepInstan == spawn.Team1_JianYa)
                    {
                        if (likeShiBingNum(layer.Team1))
                            InstanEntity(spawn, tmepInstan, layer.Team1);
                    }
                    else
                        InstanEntity(spawn, tmepInstan, layer.Team1);
                }
            }
            else
            {
                if (tmepInstan == spawn.Team1_JianYa)
                {
                    if (likeShiBingNum(layer.Team1))
                        InstanEntity(spawn, tmepInstan, layer.Team1);
                }
                else
                    InstanEntity(spawn, tmepInstan, layer.Team1);
            }

            return;
        }

        if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad1))//��F2��С����Ϊ����2
            tmepInstan = spawn.Team2_BaoLei;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad2))
            tmepInstan = spawn.Team2_ChangGong;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad3))
            tmepInstan = spawn.Team2_JianYa;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad4))
            tmepInstan = spawn.Team2_HuGuang;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad5))
            tmepInstan = spawn.Team2_PaChong;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad6))
            tmepInstan = spawn.Team2_TieChui;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad7))
            tmepInstan = spawn.Team2_YeMa;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad8))
            tmepInstan = spawn.Team2_BaoYu;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad9))
            tmepInstan = spawn.Team2_BingFeng;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha1))
            tmepInstan = spawn.Team2_BaZhu;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha2))
            tmepInstan = spawn.Team2_GangQiu;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha3))
            tmepInstan = spawn.Team2_FengHuang;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha4))
            tmepInstan = spawn.Team2_ZhanZhengGongChang;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha5))
            tmepInstan = spawn.Team2_HuoShen;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha6))
            tmepInstan = spawn.Team2_HaiKe;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha7))
            tmepInstan = spawn.Team2_RongDian;
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha8))
            tmepInstan = spawn.Team2_XiNiu;


        if (tmepInstan != Entity.Null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                for (int i = 0; i < 50; ++i)
                {
                    if (tmepInstan == spawn.Team2_JianYa)
                    {
                        if (likeShiBingNum(layer.Team2))
                            InstanEntity(spawn, tmepInstan, layer.Team2);
                    }
                    else
                        InstanEntity(spawn, tmepInstan, layer.Team2);
                }
            }
            else
            {
                if (tmepInstan == spawn.Team2_JianYa)
                {
                    if (likeShiBingNum(layer.Team2))
                        InstanEntity(spawn, tmepInstan, layer.Team2);
                }
                else
                    InstanEntity(spawn, tmepInstan, layer.Team2);
            }
            return;
        }
    }
    //��Ұ���
    void PlayerInput(in Spawn spawn, ref TeamManager teamMager)
    {
        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad1))
            NewPlayer("OpenID_1_1", "��ķ", "UI/Image/��ķͷ��", 1, layer.Team1, spawn);
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Keypad2))
            NewPlayer("OpenID_1_2", "������", "UI/Image/������ͷ��", 8, layer.Team1, spawn);
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad1))
            NewPlayer("OpenID_2_1", "˫���", "UI/Image/˫���ͷ��", 35, layer.Team2, spawn);
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Keypad2))
            NewPlayer("OpenID_2_2", "��С��", "UI/Image/��С��ͷ��", 13, layer.Team2, spawn);
    }
    //ʵ����ʿ��
    void ShiBingInput_2(in Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager.SelectedPlayer == null)
            return;
        Entity tmepInstan = Entity.Null;
        ShiBingName shibingName = ShiBingName.Null;
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad1))//��F1��С����Ϊ����1
        {
            switch(teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_BaoLei;break;
                case layer.Team2: tmepInstan = spawn.Team2_BaoLei;break;
            }
            shibingName = ShiBingName.BaoLei;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad2))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_ChangGong; break;
                case layer.Team2: tmepInstan = spawn.Team2_ChangGong; break;
            }
            shibingName = ShiBingName.ChangGong;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad3))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_JianYa; break;
                case layer.Team2: tmepInstan = spawn.Team2_JianYa; break;
            }
            shibingName = ShiBingName.JianYa;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad4))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_HuGuang; break;
                case layer.Team2: tmepInstan = spawn.Team2_HuGuang; break;
            }
            shibingName = ShiBingName.HuGuang;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad5))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_PaChong; break;
                case layer.Team2: tmepInstan = spawn.Team2_PaChong; break;
            }
            shibingName = ShiBingName.PaChong;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad6))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_TieChui; break;
                case layer.Team2: tmepInstan = spawn.Team2_TieChui; break;
            }
            shibingName = ShiBingName.TieChui;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad7))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_YeMa; break;
                case layer.Team2: tmepInstan = spawn.Team2_YeMa; break;
            }
            shibingName = ShiBingName.YeMa;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad8))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_BaoYu; break;
                case layer.Team2: tmepInstan = spawn.Team2_BaoYu; break;
            }
            shibingName = ShiBingName.BaoYu;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad9))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_BingFeng; break;
                case layer.Team2: tmepInstan = spawn.Team2_BingFeng; break;
            }
            shibingName = ShiBingName.BingFeng;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_BaZhu; break;
                case layer.Team2: tmepInstan = spawn.Team2_BaZhu; break;
            }
            shibingName = ShiBingName.BaZhu;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_GangQiu; break;
                case layer.Team2: tmepInstan = spawn.Team2_GangQiu; break;
            }
            shibingName = ShiBingName.GangQiu;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_FengHuang; break;
                case layer.Team2: tmepInstan = spawn.Team2_FengHuang; break;
            }
            shibingName = ShiBingName.FengHuang;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_ZhanZhengGongChang; break;
                case layer.Team2: tmepInstan = spawn.Team2_ZhanZhengGongChang; break;
            }
            shibingName = ShiBingName.ZhanZhengGongChang;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_HuoShen; break;
                case layer.Team2: tmepInstan = spawn.Team2_HuoShen; break;
            }
            shibingName = ShiBingName.HuoShen;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_HaiKe; break;
                case layer.Team2: tmepInstan = spawn.Team2_HaiKe; break;
            }
            shibingName = ShiBingName.HaiKe;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_RongDian; break;
                case layer.Team2: tmepInstan = spawn.Team2_RongDian; break;
            }
            shibingName = ShiBingName.RongDian;
        }
        else if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha8))
        {
            switch (teamMager.SelectedPlayer.m_Team)
            {
                case layer.Team1: tmepInstan = spawn.Team1_XiNiu; break;
                case layer.Team2: tmepInstan = spawn.Team2_XiNiu; break;
            }
            shibingName = ShiBingName.XiNiu;
        }
        if (tmepInstan != Entity.Null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                for (int i = 0; i < 50; ++i)
                {
                    LikeOrGiftShiBing(ref tmepInstan,in shibingName ,in spawn);
                }
            }
            else
            {
                LikeOrGiftShiBing(ref tmepInstan,in shibingName , in spawn);
            }
            return;
        }

    }
    //�����ﰴ��
    void GiftShiBingInput(in Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        var giftMager = GiftManager.Instance;
        if (teamMager == null || giftMager == null || teamMager.SelectedPlayer == null) return;
        TKMessageData TKdata = new TKMessageData();
        TKdata.OpenID = teamMager.SelectedPlayer.m_Open_ID;
        if ((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad1)))
        {
            TKdata.GiftType = "��Ů��";
            TKdata.GiftNum = "1";
        }
        else if((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad2)))
        {
            TKdata.GiftType = "����Ȧ";
            TKdata.GiftNum = "1";
        }
        else if ((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad3)))
        {
            TKdata.GiftType = "�������";
            TKdata.GiftNum = "1";
        }
        else if ((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad4)))
        {
            TKdata.GiftType = "��ħը��";
            TKdata.GiftNum = "1";
        }
        else if ((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad5)))
        {
            TKdata.GiftType = "���ؿ�Ͷ";
            TKdata.GiftNum = "1";
        }
        else if ((Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Keypad6)))
        {
            TKdata.GiftType = "��������";
            TKdata.GiftNum = "1";
        }
        else
        {
            TKdata.OpenID = "";
        }
        if (Input.GetKey(KeyCode.Space))
            TKdata.GiftNum = "5";
        if (!string.IsNullOrWhiteSpace(TKdata.OpenID))
            giftMager.TKTOGiftType(in TKdata);

    }
    //����˳����������������������
    void UpdataGiftQueueList(in Spawn spawn)
    {
        var GiftMager = GiftManager.Instance;
        if (GiftMager == null)
            return;
        if (GiftMager.GiftQueueList.Count <= 0) return;
        var gift = GiftMager.GiftQueueList.FirstOrDefault();
        for (int i = 0; i < gift.m_GiftNum; ++i)
        {
            InstantiateGift(ref gift, in spawn);//ʵ��������
            //��������� �� ���˼���
            var player = TeamManager.teamManager.GetPlayer(gift.m_OpneID);
            if (player == null) continue;
            player.m_GiftScore += gift.m_GiftIntegral * GiftMager.GiftIntegralMagnification;
            player.m_TotalVoiceWave += gift.m_VoiceWave;
        }

        //���������������Ч
        GiftMager.GiftEffectList.Add(gift);
        //�������������֪ͨ
        GiftMager.AddGiftOntice(gift);
        //�ж��Ƿ����׳佱��
        GiftMager.CheckFirstRechargeGift(in gift, spawn, EntityManager);

        //�������꣬�͵�����һ��Ԫ��
        GiftMager.GiftQueueList.RemoveAt(0);//������һ��Ԫ��
    }
    //��ӵ��ޱ��������
    void LikeOrGiftShiBing(ref Entity tmepInstan, in ShiBingName sbName, in Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        var EntiUIMager = EntityUIManager.Instance;
        if (teamMager == null || EntiUIMager == null) return;
        if (sbName == ShiBingName.JianYa)/*tmepInstan == spawn.Team1_JianYa || tmepInstan == spawn.Team2_JianYa*/
        {
            if (likeShiBingNum(teamMager.SelectedPlayer.m_Team))
            {
                var likeSB = InstanEntity(spawn, tmepInstan, teamMager.SelectedPlayer.m_Team);
                UpDataComponentLookup();
                if (likeSB != Entity.Null)
                {
                    //teamMager.SelectedPlayer.m_LikeShiBingList.Add(likeSB);
                }
            }
        }
        else
        {
            //�ۼ����ﵥλ�������Ƿ�Ϊ������λ
            //if (giftAccMager.AddGiftNum(ref teamMager.SelectedPlayer, sbName))
            //    tmepInstan = GetLevelUpShiBingEntity(in sbName, in teamMager.SelectedPlayer.m_Team, in spawn);
            
            var GiftSB = InstanEntity(spawn, tmepInstan, teamMager.SelectedPlayer.m_Team);
            UpDataComponentLookup();
            if (GiftSB != Entity.Null)
            {
                if(teamMager.SelectedPlayer.m_comdType != commandType.NUll)//���������
                {
                    if(EntityManager.HasComponent<BarrageCommand>(GiftSB))
                    {
                        var barageComd = m_BarrageCommand[GiftSB];
                        barageComd.command = teamMager.SelectedPlayer.m_comdType;
                        EntityManager.SetComponentData(GiftSB, barageComd);
                    }
                }
                //��ʿ�������ҵ�EntityID
                EntityManager.AddComponentData(GiftSB, new EntityOpenID
                {
                    PlayerEntiyID = TeamManager.teamManager.SelectedPlayer.m_Entity_ID,
                });
                teamMager.SelectedPlayer.m_GiftShiBingList.Add(GiftSB);
                //���ͷ��
                EntiUIMager.AvatarDisplay(in teamMager.SelectedPlayer, GiftSB,in sbName, EntityManager, false);
                //�����Entityʿ�����ͷ�����
                EntityManager.AddComponentData(GiftSB, new AvatarName());
            }
        }
    }
    //ʵ��������
    void InstantiateGift(ref GiftType gift, in Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        var giftMager = GiftManager.Instance;
        var EntiUIMager = EntityUIManager.Instance;
        var palyer = teamMager.GetPlayer(gift.m_OpneID);
        if (giftMager == null || palyer == null || EntiUIMager == null) return;

        //�ۼ����ﵥλ�������Ƿ�Ϊ������λ
        Entity tmepInstan = Entity.Null;
        bool b = giftMager.AddGiftNum(ref palyer, ref gift);
        tmepInstan = spawn.ShiBingNameEntity(in gift.m_shibingName, in gift.m_team, in spawn, b);
        for (int i = 0; i < gift.m_ShiBingNum; ++i)
        {
            var GiftSB = InstanEntity(spawn, tmepInstan, gift.m_team);
            //��ʿ�������ҵ�EntityID
            EntityManager.AddComponentData(GiftSB, new EntityOpenID
            {
                PlayerEntiyID = TeamManager.teamManager.GetPlayer(gift.m_OpneID).m_Entity_ID,
            });
            UpDataComponentLookup();
            if (GiftSB != Entity.Null)
            {
                if (palyer.m_comdType != commandType.NUll)//���������
                {
                    if (EntityManager.HasComponent<BarrageCommand>(GiftSB))
                    {
                        var barageComd = m_BarrageCommand[GiftSB];
                        barageComd.command = palyer.m_comdType;
                        EntityManager.SetComponentData(GiftSB, barageComd);
                    }
                }

                palyer.m_GiftShiBingList.Add(GiftSB);
                //�������˺�ʵ����һ��ͷ��
                EntiUIMager.AddHeadshot(in gift.m_OpneID, in gift.m_shibingName, in GiftSB, EntityManager);
                //AddHeadshot(in gift.m_OpneID, in gift.m_shibingName, ref palyer, in GiftSB);
            }
        }

    }
    //���ޱ�ʵ����
    void InstanceList(ref TeamManager teamMager,in Spawn spawn)
    {
        var EntiUIMager = EntityUIManager.Instance;
        var gameRoot = GameRoot.Instance;
        if (teamMager.SelectedPlayer == null || EntiUIMager == null || gameRoot == null) return;
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; ++i)
            {
                if (likeShiBingNum(teamMager.SelectedPlayer.m_Team))
                {
                    Entity tmepInstan = Entity.Null;
                    switch (teamMager.SelectedPlayer.m_Team)
                    {
                        case layer.Team1: tmepInstan = spawn.Team1_JianYa; break;
                        case layer.Team2: tmepInstan = spawn.Team2_JianYa; break;
                    }
                    var likeSB = InstanEntity(spawn, tmepInstan, teamMager.SelectedPlayer.m_Team);
                    UpDataComponentLookup();
                    if (likeSB != Entity.Null)
                    {
                        teamMager.SelectedPlayer.m_LikeShiBingList.Add(likeSB);
                    }
                    var SBname = ShiBingName.JianYa;
                    EntiUIMager.AddHeadshot(in teamMager.SelectedPlayer.m_Open_ID, in SBname, in likeSB, EntityManager);
                    gameRoot.Cur_LikeNum += 1;
                    //AddHeadshot(in teamMager.SelectedPlayer.m_Open_ID, in SBname, ref teamMager.SelectedPlayer, in likeSB);
                }
            }

            //��һ�����аٷ�֮30�ļ��ʳ�������
            var range = UnityEngine.Random.Range(0f, 10f);
            if (range <= teamMager.PaChongOdds)
            {
                var pachongInstan = teamMager.SelectedPlayer.m_Team == layer.Team1 ? spawn.Team1_PaChong : spawn.Team2_PaChong;
                var Pachong = InstanEntity(spawn, pachongInstan, teamMager.SelectedPlayer.m_Team);
                UpDataComponentLookup();
                if (Pachong != Entity.Null)
                    teamMager.SelectedPlayer.m_LikeShiBingList.Add(Pachong);
                var SBname = ShiBingName.JianYa;
                EntiUIMager.AddHeadshot(in teamMager.SelectedPlayer.m_Open_ID, in SBname, in Pachong, EntityManager);
                gameRoot.Cur_LikeNum += 1;
            }
        }
    }
    //�������
    void NewPlayer(string openID, string nick, string avatar, int rank, layer team, Spawn spawn)
    {
        var teamMager = TeamManager.teamManager;
        var giftMager = GiftManager.Instance;
        var intoNoticeMager = IntoNoticeManager.Instance;
        if (teamMager == null || intoNoticeMager == null || giftMager == null) return;
        if(team == layer.Team1)
        {
            if (teamMager._Dic_Team1.ContainsKey(openID))//����������ɫ���˳���û�о�New
            {
                teamMager.SelectedPlayer = teamMager._Dic_Team1[openID];
                return;
            }
            else
            {
                var player = new PlayerData
                {
                    m_Open_ID = openID,
                    m_Nick = nick,
                    m_Avatar = avatar,
                    m_Rank = rank,
                    m_Team = team,
                    m_shootTarget = null,
                    m_comdType = commandType.NUll,
                    m_Entity_ID = EntityManager.Instantiate(spawn.PlayerEntityID),
                };
                giftMager.InitGiftNum(ref player);
                teamMager._Dic_Team1.Add(player.m_Open_ID, player);
                teamMager._Dic_Team1OpenID.Add(player.m_Entity_ID, player.m_Open_ID);
                teamMager.SelectedPlayer = player;
                //��������ݼ��ص��볡֪ͨ�б�
                intoNoticeMager.IntoNoticeQueueList.Add(player);
            }
        }
        else if(team == layer.Team2)
        {
            if (teamMager._Dic_Team2.ContainsKey(openID))//����������ɫ���˳���û�о�New
            {
                teamMager.SelectedPlayer = teamMager._Dic_Team2[openID];
                return;
            }
            else
            {
                var player = new PlayerData
                {
                    m_Open_ID = openID,
                    m_Nick = nick,
                    m_Avatar = avatar,
                    m_Rank = rank,
                    m_Team = team,
                    m_shootTarget = null,
                    m_comdType = commandType.NUll,
                    m_Entity_ID = EntityManager.Instantiate(spawn.PlayerEntityID),
                };
                giftMager.InitGiftNum(ref player);
                teamMager._Dic_Team2.Add(player.m_Open_ID, player);
                teamMager._Dic_Team2OpenID.Add(player.m_Entity_ID, player.m_Open_ID);
                teamMager.SelectedPlayer = player;
                //��������ݼ��ص��볡֪ͨ�б�
                intoNoticeMager.IntoNoticeQueueList.Add(player);
            }
        }


    }
    //���ﰴ��
    void MonsterInput(in Spawn spawn)
    {
        var MonsterMager = MonsterManager.instance;
        if (MonsterMager == null) return;

        Entity tmepInstan = Entity.Null;
        if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad1))
            tmepInstan = spawn.Monster_1;
        else if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad3))
            tmepInstan = spawn.Monster_3;
        else if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad4))
            tmepInstan = spawn.Monster_4;
        else if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad5))
            tmepInstan = spawn.Monster_5;
        else if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad6))
            tmepInstan = spawn.Monster_6;
        else if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.Keypad7))
            tmepInstan = spawn.Monster_7;

        if (tmepInstan != Entity.Null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                for (int i = 0; i < 50; ++i)
                {
                    var monster = InstanEntity(spawn, tmepInstan, layer.Neutral);
                    MonsterMager.MonsterDic.Add(monster, new MonsterData());
                }
            }
            else
            {
                var monster = InstanEntity(spawn, tmepInstan, layer.Neutral);
                MonsterMager.MonsterDic.Add(monster, new MonsterData());
            }
        }
    }
    //���޴ﵽ������ʵ������Ϸ�ѶȵĹ���
    void Like_Monster(in Spawn spawn)
    {
        var gameRoot = GameRoot.Instance;
        var monsterMager = MonsterManager.instance;
        if (gameRoot == null || monsterMager == null) return;
        if (gameRoot.Cur_LikeNum < gameRoot.LikeNum)
            return;
        gameRoot.Cur_LikeNum = 0;

        //������Ϸ�ȼ�������
        for (int i = 0; i < gameRoot.MonsterNum; ++i)
        {
            var monster = InstanEntity(spawn, spawn.Monster_1, layer.Neutral);
            monsterMager.MonsterDic.Add(monster, new MonsterData());

            monster = InstanEntity(spawn, spawn.Monster_5, layer.Neutral);
            monsterMager.MonsterDic.Add(monster, new MonsterData());

            monster = InstanEntity(spawn, spawn.Monster_6, layer.Neutral);
            monsterMager.MonsterDic.Add(monster, new MonsterData());

            monster = InstanEntity(spawn, spawn.Monster_7, layer.Neutral);
            monsterMager.MonsterDic.Add(monster, new MonsterData());
        }

    }
    //ʵ����ʿ��
    Entity InstanEntity(in Spawn spawn, Entity entity, layer id)
    {
        if(!EntityManager.Exists(entity))
        {
            return Entity.Null;
        }

        var shibing = EntityManager.Instantiate(entity);
        Entity enemyjidi = Entity.Null;
        Entity jidiFirePoint = Entity.Null;

        UpDataComponentLookup();
        if (id == layer.Team1)
        {
            jidiFirePoint = m_jidi[spawn.JiDi_Team1].FirePoint;
            enemyjidi = spawn.JiDi_Team2;
        }
        else if(id == layer.Team2)
        {
            jidiFirePoint = m_jidi[spawn.JiDi_Team2].FirePoint;
            enemyjidi = spawn.JiDi_Team1;
        }
        else if (id == layer.Neutral)
        {
            jidiFirePoint = spawn.MonsterBirthPoint;
        }

        if(id == layer.Team1 || id == layer.Team2)
        {
            //���ָ�ӹ��������
            EntityManager.AddComponentData(shibing, new BarrageCommand
            {
                command = commandType.NUll,
                Comd_ShootEntity = Entity.Null,
            });
            //��ӻ������
            EntityManager.AddComponentData(shibing, new Integral
            {
                ATIntegral = 0,
                AttackMeEntity = Entity.Null,
            });
        }
        if(jidiFirePoint == Entity.Null)
        {
            return Entity.Null;
        }
        UpDataComponentLookup();

        var shibingtransform = EntityManager.GetComponentData<LocalTransform>(shibing);
        var jidipoint = m_LocalToWorld[jidiFirePoint].Position;
        if(id != layer.Neutral)
        {
            jidipoint.x = UnityEngine.Random.Range(50f, -50f);//(-225f, 225f);
            jidipoint.z = jidipoint.z + UnityEngine.Random.Range(20f, -20f);//(100f, -100f);//
        }
        shibingtransform.Position = jidipoint;
        //BaZhuInstanPos(shibing, id,ref shibingtransform,ref jidipoint);

        shibingtransform.Rotation = m_LocalToWorld[jidiFirePoint].Rotation;
        EntityManager.SetComponentData(shibing, shibingtransform);

        var shibingChange = EntityManager.GetComponentData<ShiBingChange>(shibing);
        //shibingChange.Dir = math.normalize(m_transform[enemyjidi].Position - m_transform[shibing].Position);
        shibingChange.Act = ActState.Idle;
        shibingChange.enemyJiDi = enemyjidi;


        EntityManager.AddComponentData(shibing, new Idle());
        EntityManager.SetComponentEnabled<Idle>(shibing, false);
        EntityManager.AddComponentData(shibing, new Walk());
        EntityManager.SetComponentEnabled<Walk>(shibing, true);
        EntityManager.AddComponentData(shibing, new Move());
        EntityManager.SetComponentEnabled<Move>(shibing, false);
        EntityManager.AddComponentData(shibing, new Fire());
        EntityManager.SetComponentEnabled<Fire>(shibing, false);

        //�г�����Ϊ�Ļ�=====
        if (!EntityManager.HasComponent<ShiBing>(shibing) || !AppearManager.instance)
            return shibing;
        if (EntityManager.HasComponent<Appear>(shibing))
        {
            EntityManager.SetComponentEnabled<Walk>(shibing, false);
            shibingChange.Act = ActState.Appear;
            AppearManager.instance.RunAppear(shibing, shibingtransform.Position, shibingtransform.Rotation);//�������ʿ���ĳ�����ʽ
        }
        //--------------------
        EntityManager.SetComponentData(shibing, shibingChange);
        return shibing;

    }
    //����ʵ����λ�ò�ͬ
    void BaZhuInstanPos(Entity shibing,layer id,ref LocalTransform shibingtransform,ref float3 jidipoint)
    {
        if (EntityManager.GetComponentData<ShiBing>(shibing).Name != ShiBingName.BaZhu)
            return;
        if (id == layer.Team1)
            shibingtransform.Position.z = jidipoint.z - 1.14f;//shibingtransform.Position.z = jidipoint.z - 100f;
        else if (id == layer.Team2)
            shibingtransform.Position.z = jidipoint.z + 1.14f;//shibingtransform.Position.z = jidipoint.z + 100f;
        shibingtransform.Position.y = jidipoint.y + 22.8f;//shibingtransform.Position.y = jidipoint.y + 200f;
    }
    //���Ƶ��ޱ��ĸ���
    bool likeShiBingNum(layer id)
    {
        var TemaManger = TeamManager.teamManager;
        bool b = true;
        if (id == layer.Team1 && TemaManger.Tema1_LikeSoldierAllNum >= TemaManger.Tema1_LikeSoldierMaxNum)
        {
            b = false;
            TemaManger.Tema1_LikeSoldierQueueNum += 1;
        }
        else if (id == layer.Team2 && TemaManger.Tema2_LikeSoldierAllNum >= TemaManger.Tema2_LikeSoldierMaxNum)
        {
            b = false;
            TemaManger.Tema2_LikeSoldierQueueNum += 1;
        }
        return b;
    }
    //ʵ�������ޱ�
    void InstanLikeShiBing(ref TeamManager TemaManger, in Spawn spawn, layer id)
    {
        //���㳡�ϻ�����ٵ��ޱ��ű���
        int MaxNum = 0;
        int AllNum = 0;
        int QueueNum = 0;
        Entity LikeShiBingEnti = Entity.Null;
        if(id == layer.Team1)
        {
            MaxNum = TemaManger.Tema1_LikeSoldierMaxNum;
            AllNum = TemaManger.Tema1_LikeSoldierAllNum;
            QueueNum = TemaManger.Tema1_LikeSoldierQueueNum;
            LikeShiBingEnti = spawn.Team1_JianYa;
        }
        else if(id == layer.Team2)
        {
            MaxNum = TemaManger.Tema2_LikeSoldierMaxNum;
            AllNum = TemaManger.Tema2_LikeSoldierAllNum;
            QueueNum = TemaManger.Tema2_LikeSoldierQueueNum;
            LikeShiBingEnti = spawn.Team2_JianYa;
        }
        if (QueueNum <= 0)//û���Ŷӵ��˾��˳�
            return;
        int difference = MaxNum - AllNum;
        if (difference <= 0)
            return;
        for (int i = 0; i < difference; ++i)
        {
            InstanEntity(spawn, LikeShiBingEnti, id);
            QueueNum -= 1;
            if (QueueNum <= 0)
                break;
        }
        if(id == layer.Team1)
            TemaManger.Tema1_LikeSoldierQueueNum = QueueNum;
        else if(id == layer.Team2)
            TemaManger.Tema2_LikeSoldierQueueNum = QueueNum;

    }
    //ʵ��������ѡ���ʿ��
    void InstanCardShiBing(in Spawn spawn)
    {
        var CardMager = CardManager.Instance;
        if (CardMager == null)
            return;
        if (CardMager.Team1_randomCardData != null)
        {
            var name = CardMager.Team1_randomCardData.m_RandomCard.m_ImagePath;
            var team = CardMager.Team1_randomCardData.m_Team;
            var num = CardMager.Team1_randomCardData.m_RandomCard.m_Num;
            var shibingEntity = spawn.ShiBingNameEntity(in name, in team, in spawn);
            for (int i = 0; i < num; ++i)
            {
                InstanEntity(in spawn, shibingEntity, team);
            }
            CardMager.Team1_randomCardData = null;
        }
        if(CardMager.Team2_randomCardData != null)
        {
            var name = CardMager.Team2_randomCardData.m_RandomCard.m_ImagePath;
            var team = CardMager.Team2_randomCardData.m_Team;
            var num = CardMager.Team2_randomCardData.m_RandomCard.m_Num;
            var shibingEntity = spawn.ShiBingNameEntity(in name, in team, in spawn);
            for (int i = 0; i < num; ++i)
            {
                InstanEntity(in spawn, shibingEntity, team);
            }
            CardMager.Team2_randomCardData = null;
        }

    }

    //��������ȥ�������˵ģ����߲��Ϸ��Ĺ���
    void RemoveEntityMonster()
    {
        var monsterMager = MonsterManager.instance;
        if (monsterMager == null) return;

        //var removeList = new List<Entity>();
        //foreach(KeyValuePair<Entity,MonsterData> pair in monsterMager.MonsterDic)
        //{
        //    if (!EntityManager.Exists(pair.Key))
        //        removeList.Add(pair.Key);
        //}
        //foreach (var item in removeList)
        //    monsterMager.MonsterDic.Remove(item);

        var removeList1 = monsterMager.MonsterDic
        .Where(pair => !EntityManager.Exists(pair.Key))
        .Select(pair => pair.Key)
        .ToList();

        foreach (var item in removeList1)
            monsterMager.MonsterDic.Remove(item);
    }
    //���Լ���ӵ�������ҵ�ʿ������
    void PlayerEntiIDByPlayerShibingList()
    {
        Entities.ForEach((Entity enti, AddPlayerShiBingList AddSBList) =>
        {
            var teamMager = TeamManager.teamManager;
            var EntiUIMager = EntityUIManager.Instance;
            if (!EntityManager.HasComponent<EntityOpenID>(enti) || teamMager == null ||
                EntiUIMager == null)
                return;
            //ͨ��EntityID�ҵ�������
            var entiID = EntityManager.GetComponentData<EntityOpenID>(enti);
            var Player = teamMager.EntityIDByPlayerData(entiID.PlayerEntiyID);
            if (Player == null) return;
            var shibingName = EntityManager.GetComponentData<ShiBing>(enti).Name;
            if (shibingName == ShiBingName.JianYa)
                Player.m_LikeShiBingList.Add(enti);
            else
                Player.m_GiftShiBingList.Add(enti);
            EntiUIMager.AddHeadshot(in Player.m_Open_ID, in shibingName, in enti, EntityManager);

        }).WithBurst().WithStructuralChanges().Run();

    }


}
