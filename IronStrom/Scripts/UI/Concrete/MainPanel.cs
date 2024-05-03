using DashGame;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/MainPanel";
    public MainPanel() : base(new UIType(path)) { }


    public override void OnEnter()
    {
        //����������������õİ�ť
        var buttonMainSetting = _UITool.GetOrAddComponentInChildren<Button>("Button_MainSetting");
        if(buttonMainSetting != null)
        {
            buttonMainSetting.onClick.AddListener(() =>
            {
                Debug.Log("   ����˲������ð�ť");
                Push(new MainSettingPanel());
            });
        }

        //�������������õİ�ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Setting").onValueChanged.AddListener(delegate
        {
            Debug.Log("   ��������ð�ť");
            Push(new SettingPanel());
        });
        Debug.Log("����������");
        //��ʼ���õ�����Ѫ��Slider
        InitGetJiDiHpSlider();
        //�õ�����UI
        InitScore();
        //��ʼ��������
        PlayMusic();
        //��ʼ�����׶�
        InitDifficulty();
        //��ʼ����Ϸʱ��
        InitGameTime();

        ////���������ر������
        //_UITool.GetOrAddComponentInChildren<Button>("Button_Quit").onClick.AddListener(() =>
        //{
        //    GameRoot.Instance._SceneSystem.SetScene(new StartScenes());
        //    _PanelManager.Pop();
        //});
    }

    //��ʼ���õ�����Ѫ��Slider
    void InitGetJiDiHpSlider()
    {
        //��������ص�һʱ���Ѫ�������������
        var teamMager = TeamManager.teamManager;
        teamMager.Team1_JiDiHPSlider = _UITool.GetOrAddComponentInChildren<Slider>("Team1_JiDiHP");
        teamMager.Team2_JiDiHPSlider = _UITool.GetOrAddComponentInChildren<Slider>("Team2_JiDiHP");
        var Obj = _UITool.FindChildGameObject("Team1_HP");
        if (Obj)
            teamMager.Team1_JiDiHPText = Obj.GetComponent<TextMeshProUGUI>();
        Obj = _UITool.FindChildGameObject("Team2_HP");
        if (Obj)
            teamMager.Team2_JiDiHPText = Obj.GetComponent<TextMeshProUGUI>();
    }

    //��������һʱ���õ�
    void InitScore()
    {
        var ScoreMager = ScoreManager.instance;
        if(ScoreMager == null)
        {
            Debug.Log($"  ��������ֹ�����δ��ʼ����δ���õ�����UI");
            return;
        }
        ScoreMager.TotalScoreText = _UITool.GetOrAddComponentInChildren<TextMeshProUGUI>("JiFen");
    }

    //��ʼ��������
    void PlayMusic()
    {
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        audioMager.currentTrackIndex = 1;
        audioMager.PlayMusic();
    }

    //��ʼ�����׶�
    void InitDifficulty()
    {
        var gameRoot = GameRoot.Instance;
        //DifficultyData
        TextAsset ta = Resources.Load<TextAsset>("Config/DifficultyData");
        JsonData Jdata = JsonMapper.ToObject(ta.text);
        JsonData jsondata = null;
        for (int i = 0; i < Jdata.Count; ++i)
        {
            jsondata = Jdata[i];
            var gameDifficulty = JsonUtil.ToEnum<GameDifficulty>(jsondata, "id");
            if (gameDifficulty == gameRoot.gameDifficulty)
                break;
            if (i == Jdata.Count - 1)
                jsondata = null;
        }
        if (jsondata != null && gameRoot.gameDifficulty != GameDifficulty.NUll)
        {
            gameRoot.JiDiHP = JsonUtil.ToFloat(jsondata, "JiDi_HP");
            gameRoot.LikeNum = JsonUtil.ToInt(jsondata, "LikeNum");
            gameRoot.MonsterNum = JsonUtil.ToInt(jsondata, "MonsterNum");
            var AwardShiBingName = JsonUtil.ToString(jsondata, "AwardShiBingName").Split(",").ToList();
            var AwardShiBingNum = JsonUtil.ToString(jsondata, "AwardShiBingNum").Split(",").ToList();

            for (int i = 0; i < AwardShiBingName.Count; i++)
            {
                //����ö����
                ShiBingName sbName = (ShiBingName)Enum.Parse(typeof(ShiBingName), AwardShiBingName[i]);
                //��������
                int num = int.Parse(AwardShiBingNum[i]);
                // ��ӵ��ֵ�
                if(!gameRoot.AwardShiBingNumDic.ContainsKey(sbName))
                    gameRoot.AwardShiBingNumDic.Add(sbName, num);
            }
        }


    }

    //��ʼ����Ϸʱ��
    void InitGameTime()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;

        //���Text GameTime


    }
}
