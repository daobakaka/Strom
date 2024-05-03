using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;


public enum layer//�㼶
{
    NUll = 0,//��
    Team1 = 1,
    Team2,
    Team1Detection,//����1�ļ��
    Team2Detection,
    Team1Bullet,//�ӵ�
    Team2Bullet,//�ӵ�
    Neutral,//������λ
    Parasitic,//����
    Map,//��ͼ
    Shield,//����
    
}

public enum ActState//��Ϊ״̬
{
    NULL,//��
    Idle,//Ĭ��֮��
    Walk,//Ĭ����ǰ��
    Walk_R,//���߶���
    Walk_L,//���߶���
    Move,//�ƶ�
    Fire,//���
    Ready,//׼���׶�
    Attack,//����
    die,//����
    NotMove,//����
    Appear,//����
}

public enum WinTeam//��ʤ����
{ 
    Null,
    Team1,
    Team2,
}


public class TeamManager : MonoBehaviour
{
    private static TeamManager _TeamManager;
    public static TeamManager teamManager { get { return _TeamManager; } }


    public float Team1_JiDiHP;
    public float Team2_JiDiHP;
    public Slider Team1_JiDiHPSlider;
    public Slider Team2_JiDiHPSlider;
    public TextMeshProUGUI Team1_JiDiHPText;
    public TextMeshProUGUI Team2_JiDiHPText;
    public Entity Team1_JiDiEntity;
    public Entity Team2_JiDiEntity;

    [Tooltip("ʿ�������ʱ��")] public int DetectionNum;
    [HideInInspector] public int Cur_DetectionNum;
    [Tooltip("�ӵ������ʱ��")] public float BulletDetectionTime;
    [HideInInspector] public float Cur_BulletDetectionTime;

    //������������
    public Dictionary<string, PlayerData> _Dic_Team1 = new Dictionary<string, PlayerData>();//<OpenID,���>
    public Dictionary<string, PlayerData> _Dic_Team2 = new Dictionary<string, PlayerData>();//<OpenID,���>
    public Dictionary<Entity, string> _Dic_Team1OpenID = new Dictionary<Entity, string>();//<EntityID,OpenID>
    public Dictionary<Entity, string> _Dic_Team2OpenID = new Dictionary<Entity, string>();//<EntityID,OpenID>


    public PlayerData SelectedPlayer;//ѡ�е����
    public GiftType SelectedGift;//ѡ���е����������Զ�����
    public int SelectedGiftNum;//ѡ��������ĸ���
    public bool Is_SelectedAutoGift;//�Ƿ��Զ�����

    [HideInInspector] public int OnGpuEntityNum;//����GPU���ٵ�Entity��λ��
    [HideInInspector] public int NoGpuEntityNum;//û��GPU���ٵ�Entity��λ��
    [HideInInspector] public int UnityAniEntityNum;//Unity������Entity��λ��
    [HideInInspector] public int TotalEntityNum;//�ܹ���Entityʿ����λ��λ��
    [HideInInspector] public int BulletNum;//�ӵ�������
    [HideInInspector] public int AttackBoxNum;//�������Ӹ�����


    [HideInInspector][Tooltip("����1����ʿ�����ŶӸ���")] public int Tema1_LikeSoldierQueueNum;
    [HideInInspector][Tooltip("����2����ʿ�����ŶӸ���")] public int Tema2_LikeSoldierQueueNum;
    [HideInInspector][Tooltip("����1����ʿ��ͬ������")] public int Tema1_LikeSoldierAllNum;
    [HideInInspector][Tooltip("����2����ʿ��ͬ������")] public int Tema2_LikeSoldierAllNum;
    [Tooltip("����1����ʿ�����ͬ������")] public int Tema1_LikeSoldierMaxNum;
    [Tooltip("����2����ʿ�����ͬ������")] public int Tema2_LikeSoldierMaxNum;
    [Tooltip("������ּ���")] public int PaChongOdds;//������ּ���

    [HideInInspector] public Dictionary<string, int> PlayerHeadshot;//����ʿ������������ҵ�ͷ��
    
    [Tooltip("���ͷ���������������һ��ͷ��")] public int PlayerHeadshotinterval;



    public WinTeam m_WinTeam;//��ʤ����

    private bool InitJiDiHPSlider;//��ʼ������Slider
    public bool InitGetJiDiEntity;//��ʼ������Slider
    public bool Is_GameOver;//��Ϸ�Ƿ����

    private void Awake()
    {
        _TeamManager = this;
        Application.targetFrameRate = 60;

        OnGpuEntityNum = 0;
        NoGpuEntityNum = 0;
        UnityAniEntityNum = 0;
        TotalEntityNum = 0;
        BulletNum = 0;
        AttackBoxNum = 0;

        Cur_DetectionNum = DetectionNum;

        Cur_BulletDetectionTime = BulletDetectionTime;

        Team1_JiDiHP = 0;
        Team2_JiDiHP = 0;
        Team1_JiDiEntity = Entity.Null;
        Team2_JiDiEntity = Entity.Null;

        PlayerHeadshot = new Dictionary<string, int>();

        m_WinTeam = WinTeam.Null;

        InitJiDiHPSlider = true;
        InitGetJiDiEntity = true;
        Is_GameOver = false;

        SelectedGift = new GiftType();
        SelectedGiftNum = 1;
        Is_SelectedAutoGift = false;
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //��UI��ʵʱ��ʾ����HP
        UpDataJiDiHPToUI();

    }

    //��UI��ʵʱ��ʾ����HP
    public void UpDataJiDiHPToUI()
    {
        if (Team1_JiDiHPText == null || Team2_JiDiHPText == null ||
           Team1_JiDiHPSlider == null || Team2_JiDiHPSlider == null)
            return;
        //Team1_JiDiHPSlider.value = Team1_JiDiHP;
        Team1_JiDiHPText.text = Team1_JiDiHP.ToString();

        //Team2_JiDiHPSlider.value = Team2_JiDiHP;
        Team2_JiDiHPText.text = Team2_JiDiHP.ToString();

        //��ʼ������HPSlider
        if(InitJiDiHPSlider && Team1_JiDiHP != 0 && Team2_JiDiHP != 0)
        {
            Team1_JiDiHPSlider.minValue = 0;
            Team1_JiDiHPSlider.maxValue = Team1_JiDiHP;
            Team2_JiDiHPSlider.minValue = 0;
            Team2_JiDiHPSlider.maxValue = Team2_JiDiHP;
            InitJiDiHPSlider = false;
            Debug.Log("����Slider��ʼ���ɹ�");
        }
    }
    //�������OpenID�ҵ����
    public PlayerData GetPlayer(string OpenID)
    {
        PlayerData player = new PlayerData();

        if(_Dic_Team1.ContainsKey(OpenID))
            player = _Dic_Team1[OpenID];//�õ����
        else if(_Dic_Team2.ContainsKey(OpenID))
            player = _Dic_Team2[OpenID];
        else
            Debug.Log($"  û��{OpenID}�������");
        return player;
    }
    //�����ʽ����
    public string GetShiBingName(in ShiBingName sbName)
    {
        string shibingname = "";

        switch (sbName)
        {
            case ShiBingName.PaChong: shibingname = "����"; break;
            case ShiBingName.JianYa: shibingname = "����սʿ"; break;
            case ShiBingName.ChangGong: shibingname = "��������"; break;
            case ShiBingName.HuGuang: shibingname = "�������"; break;
            case ShiBingName.YeMa: shibingname = "Ұ��ս��"; break;
            case ShiBingName.TieChui: shibingname = "����ս��"; break;
            case ShiBingName.GangQiu: shibingname = "����ս��"; break;
            case ShiBingName.BaoYu: shibingname = "����ս��"; break;
            case ShiBingName.FengHuang: shibingname = "���ս��"; break;
            case ShiBingName.XiNiu: shibingname = "��Ыս��"; break;
            case ShiBingName.HaiKe: shibingname = "�ڿ�"; break;
            case ShiBingName.HuoShen: shibingname = "�������"; break;
            case ShiBingName.BaoLei: shibingname = "���ݻ���"; break;
            case ShiBingName.RongDian: shibingname = "��������"; break;
            case ShiBingName.BaZhu: shibingname = "����ս��"; break;
            case ShiBingName.ZhanZhengGongChang: shibingname = "�Ǽʹ���"; break;
            case ShiBingName.BingFeng: shibingname = "����ս��"; break;
        }
        return shibingname;
    }
    //ͨ��EntityID���PlayerData(���)
    public PlayerData EntityIDByPlayerData(Entity entiID)
    {
        PlayerData player = new PlayerData();
        if (_Dic_Team1OpenID.ContainsKey(entiID))
            player = _Dic_Team1[_Dic_Team1OpenID[entiID]];
        else if(_Dic_Team2OpenID.ContainsKey(entiID))
            player = _Dic_Team2[_Dic_Team2OpenID[entiID]];
        return player;
    }
    //����һ��ʿ�������������������
    public Entity InstantiateShiBing(in PlayerData Player, Entity Instanshibing, Entity Pos, Entity enemyjidi, EntityManager entiMager)
    {
        Entity shibing = Entity.Null;
        shibing = entiMager.Instantiate(Instanshibing);
        var localtoWorld = entiMager.GetComponentData<LocalToWorld>(Pos);
        entiMager.SetComponentData(shibing, new LocalTransform
        {
            Position = localtoWorld.Position,
            Rotation = localtoWorld.Rotation,
            Scale = 1,
        });
        //�޸�ShiBingChange���
        entiMager.AddComponentData(shibing, new ShiBingChange
        {
            Act = ActState.Idle,
            enemyJiDi = enemyjidi,
        });
        //���ָ�ӹ��������
        entiMager.AddComponentData(shibing, new BarrageCommand
        {
            command = commandType.NUll,
            Comd_ShootEntity = Entity.Null,
        });
        //��ӻ������
        entiMager.AddComponentData(shibing, new Integral
        {
            ATIntegral = 0,
            AttackMeEntity = Entity.Null,
        });
        //��ʿ�������ҵ�EntityID
        entiMager.AddComponentData(shibing, new EntityOpenID
        {
            PlayerEntiyID = Player.m_Entity_ID,
        });
        //�����Ϊ���
        entiMager.AddComponentData(shibing, new Idle());
        entiMager.SetComponentEnabled<Idle>(shibing, false);
        entiMager.AddComponentData(shibing, new Walk());
        entiMager.SetComponentEnabled<Walk>(shibing, true);
        entiMager.AddComponentData(shibing, new Move());
        entiMager.SetComponentEnabled<Move>(shibing, false);
        entiMager.AddComponentData(shibing, new Fire());
        entiMager.SetComponentEnabled<Fire>(shibing, false);
        if (Player.m_comdType != commandType.NUll)//���������
        {
            if (entiMager.HasComponent<BarrageCommand>(shibing))
            {
                //UpDataComponentLookup();
                var barageComd = entiMager.GetComponentData<BarrageCommand>(shibing);//m_BarrageCommand[shibing];
                barageComd.command = Player.m_comdType;
                entiMager.SetComponentData(shibing, barageComd);
            }
        }
        Player.m_GiftShiBingList.Add(shibing);
        //���ͷ��
        var shibingName = entiMager.GetComponentData<ShiBing>(shibing).Name;
        EntityUIManager.Instance.AddHeadshot(in Player.m_Open_ID, in shibingName, in shibing, entiMager);
        return shibing;
    }
    //��ӻ�ʤ���
    public void AddWinPanel()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;
        //��ͣ��Ϸ
        gameRoot.PauseGame();
        //��ӻ�ʤ���
        var panelMager = PanelManager.instance;//new PanelManager();
        //panelMager.Push(new WinPanel());
        panelMager.Push(new ResultPanel());
        Is_GameOver = true;
        if (m_WinTeam == WinTeam.Team1)
        {
            Debug.Log("    ����1��ʤ");
        }
        else if (m_WinTeam == WinTeam.Team2)
        {
            Debug.Log("    ����2��ʤ");
        }
    }
    /// <summary>
    /// ��ȡ�������
    /// </summary>
    /// <returns></returns>
    public List<PlayerData> GetAllPlayer() 
    {
        List<PlayerData> playerDatas = new List<PlayerData>();

        foreach (var item in _Dic_Team1)
        {
            playerDatas.Add(item.Value);
        }
        foreach (var item in _Dic_Team2)
        {
            playerDatas.Add(item.Value);
        }
        return playerDatas;
    }
}
