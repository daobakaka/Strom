using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Linq;

public class RandomCard
{
    public string m_ImagePath;
    public string m_Name;
    public int m_Num;
}
//传给DOTS，JiDiSystem的玩家选择卡牌数据
public class RandomCardData
{
    public RandomCard m_RandomCard;
    public layer m_Team;
}

public class CardManager : MonoBehaviour
{
    private static CardManager _CardManager;
    public static CardManager Instance { get { return _CardManager; } }

    [Tooltip("卡牌出现时间")] public float CardAppearTime;
    public float Cur_CardAppearTime;
    [Tooltip("卡牌存在时间")] public float CardCountdown;
    private float Cur_CardCountdown;
    [Tooltip("展示的卡牌数量")] public int ShowCardNum;

    public RandomCardData Team1_randomCardData;//传给DOTS，JiDiSystem的玩家选择卡牌数据
    public RandomCardData Team2_randomCardData;//传给DOTS，JiDiSystem的玩家选择卡牌数据


    private List<RandomCard> AllCard;//所有卡牌
    public List<RandomCard> randomCard;//随机卡牌
    public Dictionary<string, int> Team1_PlayerSelect;//记录玩家的选择
    public Dictionary<string, int> Team2_PlayerSelect;//记录玩家的选择
    private string PlayerSelectNum;//玩家选择的数字;

    private BasePanel m_CardPanel;//拿到卡牌面板



    private bool Is_Carding;//卡牌是否在展示中
    private void Awake()
    {
        _CardManager = this;
        Is_Carding = false;
        Team1_randomCardData = null;
        Team2_randomCardData = null;
        Cur_CardAppearTime = CardAppearTime;
        Cur_CardCountdown = CardCountdown;
        AllCard = new List<RandomCard>();
        randomCard = new List<RandomCard>();
        Team1_PlayerSelect = new Dictionary<string, int>();
        Team2_PlayerSelect = new Dictionary<string, int>();
        PlayerSelectNum = "";

        RandomCard randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "01野马战车";
        randomCardTemp.m_Name = "野马战车";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "02尖牙";
        randomCardTemp.m_Name = "尖牙";
        randomCardTemp.m_Num = 50;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "03激光战车";
        randomCardTemp.m_Name = "激光战车";
        randomCardTemp.m_Num = 10;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "04暴雨战车";
        randomCardTemp.m_Name = "暴雨战车";
        randomCardTemp.m_Num = 6;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "05霸主战舰";
        randomCardTemp.m_Name = "霸主战舰";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "06星际工厂";
        randomCardTemp.m_Name = "星际工厂";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "07长弓机甲";
        randomCardTemp.m_Name = "长弓机甲";
        randomCardTemp.m_Num = 10;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "08蜂王战机";
        randomCardTemp.m_Name = "蜂王战机";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "09凤凰战机";
        randomCardTemp.m_Name = "凤凰战机";
        randomCardTemp.m_Num = 5;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "10黑客";
        randomCardTemp.m_Name = "黑客";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "11弧光机甲";
        randomCardTemp.m_Name = "弧光机甲";
        randomCardTemp.m_Num = 30;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "12狂蝎战车";
        randomCardTemp.m_Name = "狂蝎战车";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "13堡垒机甲";
        randomCardTemp.m_Name = "堡垒机甲";
        randomCardTemp.m_Num = 3;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "14铁锤战车";
        randomCardTemp.m_Name = "铁锤战车";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "15激光电磁炮";
        randomCardTemp.m_Name = "激光电磁炮";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "17火神机甲";
        randomCardTemp.m_Name = "火神机甲";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);


        //玩家选择
        for (int i = 1; i <= ShowCardNum; ++i)
        {
            Team1_PlayerSelect.Add(i.ToString(), 0);
            Team2_PlayerSelect.Add(i.ToString(), 0);
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CardAppear();
        StatisticalSelection();
        InuptCard();

    }

    //卡牌出现倒计时
    void CardAppear()
    {
        if (Is_Carding)//卡牌出现就退出
            return;
        Cur_CardAppearTime -= Time.deltaTime;
        if (Cur_CardAppearTime > 0)
            return;
        else
            Cur_CardAppearTime = CardAppearTime;


        SelectRandomCard();

    }

    //随机选择六个卡牌,实例化随机卡牌Panel
    void SelectRandomCard()
    {
        System.Random rnd = new System.Random();
        // 如果AllCard列表中至少有六个元素
        if (AllCard.Count >= ShowCardNum)
        {
            // 创建一个随机卡牌的临时列表，以避免修改原始列表
            List<RandomCard> tempAllCard = new List<RandomCard>(AllCard);
            // 随机选择六次卡牌
            for (int i = 0; i < ShowCardNum; i++)
            {
                // 从0到tempAllCard.Count - 1之间随机选择一个索引
                int randomIndex = rnd.Next(tempAllCard.Count);
                // 添加选中的卡牌到randomCard列表
                randomCard.Add(tempAllCard[randomIndex]);
                // 从临时列表中移除已经选中的卡牌
                tempAllCard.RemoveAt(randomIndex);
            }

            //创建拿到卡牌面板
            PanelManager panelManager = PanelManager.instance;//new PanelManager();
            m_CardPanel = new CardPanel();
            panelManager.Push(m_CardPanel);
            Is_Carding = true;
            Debug.Log($"  创建CardPanel面板{m_CardPanel}");
        }
        else 
        {
            Debug.Log($" 随机卡牌数量不够{ShowCardNum}个");
        }

    }

    //记录玩家按键
    void InuptCard()
    {
        if (Is_Carding == false)//卡牌没有出现就退出
            return;

        if(Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerSelectNum = "1";
        }
        else if(Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerSelectNum = "2";
        }
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerSelectNum = "3";
        }
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerSelectNum = "4";
        }
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerSelectNum = "5";
        }
        else if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayerSelectNum = "6";
        }
        Debug.Log($"  队伍1玩家按下{PlayerSelectNum}");
        if(Team1_PlayerSelect.ContainsKey(PlayerSelectNum))
            Team1_PlayerSelect[PlayerSelectNum] += 1;
        PlayerSelectNum = "";
        if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerSelectNum = "1";
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerSelectNum = "2";
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerSelectNum = "3";
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerSelectNum = "4";
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerSelectNum = "5";
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayerSelectNum = "6";
        }
        Debug.Log($"  队伍2玩家按下{PlayerSelectNum}");
        if (Team2_PlayerSelect.ContainsKey(PlayerSelectNum))
            Team2_PlayerSelect[PlayerSelectNum] += 1;
        PlayerSelectNum = "";

    }

    //卡牌出现倒计时，统计玩家选择
    void StatisticalSelection()
    {
        if (Is_Carding == false)//卡牌没有出现就退出
            return;
        Cur_CardCountdown -= Time.deltaTime;
        if (Cur_CardCountdown > 0)
            return;
        else
            Cur_CardCountdown = CardCountdown;

        //统计玩家的选择
        if(Team1_PlayerSelect.Count > 0)
        {
            layer team = layer.Team1;
            Team1_randomCardData = new RandomCardData();
            LINQCard(Team1_PlayerSelect, in team, ref Team1_randomCardData);
        }
        if(Team2_PlayerSelect.Count > 0)
        {
            layer team = layer.Team2;
            Team2_randomCardData = new RandomCardData();
            LINQCard(Team2_PlayerSelect, in team, ref Team2_randomCardData);
        }

        randomCard.Clear();
        m_CardPanel.Pop();//选择时间结束删除面板
        Is_Carding = false;
    }

    //打包玩家选择的卡牌数据,准备给JiDiSystem用来实例化士兵 
    void LINQCard(Dictionary<string, int> playerSelect,in layer team,ref RandomCardData randCardData)
    {
        Debug.Log($"  队伍{team}的选择为。1: {playerSelect["1"]}票\n" +
            $"2: {playerSelect["2"]}票\n" +
            $"3: {playerSelect["3"]}票\n" +
            $"4: {playerSelect["4"]}票\n" +
            $"5: {playerSelect["5"]}票\n" +
            $"6: {playerSelect["6"]}票\n");
        // 使用LINQ找出最大的int值
        var maxEntry = playerSelect.OrderByDescending(entry => entry.Value).First();
        string keyWithMaxValue = maxEntry.Key; // 拥有最大int值的键
        int maxValue = maxEntry.Value; // 最大的int值

        bool b = int.TryParse(keyWithMaxValue, out int key);
        if (b && key <= ShowCardNum)
        {
            Debug.Log($"  key为：{key - 1}");
            var randomcard = new RandomCard();
            randomcard.m_ImagePath = randomCard[key - 1].m_ImagePath;
            randomcard.m_Num = randomCard[key - 1].m_Num;
            randCardData.m_RandomCard = randomcard;
            randCardData.m_Team = team;
        }
        else
        {
            Debug.Log("  卡牌下标越界");
        }


        foreach(var pair in playerSelect.ToList())
        {
            playerSelect[pair.Key] = 0;
        }
    }


}
