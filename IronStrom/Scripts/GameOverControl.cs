using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverControl : MonoBehaviour
{
    private static GameOverControl _GameOverControl;
    public static GameOverControl intance { get { return _GameOverControl; } }

    
    public List<PlayerData> LocalScoreList;//���ַ���
    public List<PlayerData> Day_RankList;//������
    public List<PlayerData> Week_RankList;//������
    public List<PlayerData> Month_RankList;//������


    private void Awake()
    {
        _GameOverControl = this;

        LocalScoreList = new List<PlayerData>();
        Day_RankList = new List<PlayerData>();
        Week_RankList = new List<PlayerData>();
        Month_RankList = new List<PlayerData>();

    }

    //��Ϸ����ʱ�ϴ��������������ֻ�������
    public void GameOver(WinTeam winteam)
    {
        JsonData json = JsonMapper.ToObject("[]");
        var playerAll = TeamManager.teamManager.GetAllPlayer();
        foreach (var item in playerAll)
        {
            JsonData data = new JsonData();
            data["open_id"] = item.m_Open_ID;
            data["nick"] = item.m_Nick;
            data["avatar"] = item.m_Avatar;

            if (item.m_Team == layer.Team1 && winteam == WinTeam.Team1)
                data["win"] = 1;
            else if (item.m_Team == layer.Team2 && winteam == WinTeam.Team2)
                data["win"] = 0;

            json.Add(data);
        }
        //�ϴ�����������õ��ַ�������
        XwfRequest.Record(json).ResultEvent.AddListener((bool success, JsonData resData) =>
        {
            if (success)
            {
                //�ɹ��ص�
                for (int i = 0; i < resData.Count; i++)
                {
                    //LocalScoreList.Add(new RankData(i + 1, resData[i]));
                    var playerdata = new PlayerData();
                    playerdata.InitData(i + 1, resData[i]);
                    LocalScoreList.Add(playerdata);
                }
            }
            else
            {
                DelayCall.Call(() =>
                {
                   //ʧ�ܻص�
                }, 5f);
            }
        });
    }

    //�������������
    public void Get_Rank(RankType rankType)
    {
        XwfRequest.GetLeaders(rankType).ResultEvent.AddListener((bool success, JsonData resData) =>
        {
            if(success)
            {
                switch(rankType)
                {
                    case RankType.Daily:
                        for (int i = 0; i < resData.Count; ++i)
                        {
                            var playerdata = new PlayerData();
                            playerdata.InitData(i + 1, resData[i]);
                            Day_RankList.Add(playerdata);
                        } break;
                    case RankType.Weekly:
                        for (int i = 0; i < resData.Count; ++i)
                        {
                            var playerdata = new PlayerData();
                            playerdata.InitData(i + 1, resData[i]);
                            Week_RankList.Add(playerdata);
                        } break;
                    case RankType.Monthly:
                        for (int i = 0; i < resData.Count; ++i)
                        {
                            var playerdata = new PlayerData();
                            playerdata.InitData(i + 1, resData[i]);
                            Month_RankList.Add(playerdata);
                        }break;
                }
            }
            else
            {
                DelayCall.Call(() => { /*ʧ�ܻص�*/ }, 5f);
            }
        });
    }


    //�����Ѿ�����˷�����������
    public void InitPlayerRankingData()
    {
        //LocalScoreList.Clear();
        //Day_RankList.Clear();
        //Week_RankList.Clear();
        //Month_RankList.Clear();


        //LocalScoreList
        //var player = new PlayerData();
        //player.m_Open_ID = "OpenID_1_1";
        //player.m_Nick = "��ķ";
        //player.m_Avatar = "UI/Image/��ķͷ��";
        //player.m_Rank = 1;

        //Day_RankList.Add()

        LocalScoreList = TeamManager.teamManager.GetAllPlayer();
        LocalScoreList.Sort((x, y) => (y.m_GiftScore + y.m_ATScore).CompareTo(x.m_GiftScore + x.m_ATScore));

        var playerList = TeamManager.teamManager.GetAllPlayer();
        foreach(var playerdata in playerList)
        {
            playerdata.m_Score_total_day = Random.Range(0,100000);
            playerdata.m_Score_total_month = Random.Range(0, 100000);
            playerdata.m_Last_day_rank = Random.Range(0, 100000);
        }
        Day_RankList = new List<PlayerData>(playerList);
        Day_RankList.Sort((x, y) => y.m_Score_total_day.CompareTo(x.m_Score_total_day));
        Week_RankList = new List<PlayerData>(playerList);
        Week_RankList.Sort((x, y) => y.m_Last_day_rank.CompareTo(x.m_Last_day_rank));
        Month_RankList = new List<PlayerData>(playerList);
        Month_RankList.Sort((x, y) => y.m_Score_total_month.CompareTo(x.m_Score_total_month));

    }

}
