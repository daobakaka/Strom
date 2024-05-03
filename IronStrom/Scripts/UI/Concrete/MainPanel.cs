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
        //按下主界面测似设置的按钮
        var buttonMainSetting = _UITool.GetOrAddComponentInChildren<Button>("Button_MainSetting");
        if(buttonMainSetting != null)
        {
            buttonMainSetting.onClick.AddListener(() =>
            {
                Debug.Log("   点击了测试设置按钮");
                Push(new MainSettingPanel());
            });
        }

        //按下主界面设置的按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Setting").onValueChanged.AddListener(delegate
        {
            Debug.Log("   点击了设置按钮");
            Push(new SettingPanel());
        });
        Debug.Log("进入主界面");
        //初始化拿到基地血条Slider
        InitGetJiDiHpSlider();
        //拿到积分UI
        InitScore();
        //开始播放音乐
        PlayMusic();
        //初始化难易度
        InitDifficulty();
        //初始化游戏时间
        InitGameTime();

        ////主场景返回标题界面
        //_UITool.GetOrAddComponentInChildren<Button>("Button_Quit").onClick.AddListener(() =>
        //{
        //    GameRoot.Instance._SceneSystem.SetScene(new StartScenes());
        //    _PanelManager.Pop();
        //});
    }

    //初始化拿到基地血条Slider
    void InitGetJiDiHpSlider()
    {
        //主界面加载第一时间把血条给队伍管理器
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

    //主场景第一时间拿到
    void InitScore()
    {
        var ScoreMager = ScoreManager.instance;
        if(ScoreMager == null)
        {
            Debug.Log($"  主界面积分管理器未初始化，未能拿到积分UI");
            return;
        }
        ScoreMager.TotalScoreText = _UITool.GetOrAddComponentInChildren<TextMeshProUGUI>("JiFen");
    }

    //开始播放音乐
    void PlayMusic()
    {
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        audioMager.currentTrackIndex = 1;
        audioMager.PlayMusic();
    }

    //初始化难易度
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
                //解析枚举名
                ShiBingName sbName = (ShiBingName)Enum.Parse(typeof(ShiBingName), AwardShiBingName[i]);
                //解析数量
                int num = int.Parse(AwardShiBingNum[i]);
                // 添加到字典
                if(!gameRoot.AwardShiBingNumDic.ContainsKey(sbName))
                    gameRoot.AwardShiBingNumDic.Add(sbName, num);
            }
        }


    }

    //初始化游戏时间
    void InitGameTime()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;

        //获得Text GameTime


    }
}
