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
    [Tooltip("ͷ������Ѫ��")]public GameObject AvatarName;//ͷ������
    [Tooltip("����Ѫ��")] public GameObject ShieldSlider;
    [HideInInspector] public bool Is_DisplayAvatarName;//�Ƿ���ʾͷ������

    [HideInInspector] public Dictionary<string, Dictionary<ShiBingName, int>> PlayerHeadShot;//���ݲ�ͬʿ���������ֲ�ͬ�������������
    [Header("��ͬ���֣����ٸ�ʿ��һ�����ͷ��")]
    [Tooltip("���ޱ�")] public int PlayerHead_Like;
    [Tooltip("����")] public int PlayerHead_HuGuang;
    [Tooltip("Ұ��")] public int PlayerHead_YeMa;
    [Tooltip("����")] public int PlayerHead_BingFeng;
    [Tooltip("����")] public int PlayerHead_BaoYu;
    [Tooltip("����")] public int PlayerHead_BaZhu;
    [Tooltip("ս������")] public int PlayerHead_WarFactory;
    [Tooltip("����")] public int PlayerHead_BaoLei;
    [Tooltip("����")] public int PlayerHead_TieChui;
    [Tooltip("���")] public int PlayerHead_FengHuang;
    [Tooltip("����")] public int PlayerHead_HuoShen;
    //[Tooltip("����ս��")] public int PlayerHead_SuperBaZhu;
    //[Tooltip("��������")] public int PlayerHead_SuperWarFactory;
    [Tooltip("����")] public int PlayerHead_ChangGong;
    [Tooltip("�۵�")] public int PlayerHead_RongDian;
    [Tooltip("����")] public int PlayerHead_GangQiu;
    [Tooltip("�ڿ�")] public int PlayerHead_Haike;
    [Tooltip("Ϭţ")] public int PlayerHead_XiNiu;
    [Tooltip("����")] public int PlayerHead_PaChong;
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
        //    Debug.Log($"ͷ���ֵ䳤��Ϊ��{AvatarNameDic.Count}.KeyΪ��{pair.Key}.");
        //}
    }

    //ͷ��Objͬ��Entityʿ����λ��
    public void AvatarSynEntityPos(Entity entity, float3 pos)
    {
        if (!AvatarNameDic.ContainsKey(entity))
            return;
        //var AvatarPos = Camera.main.WorldToScreenPoint(pos);
        //AvatarPos.z = 0;
        AvatarNameDic[entity].transform.position = pos;
    }
    //����ʿ�����ֻ������ʿ����ͷ������
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
    //ͷ��������ʾ
    public void AvatarDisplay(in PlayerData player, Entity enti, in ShiBingName SBname, EntityManager EntiMager, bool DirectlyNeeded)
    {
        //GameObject parent = GameObject.Find("Canvas");
        //if (!parent)
        //{
        //    Debug.LogError("        Canvas������");
        //    return;
        //}
        //Transform avaDisTransform = parent.transform.Find("AvatarDisplay");
        //if (avaDisTransform == null)
        //{
        //    Debug.LogError("        AvatarDisplay������");
        //    return;
        //}
        GameObject avaDisTransform = GameObject.Find("AvatarDisplay");
        if (avaDisTransform == null)
        {
            Debug.LogError("        AvatarDisplay������");
            return;
        }
        Debug.Log("  9");
        var avatarname = GameObject.Instantiate(AvatarName, avaDisTransform.transform);

        //ΪCanvas���������
        var canvas = avatarname.GetComponent<Canvas>();
        if(canvas != null)
            canvas.worldCamera = Camera.main;


        var SynAvatrName = avatarname.GetComponent<SynchronizeAvatars>();
        if (SynAvatrName == null)
        {
            Debug.Log($"û��SynchronizeAvatars���");
            return;
        }
        Debug.Log("  10");
        SynAvatrName.SynEntity = enti;
        SynAvatrName._name.text = player.m_Nick;
        //var sprite = Resources.Load<Sprite>(player.m_Avatar);//���ͷ��
        //if (sprite == null)
        //    Debug.Log($"  û������ͼƬ");
        //SynAvatrName._avatar.sprite = sprite;

        //����ĵ�λ�Ƿ���ҪѪ��
        NeedHPSlider(ref SynAvatrName, SBname, EntiMager, DirectlyNeeded);
        //��Entity��ΪKey��ͷ����Ϊֵ�������ֵ�
        AvatarNameDic.Add(enti, avatarname);
    }
    //ÿPlayerHeadshotinterval������ʵ����һ��ͷ�� 
    public void AddHeadshot(in string OpneID, in ShiBingName SBname, in Entity GiftSB, EntityManager EntiMager)
    {
        var teamMager = TeamManager.teamManager;
        int HeadShotNum = ShiBingNameByHeadShotNum(SBname);
        var Player = teamMager.GetPlayer(OpneID);
        if (HeadShotNum <= 0 || Player == null || teamMager == null) return;
        //����������ң��͸��������ʿ����ӻ��ۻ�ͷ��
        if (PlayerHeadShot.ContainsKey(OpneID))
        {
            var playerheadshotDic = PlayerHeadShot[OpneID];
            //��û������ʿ�����о��ۼӸ�����û�о����
            if (playerheadshotDic.ContainsKey(SBname))
            {
                playerheadshotDic[SBname] += 1;
                //�������ʿ��ͷ���������˺󣬾����һ��ͷ��
                if (playerheadshotDic[SBname] >= HeadShotNum)
                {
                    //���ͷ��
                    AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
                    //�����Entityʿ�����ͷ�����
                    EntiMager.AddComponentData(GiftSB, new AvatarName());
                    playerheadshotDic[SBname] = 0;
                }
            }
            else
            {
                playerheadshotDic.Add(SBname, 1);
                //���ͷ��
                AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
                //�����Entityʿ�����ͷ�����
                EntiMager.AddComponentData(GiftSB, new AvatarName());
            }
        }
        else//���û�������Ҿ����������
        {
            var shibingNum = new Dictionary<ShiBingName, int> { { SBname, 1 } };
            PlayerHeadShot.Add(OpneID, shibingNum);
            //���ͷ��
            AvatarDisplay(in Player, GiftSB, SBname, EntiMager, false);
            //�����Entityʿ�����ͷ�����
            EntiMager.AddComponentData(GiftSB, new AvatarName());
        }
    }
    //�ж������λ�Ƿ�ҪѪ�������ҳ�ʼ��Ѫ��
    void NeedHPSlider(ref SynchronizeAvatars SynAva, in ShiBingName SBname, EntityManager entiMager, bool DirectlyNeeded)
    {
        //����HPSlider
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
        //�ж����ShiBing�費��ҪѪ��
        if (!DirectlyNeeded)//�Ƿ�Ϊֱ����Ҫ
        {
            if (!WhoCanHaveHPSlider(in SBname))
                SynAva._HPSlider.SetActive(false);
        }
    }
    //ͬ��Entityʿ����HP��Slider
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
    //˭������Ѫ��
    bool WhoCanHaveHPSlider(in ShiBingName sbName)
    {
        bool b = false;
        int HandNum = ShiBingNameByHeadShotNum(in sbName);
        if (HandNum == 1)
            b = true;
        return b;
    }
    //ΪEntityʿ�����һ��ͷ������Ѫ���߷���
    public void ByEntityAddAvatar(Entity entity, EntityManager entiMager)
    {
        //������Entity�Ѿ���ͷ���˾Ͳ����
        if (AvatarNameDic.ContainsKey(entity)) return;
        Debug.Log("  5");
        var teamMager = TeamManager.teamManager;
        if (!entiMager.Exists(entity) || teamMager == null) return;
        Debug.Log("  6");
        if (!entiMager.HasComponent<EntityOpenID>(entity) || !entiMager.HasComponent<ShiBing>(entity)) return;
        Debug.Log("  7");
        //ͨ�����Entityʿ����������
        var entiOpenID = entiMager.GetComponentData<EntityOpenID>(entity);
        PlayerData player = teamMager.EntityIDByPlayerData(entiOpenID.PlayerEntiyID);
        if (player == null) return;
        Debug.Log("  8");
        //Ϊ���Entity���ͷ��
        var shibingName = entiMager.GetComponentData<ShiBing>(entity).Name;
        AvatarDisplay(in player, entity, in shibingName, entiMager, true);
    }
    //Ϊһ�����Ҳ߷��ĵ�λ���ͷ��Ͳ߷�Ѫ��
    public void AddMutinySlider(Entity entity, EntityManager entiMager)
    {
        //���ͷ���Ѫ��
        ByEntityAddAvatar(entity, entiMager);
        //�򿪺ͳ�ʼ���߷���
        //ͨ���ҵ�Entity �ҵ��ҵ�ͷ��Obj
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
    //Ϊ�߷�ֵ��Ϊ0��ʿ��ͬ���߷���
    public void SynMutinySlider(Entity entity, EntityManager entiMager)
    {
        //���ж���SX�еĲ߷�ֵ�Ƿ�Ϊ0����Ϊ0�����߷�Slider
        if (!entiMager.Exists(entity)) return;
        if (!entiMager.HasComponent<SX>(entity) ||
            !entiMager.HasComponent<ShiBing>(entity))
            return;

        var sx = entiMager.GetComponentData<SX>(entity);
        var shibingCompt = entiMager.GetComponentData<ShiBing>(entity);
        //ͨ���ҵ�Entity �ҵ��ҵ�ͷ��Obj
        if (!AvatarNameDic.ContainsKey(entity)) return;
        var AvatarObj = AvatarNameDic[entity];
        var SynAva = AvatarObj.GetComponent<SynchronizeAvatars>();
        if (sx.MutinyValue <= 0)//����߷�ֵ<= 0�͹رղ߷�ֵ
        {
            if(!WhoCanHaveHPSlider(shibingCompt.Name))
                SynAva._HPSlider.SetActive(false);
            SynAva._MutinySlider.SetActive(false);
        }
        else//��Ϊ��Ϳ���߷�ֵ
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


    //Ϊһ��������ӻ���UI��
    public void AddShieldSlider(Entity enti, Shield shield, EntityManager entiMager)
    {
        GameObject avaDisTransform = GameObject.Find("AvatarDisplay");
        if (avaDisTransform == null)
        {
            Debug.LogError("        AvatarDisplay������");
            return;
        }

        var shieldSlider = GameObject.Instantiate(ShieldSlider, avaDisTransform.transform);
        var SynAvatr = shieldSlider.GetComponent<SynchronizeAvatars>();
        if (SynAvatr == null)
        {
            Debug.Log($"û��SynchronizeAvatars���");
            return;
        }

        SynAvatr.SynEntity = enti;

        entiMager.AddComponentData(enti, new ShieldUI());
        ShieldSliderDic.Add(enti, shieldSlider);
    }
    //ͬ������UI��λ�ú�ֵ
    public void SynShieldUI(Entity entity, float3 pos, EntityManager entiMager)
    {
        if (!ShieldSliderDic.ContainsKey(entity))
            return;
        //ͬ������UI��λ��
        ShieldSliderDic[entity].transform.position = pos;
        //ͬ��������
        var SynAva = ShieldSliderDic[entity].GetComponent<SynchronizeAvatars>();
        var SynEntiSX = entiMager.GetComponentData<SX>(SynAva.SynEntity);
        var currentHealth = Mathf.Clamp(SynEntiSX.Cur_HP, 0, SynEntiSX.HP);
        float fillAmount = currentHealth / SynEntiSX.HP;
        SynAva._HPImage.fillAmount = fillAmount;
        //ͬ������ֵ��ʾ
        SynAva._name.text = $"����ֵ:{SynEntiSX.Cur_HP}";

    }

}
