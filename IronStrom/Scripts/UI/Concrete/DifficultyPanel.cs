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

        //��ʼ���Ѷ�ѡ������ϵİ���
        InitDifficultyPanel(gameRoot);
        //�Ѷ�ѡ��ť
        DifficultyButton(gameRoot);
        //��ʼ��ʱ��ѡ������ϵİ���
        InitGameTimePanel(gameRoot);
        //ʱ��ѡ��ť
        GameTimeButton(gameRoot);
        //��������
        PlayMusic();
        //����˿�����ť
        _UITool.GetOrAddComponentInChildren<Button>("Play_Button").onClick.AddListener(() =>
        {
            Debug.Log("   ����˿�ʼ��ť");
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });
        //����ʼ�������õİ�ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Setting").onValueChanged.AddListener(delegate
        {
            Debug.Log("   ��������ð�ť");
            Push(new SettingPanel());
        });


    }

    //��ʼ���Ѷ�ѡ������ϵİ���
    void InitDifficultyPanel(GameRoot gameRoot)
    {
        switch(gameRoot.gameDifficulty)
        {
            case GameDifficulty.Easy: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Simple").isOn = true;break;
            case GameDifficulty.Normal: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Normal").isOn = true;break;
            case GameDifficulty.Hard: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Difficulty").isOn = true;break;
        }
    }
    //�Ѷ�ѡ��ť
    void DifficultyButton(GameRoot gameRoot)
    {
        //����˼򵥰�ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Simple").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Easy;
        });
        //�������ͨ��ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Normal").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Normal;
        });
        //�������ͨ��ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_Difficulty").onValueChanged.AddListener(delegate
        {
            gameRoot.gameDifficulty = GameDifficulty.Hard;
        });
    }
    //��ʼ��ʱ��ѡ������ϵİ���
    void InitGameTimePanel(GameRoot gameRoot)
    {
        //ShortTerm,//��ʱ��
        //MediumTerm,//��ʱ��
        //LongTerm,//��ʱ��
        switch (gameRoot.gameTime)
        {
            case GameTime.ShortTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_ShortTerm").isOn = true; break;
            case GameTime.MediumTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_MediumTerm").isOn = true; break;
            case GameTime.LongTerm: _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_LongTerm").isOn = true; break;
        }
    }
    //ʱ��ѡ��ť
    void GameTimeButton(GameRoot gameRoot)
    {
        //����˼򵥰�ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_ShortTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.ShortTerm;
        });
        //�������ͨ��ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_MediumTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.MediumTerm;
        });
        //�������ͨ��ť
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle_LongTerm").onValueChanged.AddListener(delegate
        {
            gameRoot.gameTime = GameTime.LongTerm;
        });
    }
    //��������
    void PlayMusic()
    {
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        audioMager.PlayMusic();
        audioMager.MusicSource.loop = true;
    }

}