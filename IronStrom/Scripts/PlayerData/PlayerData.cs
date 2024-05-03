using DashGame;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerData
{
    //����
    public int m_Index;
    //ΨһID
    public string m_Open_ID;
    //���EntityID,��ΪDOTS�е�ΨһID
    public Entity m_Entity_ID;
    //�ǳ�
    public string m_Nick;
    //ͷ��
    public string m_Avatar;
    //��������
    public int m_Rank;
    //����
    public layer m_Team;
    //�������
    public int m_GiftScore;
    //�˺�����
    public float m_ATScore;
    //��������ĵз����
    public string m_shootTarget;
    //������ʽ
    public commandType m_comdType;
    //���������
    public int m_TotalVoiceWave;
    //���˼�������
    public SceneBombType m_sceneBombType;
    //-------------------------------
    //�ܻ���
    public int m_Score_total;
    //���ܻ���
    public int m_Score_total_day;
    //���ܻ���
    public int m_Score_total_month;
    //��������
    public int m_Last_week_rank;
    //��������
    public int m_Last_day_rank;
    //��������
    public int m_Last_month_rank;
    //���ܻ���
    public int m_Last_week_score;
    //���ջ���
    public int m_Last_day_score;
    //���»��� 
    public int m_Last_month_score;
    //��������
    public List<string> m_kindList;
    //-------------------------------

    //����ʿ��
    public List<Entity> m_GiftShiBingList;
    //����ʿ��
    public List<Entity> m_LikeShiBingList;
    //������ۻ�����
    public Dictionary<string, int> m_GiftNumDic;

    public PlayerData()
    {
        m_GiftShiBingList = new List<Entity>();
        m_LikeShiBingList = new List<Entity>();
        m_GiftNumDic = new Dictionary<string, int>();
        m_TotalVoiceWave = 0;
        m_sceneBombType = SceneBombType.Shile;
    }
    //��ʼ��Json�ļ�
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
