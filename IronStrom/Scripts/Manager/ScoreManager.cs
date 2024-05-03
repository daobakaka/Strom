using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _ScoreManager;
    public static ScoreManager instance { get { return _ScoreManager; } }

    [HideInInspector] public float TotalScore;//�ܻ���
    [HideInInspector] public TextMeshProUGUI TotalScoreText;

    public PlayerData[] Team1_ScoreRankingArr;//����1�Ļ�������
    public PlayerData[] Team2_ScoreRankingArr;//����2�Ļ�������

    [Tooltip("����1���ֻ�������")] public GameObject Team1_ScoreRankingObj;
    [Tooltip("����2���ֻ�������")] public GameObject Team2_ScoreRankingObj;



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
        //ͳ��������ҵ��ܻ���
        StatisticsScore();
        //������������ʾ��UI��
        PlayerScoreToUI();
        //������1 2���������ó�ǰ����
        SortAndPickTopPlayers();

    }

    //ͳ��������ҵ��ܻ���
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

        TotalScoreText.text = (TotalScore / 10000f).ToString("F2") + " ��";
        //Debug.Log($"  ����1����Ϊ��1{Team1_ScoreRankingArr[0].m_Nick}.2{Team1_ScoreRankingArr[1].m_Nick}.3{Team1_ScoreRankingArr[2].m_Nick}.");
    }
    //���㵱ǰ��������
    void CountCurrentRanking(in PlayerData player, ref PlayerData[] ScoreRanking)
    {
        float playerScore = player.m_ATScore + player.m_GiftScore;// ����ҵ��ܷ�
        int insertIndex = ScoreRanking.Length;// �ҵ������Ӧ�ò����λ��
        for (int i = 0; i < ScoreRanking.Length; ++i)
        {
            // ȷ�������null��ScoreRanking[i]���в���
            if (ScoreRanking[i] == null)
            {
                insertIndex = i; // �����Ӧ�ò����λ��
                break; // �ҵ�λ�ã�����ѭ��
            }
            // ������ҵ��ܷ�
            float existingPlayerScore = ScoreRanking[i].m_ATScore + ScoreRanking[i].m_GiftScore;
            if (playerScore > existingPlayerScore)
            {
                insertIndex = i;// �����Ӧ�ò����λ��
                break; // �ҵ�λ�ã�����ѭ��
            }
        }
        // �������Ҳ�����ͷ֣�����Ҫ�ƶ������е�Ԫ�����ڳ��ռ�
        if (insertIndex < ScoreRanking.Length)
        {
            // �������б��ĩβ��ʼ���������������ƶ�һλ��Ϊ������ڳ��ռ�
            for (int i = ScoreRanking.Length - 1; i > insertIndex; --i)
                ScoreRanking[i] = ScoreRanking[i - 1];
            // ��������ҵ�����
            ScoreRanking[insertIndex] = player;
        }
    }

    //������1 2���������ó�ǰ����
    void SortAndPickTopPlayers()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        // ��������ݣ�KeyValuePair<TKey, TValue>���ͣ���ӵ�һ�����б���
        var playersList = teamMager._Dic_Team1.Values.ToList();
        // ����ҵ��ִܷӸߵ��ͽ�������
        var sortedPlayers = playersList.OrderByDescending(player => player.m_ATScore + player.m_GiftScore).ToList();
        for (int i = 0; i < sortedPlayers.Count; ++i)
            Team1_ScoreRankingArr[i] = sortedPlayers[i];
       


        // ��������ݣ�KeyValuePair<TKey, TValue>���ͣ���ӵ�һ�����б���
        playersList = teamMager._Dic_Team2.Values.ToList();
        // ����ҵ��ִܷӸߵ��ͽ�������
        sortedPlayers = playersList.OrderByDescending(player => player.m_ATScore + player.m_GiftScore).ToList();
        for (int i = 0; i < sortedPlayers.Count; ++i)
            Team2_ScoreRankingArr[i] = sortedPlayers[i];
    }
    //������������ʾ��UI��
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
