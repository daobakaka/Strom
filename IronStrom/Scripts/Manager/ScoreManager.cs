using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _ScoreManager;
    public static ScoreManager instance { get { return _ScoreManager; } }

    [HideInInspector] public float TotalScore;//总积分
    [HideInInspector] public TextMeshProUGUI TotalScoreText;

    public PlayerData[] Team1_ScoreRankingArr;//队伍1的积分排名
    public PlayerData[] Team2_ScoreRankingArr;//队伍2的积分排名

    [Tooltip("队伍1当局积分排名")] public GameObject Team1_ScoreRankingObj;
    [Tooltip("队伍2当局积分排名")] public GameObject Team2_ScoreRankingObj;



    private void Awake()
    {
        _ScoreManager = this;
        Team1_ScoreRankingArr = new PlayerData[3];
        for (int i = 0; i < 3; ++i)
            Team1_ScoreRankingArr[i] = new PlayerData();
        Team2_ScoreRankingArr = new PlayerData[3];
        for (int i = 0; i < 3; ++i)
            Team2_ScoreRankingArr[i] = new PlayerData();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //统计所有玩家的总积分
        StatisticsScore();
        //将积分排名显示在UI上
        PlayerScoreToUI();
        //将队伍1 2进行排序拿出前三名
        SortAndPickTopPlayers();

    }

    //统计所有玩家的总积分
    void StatisticsScore()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null || TotalScoreText == null) return;
        TotalScore = 0;
        foreach (var pair in teamMager._Dic_Team1)
        {
            TotalScore += pair.Value.m_GiftScore;
            TotalScore += pair.Value.m_ATScore;
            PlayerData player = pair.Value;
            //CountCurrentRanking(in player, ref Team1_ScoreRankingArr);
        }
        foreach (var pair in teamMager._Dic_Team2)
        {
            TotalScore += pair.Value.m_GiftScore;
            TotalScore += pair.Value.m_ATScore;
            PlayerData player = pair.Value;
            //CountCurrentRanking(in player, ref Team2_ScoreRankingArr);
        }

        TotalScoreText.text = (TotalScore / 10000f).ToString("F2") + " 万";
        //Debug.Log($"  队伍1排名为：1{Team1_ScoreRankingArr[0].m_Nick}.2{Team1_ScoreRankingArr[1].m_Nick}.3{Team1_ScoreRankingArr[2].m_Nick}.");
    }
    //计算当前积分排名
    void CountCurrentRanking(in PlayerData player, ref PlayerData[] ScoreRanking)
    {
        float playerScore = player.m_ATScore + player.m_GiftScore;// 新玩家的总分
        int insertIndex = ScoreRanking.Length;// 找到新玩家应该插入的位置
        for (int i = 0; i < ScoreRanking.Length; ++i)
        {
            // 确保不会对null的ScoreRanking[i]进行操作
            if (ScoreRanking[i] == null)
            {
                insertIndex = i; // 新玩家应该插入的位置
                break; // 找到位置，跳出循环
            }
            // 现有玩家的总分
            float existingPlayerScore = ScoreRanking[i].m_ATScore + ScoreRanking[i].m_GiftScore;
            if (playerScore > existingPlayerScore)
            {
                insertIndex = i;// 新玩家应该插入的位置
                break; // 找到位置，跳出循环
            }
        }
        // 如果新玩家不是最低分，则需要移动数组中的元素以腾出空间
        if (insertIndex < ScoreRanking.Length)
        {
            // 从排名列表的末尾开始，将玩家数据向后移动一位，为新玩家腾出空间
            for (int i = ScoreRanking.Length - 1; i > insertIndex; --i)
                ScoreRanking[i] = ScoreRanking[i - 1];
            // 插入新玩家的数据
            ScoreRanking[insertIndex] = player;
        }
    }

    //将队伍1 2进行排序拿出前三名
    void SortAndPickTopPlayers()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        // 将玩家数据（KeyValuePair<TKey, TValue>类型）添加到一个新列表中
        var playersList = teamMager._Dic_Team1.Values.ToList();
        // 按玩家的总分从高到低进行排序
        var sortedPlayers = playersList.OrderByDescending(player => player.m_ATScore + player.m_GiftScore).ToList();
        for (int i = 0; i < sortedPlayers.Count; ++i)
            Team1_ScoreRankingArr[i] = sortedPlayers[i];
       


        // 将玩家数据（KeyValuePair<TKey, TValue>类型）添加到一个新列表中
        playersList = teamMager._Dic_Team2.Values.ToList();
        // 按玩家的总分从高到低进行排序
        sortedPlayers = playersList.OrderByDescending(player => player.m_ATScore + player.m_GiftScore).ToList();
        for (int i = 0; i < sortedPlayers.Count; ++i)
            Team2_ScoreRankingArr[i] = sortedPlayers[i];
    }
    //将积分排名显示在UI上
    void PlayerScoreToUI()
    {
        ScoreData scoreData = Team1_ScoreRankingObj.GetComponent<ScoreData>();
        if(scoreData != null)
        {
            if (string.IsNullOrWhiteSpace(Team1_ScoreRankingArr[0].m_Nick))
                scoreData.Ranking1.SetActive(false);
            else
            {
                scoreData.Ranking1.SetActive(true);
                scoreData.Num1_Headshot.sprite = Resources.Load<Sprite>(Team1_ScoreRankingArr[0].m_Avatar);
                scoreData.Num1_PlayerName.text = Team1_ScoreRankingArr[0].m_Nick;
            }

            if (string.IsNullOrWhiteSpace(Team1_ScoreRankingArr[1].m_Nick))
                scoreData.Ranking2.SetActive(false);
            else
            {
                scoreData.Ranking2.SetActive(true);
                scoreData.Num2_Headshot.sprite = Resources.Load<Sprite>(Team1_ScoreRankingArr[1].m_Avatar);
                scoreData.Num2_PlayerName.text = Team1_ScoreRankingArr[1].m_Nick;
            }

            if (string.IsNullOrWhiteSpace(Team1_ScoreRankingArr[2].m_Nick))
                scoreData.Ranking3.SetActive(false);
            else
            {
                scoreData.Ranking3.SetActive(true);
                scoreData.Num3_Headshot.sprite = Resources.Load<Sprite>(Team1_ScoreRankingArr[2].m_Avatar);
                scoreData.Num3_PlayerName.text = Team1_ScoreRankingArr[2].m_Nick;
            }
        }

        scoreData = Team2_ScoreRankingObj.GetComponent<ScoreData>();
        if(scoreData != null)
        {
            if(string.IsNullOrWhiteSpace(Team2_ScoreRankingArr[0].m_Nick))
                scoreData.Ranking1.SetActive(false);
            else
            {
                scoreData.Ranking1.SetActive(true);
                scoreData.Num1_Headshot.sprite = Resources.Load<Sprite>(Team2_ScoreRankingArr[0].m_Avatar);
                scoreData.Num1_PlayerName.text = Team2_ScoreRankingArr[0].m_Nick;
            }

            if (string.IsNullOrWhiteSpace(Team2_ScoreRankingArr[1].m_Nick))
                scoreData.Ranking2.SetActive(false);
            else
            {
                scoreData.Ranking2.SetActive(true);
                scoreData.Num2_Headshot.sprite = Resources.Load<Sprite>(Team2_ScoreRankingArr[1].m_Avatar);
                scoreData.Num2_PlayerName.text = Team2_ScoreRankingArr[1].m_Nick;
            }
            if (string.IsNullOrWhiteSpace(Team2_ScoreRankingArr[2].m_Nick))
                scoreData.Ranking3.SetActive(false);
            else
            {
                scoreData.Ranking3.SetActive(true);
                scoreData.Num3_Headshot.sprite = Resources.Load<Sprite>(Team2_ScoreRankingArr[2].m_Avatar);
                scoreData.Num3_PlayerName.text = Team2_ScoreRankingArr[2].m_Nick;
            }

        }
    }
    
}
