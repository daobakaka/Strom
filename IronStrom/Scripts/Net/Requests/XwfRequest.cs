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
    /// 日榜
    /// </summary>
    Daily,
    /// <summary>
    /// 周榜榜
    /// </summary>
    Weekly,
    /// <summary>
    /// 月榜
    /// </summary>
    Monthly,
}
public class UserData
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public string open_id;
    /// <summary>
    /// 昵称
    /// </summary>
    public string nick;
    /// <summary>
    /// 头像
    /// </summary>
    public string avatar;
    /// <summary>
    /// 连胜
    /// </summary>
    public int wins;
    /// <summary>
    /// 排名（世界排行 周榜）
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
    /// 昨日排行
    /// </summary>
    public int LastDayRank;
    public int LastDayScore;
    /// <summary>
    /// 上周排行
    /// </summary>
    public int LastWeekRank;
    public int LastWeekScore;
    /// <summary>
    /// 上月排行
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
    /// 索引
    /// </summary>
    public int index;
    /// <summary>
    /// 唯一ID
    /// </summary>
    public string open_id;
    /// <summary>
    /// 昵称
    /// </summary>
    public string nick;
    /// <summary>
    /// 头像
    /// </summary>
    public string avatar;
    /// <summary>
    /// 本周排行榜(不一定会有值 用排行榜的索引做值)
    /// </summary>
    public int rank;
    /// <summary>
    /// 总积分(本周积分)
    /// </summary>
    public int score_total;
    /// <summary>
    /// 日总积分
    /// </summary>
    public int score_total_day;
    /// <summary>
    /// 月总积分
    /// </summary>
    public int score_total_month;
    /// <summary>
    /// 上周排名
    /// </summary>
    public int last_week_rank;
    /// <summary>
    /// 昨日排名
    /// </summary>
    public int last_day_rank;
    /// <summary>
    /// 上月排名
    /// </summary>
    public int last_month_rank;
    /// <summary>
    /// 上周积分
    /// </summary>
    public int last_week_score;
    /// <summary>
    /// 昨日积分
    /// </summary>
    public int last_day_score;
    /// <summary>
    /// 上月积分
    /// </summary>
    public int last_month_score;
    /// <summary>
    /// 最高波数
    /// </summary>
    public int highest_wave;
    /// <summary>
    /// 共享列表
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
    /// 结算上传分数
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
    /// 请求某位玩家数据
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
    /// 请求前100名玩家 用于入场效果
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
    /// 日榜 周榜 月榜
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
            Debug.LogError("获取排行榜错误");
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
    /// 主播帮
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

        //is_gift 1开 0关
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
