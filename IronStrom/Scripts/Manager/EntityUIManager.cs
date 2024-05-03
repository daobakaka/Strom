using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class EntityUIManager : MonoBehaviour
{
    private static EntityUIManager _Instance;
    public static EntityUIManager Instance { get { return _Instance; } }


    public Dictionary<Entity, GameObject> AvatarNameDic;
    public Dictionary<Entity, GameObject> ShieldSliderDic;
    [Tooltip("头像名字血条")]public GameObject AvatarName;//头像名字
    [Tooltip("护盾血条")] public GameObject ShieldSlider;
    [HideInInspector] public bool Is_DisplayAvatarName;//是否显示头像名字

    [HideInInspector] public Dictionary<string, Dictionary<ShiBingName, int>> PlayerHeadShot;//根据不同士兵个数出现不同数量的玩家名字
    [Header("不同兵种，多少个士兵一个玩家头像")]
    [Tooltip("点赞兵")] public int PlayerHead_Like;
    [Tooltip("弧光")] public int PlayerHead_HuGuang;
    [Tooltip("野马")] public int PlayerHead_YeMa;
    [Tooltip("兵锋")] public int PlayerHead_BingFeng;
    [Tooltip("暴雨")] public int PlayerHead_BaoYu;
    [Tooltip("霸主")] public int PlayerHead_BaZhu;
    [Tooltip("战争工厂")] public int PlayerHead_WarFactory;
    [Tooltip("堡垒")] public int PlayerHead_BaoLei;
    [Tooltip("铁锤")] public int PlayerHead_TieChui;
    [Tooltip("凤凰")] public int PlayerHead_FengHuang;
    [Tooltip("火神")] public int PlayerHead_HuoShen;
    //[Tooltip("超级战舰")] public int PlayerHead_SuperBaZhu;
    //[Tooltip("超级工厂")] public int PlayerHead_SuperWarFactory;
    [Tooltip("长弓")] public int PlayerHead_ChangGong;
    [Tooltip("熔点")] public int PlayerHead_RongDian;
    [Tooltip("钢球")] public int PlayerHead_GangQiu;
    [Tooltip("黑客")] public int PlayerHead_Haike;
    [Tooltip("犀牛")] public int PlayerHead_XiNiu;
    [Tooltip("爬虫")] public int PlayerHead_PaChong;
    private void Awake()
    {
        _Instance = this;
        Is_DisplayAvatarName = true;
        AvatarNameDic = new Dictionary<Entity, GameObject>();
        ShieldSliderDic = new Dictionary<Entity, GameObject>();
        PlayerHeadShot = new Dictionary<string, Dictionary<ShiBingName, int>>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //foreach(var pair in AvatarNameDic)
        //{
        //    Debug.Log($"头像字典长度为：{AvatarNameDic.Count}.Key为：{pair.Key}.");
        //}
    }

    //头像Obj同步Entity士兵的位置
    public void AvatarSynEntityPos(Entity entity, float3 pos)
    {
        if (!AvatarNameDic.ContainsKey(entity))
            return;
        //var AvatarPos = Camera.main.WorldToScreenPoint(pos);
        //AvatarPos.z = 0;
        AvatarNameDic[entity].transform.position = pos;
    }
    //根据士兵名字获得这种士兵的头像间隔数
    public int ShiBingNameByHeadShotNum(in ShiBingName name)
    {
        switch(name)
        {
            case ShiBingName.JianYa: return PlayerHead_Like;
            case ShiBingName.HuGuang: return PlayerHead_HuGuang;
            case ShiBingName.YeMa: return PlayerHead_YeMa;
            case ShiBingName.BingFeng: return PlayerHead_BingFeng;
            case ShiBingName.BaoYu: return PlayerHead_BaoYu;
            case ShiBingName.BaZhu: return PlayerHead_BaZhu;
            case ShiBingName.ZhanZhengGongChang: return PlayerHead_WarFactory;
            case ShiBingName.BaoLei: return PlayerHead_BaoLei;
            case ShiBingName.TieChui: return PlayerHead_TieChui;
            case ShiBingName.FengHuang: return PlayerHead_FengHuang;
            case ShiBingName.HuoShen: return PlayerHead_HuoShen;
            case ShiBingName.ChangGong: return PlayerHead_ChangGong;
            case ShiBingName.RongDian: return PlayerHead_RongDian;
            case ShiBingName.GangQiu: return PlayerHead_GangQiu;
            case ShiBingName.HaiKe: return PlayerHead_Haike;
            case ShiBingName.XiNiu: return PlayerHead_XiNiu;
            case ShiBingName.PaChong: return PlayerHead_PaChong;
        }
        return -1;
    }
    //头像名字显示
    public void AvatarDisplay(in PlayerData player, Entity enti, in ShiBingName SBname, EntityManager EntiMager, bool DirectlyNeeded)
    {
        //GameObject parent = GameObject.Find("Canvas");
        //if (!parent)
        //{
        //    Debug.LogError("        Canvas不存在");
        //    return;
        //}
        //Transform avaDisTransform = parent.transform.Find("AvatarDisplay");
        //if (avaDisTransform == null)
        //{
        //    Debug.LogError("        AvatarDisplay不存在");
        //    return;
        //}
        GameObject avaDisTransform = GameObject.Find("AvatarDisplay");
        if (avaDisTransform == null)
        {
            Debug.LogError("        AvatarDisplay不存在");
            return;
        }
        Debug.Log("  9");
        var avatarname = GameObject.Instantiate(AvatarName, avaDisTransform.transform);

        //为Canvas设置主相机
        var canvas = avatarname.GetComponent<Canvas>();
        if(canvas != null)
            canvas.worldCamera = Camera.main;


        var SynAvatrName = avatarname.GetComponent<SynchronizeAvatars>();
        if (SynAvatrName == null)
        {
            Debug.Log($"没有SynchronizeAvatars组件");
            return;
        }
        Debug.Log("  10");
        SynAvatrName.SynEntity = enti;
        SynAvatrName._name.text = player.m_Nick;
        //var sprite = Resources.Load<Sprite>(player.m_Avatar);//获得头像
        //if (sprite == null)
        //    Debug.Log($"  没有这张图片");
        //SynAvatrName._avatar.sprite = sprite;

        //这个的单位是否需要血条
        NeedHPSlider(ref SynAvatrName, SBname, EntiMager, DirectlyNeeded);
        //将Entity作为Key，头像作为值，存入字典
        AvatarNameDic.Add(enti, avatarname);
    }
    //每PlayerHeadshotinterval个数后实例化一个头像 
    public void AddHeadshot(in string OpneID, in ShiBingName SBname, in Entity GiftSB, EntityManager EntiMager)
    {
        var teamMager = TeamManager.teamManager;
        int HeadShotNum = ShiBingNameByHeadShotNum(SBname);
        var Player = teamMager.GetPlayer(OpneID);
        if (HeadShotNum <= 0 || Player == null || teamMager == null) return;
        //如果有这个玩家，就根据礼物的士兵添加或累积头像
        if (PlayerHeadShot.ContainsKey(OpneID))
        {
            var playerheadshotDic = PlayerHeadShot[OpneID];
            //有没有这种士兵，有就累加个数，没有就添加
            if (playerheadshotDic.ContainsKey(SBname))
            {
                playerheadshotDic[SBname] += 1;
                //如果这种士兵头像间隔数到了后，就添加一个头像
                if (playerheadshotDic[SBname] >= HeadShotNum)
                {
                    //添加头像
                    AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
                    //给这个Entity士兵添加头像组件
                    EntiMager.AddComponentData(GiftSB, new AvatarName());
                    playerheadshotDic[SBname] = 0;
                }
            }
            else
            {
                playerheadshotDic.Add(SBname, 1);
                //添加头像
                AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
                //给这个Entity士兵添加头像组件
                EntiMager.AddComponentData(GiftSB, new AvatarName());
            }
        }
        else//如果没有这个玩家就添加这个玩家
        {
            var shibingNum = new Dictionary<ShiBingName, int> { { SBname, 1 } };
            PlayerHeadShot.Add(OpneID, shibingNum);
            //添加头像
            AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
            //给这个Entity士兵添加头像组件
            EntiMager.AddComponentData(GiftSB, new AvatarName());
        }
    }
    //判断这个单位是否要血条，并且初始化血条
    void NeedHPSlider(ref SynchronizeAvatars SynAva, in ShiBingName SBname, EntityManager entiMager, bool DirectlyNeeded)
    {
        //设置HPSlider
        SynAva._HPSlider.SetActive(true);
        if (!entiMager.Exists(SynAva.SynEntity) || !entiMager.HasComponent<SX>(SynAva.SynEntity))
            return;
        var SynEntiSX = entiMager.GetComponentData<SX>(SynAva.SynEntity);
        var currentHealth = Mathf.Clamp(SynEntiSX.Cur_HP, 0, SynEntiSX.HP);
        float fillAmount = currentHealth / SynEntiSX.HP;
        SynAva._HPImage.fillAmount = fillAmount;
        //var HPSlider = SynAva._HPSlider.GetComponent<Slider>();
        //HPSlider.maxValue = SynEntiSX.HP;
        //HPSlider.minValue = 0;
        //判断这个ShiBing需不需要血条
        if (!DirectlyNeeded)//是否为直接需要
        {
            if (!WhoCanHaveHPSlider(in SBname))
                SynAva._HPSlider.SetActive(false);
        }
    }
    //同步Entity士兵的HP给Slider
    public void SynEntityShiBingHP(in Entity enti, EntityManager entiMager)
    {
        if (!AvatarNameDic.ContainsKey(enti))
            return;
        var SynAva = AvatarNameDic[enti].GetComponent<SynchronizeAvatars>();
        var SynEntiSX = entiMager.GetComponentData<SX>(SynAva.SynEntity);
        var currentHealth = Mathf.Clamp(SynEntiSX.Cur_HP, 0, SynEntiSX.HP);
        float fillAmount = currentHealth / SynEntiSX.HP;
        SynAva._HPImage.fillAmount = fillAmount;

        //var HPSlider = SynAva._HPSlider.GetComponent<Slider>();
        //HPSlider.value = SynEntiSX.Cur_HP;
    }
    //谁可以有血条
    bool WhoCanHaveHPSlider(in ShiBingName sbName)
    {
        bool b = false;
        int HandNum = ShiBingNameByHeadShotNum(in sbName);
        if (HandNum == 1)
            b = true;
        return b;
    }
    //为Entity士兵添加一个头像名字血条策反条
    public void ByEntityAddAvatar(Entity entity, EntityManager entiMager)
    {
        //如果这个Entity已经有头像了就不添加
        if (AvatarNameDic.ContainsKey(entity)) return;
        Debug.Log("  5");
        var teamMager = TeamManager.teamManager;
        if (!entiMager.Exists(entity) || teamMager == null) return;
        Debug.Log("  6");
        if (!entiMager.HasComponent<EntityOpenID>(entity) || !entiMager.HasComponent<ShiBing>(entity)) return;
        Debug.Log("  7");
        //通过这个Entity士兵获得这玩家
        var entiOpenID = entiMager.GetComponentData<EntityOpenID>(entity);
        PlayerData player = teamMager.EntityIDByPlayerData(entiOpenID.PlayerEntiyID);
        if (player == null) return;
        Debug.Log("  8");
        //为这个Entity添加头像
        var shibingName = entiMager.GetComponentData<ShiBing>(entity).Name;
        AvatarDisplay(in player, entity, in shibingName, entiMager, true);
    }
    //为一个被我策反的单位添加头像和策反血条
    public void AddMutinySlider(Entity entity, EntityManager entiMager)
    {
        //添加头像和血条
        ByEntityAddAvatar(entity, entiMager);
        //打开和初始化策反条
        //通过我的Entity 找到我的头像Obj
        if (!AvatarNameDic.ContainsKey(entity)) return;
        if (!entiMager.HasComponent<SX>(entity)) return;
        var AvatarObj = AvatarNameDic[entity];
        var SynAva = AvatarObj.GetComponent<SynchronizeAvatars>();
        SynAva._MutinySlider.SetActive(true);
        var SynEntiSX = entiMager.GetComponentData<SX>(SynAva.SynEntity);
        var currentHealth = Mathf.Clamp(SynEntiSX.MutinyValue, 0, SynEntiSX.HP);
        float fillAmount = currentHealth / SynEntiSX.HP;
        SynAva._MutinyImage.fillAmount = fillAmount;
        //var MutinySlider = SynAva._MutinySlider.GetComponent<Slider>();
        //MutinySlider.maxValue = SynEntiSX.HP;
        //MutinySlider.minValue = 0;
        //MutinySlider.value = SynEntiSX.MutinyValue;
    }
    //为策反值不为0的士兵同步策反条
    public void SynMutinySlider(Entity entity, EntityManager entiMager)
    {
        //先判断我SX中的策反值是否为0，不为0开启策反Slider
        if (!entiMager.Exists(entity)) return;
        if (!entiMager.HasComponent<SX>(entity) ||
            !entiMager.HasComponent<ShiBing>(entity))
            return;

        var sx = entiMager.GetComponentData<SX>(entity);
        var shibingCompt = entiMager.GetComponentData<ShiBing>(entity);
        //通过我的Entity 找到我的头像Obj
        if (!AvatarNameDic.ContainsKey(entity)) return;
        var AvatarObj = AvatarNameDic[entity];
        var SynAva = AvatarObj.GetComponent<SynchronizeAvatars>();
        if (sx.MutinyValue <= 0)//如果策反值<= 0就关闭策反值
        {
            if(!WhoCanHaveHPSlider(shibingCompt.Name))
                SynAva._HPSlider.SetActive(false);
            SynAva._MutinySlider.SetActive(false);
        }
        else//不为零就开打策反值
        {
            if (!WhoCanHaveHPSlider(shibingCompt.Name))
                SynAva._HPSlider.SetActive(true);
            SynAva._MutinySlider.SetActive(true);
            var currentHealth = Mathf.Clamp(sx.MutinyValue, 0, sx.HP);
            float fillAmount = currentHealth / sx.HP;
            SynAva._MutinyImage.fillAmount = fillAmount;
            //var MutinySlider = SynAva._MutinySlider.GetComponent<Slider>();
            //MutinySlider.value = sx.MutinyValue;
        }
    }


    //为一个护盾添加护盾UI条
    public void AddShieldSlider(Entity enti, Shield shield, EntityManager entiMager)
    {
        GameObject avaDisTransform = GameObject.Find("AvatarDisplay");
        if (avaDisTransform == null)
        {
            Debug.LogError("        AvatarDisplay不存在");
            return;
        }

        var shieldSlider = GameObject.Instantiate(ShieldSlider, avaDisTransform.transform);
        var SynAvatr = shieldSlider.GetComponent<SynchronizeAvatars>();
        if (SynAvatr == null)
        {
            Debug.Log($"没有SynchronizeAvatars组件");
            return;
        }

        SynAvatr.SynEntity = enti;

        entiMager.AddComponentData(enti, new ShieldUI());
        ShieldSliderDic.Add(enti, shieldSlider);
    }
    //同步护盾UI的位置和值
    public void SynShieldUI(Entity entity, float3 pos, EntityManager entiMager)
    {
        if (!ShieldSliderDic.ContainsKey(entity))
            return;
        //同步护盾UI的位置
        ShieldSliderDic[entity].transform.position = pos;
        //同步护盾条
        var SynAva = ShieldSliderDic[entity].GetComponent<SynchronizeAvatars>();
        var SynEntiSX = entiMager.GetComponentData<SX>(SynAva.SynEntity);
        var currentHealth = Mathf.Clamp(SynEntiSX.Cur_HP, 0, SynEntiSX.HP);
        float fillAmount = currentHealth / SynEntiSX.HP;
        SynAva._HPImage.fillAmount = fillAmount;
        //同步护盾值显示
        SynAva._name.text = $"护盾值:{SynEntiSX.Cur_HP}";

    }

}
