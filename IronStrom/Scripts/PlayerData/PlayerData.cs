using DashGame;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerData
{
    //索引
    public int m_Index;
    //唯一ID
    public string m_Open_ID;
    //玩家EntityID,作为DOTS中的唯一ID
    public Entity m_Entity_ID;
    //昵称
    public string m_Nick;
    //头像
    public string m_Avatar;
    //世界排名
    public int m_Rank;
    //队伍
    public layer m_Team;
    //礼物积分
    public int m_GiftScore;
    //伤害积分
    public float m_ATScore;
    //攻击命令的敌方玩家
    public string m_shootTarget;
    //攻击方式
    public commandType m_comdType;
    //玩家总音浪
    public int m_TotalVoiceWave;
    //音浪技能名字
    public SceneBombType m_sceneBombType;
    //-------------------------------
    //总积分
    public int m_Score_total;
    //日总积分
    public int m_Score_total_day;
    //月总积分
    public int m_Score_total_month;
    //上周排名
    public int m_Last_week_rank;
    //周日排名
    public int m_Last_day_rank;
    //上月排名
    public int m_Last_month_rank;
    //上周积分
    public int m_Last_week_score;
    //昨日积分
    public int m_Last_day_score;
    //上月积分 
    public int m_Last_month_score;
    //共享链表
    public List<string> m_kindList;
    //-------------------------------

    //礼物士兵
    public List<Entity> m_GiftShiBingList;
    //点赞士兵
    public List<Entity> m_LikeShiBingList;
    //礼物的累积个数
    public Dictionary<string, int> m_GiftNumDic;

    public PlayerData()
    {
        m_GiftShiBingList = new List<Entity>();
        m_LikeShiBingList = new List<Entity>();
        m_GiftNumDic = new Dictionary<string, int>();
        m_TotalVoiceWave = 0;
        m_sceneBombType = SceneBombType.Shile;
    }
    //初始化Json文件
    public void InitData(int index, JsonData json)
    {
        if (json.Equals("")) return;
        this.m_Index = index;
        m_Open_ID = JsonUtil.ToString(json, "open_id");
        if (json["nick"] != null)
            m_Nick = JsonUtil.ToString(json, "nick");
        if (json["avatar"] != null)
            m_Avatar = JsonUtil.ToString(json, "avatar");
        m_Rank = JsonUtil.ToInt(json, "rank");
        m_Score_total = JsonUtil.ToInt(json, "score_total");
        m_Score_total_day = JsonUtil.ToInt(json, "score_total_day");
        m_Score_total_month = JsonUtil.ToInt(json, "score_total_month");
        m_Last_week_rank = JsonUtil.ToInt(json, "last_week_rank");
        m_Last_day_rank = JsonUtil.ToInt(json, "last_day_rank");
        m_Last_month_rank = JsonUtil.ToInt(json, "last_month_rank");
        m_Last_week_score = JsonUtil.ToInt(json, "last_week_score");
        m_Last_day_score = JsonUtil.ToInt(json, "last_day_score");
        m_Last_month_score = JsonUtil.ToInt(json, "last_month_score");
        //highest_wave = JsonUtil.ToInt(json, "highest_wave");
        m_kindList = JsonUtil.ToStringList(json, "kind");
    }

}
