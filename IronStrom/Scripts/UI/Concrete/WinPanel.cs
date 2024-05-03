using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/WinPanel";

    public WinPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        //按下重新开始的按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_Restart").onClick.AddListener(() =>
        {
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });

        //按下重新开始的按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_Exit").onClick.AddListener(() =>
        {
            Application.Quit();
        });

        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        string win = "";
        if (teamMager.m_WinTeam == WinTeam.Team1)
            win = "恭喜 黄队 获胜！！！";
        else if(teamMager.m_WinTeam == WinTeam.Team2)
            win = "恭喜 蓝队 获胜！！！";
        if (string.IsNullOrWhiteSpace(win)) return;
        _UITool.GetOrAddComponentInChildren<TextMeshProUGUI>("Win").text = win;

    }

}
