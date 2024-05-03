using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    private TextMeshProUGUI countdownText; // UI Text���������
    private TimeSpan countdownTime; // ���ڼ��㵹��ʱ
    private float remainingTime; // ʣ��ʱ�䣨�룩

    [Tooltip("��Ϸʱ�䣬��/����")] public float shortTermMinute;
    [Tooltip("��Ϸʱ�䣬��/����")] public float mediumTermMinute;
    [Tooltip("��Ϸʱ�䣬��/����")] public float longTermMinute;

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
            // ��ȥ��ȥ��ʱ��
            remainingTime -= Time.deltaTime;
            countdownTime = TimeSpan.FromSeconds(remainingTime);
            UpdateCountdownText();
        }
        else
        {
            countdownText.text = "00:00";
            // ����ʱ����ʱ������ִ��
            GameOver();
        }
    }
    // ����UI TextΪ����ʱʱ��
    void UpdateCountdownText()
    {
        countdownText.text = countdownTime.ToString(@"mm\:ss");
    }
    //ʱ�䵽�˽�����Ϸ��ʵ�����������
    void GameOver()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager.Is_GameOver) return;
        teamMager.m_WinTeam = teamMager.Team1_JiDiHP >= teamMager.Team2_JiDiHP ? WinTeam.Team1 : WinTeam.Team2;
        teamMager.AddWinPanel();
    }
}
