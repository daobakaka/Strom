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


public enum layer//层级
{
    NUll = 0,//空
    Team1 = 1,
    Team2,
    Team1Detection,//队伍1的检测
    Team2Detection,
    Team1Bullet,//子弹
    Team2Bullet,//子弹
    Neutral,//中立单位
    Parasitic,//寄生
    Map,//地图
    Shield,//护盾
    
}

public enum ActState//行为状态
{
    NULL,//空
    Idle,//默认之后
    Walk,//默认往前走
    Walk_R,//右走动画
    Walk_L,//左走动画
    Move,//移动
    Fire,//射击
    Ready,//准备阶段
    Attack,//攻击
    die,//死亡
    NotMove,//禁锢
    Appear,//出场
}

public enum WinTeam//获胜队伍
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

    [Tooltip("士兵检测间隔时间")] public int DetectionNum;
    [HideInInspector] public int Cur_DetectionNum;
    [Tooltip("子弹检测间隔时间")] public float BulletDetectionTime;
    [HideInInspector] public float Cur_BulletDetectionTime;

    //两个队伍的玩家
    public Dictionary<string, PlayerData> _Dic_Team1 = new Dictionary<string, PlayerData>();//<OpenID,玩家>
    public Dictionary<string, PlayerData> _Dic_Team2 = new Dictionary<string, PlayerData>();//<OpenID,玩家>
    public Dictionary<Entity, string> _Dic_Team1OpenID = new Dictionary<Entity, string>();//<EntityID,OpenID>
    public Dictionary<Entity, string> _Dic_Team2OpenID = new Dictionary<Entity, string>();//<EntityID,OpenID>


    public PlayerData SelectedPlayer;//选中的玩家
    public GiftType SelectedGift;//选择中的礼物用于自动出兵
    public int SelectedGiftNum;//选择中礼物的个数
    public bool Is_SelectedAutoGift;//是否自动出兵

    [HideInInspector] public int OnGpuEntityNum;//开启GPU加速的Entity单位；
    [HideInInspector] public int NoGpuEntityNum;//没有GPU加速的Entity单位；
    [HideInInspector] public int UnityAniEntityNum;//Unity动画的Entity单位；
    [HideInInspector] public int TotalEntityNum;//总共的Entity士兵单位单位；
    [HideInInspector] public int BulletNum;//子弹个数；
    [HideInInspector] public int AttackBoxNum;//攻击盒子个数；


    [HideInInspector][Tooltip("队伍1点赞士兵的排队个数")] public int Tema1_LikeSoldierQueueNum;
    [HideInInspector][Tooltip("队伍2点赞士兵的排队个数")] public int Tema2_LikeSoldierQueueNum;
    [HideInInspector][Tooltip("队伍1点赞士兵同屏个数")] public int Tema1_LikeSoldierAllNum;
    [HideInInspector][Tooltip("队伍2点赞士兵同屏个数")] public int Tema2_LikeSoldierAllNum;
    [Tooltip("队伍1点赞士兵最大同屏个数")] public int Tema1_LikeSoldierMaxNum;
    [Tooltip("队伍2点赞士兵最大同屏个数")] public int Tema2_LikeSoldierMaxNum;
    [Tooltip("爬虫出现几率")] public int PaChongOdds;//爬虫出现几率

    [HideInInspector] public Dictionary<string, int> PlayerHeadshot;//根据士兵个数出现玩家的头像
    
    [Tooltip("玩家头像间隔数，间隔多少一个头像")] public int PlayerHeadshotinterval;



    public WinTeam m_WinTeam;//获胜队伍

    private bool InitJiDiHPSlider;//初始化基地Slider
    public bool InitGetJiDiEntity;//初始化基地Slider
    public bool Is_GameOver;//游戏是否结束

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
        //在UI上实时显示基地HP
        UpDataJiDiHPToUI();

    }

    //在UI上实时显示基地HP
    public void UpDataJiDiHPToUI()
    {
        if (Team1_JiDiHPText == null || Team2_JiDiHPText == null ||
           Team1_JiDiHPSlider == null || Team2_JiDiHPSlider == null)
            return;
        //Team1_JiDiHPSlider.value = Team1_JiDiHP;
        Team1_JiDiHPText.text = Team1_JiDiHP.ToString();

        //Team2_JiDiHPSlider.value = Team2_JiDiHP;
        Team2_JiDiHPText.text = Team2_JiDiHP.ToString();

        //初始化基地HPSlider
        if(InitJiDiHPSlider && Team1_JiDiHP != 0 && Team2_JiDiHP != 0)
        {
            Team1_JiDiHPSlider.minValue = 0;
            Team1_JiDiHPSlider.maxValue = Team1_JiDiHP;
            Team2_JiDiHPSlider.minValue = 0;
            Team2_JiDiHPSlider.maxValue = Team2_JiDiHP;
            InitJiDiHPSlider = false;
            Debug.Log("基地Slider初始化成功");
        }
    }
    //根据玩家OpenID找到玩家
    public PlayerData GetPlayer(string OpenID)
    {
        PlayerData player = new PlayerData();

        if(_Dic_Team1.ContainsKey(OpenID))
            player = _Dic_Team1[OpenID];//拿到玩家
        else if(_Dic_Team2.ContainsKey(OpenID))
            player = _Dic_Team2[OpenID];
        else
            Debug.Log($"  没有{OpenID}这名玩家");
        return player;
    }
    //获得制式名字
    public string GetShiBingName(in ShiBingName sbName)
    {
        string shibingname = "";

        switch (sbName)
        {
            case ShiBingName.PaChong: shibingname = "爬虫"; break;
            case ShiBingName.JianYa: shibingname = "机甲战士"; break;
            case ShiBingName.ChangGong: shibingname = "长弓机甲"; break;
            case ShiBingName.HuGuang: shibingname = "弧光机甲"; break;
            case ShiBingName.YeMa: shibingname = "野马战车"; break;
            case ShiBingName.TieChui: shibingname = "铁锤战车"; break;
            case ShiBingName.GangQiu: shibingname = "激光战车"; break;
            case ShiBingName.BaoYu: shibingname = "暴雨战车"; break;
            case ShiBingName.FengHuang: shibingname = "凤凰战机"; break;
            case ShiBingName.XiNiu: shibingname = "狂蝎战车"; break;
            case ShiBingName.HaiKe: shibingname = "黑客"; break;
            case ShiBingName.HuoShen: shibingname = "火神机甲"; break;
            case ShiBingName.BaoLei: shibingname = "堡垒机甲"; break;
            case ShiBingName.RongDian: shibingname = "激光电磁炮"; break;
            case ShiBingName.BaZhu: shibingname = "霸主战舰"; break;
            case ShiBingName.ZhanZhengGongChang: shibingname = "星际工厂"; break;
            case ShiBingName.BingFeng: shibingname = "蜂王战机"; break;
        }
        return shibingname;
    }
    //通过EntityID获得PlayerData(玩家)
    public PlayerData EntityIDByPlayerData(Entity entiID)
    {
        PlayerData player = new PlayerData();
        if (_Dic_Team1OpenID.ContainsKey(entiID))
            player = _Dic_Team1[_Dic_Team1OpenID[entiID]];
        else if(_Dic_Team2OpenID.ContainsKey(entiID))
            player = _Dic_Team2[_Dic_Team2OpenID[entiID]];
        return player;
    }
    //创建一个士兵，并赋予他各种组件
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
        //修改ShiBingChange组件
        entiMager.AddComponentData(shibing, new ShiBingChange
        {
            Act = ActState.Idle,
            enemyJiDi = enemyjidi,
        });
        //添加指挥官命令组件
        entiMager.AddComponentData(shibing, new BarrageCommand
        {
            command = commandType.NUll,
            Comd_ShootEntity = Entity.Null,
        });
        //添加积分组件
        entiMager.AddComponentData(shibing, new Integral
        {
            ATIntegral = 0,
            AttackMeEntity = Entity.Null,
        });
        //让士兵获得玩家的EntityID
        entiMager.AddComponentData(shibing, new EntityOpenID
        {
            PlayerEntiyID = Player.m_Entity_ID,
        });
        //添加行为组件
        entiMager.AddComponentData(shibing, new Idle());
        entiMager.SetComponentEnabled<Idle>(shibing, false);
        entiMager.AddComponentData(shibing, new Walk());
        entiMager.SetComponentEnabled<Walk>(shibing, true);
        entiMager.AddComponentData(shibing, new Move());
        entiMager.SetComponentEnabled<Move>(shibing, false);
        entiMager.AddComponentData(shibing, new Fire());
        entiMager.SetComponentEnabled<Fire>(shibing, false);
        if (Player.m_comdType != commandType.NUll)//如果有命令
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
        //添加头像
        var shibingName = entiMager.GetComponentData<ShiBing>(shibing).Name;
        EntityUIManager.Instance.AddHeadshot(in Player.m_Open_ID, in shibingName, in shibing, entiMager);
        return shibing;
    }
    //添加获胜面板
    public void AddWinPanel()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;
        //暂停游戏
        gameRoot.PauseGame();
        //添加获胜面板
        var panelMager = PanelManager.instance;//new PanelManager();
        //panelMager.Push(new WinPanel());
        panelMager.Push(new ResultPanel());
        Is_GameOver = true;
        if (m_WinTeam == WinTeam.Team1)
        {
            Debug.Log("    队伍1获胜");
        }
        else if (m_WinTeam == WinTeam.Team2)
        {
            Debug.Log("    队伍2获胜");
        }
    }
    /// <summary>
    /// 获取所有玩家
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
