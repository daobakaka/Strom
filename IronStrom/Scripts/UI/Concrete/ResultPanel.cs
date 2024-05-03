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
        //底部按钮初始化
        BottomButtonInit();
        //日周月榜按钮初始化
        DayWeekMonthButtonInit();
        //点击关闭按钮
        _UITool.GetOrAddComponentInChildren<Button>("CloseBtn").onClick.AddListener(() =>
        {
            Pop();
        });

        var gameOverCtrl = GameOverControl.intance;
        if (gameOverCtrl == null) return;
        //假设已经获得了服务器的数据
        gameOverCtrl.InitPlayerRankingData();
        //给所有榜单做好排名
        AllRanking(ref gameOverCtrl);
    }


    //底部按钮初始化
    void BottomButtonInit()
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        //点击本局积分榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("本局积分榜").onClick.AddListener(() =>
        {
            Debug.Log("点击了 本局积分榜");
            resultPanneldata.DayWeekMonthButton.SetActive(false);
            resultPanneldata.LocalRanking.SetActive(true);
            //resultPanneldata.DayRanking.SetActive(false);
            //resultPanneldata.WeekRanking.SetActive(false);
            //resultPanneldata.MonthRanking.SetActive(false);
            //resultPanneldata.AnchorRanking.SetActive(false);
            BottomButtonInitPressButton("本局积分榜");
            DayWeekMonthRanking("");
        });
        //点击排行榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("排行榜").onClick.AddListener(() =>
        {
            Debug.Log("点击了 排行榜");
            resultPanneldata.DayWeekMonthButton.SetActive(true);
            resultPanneldata.LocalRanking.SetActive(false);
            //resultPanneldata.DayRanking.SetActive(true);
            //resultPanneldata.WeekRanking.SetActive(false);
            //resultPanneldata.MonthRanking.SetActive(false);
            //resultPanneldata.AnchorRanking.SetActive(false);
            BottomButtonInitPressButton("排行榜");
        });
        //点击继续按钮
        _UITool.GetOrAddComponentInChildren<Button>("继续按钮").onClick.AddListener(() =>
        {
            Debug.Log("点击了 继续按钮");
            Time.timeScale = 1;
            BottomButtonInitPressButton("继续按钮");
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());//重新开始
        });
        //点击返回按钮
        _UITool.GetOrAddComponentInChildren<Button>("返回按钮").onClick.AddListener(() =>
        {
            Debug.Log("点击了 返回按钮");
            Time.timeScale = 1;
            BottomButtonInitPressButton("返回按钮");
            GameRoot.Instance._SceneSystem.SetScene(new StartScenes());//返回开始场景
        });
    }
    //日周月榜按钮初始化
    void DayWeekMonthButtonInit()
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        resultPanneldata.DayWeekMonthButton.SetActive(true);
        //点击日榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("日榜Button").onClick.AddListener(() =>
        {
            Debug.Log("点击了 日榜按钮");
            DayWeekMonthRanking("日榜");
            DayWeekMonthButtonPressButton("日榜Button");
        });
        //点击周榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("周榜Button").onClick.AddListener(() =>
        {
            Debug.Log("点击了 周榜按钮");
            DayWeekMonthRanking("周榜");
            DayWeekMonthButtonPressButton("周榜Button");
        });
        //点击月榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("月榜Button").onClick.AddListener(() =>
        {
            Debug.Log("点击了 月榜按钮");
            DayWeekMonthRanking("月榜");
            DayWeekMonthButtonPressButton("月榜Button");
        });
        //点击主播榜按钮
        _UITool.GetOrAddComponentInChildren<Button>("主播榜Button").onClick.AddListener(() =>
        {
            Debug.Log("点击了 主播榜按钮");
            DayWeekMonthRanking("主播榜");
            DayWeekMonthButtonPressButton("主播榜Button");
        });
        resultPanneldata.DayWeekMonthButton.SetActive(false);
    }
    //底部按钮,按钮按下其他按钮抬起
    void BottomButtonInitPressButton(string buttonName)
    {
        _UITool.GetOrAddComponentInChildren<Button>("本局积分榜").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("排行榜").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("继续按钮").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("返回按钮").interactable = true;

        _UITool.GetOrAddComponentInChildren<Button>(buttonName).interactable = false;
    }
    //日周月榜按钮,按钮按下其他按钮抬起
    void DayWeekMonthButtonPressButton(string buttonName)
    {
        _UITool.GetOrAddComponentInChildren<Button>("日榜Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("周榜Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("月榜Button").interactable = true;
        _UITool.GetOrAddComponentInChildren<Button>("主播榜Button").interactable = true;

        _UITool.GetOrAddComponentInChildren<Button>(buttonName).interactable = false;
    }
    //日周月主播榜，只显示一个
    void DayWeekMonthRanking(string ranking)
    {
        var resultPanneldata = _UITool.GetGameObject().GetComponent<ResultPanelData>();
        resultPanneldata.DayRanking.SetActive(false);
        resultPanneldata.WeekRanking.SetActive(false);
        resultPanneldata.MonthRanking.SetActive(false);
        resultPanneldata.AnchorRanking.SetActive(false);

        switch(ranking)
        {
            case "日榜": resultPanneldata.DayRanking.SetActive(true);break;
            case "周榜": resultPanneldata.WeekRanking.SetActive(true);break;
            case "月榜": resultPanneldata.MonthRanking.SetActive(true);break;
            case "主播榜": resultPanneldata.AnchorRanking.SetActive(true);break;
        }
    }
    //给所有榜单做好排名
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
