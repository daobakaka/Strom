using DashGame;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public enum RankType 
{
    /// <summary>
    /// �հ�
    /// </summary>
    Daily,
    /// <summary>
    /// �ܰ��
    /// </summary>
    Weekly,
    /// <summary>
    /// �°�
    /// </summary>
    Monthly,
}
public class UserData
{
    /// <summary>
    /// ΨһID
    /// </summary>
    public string open_id;
    /// <summary>
    /// �ǳ�
    /// </summary>
    public string nick;
    /// <summary>
    /// ͷ��
    /// </summary>
    public string avatar;
    /// <summary>
    /// ��ʤ
    /// </summary>
    public int wins;
    /// <summary>
    /// �������������� �ܰ�
    /// </summary>
    public int rank;
    /// <summary>
    /// ribang
    /// </summary>
    public int DayRank;
    /// <summary>
    /// ribang
    /// </summary>
    public int MonthlyRank;

    /// <summary>
    /// ��������
    /// </summary>
    public int LastDayRank;
    public int LastDayScore;
    /// <summary>
    /// ��������
    /// </summary>
    public int LastWeekRank;
    public int LastWeekScore;
    /// <summary>
    /// ��������
    /// </summary>
    public int LastMonthlyRank;
    public int LastMonthlyScore;

    public UserData(JsonData json) 
    {
        if (json.Equals("")) return;
        open_id = JsonUtil.ToString(json, "open_id");
        nick = JsonUtil.ToString(json, "nick");
        avatar = JsonUtil.ToString(json, "avatar");
        wins = JsonUtil.ToInt(json, "wins");
        //rank=

        LastDayScore = JsonUtil.ToInt(json, "last_day_score");
        LastWeekScore = JsonUtil.ToInt(json, "last_week_score");
        LastMonthlyScore = JsonUtil.ToInt(json, "last_month_score");

   
    }

}
public class RankData
{
    /// <summary>
    /// ����
    /// </summary>
    public int index;
    /// <summary>
    /// ΨһID
    /// </summary>
    public string open_id;
    /// <summary>
    /// �ǳ�
    /// </summary>
    public string nick;
    /// <summary>
    /// ͷ��
    /// </summary>
    public string avatar;
    /// <summary>
    /// �������а�(��һ������ֵ �����а��������ֵ)
    /// </summary>
    public int rank;
    /// <summary>
    /// �ܻ���(���ܻ���)
    /// </summary>
    public int score_total;
    /// <summary>
    /// ���ܻ���
    /// </summary>
    public int score_total_day;
    /// <summary>
    /// ���ܻ���
    /// </summary>
    public int score_total_month;
    /// <summary>
    /// ��������
    /// </summary>
    public int last_week_rank;
    /// <summary>
    /// ��������
    /// </summary>
    public int last_day_rank;
    /// <summary>
    /// ��������
    /// </summary>
    public int last_month_rank;
    /// <summary>
    /// ���ܻ���
    /// </summary>
    public int last_week_score;
    /// <summary>
    /// ���ջ���
    /// </summary>
    public int last_day_score;
    /// <summary>
    /// ���»���
    /// </summary>
    public int last_month_score;
    /// <summary>
    /// ��߲���
    /// </summary>
    public int highest_wave;
    /// <summary>
    /// �����б�
    /// </summary>
    public List<string> kind;
    public RankData(int index,JsonData json)
    {
        if (json.Equals("")) return;
        this.index = index;
        open_id = JsonUtil.ToString(json, "open_id");
        if (json["nick"] != null)
            nick = JsonUtil.ToString(json, "nick");
        if (json["avatar"] != null)
            avatar = JsonUtil.ToString(json, "avatar");
        rank = JsonUtil.ToInt(json, "rank");
        score_total = JsonUtil.ToInt(json, "score_total");
        score_total_day = JsonUtil.ToInt(json, "score_total_day");
        score_total_month = JsonUtil.ToInt(json, "score_total_month");
        last_week_rank = JsonUtil.ToInt(json, "last_week_rank");
        last_day_rank = JsonUtil.ToInt(json, "last_day_rank");
        last_month_rank = JsonUtil.ToInt(json, "last_month_rank");
        last_week_score = JsonUtil.ToInt(json, "last_week_score");
        last_day_score = JsonUtil.ToInt(json, "last_day_score");
        last_month_score = JsonUtil.ToInt(json, "last_month_score");
        highest_wave = JsonUtil.ToInt(json, "highest_wave");
        //JsonData kinditem = json["kind"];
        //for (int i = 0; i < kinditem.Count; i++)
        //{
        //    kind.Add(kinditem[i].ToString());
        //}
        kind =  JsonUtil.ToStringList(json, "kind");
    }
}
public class XwfRequest : HttpRequest
{

    public const string GAME_CODE = "startrek";


    public static string PORT_DOUYIN = "douyin";
    public const string PORT_SHIPIN = "shipin";
    public const string PORT_Appid = "tte3ce95737cfd9cdb10";


    public const bool isWild = false;

    public static string currentPort = PORT_DOUYIN;

    private const string P = "MGVmhpKTM2";
    //public static XwfRequest ZhuboLogin(string roomId, string uid)
    //{
    //    URLRequestData data = new URLRequestData();
    //    XwfRequest request = new XwfRequest();

    //    data.Add("uid", uid);
    //    data.Add("version", Version.VALUE);
    //    data.Add("device", SystemInfo.deviceUniqueIdentifier + "2");

    //    request.m = "/anchor/login/";
    //    request.Send(data);
    //    return request;
    //}









    /// <summary>
    /// �����ϴ�����
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static XwfRequest Record(JsonData json)
    {
        URLRequestData data = new URLRequestData();
        XwfRequest request = new XwfRequest();

        JsonData pack = new JsonData();
        pack["top"] = 3;
        pack["uid"] = zhuboID;
        pack["game"] = GAME_CODE;
        pack["port"] = currentPort;
        pack["data"] = json;
        data.AddRaw(pack.ToJson());

        request.m = "/score/";
        request.Send(data);
        return request;
    }
    /// <summary>
    /// ����ĳλ�������
    /// </summary>
    /// <returns></returns>
    public static XwfRequest GetUserData(string openid)
    {
        URLRequestData data = new URLRequestData();
        XwfRequest request = new XwfRequest();

        JsonData pack = new JsonData();
        data.AddRaw(pack.ToJson());

        request.m = $"/score/{openid}/info/";
        request.Send(data, false, URLRequest.Method.GET);
        return request;
    }
    /// <summary>
    /// ����ǰ100����� �����볡Ч��
    /// </summary>
    /// <returns></returns>
    public static XwfRequest GetTop100()
    {
        URLRequestData data = new URLRequestData();
        XwfRequest request = new XwfRequest();
        data.Add("type", "weekly");

        request.m = $"/score/{100}/topId/";
        request.Send(data, false, URLRequest.Method.GET);
        return request;
    }

    /// <summary>
    /// �հ� �ܰ� �°�
    /// </summary>
    /// <returns></returns>
    public static XwfRequest GetLeaders(RankType rankType)
    {
        string str = "";
        switch (rankType)
        {
            case RankType.Daily:
                str = "daily";
                break;
            case RankType.Weekly:
                str = "weekly";
                break;
            case RankType.Monthly:
                str = "monthly";
                break;
        }
        if (string.IsNullOrEmpty(str)) 
        {
            Debug.LogError("��ȡ���а����");
        }
        URLRequestData data = new URLRequestData();
        XwfRequest request = new XwfRequest();
        data.Add("amount", 100);
        data.Add("type", str);
        request.m = "/score/top50/";
        request.Send(data, false, URLRequest.Method.GET);
        return request;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <returns></returns>
    public static XwfRequest GetAnchors()
    {
        URLRequestData data = new URLRequestData();
        XwfRequest request = new XwfRequest();
        //data.Add("amount", GameConfig.rankNumber);
        //data.Add("type", str);
        request.m = "/score/anchor_top/";
        request.Send(data, false, URLRequest.Method.GET);
        return request;

        //is_gift 1�� 0��
    }



    public static string zhuboID;
    private static string newURL = "https://hudong.zhijianhuyou.com/v1";
    //private static string newURL = "http://192.168.31.199:8000/v1";

    protected override string GetURL()
    {
        return newURL + m;
    }

    protected override void Send(URLRequestData data, bool showPending = false, URLRequest.Method method = URLRequest.Method.POST)
    {
        data.Add("game", GAME_CODE);
        data.Add("port", currentPort);
        base.Send(data, showPending, method);
    }


    override protected void Received(URLRequestResult requestResult)
    {
        if (showPending)
        {
            //ProgressPanel.Hide();
        }

        try
        {
            if (requestResult.success)
            {
                JsonData data = requestResult.GetJson();
                if (data == null)
                {
                    Debuger.LogError("invilid data : " + requestResult.GetString());
                    Debuger.LogError("error : " + requestResult.GetError());
                    ResultEvent.Invoke(false, null);
                }
                else if (JsonUtil.ContainKey(data, "code"))
                {
                    int code = JsonUtil.ToInt(data, "code");
                    if (code == 0)
                    {
                        JsonData resultData = JsonUtil.ContainKey(data, "data") ? data["data"] : new JsonData();
                        ResultEvent.Invoke(true, resultData);
                    }
                    else
                    {
                        Debuger.LogError("Error code : " + code); 
                        ResultEvent.Invoke(false, data);
                    }
                }
                else
                {
                    Debuger.LogError("Error : " + data.ToJson());
                    ResultEvent.Invoke(false, data);
                }
            }
            else
            {
                ResultEvent.Invoke(false, null);
            }
        }
        catch (System.Exception e)
        {
            Debuger.LogException(e);
        }

        ResultEvent.RemoveAllListeners();
    }

}
