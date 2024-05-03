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
//����DOTS��JiDiSystem�����ѡ��������
public class RandomCardData
{
    public RandomCard m_RandomCard;
    public layer m_Team;
}

public class CardManager : MonoBehaviour
{
    private static CardManager _CardManager;
    public static CardManager Instance { get { return _CardManager; } }

    [Tooltip("���Ƴ���ʱ��")] public float CardAppearTime;
    public float Cur_CardAppearTime;
    [Tooltip("���ƴ���ʱ��")] public float CardCountdown;
    private float Cur_CardCountdown;
    [Tooltip("չʾ�Ŀ�������")] public int ShowCardNum;

    public RandomCardData Team1_randomCardData;//����DOTS��JiDiSystem�����ѡ��������
    public RandomCardData Team2_randomCardData;//����DOTS��JiDiSystem�����ѡ��������


    private List<RandomCard> AllCard;//���п���
    public List<RandomCard> randomCard;//�������
    public Dictionary<string, int> Team1_PlayerSelect;//��¼��ҵ�ѡ��
    public Dictionary<string, int> Team2_PlayerSelect;//��¼��ҵ�ѡ��
    private string PlayerSelectNum;//���ѡ�������;

    private BasePanel m_CardPanel;//�õ��������



    private bool Is_Carding;//�����Ƿ���չʾ��
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
        randomCardTemp.m_ImagePath = "01Ұ��ս��";
        randomCardTemp.m_Name = "Ұ��ս��";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "02����";
        randomCardTemp.m_Name = "����";
        randomCardTemp.m_Num = 50;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "03����ս��";
        randomCardTemp.m_Name = "����ս��";
        randomCardTemp.m_Num = 10;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "04����ս��";
        randomCardTemp.m_Name = "����ս��";
        randomCardTemp.m_Num = 6;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "05����ս��";
        randomCardTemp.m_Name = "����ս��";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "06�Ǽʹ���";
        randomCardTemp.m_Name = "�Ǽʹ���";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "07��������";
        randomCardTemp.m_Name = "��������";
        randomCardTemp.m_Num = 10;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "08����ս��";
        randomCardTemp.m_Name = "����ս��";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "09���ս��";
        randomCardTemp.m_Name = "���ս��";
        randomCardTemp.m_Num = 5;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "10�ڿ�";
        randomCardTemp.m_Name = "�ڿ�";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "11�������";
        randomCardTemp.m_Name = "�������";
        randomCardTemp.m_Num = 30;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "12��Ыս��";
        randomCardTemp.m_Name = "��Ыս��";
        randomCardTemp.m_Num = 20;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "13���ݻ���";
        randomCardTemp.m_Name = "���ݻ���";
        randomCardTemp.m_Num = 3;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "14����ս��";
        randomCardTemp.m_Name = "����ս��";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "15��������";
        randomCardTemp.m_Name = "��������";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);

        randomCardTemp = new RandomCard();
        randomCardTemp.m_ImagePath = "17�������";
        randomCardTemp.m_Name = "�������";
        randomCardTemp.m_Num = 1;
        AllCard.Add(randomCardTemp);


        //���ѡ��
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

    //���Ƴ��ֵ���ʱ
    void CardAppear()
    {
        if (Is_Carding)//���Ƴ��־��˳�
            return;
        Cur_CardAppearTime -= Time.deltaTime;
        if (Cur_CardAppearTime > 0)
            return;
        else
            Cur_CardAppearTime = CardAppearTime;


        SelectRandomCard();

    }

    //���ѡ����������,ʵ�����������Panel
    void SelectRandomCard()
    {
        System.Random rnd = new System.Random();
        // ���AllCard�б�������������Ԫ��
        if (AllCard.Count >= ShowCardNum)
        {
            // ����һ��������Ƶ���ʱ�б��Ա����޸�ԭʼ�б�
            List<RandomCard> tempAllCard = new List<RandomCard>(AllCard);
            // ���ѡ�����ο���
            for (int i = 0; i < ShowCardNum; i++)
            {
                // ��0��tempAllCard.Count - 1֮�����ѡ��һ������
                int randomIndex = rnd.Next(tempAllCard.Count);
                // ���ѡ�еĿ��Ƶ�randomCard�б�
                randomCard.Add(tempAllCard[randomIndex]);
                // ����ʱ�б����Ƴ��Ѿ�ѡ�еĿ���
                tempAllCard.RemoveAt(randomIndex);
            }

            //�����õ��������
            PanelManager panelManager = PanelManager.instance;//new PanelManager();
            m_CardPanel = new CardPanel();
            panelManager.Push(m_CardPanel);
            Is_Carding = true;
            Debug.Log($"  ����CardPanel���{m_CardPanel}");
        }
        else 
        {
            Debug.Log($" ���������������{ShowCardNum}��");
        }

    }

    //��¼��Ұ���
    void InuptCard()
    {
        if (Is_Carding == false)//����û�г��־��˳�
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
        Debug.Log($"  ����1��Ұ���{PlayerSelectNum}");
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
        Debug.Log($"  ����2��Ұ���{PlayerSelectNum}");
        if (Team2_PlayerSelect.ContainsKey(PlayerSelectNum))
            Team2_PlayerSelect[PlayerSelectNum] += 1;
        PlayerSelectNum = "";

    }

    //���Ƴ��ֵ���ʱ��ͳ�����ѡ��
    void StatisticalSelection()
    {
        if (Is_Carding == false)//����û�г��־��˳�
            return;
        Cur_CardCountdown -= Time.deltaTime;
        if (Cur_CardCountdown > 0)
            return;
        else
            Cur_CardCountdown = CardCountdown;

        //ͳ����ҵ�ѡ��
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
        m_CardPanel.Pop();//ѡ��ʱ�����ɾ�����
        Is_Carding = false;
    }

    //������ѡ��Ŀ�������,׼����JiDiSystem����ʵ����ʿ�� 
    void LINQCard(Dictionary<string, int> playerSelect,in layer team,ref RandomCardData randCardData)
    {
        Debug.Log($"  ����{team}��ѡ��Ϊ��1: {playerSelect["1"]}Ʊ\n" +
            $"2: {playerSelect["2"]}Ʊ\n" +
            $"3: {playerSelect["3"]}Ʊ\n" +
            $"4: {playerSelect["4"]}Ʊ\n" +
            $"5: {playerSelect["5"]}Ʊ\n" +
            $"6: {playerSelect["6"]}Ʊ\n");
        // ʹ��LINQ�ҳ�����intֵ
        var maxEntry = playerSelect.OrderByDescending(entry => entry.Value).First();
        string keyWithMaxValue = maxEntry.Key; // ӵ�����intֵ�ļ�
        int maxValue = maxEntry.Value; // ����intֵ

        bool b = int.TryParse(keyWithMaxValue, out int key);
        if (b && key <= ShowCardNum)
        {
            Debug.Log($"  keyΪ��{key - 1}");
            var randomcard = new RandomCard();
            randomcard.m_ImagePath = randomCard[key - 1].m_ImagePath;
            randomcard.m_Num = randomCard[key - 1].m_Num;
            randCardData.m_RandomCard = randomcard;
            randCardData.m_Team = team;
        }
        else
        {
            Debug.Log("  �����±�Խ��");
        }


        foreach(var pair in playerSelect.ToList())
        {
            playerSelect[pair.Key] = 0;
        }
    }


}
