using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    private TextMeshProUGUI countdownText; // UI Text组件的引用
    private TimeSpan countdownTime; // 用于计算倒计时
    private float remainingTime; // 剩余时间（秒）

    [Tooltip("游戏时间，短/分钟")] public float shortTermMinute;
    [Tooltip("游戏时间，中/分钟")] public float mediumTermMinute;
    [Tooltip("游戏时间，长/分钟")] public float longTermMinute;

    private void Awake()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var gameRoot = GameRoot.Instance;
        remainingTime = 0;
        switch (gameRoot.gameTime)
        {
            case GameTime.ShortTerm: remainingTime = shortTermMinute * 60;break;
            case GameTime.MediumTerm: remainingTime = mediumTermMinute * 60;break;
            case GameTime.LongTerm: remainingTime = longTermMinute * 60;break;
        }
        countdownTime = TimeSpan.FromSeconds(remainingTime);
        UpdateCountdownText();
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            // 减去过去的时间
            remainingTime -= Time.deltaTime;
            countdownTime = TimeSpan.FromSeconds(remainingTime);
            UpdateCountdownText();
        }
        else
        {
            countdownText.text = "00:00";
            // 倒计时结束时在这里执行
            GameOver();
        }
    }
    // 更新UI Text为倒计时时间
    void UpdateCountdownText()
    {
        countdownText.text = countdownTime.ToString(@"mm\:ss");
    }
    //时间到了结束游戏，实例化结束面板
    void GameOver()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager.Is_GameOver) return;
        teamMager.m_WinTeam = teamMager.Team1_JiDiHP >= teamMager.Team2_JiDiHP ? WinTeam.Team1 : WinTeam.Team2;
        teamMager.AddWinPanel();
    }
}
