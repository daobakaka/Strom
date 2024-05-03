using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/StartPanel";
    public StartPanel() : base(new UIType(path)){ }


    public override void OnEnter()
    {
        _UITool.GetOrAddComponentInChildren<Button>("Button_Setting").onClick.AddListener(() =>
        {
            Debug.Log("   点击了设置按钮");
            Push(new SettingPanel());
        });
        _UITool.GetOrAddComponentInChildren<Button>("Button _Play").onClick.AddListener(() =>
        {
            Debug.Log("   点击了开始按钮");
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });
        _UITool.GetOrAddComponentInChildren<Button>("Button _Difficulty").onClick.AddListener(() =>
        {
            Push(new DifficultyPanel());
        });

        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        audioMager.PlayMusic();
        audioMager.MusicSource.loop = true;
    }

}
