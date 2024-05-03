using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameDifficulty
{
    NUll,
    Easy,//简单
    Normal,//普通
    Hard,//困难
}
public enum GameTime
{
    Null,
    ShortTerm,//短时间
    MediumTerm,//中时间
    LongTerm,//长时间
}

//管理全局
public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }
    //场景管理器
    public SceneSystem _SceneSystem { get; private set; }
    //显示一个面板
    public UnityAction<BasePanel> Push { get; private set; }


    //游戏难易度
    [HideInInspector] public GameDifficulty gameDifficulty;
    //游戏时间
    [HideInInspector] public GameTime gameTime;
    //不同难度等级的
    [HideInInspector] public float JiDiHP;//基地血量
    [HideInInspector] public int LikeNum;//点赞数量达到后出现野怪
    [HideInInspector] public int Cur_LikeNum;
    [HideInInspector] public int MonsterNum;//怪物数量
    [HideInInspector] public Dictionary<ShiBingName, int> AwardShiBingNumDic;//奖励士兵数量


    //礼物特效是否可以播放
    [HideInInspector] public bool Is_Gift1EffectsCanPlayed;//是否能播放仙女棒特效
    [HideInInspector] public bool Is_Gift2EffectsCanPlayed;//是否能播放甜甜圈特效
    [HideInInspector] public bool Is_Gift3EffectsCanPlayed;//是否能播放能量电池特效
    [HideInInspector] public bool Is_Gift4EffectsCanPlayed;//是否能播放恶魔炸弹特效
    [HideInInspector] public bool Is_Gift5EffectsCanPlayed;//是否能播放神秘空投特效
    [HideInInspector]  public bool Is_Gift6EffectsCanPlayed;//是否能播放超能喷射特效


    //礼物图状态
    [HideInInspector] public GiftImage giftImage;//礼物图的排布


    //按F12在实例化礼物和实例化士兵之间切换
    [HideInInspector] public bool GiftOrShiBing;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        _SceneSystem = new SceneSystem();

        DontDestroyOnLoad(gameObject);
        Debug.Log("GameRootAwake");

        AwardShiBingNumDic = new Dictionary<ShiBingName, int>();
        gameDifficulty = GameDifficulty.Normal;
        gameTime = GameTime.MediumTerm;
        Cur_LikeNum = 0;


        Is_Gift1EffectsCanPlayed = true;
        Is_Gift2EffectsCanPlayed = true;
        Is_Gift3EffectsCanPlayed = true;
        Is_Gift4EffectsCanPlayed = true;
        Is_Gift5EffectsCanPlayed = true;
        Is_Gift6EffectsCanPlayed = true;


        giftImage = GiftImage.VErtical;


        GiftOrShiBing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _SceneSystem.SetScene(new StartScenes());
    }

    // Update is called once per frame
    void Update()
    {
        //是否已经是主场景了
        LoadMainScene();
        //按F12，实例化礼物和实例化士兵之间进行切换
        HoldF12();
    }

    //框架之外设置Push
    public void SetAction(UnityAction<BasePanel> push)
    {
        Push = push;
    }

    //是否已经是主场景了
    void LoadMainScene()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
            return;

        GameObject parent = GameObject.Find("Canvas");
        Transform[] trans = parent.GetComponentsInChildren<Transform>();
        foreach (Transform item in trans)
        {
            if (item.name == "MainPanel")
                return;
        }

        Instance._SceneSystem.SetScene(new MainScene());
    }

    //暂停游戏
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    //开始游戏
    public void PlayGame()
    {
        Time.timeScale = 1;
    }

    //按F12，实例化礼物和实例化士兵之间进行切换
    void HoldF12()
    {
        if(Input.GetKeyDown(KeyCode.F12))
            GiftOrShiBing = GiftOrShiBing == true ? false : true;
    }

}
