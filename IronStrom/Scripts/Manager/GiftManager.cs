using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
public class GiftType//我们的礼物数据
{
    public string m_GiftName;//礼物的名字
    public layer m_team;//送礼物的玩家队伍
    public ShiBingName m_shibingName;//礼物送什么士兵
    public int m_ShiBingNum;//士兵个数
    public int m_GiftNum;//礼物个数
    public string m_Effects;//礼物特效
    public string m_OpneID;//送礼玩家OpenID
    public int m_GiftIntegral;//礼物积分
    public string m_GiftMissing;//还差多少礼物升级
    public int m_VoiceWave;//这个礼物价值多少音浪
}

public class TKMessageData//抖音的礼物数据
{
    public string OpenID;//玩家ID
    public string GiftType;//礼物类型
    public string GiftNum;//礼物个数
}

public enum GiftImage//礼物图的排布
{
    NUll,
    Heel,//横
    VErtical,//竖
}

public class GiftManager : MonoBehaviour
{
    private static GiftManager _GiftManager;
    public static GiftManager Instance { get { return _GiftManager; } }

    private GiftType CurrentGiftPlaying;//正在播放特效的礼物


    public List<GiftType> GiftQueueList;//礼物排队链表
    public List<GiftType> GiftEffectList;//礼物特效链表
    public List<GiftType> Team1_GiftNoticeEffectList;//礼物特效链表
    public List<GiftType> Team2_GiftNoticeEffectList;//礼物特效链表
    [HideInInspector] public bool Is_EffectsPlaying;//是否正在播放礼物特效
    [HideInInspector] public bool Is_Team1_EffectsNoticePlaying;//是否正在播放礼物通知
    [HideInInspector] public bool Is_Team2_EffectsNoticePlaying;//是否正在播放礼物通知
    [Tooltip("礼物积分倍数")] public int GiftIntegralMagnification;

    [Tooltip("队伍1礼物通知")]public GameObject Team1_GiftNotifica;
    [Tooltip("队伍2礼物通知")]public GameObject Team2_GiftNotifica;

    [Header("礼物特效")]
    [Tooltip("仙女棒")] public GameObject Gift_1Effects;
    [Tooltip("甜甜圈")] public GameObject Gift_2Effects;
    [Tooltip("能量电池")] public GameObject Gift_3Effects;
    [Tooltip("恶魔炸弹")] public GameObject Gift_4Effects;
    [Tooltip("神秘空投")] public GameObject Gift_5Effects;
    [Tooltip("超能喷射")] public GameObject Gift_6Effects;
    [Tooltip("仙女棒升级")] public GameObject Gift_1UpLevelEffects;
    [Tooltip("甜甜圈升级")] public GameObject Gift_2UpLevelEffects;
    [Tooltip("能量电池升级")] public GameObject Gift_3UpLevelEffects;
    [Tooltip("恶魔炸弹升级")] public GameObject Gift_4UpLevelEffects;
    [Tooltip("神秘空投升级")] public GameObject Gift_5UpLevelEffects;
    [Tooltip("超能喷射升级")] public GameObject Gift_6UpLevelEffects;

    [Header("每个礼物送多少个升级")]
    [Tooltip("仙女棒")] public int Gift_1_UpLevelNum;
    [Tooltip("甜甜圈")] public int Gift_2_UpLevelNum;
    [Tooltip("能量电池")] public int Gift_3_UpLevelNum;
    [Tooltip("恶魔炸弹")] public int Gift_4_UpLevelNum;
    [Tooltip("神秘空投")] public int Gift_5_UpLevelNum;
    [Tooltip("超能喷射")] public int Gift_6_UpLevelNum;

    [Header("首充礼物设置")]
    public Dictionary<string, PlayerData> FirstRechargeDic;//首充充值的玩家奖励犀牛，限前20名<OpenID,玩家>
    public TextMeshProUGUI FirstRechargeText;
    [Tooltip("限制玩家首充的人数")] public int LimitPlayerNum;//限制玩家人数;

    [Header("礼物图设置")]

    [Tooltip("礼物图横")] public GameObject giftImageHeel;//礼物图横
    [Tooltip("礼物图竖")] public GameObject giftImageVErtical;//礼物图竖


    private void Awake()
    {
        _GiftManager = this;
        GiftQueueList = new List<GiftType>();
        GiftEffectList = new List<GiftType>();
        Team1_GiftNoticeEffectList = new List<GiftType>();
        Team2_GiftNoticeEffectList = new List<GiftType>();
        Is_EffectsPlaying = false;
        Is_Team1_EffectsNoticePlaying = false;
        Is_Team2_EffectsNoticePlaying = false;

        FirstRechargeDic = new Dictionary<string, PlayerData>();
        FirstRechargeText.text = $"名额：{FirstRechargeDic.Count}/{LimitPlayerNum}";


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //播放礼物特效链表
        RunGiftEffect();
        //播放礼物通知
        RunGiftNotice();
        //礼物图的显示
        GiftImageDisplay();


    }

    private void OnDestroy()
    {
        
    }

    //初始化礼物积分
    void InitGiftIntegral()
    {
        //引用表里的内容
        //GiftIntegral.Add();
    }

    //通过抖音给的数据设置我们的礼物数据
    public void TKTOGiftType(in TKMessageData Tkdata)
    {
        if (Tkdata == null || string.IsNullOrWhiteSpace(Tkdata.OpenID))
            return;
        GiftType gift = new GiftType();
        switch (Tkdata.GiftType)
        {
            case "仙女棒": AddGift_1Data(in Tkdata, ref gift); break;
            case "甜甜圈": AddGift_2Data(in Tkdata, ref gift); break;
            case "能量电池": AddGift_3Data(in Tkdata, ref gift); break;
            case "恶魔炸弹": AddGift_4Data(in Tkdata, ref gift); break;
            case "神秘空投": AddGift_5Data(in Tkdata, ref gift); break;
            case "超能喷射": AddGift_6Data(in Tkdata, ref gift); break;
        }
        if (!string.IsNullOrWhiteSpace(gift.m_OpneID))
        {
            GiftQueueList.Add(gift);
        }
    }

    void AddGift_1Data(in TKMessageData Tkdata, ref GiftType gift)//仙女棒礼物数据
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.HuGuang;
        gift.m_ShiBingNum = 5;
        gift.m_Effects = "仙女棒特效";
        gift.m_GiftIntegral = 1;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 1;
    }
    void AddGift_1UpLevelData(ref GiftType gift)//仙女棒升级礼物数据
    {
        gift.m_GiftName = "仙女棒升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaoLei;
        gift.m_ShiBingNum = 1;
        gift.m_Effects = "仙女棒升级特效";
        gift.m_GiftIntegral = 1;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [堡垒机甲]</color>";
    }
    void AddGift_2Data(in TKMessageData Tkdata, ref GiftType gift)
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.YeMa;
        gift.m_ShiBingNum = 10;
        gift.m_Effects = "甜甜圈特效";
        gift.m_GiftIntegral = 52;
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 52;
    }
    void AddGift_2UpLevelData(ref GiftType gift)//甜甜圈升级礼物数据
    {
        gift.m_GiftName = "甜甜圈升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.TieChui;
        gift.m_ShiBingNum = 1;
        gift.m_Effects = "甜甜圈升级特效";
        gift.m_GiftIntegral = 52;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [铁锤战车]</color>";
    }
    void AddGift_3Data(in TKMessageData Tkdata, ref GiftType gift)//能量电池礼物数据
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BingFeng;
        gift.m_ShiBingNum = 15;
        gift.m_Effects = "能量电池特效";
        gift.m_GiftIntegral = 99;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 99;
    }
    void AddGift_3UpLevelData(ref GiftType gift)
    {
        gift.m_GiftName = "能量电池升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.FengHuang;
        gift.m_ShiBingNum = 5;
        gift.m_Effects = "能量电池升级特效";
        gift.m_GiftIntegral = 99;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [凤凰战机]</color>";
    }//能量电池升级礼物数据
    void AddGift_4Data(in TKMessageData Tkdata, ref GiftType gift)//恶魔炸弹礼物数据
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaoYu;
        gift.m_ShiBingNum = 10;
        gift.m_Effects = "恶魔炸弹特效";
        gift.m_GiftIntegral = 199;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 199;
    }
    void AddGift_4UpLevelData(ref GiftType gift)//恶魔炸弹升级礼物数据
    {
        gift.m_GiftName = "恶魔炸弹升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.HuoShen;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "恶魔炸弹升级特效";
        gift.m_GiftIntegral = 199;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [火神机甲]</color>";
    }
    void AddGift_5Data(in TKMessageData Tkdata, ref GiftType gift)//神秘空投礼物数据
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaZhu;
        gift.m_ShiBingNum = 3;
        gift.m_Effects = "神秘空投特效";
        gift.m_GiftIntegral = 520;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 520;
    }
    void AddGift_5UpLevelData(ref GiftType gift)//神秘空投升级礼物数据
    {
        gift.m_GiftName = "神秘空投升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaZhu;
        gift.m_ShiBingNum = 3;
        gift.m_Effects = "神秘空投升级特效";
        gift.m_GiftIntegral = 520;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [超级战舰]</color>";
    }
    void AddGift_6Data(in TKMessageData Tkdata, ref GiftType gift)//超能喷射礼物数据
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.ZhanZhengGongChang;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "超能喷射特效";
        gift.m_GiftIntegral = 1200;//名字作为Key，GiftIntegral[gift.m_GiftName]查找积分
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 1200;
    }
    void AddGift_6UpLevelData(ref GiftType gift)//神秘空投升级礼物数据
    {
        gift.m_GiftName = "超能喷射升级";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.ZhanZhengGongChang;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "超能喷射升级特效";
        gift.m_GiftIntegral = 1200;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>获得 [超级工厂]</color>";
    }


    //初始化礼物士兵的礼物个数
    public void InitGiftNum(ref PlayerData player)
    {
        var giftNumDic = player.m_GiftNumDic;
        giftNumDic.Add("仙女棒", 0);
        giftNumDic.Add("甜甜圈", 0);
        giftNumDic.Add("能量电池", 0);
        giftNumDic.Add("恶魔炸弹", 0);
        giftNumDic.Add("神秘空投", 0);
        giftNumDic.Add("超能喷射", 0);
    }
    //添加这个士兵的礼物个数
    public bool AddGiftNum(ref PlayerData player, ref GiftType gift)
    {
        if (!player.m_GiftNumDic.ContainsKey(gift.m_GiftName))
            return false;

        player.m_GiftNumDic[gift.m_GiftName] += 1;
        bool b = false;
        int GiftNum = 0;
        switch (gift.m_GiftName)
        {
            case "仙女棒": GiftNum = Gift_1_UpLevelNum; break;
            case "甜甜圈": GiftNum = Gift_2_UpLevelNum; break;
            case "能量电池": GiftNum = Gift_3_UpLevelNum; break;
            case "恶魔炸弹": GiftNum = Gift_4_UpLevelNum; break;
            case "神秘空投": GiftNum = Gift_5_UpLevelNum; break;
            case "超能喷射": GiftNum = Gift_6_UpLevelNum; break;
        }

        //累积到了指定个数的礼物
        if (player.m_GiftNumDic[gift.m_GiftName] >= GiftNum)
        {
            player.m_GiftNumDic[gift.m_GiftName] = 0;
            //改变礼物为升级的礼物
            switch(gift.m_GiftName)
            {
                case "仙女棒": AddGift_1UpLevelData(ref gift); break;
                case "甜甜圈": AddGift_2UpLevelData(ref gift); break;
                case "能量电池": AddGift_3UpLevelData(ref gift); break;
                case "恶魔炸弹": AddGift_4UpLevelData(ref gift); break;
                case "神秘空投": AddGift_5UpLevelData(ref gift); break;
                case "超能喷射": AddGift_6UpLevelData(ref gift); break;
            }
            b = true;
        }
        else//没到礼物个数，还差多少个礼物升级
        {
            int Missing = GiftNum - player.m_GiftNumDic[gift.m_GiftName];
            switch(gift.m_GiftName)
            {
                case "仙女棒": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[堡垒机甲]</color>";break;
                case "甜甜圈": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[铁锤战车]</color>";break;
                case "能量电池": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[凤凰战机]</color>";break;
                case "恶魔炸弹": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[火神机甲]</color>";break;
                case "神秘空投": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[超级战舰]</color>";break;
                case "超能喷射": gift.m_GiftMissing = $"<color=#0AF90A>还差{Missing}个获得</color> <color=#F8CD0F>[超级工厂]</color>";break;
            
            }

        }

        return b;
    }


    //播放礼物特效链表
    void RunGiftEffect()
    {
        if (GiftEffectList.Count <= 0 || Is_EffectsPlaying) return;

        var gift = GiftEffectList.FirstOrDefault();
        //播放礼物特效
        PlayGiftEffects(in gift);
        //弹出第一个元素
        GiftEffectList.RemoveAt(0);

        //将礼物特效链表之后的这个玩家的所有相同礼物去掉，直到有一个不一样的礼物或人
        List<GiftType> RemoveGiftEffect = new List<GiftType>();
        foreach (var giftEffect in GiftEffectList)
        {
            if (giftEffect.m_OpneID == CurrentGiftPlaying.m_OpneID)
            {
                if (giftEffect.m_Effects == CurrentGiftPlaying.m_Effects)
                    RemoveGiftEffect.Add(giftEffect);
                else
                    break;
            }
            else
                break;
        }
        foreach (var item in RemoveGiftEffect)
            GiftEffectList.Remove(item);
    }
    //播放礼物特效
    public void PlayGiftEffects(in GiftType gift)
    {
        //播放礼物特效前先检查是否为不可播放特效的礼物(UI界面可设置是否播放礼物特效) 
        var b = CheckGiftEffectsCanPlayed(in gift);
        if (!b) return;

        GameObject gifteffect = null;
        switch (gift.m_Effects)
        {
            case "仙女棒特效": gifteffect = Gift_1Effects; break;
            case "甜甜圈特效": gifteffect = Gift_2Effects; break;
            case "能量电池特效": gifteffect = Gift_3Effects; break;
            case "恶魔炸弹特效": gifteffect = Gift_4Effects; break;
            case "神秘空投特效": gifteffect = Gift_5Effects; break;
            case "超能喷射特效": gifteffect = Gift_6Effects; break;

            case "仙女棒升级特效": gifteffect = Gift_1UpLevelEffects; break;
            case "甜甜圈升级特效": gifteffect = Gift_2UpLevelEffects; break;
            case "能量电池升级特效": gifteffect = Gift_3UpLevelEffects; break;
            case "恶魔炸弹升级特效": gifteffect = Gift_4UpLevelEffects; break;
            case "神秘空投升级特效": gifteffect = Gift_5UpLevelEffects; break;
            case "超能喷射升级特效": gifteffect = Gift_6UpLevelEffects; break;
        }
        if (gifteffect == null) return;

        gifteffect.SetActive(true);//打开特效
        //根据队伍换颜色
        var giftEffectsPlaying = gifteffect.GetComponent<GiftEffectsPlaying>();
        if (giftEffectsPlaying != null)
        {
            giftEffectsPlaying.team = gift.m_team;
        }
        Is_EffectsPlaying = true;
        //更新当前正在播放礼物
        CurrentGiftPlaying = gift;



        //播放礼物通知
        //PlayGiftNotif(in gift, in GiftMager, in spawn);
    }
    //添加礼物通知
    public void AddGiftOntice(in GiftType gift)
    {
        switch (gift.m_team)
        {
            case layer.Team1: Team1_GiftNoticeEffectList.Add(gift); break;
            case layer.Team2: Team2_GiftNoticeEffectList.Add(gift); break;
        }
    }
    //播放礼物通知链表
    void RunGiftNotice()
    {
        if(Team1_GiftNoticeEffectList.Count > 0 && !Is_Team1_EffectsNoticePlaying)
        {
            var gift = Team1_GiftNoticeEffectList.FirstOrDefault();
            PlayGiftNotice(in gift);
            Team1_GiftNoticeEffectList.RemoveAt(0);
        }
        if(Team2_GiftNoticeEffectList.Count > 0 && !Is_Team2_EffectsNoticePlaying)
        {
            var gift = Team2_GiftNoticeEffectList.FirstOrDefault();
            PlayGiftNotice(in gift);
            Team2_GiftNoticeEffectList.RemoveAt(0);
        }
    }
    //播放礼物通知
    void PlayGiftNotice(in GiftType gift)
    {
        var TeamMager = TeamManager.teamManager;
        if (TeamMager == null) return;

        GameObject GiftNotific = null;
        switch (gift.m_team)
        {
            case layer.Team1: GiftNotific = Team1_GiftNotifica; break;
            case layer.Team2: GiftNotific = Team2_GiftNotifica; break;
        }
        if (GiftNotific == null) return;
        var giftNotifData = GiftNotific.GetComponent<Gift_NotificationData>();

        GiftNotific.SetActive(true);//开始播放

        var teamMager = TeamManager.teamManager;
        if (giftNotifData == null || teamMager == null) return;
        var player = teamMager.GetPlayer(gift.m_OpneID);
        if (player == null) return;

        giftNotifData.Headshot.sprite = Resources.Load<Sprite>(player.m_Avatar);
        giftNotifData.PlayerName.text = player.m_Nick;
        giftNotifData.GiftName.text = $"{TeamMager.GetShiBingName(gift.m_shibingName)} X{gift.m_ShiBingNum * gift.m_GiftNum}";
        giftNotifData.LevelUp.text = gift.m_GiftMissing;

        switch (gift.m_team)
        {
            case layer.Team1: Is_Team1_EffectsNoticePlaying = true; break;
            case layer.Team2: Is_Team2_EffectsNoticePlaying = true; break;
        }
        
    }
    //检查是否为可播放的礼物特效
    bool CheckGiftEffectsCanPlayed(in GiftType gift)
    {
        var gameRoot = GameRoot.Instance;
        bool b = true;
        var giftEffects = gift.m_Effects;
        if(giftEffects == "仙女棒特效" || giftEffects == "仙女棒升级特效")
            b = gameRoot.Is_Gift1EffectsCanPlayed;
        else if (giftEffects == "甜甜圈特效" || giftEffects == "甜甜圈升级特效")
            b = gameRoot.Is_Gift2EffectsCanPlayed;
        else if (giftEffects == "能量电池特效" || giftEffects == "能量电池升级特效")
            b = gameRoot.Is_Gift3EffectsCanPlayed;
        else if (giftEffects == "恶魔炸弹特效" || giftEffects == "恶魔炸弹升级特效")
            b = gameRoot.Is_Gift4EffectsCanPlayed;
        else if (giftEffects == "神秘空投特效" || giftEffects == "神秘空投升级特效")
            b = gameRoot.Is_Gift5EffectsCanPlayed;
        else if (giftEffects == "超能喷射特效" || giftEffects == "超能喷射升级特效")
            b = gameRoot.Is_Gift6EffectsCanPlayed;

        return b;
    }
    //首充礼物判断
    public void CheckFirstRechargeGift(in GiftType gift, Spawn spawn, EntityManager entiMager)
    {
        //首充礼物达到限制后，不送首充单位
        if (FirstRechargeDic.Count >= LimitPlayerNum)
            return;
        //如果这个玩家已经送过首充了，就退出
        if (FirstRechargeDic.ContainsKey(gift.m_OpneID))
            return;
        else//如果没有这个玩家，就给他送一个首充单位
        {
            var teamMager = TeamManager.teamManager;
            var player = teamMager.GetPlayer(gift.m_OpneID);
            Entity shibingEnti = Entity.Null;
            Entity jidi = Entity.Null;
            Entity enemyjidi = Entity.Null;
            if (player.m_Team == layer.Team1)
            {
                shibingEnti = spawn.Team1_XiNiu;
                jidi = spawn.JiDi_Team1;
                enemyjidi = spawn.JiDi_Team2;
            }
            else if (player.m_Team == layer.Team2)
            {
                shibingEnti = spawn.Team2_XiNiu;
                jidi = spawn.JiDi_Team2;
                enemyjidi = spawn.JiDi_Team1;
            }
            teamMager.InstantiateShiBing(in player, shibingEnti, jidi, enemyjidi, entiMager);
            FirstRechargeDic.Add(gift.m_OpneID, player);//这名玩家之后不会再获得首充奖励
        }
        FirstRechargeText.text = $"名额：{FirstRechargeDic.Count}/{LimitPlayerNum}";
    }
    //礼物图的显示
    void GiftImageDisplay()
    {
        var gameRoot = GameRoot.Instance;
        if (gameRoot == null) return;
        switch(gameRoot.giftImage)
        {
            case GiftImage.NUll:
                giftImageHeel.SetActive(false);
                giftImageVErtical.SetActive(false);break;
            case GiftImage.Heel:
                giftImageHeel.SetActive(true);
                giftImageVErtical.SetActive(false); break;
            case GiftImage.VErtical:
                giftImageHeel.SetActive(false);
                giftImageVErtical.SetActive(true); break;
        }
    }

}
