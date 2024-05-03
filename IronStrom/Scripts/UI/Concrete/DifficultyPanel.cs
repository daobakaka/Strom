using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/DifficultyPanel";
    public DifficultyPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;

        //初始化难度选择面板上的按键
        InitDifficultyPanel(gameRoot);
        //难度选择按钮
        DifficultyButton(gameRoot);
        //初始化时间选择面板上的按键
        InitGameTimePanel(gameRoot);
        //时间选择按钮
        GameTimeButton(gameRoot);
        //播放音乐
        PlayMusic();
        //点击了开启按钮
        _UITool.GetOrAddComponentInChildren<Button>("Play_Button").onClick.AddListener(() =>
        {
            Debug.Log("   点击了开始按钮");
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });
        //按开始界面设置的按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Setting").onValueChanged.AddListener(delegate
        {
            Debug.Log("   点击了设置按钮");
            Push(new SettingPanel());
        });


    }

    //初始化难度选择面板上的按键
    void InitDifficultyPanel(GameRoot gameRoot)
    {
        switch(gameRoot.gameDifficulty)
        {
            case GameDifficulty.Easy: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Simple").isOn = true;break;
            case GameDifficulty.Normal: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Normal").isOn = true;break;
            case GameDifficulty.Hard: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Difficulty").isOn = true;break;
        }
    }
    //难度选择按钮
    void DifficultyButton(GameRoot gameRoot)
    {
        //点击了简单按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Simple").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Easy;
        });
        //点击了普通按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Normal").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Normal;
        });
        //点击了普通按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Difficulty").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Hard;
        });
    }
    //初始化时间选择面板上的按键
    void InitGameTimePanel(GameRoot gameRoot)
    {
        //ShortTerm,//短时间
        //MediumTerm,//中时间
        //LongTerm,//长时间
        switch (gameRoot.gameTime)
        {
            case GameTime.ShortTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_ShortTerm").isOn = true; break;
            case GameTime.MediumTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_MediumTerm").isOn = true; break;
            case GameTime.LongTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_LongTerm").isOn = true; break;
        }
    }
    //时间选择按钮
    void GameTimeButton(GameRoot gameRoot)
    {
        //点击了简单按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_ShortTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.ShortTerm;
        });
        //点击了普通按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_MediumTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.MediumTerm;
        });
        //点击了普通按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_LongTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.LongTerm;
        });
    }
    //播放音乐
    void PlayMusic()
    {
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        audioMager.PlayMusic();
        audioMager.MusicSource.loop = true;
    }

}