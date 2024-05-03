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
        //�������¿�ʼ�İ�ť
        _UITool.GetOrAddComponentInChildren<Button>("Button_Restart").onClick.AddListener(() =>
        {
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });

        //�������¿�ʼ�İ�ť
        _UITool.GetOrAddComponentInChildren<Button>("Button_Exit").onClick.AddListener(() =>
        {
            Application.Quit();
        });

        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        string win = "";
        if (teamMager.m_WinTeam == WinTeam.Team1)
            win = "��ϲ �ƶ� ��ʤ������";
        else if(teamMager.m_WinTeam == WinTeam.Team2)
            win = "��ϲ ���� ��ʤ������";
        if (string.IsNullOrWhiteSpace(win)) return;
        _UITool.GetOrAddComponentInChildren<TextMeshProUGUI>("Win").text = win;

    }

}
