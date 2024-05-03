using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/ResultPanel";
    public ResultPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        //�ײ���ť��ʼ��
        BottomButtonInit();
        //�����°�ť��ʼ��
        DayWeekMonthButtonInit();
        //����رհ�ť
        _UITool.GetOrAddComponentInChildren<Button>("CloseBtn").onClick.AddListener(() =>
        {
            Pop();
        });

        var gameOverCtrl = GameOverControl.intance;
        if (gameOverCtrl == null) return;
        //�����Ѿ�����˷�����������
        gameOverCtrl.InitPlayerRankingData();
        //�����а���������
        AllRanking(ref gameOverCtrl);
    }


    //�ײ���ť��ʼ��
    void BottomButtonInit()
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        //������ֻ��ְ�ť
        _UITool.GetOrAddComponentInChildren<Button>("���ֻ��ְ�").onClick.AddListener(() =>
        {
            Debug.Log("����� ���ֻ��ְ�");
            resultPanneldata.DayWeekMonthButton.SetActive(false);
            resultPanneldata.LocalRanking.SetActive(true);
            //resultPanneldata.DayRanking.SetActive(false);
            //resultPanneldata.WeekRanking.SetActive(false);
            //resultPanneldata.MonthRanking.SetActive(false);
            //resultPanneldata.AnchorRanking.SetActive(false);
            BottomButtonInitPressButton("���ֻ��ְ�");
            DayWeekMonthRanking("");
        });
        //������а�ť
        _UITool.GetOrAddComponentInChildren<Button>("���а�").onClick.AddListener(() =>
        {
            Debug.Log("����� ���а�");
            resultPanneldata.DayWeekMonthButton.SetActive(true);
            resultPanneldata.LocalRanking.SetActive(false);
            //resultPanneldata.DayRanking.SetActive(true);
            //resultPanneldata.WeekRanking.SetActive(false);
            //resultPanneldata.MonthRanking.SetActive(false);
            //resultPanneldata.AnchorRanking.SetActive(false);
            BottomButtonInitPressButton("���а�");
        });
        //���������ť
        _UITool.GetOrAddComponentInChildren<Button>("������ť").onClick.AddListener(() =>
        {
            Debug.Log("����� ������ť");
            Time.timeScale = 1;
            BottomButtonInitPressButton("������ť");
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());//���¿�ʼ
        });
        //������ذ�ť
        _UITool.GetOrAddComponentInChildren<Button>("���ذ�ť").onClick.AddListener(() =>
        {
            Debug.Log("����� ���ذ�ť");
            Time.timeScale = 1;
            BottomButtonInitPressButton("���ذ�ť");
            GameRoot.Instance._SceneSystem.SetScene(new StartScenes());//���ؿ�ʼ����
        });
    }
    //�����°�ť��ʼ��
    void DayWeekMonthButtonInit()
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        resultPanneldata.DayWeekMonthButton.SetActive(true);
        //����հ�ť
        _UITool.GetOrAddComponentInChildren<Button>("�հ�Button").onClick.AddListener(() =>
        {
            Debug.Log("����� �հ�ť");
            DayWeekMonthRanking("�հ�");
            DayWeekMonthButtonPressButton("�հ�Button");
        });
        //����ܰ�ť
        _UITool.GetOrAddComponentInChildren<Button>("�ܰ�Button").onClick.AddListener(() =>
        {
            Debug.Log("����� �ܰ�ť");
            DayWeekMonthRanking("�ܰ�");
            DayWeekMonthButtonPressButton("�ܰ�Button");
        });
        //����°�ť
        _UITool.GetOrAddComponentInChildren<Button>("�°�Button").onClick.AddListener(() =>
        {
            Debug.Log("����� �°�ť");
            DayWeekMonthRanking("�°�");
            DayWeekMonthButtonPressButton("�°�Button");
        });
        //���������ť
        _UITool.GetOrAddComponentInChildren<Button>("������Button").onClick.AddListener(() =>
        {
            Debug.Log("����� ������ť");
            DayWeekMonthRanking("������");
            DayWeekMonthButtonPressButton("������Button");
        });
        resultPanneldata.DayWeekMonthButton.SetActive(false);
    }
    //�ײ���ť,��ť����������ţ̌��
    void BottomButtonInitPressButton(string buttonName)
    {
        _UITool.GetOrAddComponentInChildren<Button>("���ֻ��ְ�").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("���а�").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("������ť").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("���ذ�ť").interactable = true;

        _UITool.GetOrAddComponentInChildren<Button>(buttonName).interactable = false;
    }
    //�����°�ť,��ť����������ţ̌��
    void DayWeekMonthButtonPressButton(string buttonName)
    {
        _UITool.GetOrAddComponentInChildren<Button>("�հ�Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("�ܰ�Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("�°�Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("������Button").interactable = true;

        _UITool.GetOrAddComponentInChildren<Button>(buttonName).interactable = false;
    }
    //������������ֻ��ʾһ��
    void DayWeekMonthRanking(string ranking)
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        resultPanneldata.DayRanking.SetActive(false);
        resultPanneldata.WeekRanking.SetActive(false);
        resultPanneldata.MonthRanking.SetActive(false);
        resultPanneldata.AnchorRanking.SetActive(false);

        switch(ranking)
        {
            case "�հ�": resultPanneldata.DayRanking.SetActive(true);break;
            case "�ܰ�": resultPanneldata.WeekRanking.SetActive(true);break;
            case "�°�": resultPanneldata.MonthRanking.SetActive(true);break;
            case "������": resultPanneldata.AnchorRanking.SetActive(true);break;
        }
    }
    //�����а���������
    void AllRanking(ref GameOverControl gameOverCtrl)
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        resultPanneldata.LocalRanking.GetComponent<RankingData>().LoadPlayeData(in gameOverCtrl.LocalScoreList);
        resultPanneldata.DayRanking.SetActive(true);
        resultPanneldata.WeekRanking.SetActive(true);
        resultPanneldata.MonthRanking.SetActive(true);
        resultPanneldata.DayRanking.GetComponent<RankingData>().LoadPlayeData(in gameOverCtrl.Day_RankList);
        resultPanneldata.WeekRanking.GetComponent<RankingData>().LoadPlayeData(in gameOverCtrl.Week_RankList);
        resultPanneldata.MonthRanking.GetComponent<RankingData>().LoadPlayeData(in gameOverCtrl.Month_RankList);
        resultPanneldata.DayRanking.SetActive(false);
        resultPanneldata.WeekRanking.SetActive(false);
        resultPanneldata.MonthRanking.SetActive(false);
    }

}
