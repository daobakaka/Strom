using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
public class GiftType//���ǵ���������
{
    public string m_GiftName;//���������
    public layer m_team;//���������Ҷ���
    public ShiBingName m_shibingName;//������ʲôʿ��
    public int m_ShiBingNum;//ʿ������
    public int m_GiftNum;//�������
    public string m_Effects;//������Ч
    public string m_OpneID;//�������OpenID
    public int m_GiftIntegral;//�������
    public string m_GiftMissing;//���������������
    public int m_VoiceWave;//��������ֵ��������
}

public class TKMessageData//��������������
{
    public string OpenID;//���ID
    public string GiftType;//��������
    public string GiftNum;//�������
}

public enum GiftImage//����ͼ���Ų�
{
    NUll,
    Heel,//��
    VErtical,//��
}

public class GiftManager : MonoBehaviour
{
    private static GiftManager _GiftManager;
    public static GiftManager Instance { get { return _GiftManager; } }

    private GiftType CurrentGiftPlaying;//���ڲ�����Ч������


    public List<GiftType> GiftQueueList;//�����Ŷ�����
    public List<GiftType> GiftEffectList;//������Ч����
    public List<GiftType> Team1_GiftNoticeEffectList;//������Ч����
    public List<GiftType> Team2_GiftNoticeEffectList;//������Ч����
    [HideInInspector] public bool Is_EffectsPlaying;//�Ƿ����ڲ���������Ч
    [HideInInspector] public bool Is_Team1_EffectsNoticePlaying;//�Ƿ����ڲ�������֪ͨ
    [HideInInspector] public bool Is_Team2_EffectsNoticePlaying;//�Ƿ����ڲ�������֪ͨ
    [Tooltip("������ֱ���")] public int GiftIntegralMagnification;

    [Tooltip("����1����֪ͨ")]public GameObject Team1_GiftNotifica;
    [Tooltip("����2����֪ͨ")]public GameObject Team2_GiftNotifica;

    [Header("������Ч")]
    [Tooltip("��Ů��")] public GameObject Gift_1Effects;
    [Tooltip("����Ȧ")] public GameObject Gift_2Effects;
    [Tooltip("�������")] public GameObject Gift_3Effects;
    [Tooltip("��ħը��")] public GameObject Gift_4Effects;
    [Tooltip("���ؿ�Ͷ")] public GameObject Gift_5Effects;
    [Tooltip("��������")] public GameObject Gift_6Effects;
    [Tooltip("��Ů������")] public GameObject Gift_1UpLevelEffects;
    [Tooltip("����Ȧ����")] public GameObject Gift_2UpLevelEffects;
    [Tooltip("�����������")] public GameObject Gift_3UpLevelEffects;
    [Tooltip("��ħը������")] public GameObject Gift_4UpLevelEffects;
    [Tooltip("���ؿ�Ͷ����")] public GameObject Gift_5UpLevelEffects;
    [Tooltip("������������")] public GameObject Gift_6UpLevelEffects;

    [Header("ÿ�������Ͷ��ٸ�����")]
    [Tooltip("��Ů��")] public int Gift_1_UpLevelNum;
    [Tooltip("����Ȧ")] public int Gift_2_UpLevelNum;
    [Tooltip("�������")] public int Gift_3_UpLevelNum;
    [Tooltip("��ħը��")] public int Gift_4_UpLevelNum;
    [Tooltip("���ؿ�Ͷ")] public int Gift_5_UpLevelNum;
    [Tooltip("��������")] public int Gift_6_UpLevelNum;

    [Header("�׳���������")]
    public Dictionary<string, PlayerData> FirstRechargeDic;//�׳��ֵ����ҽ���Ϭţ����ǰ20��<OpenID,���>
    public TextMeshProUGUI FirstRechargeText;
    [Tooltip("��������׳������")] public int LimitPlayerNum;//�����������;

    [Header("����ͼ����")]

    [Tooltip("����ͼ��")] public GameObject giftImageHeel;//����ͼ��
    [Tooltip("����ͼ��")] public GameObject giftImageVErtical;//����ͼ��


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
        FirstRechargeText.text = $"���{FirstRechargeDic.Count}/{LimitPlayerNum}";


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //����������Ч����
        RunGiftEffect();
        //��������֪ͨ
        RunGiftNotice();
        //����ͼ����ʾ
        GiftImageDisplay();


    }

    private void OnDestroy()
    {
        
    }

    //��ʼ���������
    void InitGiftIntegral()
    {
        //���ñ��������
        //GiftIntegral.Add();
    }

    //ͨ���������������������ǵ���������
    public void TKTOGiftType(in TKMessageData Tkdata)
    {
        if (Tkdata == null || string.IsNullOrWhiteSpace(Tkdata.OpenID))
            return;
        GiftType gift = new GiftType();
        switch (Tkdata.GiftType)
        {
            case "��Ů��": AddGift_1Data(in Tkdata, ref gift); break;
            case "����Ȧ": AddGift_2Data(in Tkdata, ref gift); break;
            case "�������": AddGift_3Data(in Tkdata, ref gift); break;
            case "��ħը��": AddGift_4Data(in Tkdata, ref gift); break;
            case "���ؿ�Ͷ": AddGift_5Data(in Tkdata, ref gift); break;
            case "��������": AddGift_6Data(in Tkdata, ref gift); break;
        }
        if (!string.IsNullOrWhiteSpace(gift.m_OpneID))
        {
            GiftQueueList.Add(gift);
        }
    }

    void AddGift_1Data(in TKMessageData Tkdata, ref GiftType gift)//��Ů����������
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.HuGuang;
        gift.m_ShiBingNum = 5;
        gift.m_Effects = "��Ů����Ч";
        gift.m_GiftIntegral = 1;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 1;
    }
    void AddGift_1UpLevelData(ref GiftType gift)//��Ů��������������
    {
        gift.m_GiftName = "��Ů������";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaoLei;
        gift.m_ShiBingNum = 1;
        gift.m_Effects = "��Ů��������Ч";
        gift.m_GiftIntegral = 1;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [���ݻ���]</color>";
    }
    void AddGift_2Data(in TKMessageData Tkdata, ref GiftType gift)
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.YeMa;
        gift.m_ShiBingNum = 10;
        gift.m_Effects = "����Ȧ��Ч";
        gift.m_GiftIntegral = 52;
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 52;
    }
    void AddGift_2UpLevelData(ref GiftType gift)//����Ȧ������������
    {
        gift.m_GiftName = "����Ȧ����";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.TieChui;
        gift.m_ShiBingNum = 1;
        gift.m_Effects = "����Ȧ������Ч";
        gift.m_GiftIntegral = 52;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [����ս��]</color>";
    }
    void AddGift_3Data(in TKMessageData Tkdata, ref GiftType gift)//���������������
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BingFeng;
        gift.m_ShiBingNum = 15;
        gift.m_Effects = "���������Ч";
        gift.m_GiftIntegral = 99;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 99;
    }
    void AddGift_3UpLevelData(ref GiftType gift)
    {
        gift.m_GiftName = "�����������";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.FengHuang;
        gift.m_ShiBingNum = 5;
        gift.m_Effects = "�������������Ч";
        gift.m_GiftIntegral = 99;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [���ս��]</color>";
    }//�������������������
    void AddGift_4Data(in TKMessageData Tkdata, ref GiftType gift)//��ħը����������
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaoYu;
        gift.m_ShiBingNum = 10;
        gift.m_Effects = "��ħը����Ч";
        gift.m_GiftIntegral = 199;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 199;
    }
    void AddGift_4UpLevelData(ref GiftType gift)//��ħը��������������
    {
        gift.m_GiftName = "��ħը������";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.HuoShen;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "��ħը��������Ч";
        gift.m_GiftIntegral = 199;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [�������]</color>";
    }
    void AddGift_5Data(in TKMessageData Tkdata, ref GiftType gift)//���ؿ�Ͷ��������
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaZhu;
        gift.m_ShiBingNum = 3;
        gift.m_Effects = "���ؿ�Ͷ��Ч";
        gift.m_GiftIntegral = 520;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 520;
    }
    void AddGift_5UpLevelData(ref GiftType gift)//���ؿ�Ͷ������������
    {
        gift.m_GiftName = "���ؿ�Ͷ����";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.BaZhu;
        gift.m_ShiBingNum = 3;
        gift.m_Effects = "���ؿ�Ͷ������Ч";
        gift.m_GiftIntegral = 520;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [����ս��]</color>";
    }
    void AddGift_6Data(in TKMessageData Tkdata, ref GiftType gift)//����������������
    {
        gift.m_GiftName = Tkdata.GiftType;
        gift.m_GiftNum = int.Parse(Tkdata.GiftNum);
        gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.ZhanZhengGongChang;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "����������Ч";
        gift.m_GiftIntegral = 1200;//������ΪKey��GiftIntegral[gift.m_GiftName]���һ���
        gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;// SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_VoiceWave = 1200;
    }
    void AddGift_6UpLevelData(ref GiftType gift)//���ؿ�Ͷ������������
    {
        gift.m_GiftName = "������������";
        gift.m_GiftNum = 1;
        //gift.m_OpneID = Tkdata.OpenID;
        gift.m_shibingName = ShiBingName.ZhanZhengGongChang;
        gift.m_ShiBingNum = 2;
        gift.m_Effects = "��������������Ч";
        gift.m_GiftIntegral = 1200;
        //gift.m_team = TeamManager.teamManager.GetPlayer(Tkdata.OpenID).m_Team;//SelectTeamByOpenID(in Tkdata.OpenID, gift.m_GiftIntegral).m_Team;
        gift.m_GiftMissing = $"<color=#F8CD0F>��� [��������]</color>";
    }


    //��ʼ������ʿ�����������
    public void InitGiftNum(ref PlayerData player)
    {
        var giftNumDic = player.m_GiftNumDic;
        giftNumDic.Add("��Ů��", 0);
        giftNumDic.Add("����Ȧ", 0);
        giftNumDic.Add("�������", 0);
        giftNumDic.Add("��ħը��", 0);
        giftNumDic.Add("���ؿ�Ͷ", 0);
        giftNumDic.Add("��������", 0);
    }
    //������ʿ�����������
    public bool AddGiftNum(ref PlayerData player, ref GiftType gift)
    {
        if (!player.m_GiftNumDic.ContainsKey(gift.m_GiftName))
            return false;

        player.m_GiftNumDic[gift.m_GiftName] += 1;
        bool b = false;
        int GiftNum = 0;
        switch (gift.m_GiftName)
        {
            case "��Ů��": GiftNum = Gift_1_UpLevelNum; break;
            case "����Ȧ": GiftNum = Gift_2_UpLevelNum; break;
            case "�������": GiftNum = Gift_3_UpLevelNum; break;
            case "��ħը��": GiftNum = Gift_4_UpLevelNum; break;
            case "���ؿ�Ͷ": GiftNum = Gift_5_UpLevelNum; break;
            case "��������": GiftNum = Gift_6_UpLevelNum; break;
        }

        //�ۻ�����ָ������������
        if (player.m_GiftNumDic[gift.m_GiftName] >= GiftNum)
        {
            player.m_GiftNumDic[gift.m_GiftName] = 0;
            //�ı�����Ϊ����������
            switch(gift.m_GiftName)
            {
                case "��Ů��": AddGift_1UpLevelData(ref gift); break;
                case "����Ȧ": AddGift_2UpLevelData(ref gift); break;
                case "�������": AddGift_3UpLevelData(ref gift); break;
                case "��ħը��": AddGift_4UpLevelData(ref gift); break;
                case "���ؿ�Ͷ": AddGift_5UpLevelData(ref gift); break;
                case "��������": AddGift_6UpLevelData(ref gift); break;
            }
            b = true;
        }
        else//û�����������������ٸ���������
        {
            int Missing = GiftNum - player.m_GiftNumDic[gift.m_GiftName];
            switch(gift.m_GiftName)
            {
                case "��Ů��": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[���ݻ���]</color>";break;
                case "����Ȧ": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[����ս��]</color>";break;
                case "�������": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[���ս��]</color>";break;
                case "��ħը��": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[�������]</color>";break;
                case "���ؿ�Ͷ": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[����ս��]</color>";break;
                case "��������": gift.m_GiftMissing = $"<color=#0AF90A>����{Missing}�����</color> <color=#F8CD0F>[��������]</color>";break;
            
            }

        }

        return b;
    }


    //����������Ч����
    void RunGiftEffect()
    {
        if (GiftEffectList.Count <= 0 || Is_EffectsPlaying) return;

        var gift = GiftEffectList.FirstOrDefault();
        //����������Ч
        PlayGiftEffects(in gift);
        //������һ��Ԫ��
        GiftEffectList.RemoveAt(0);

        //��������Ч����֮��������ҵ�������ͬ����ȥ����ֱ����һ����һ�����������
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
    //����������Ч
    public void PlayGiftEffects(in GiftType gift)
    {
        //����������Чǰ�ȼ���Ƿ�Ϊ���ɲ�����Ч������(UI����������Ƿ񲥷�������Ч) 
        var b = CheckGiftEffectsCanPlayed(in gift);
        if (!b) return;

        GameObject gifteffect = null;
        switch (gift.m_Effects)
        {
            case "��Ů����Ч": gifteffect = Gift_1Effects; break;
            case "����Ȧ��Ч": gifteffect = Gift_2Effects; break;
            case "���������Ч": gifteffect = Gift_3Effects; break;
            case "��ħը����Ч": gifteffect = Gift_4Effects; break;
            case "���ؿ�Ͷ��Ч": gifteffect = Gift_5Effects; break;
            case "����������Ч": gifteffect = Gift_6Effects; break;

            case "��Ů��������Ч": gifteffect = Gift_1UpLevelEffects; break;
            case "����Ȧ������Ч": gifteffect = Gift_2UpLevelEffects; break;
            case "�������������Ч": gifteffect = Gift_3UpLevelEffects; break;
            case "��ħը��������Ч": gifteffect = Gift_4UpLevelEffects; break;
            case "���ؿ�Ͷ������Ч": gifteffect = Gift_5UpLevelEffects; break;
            case "��������������Ч": gifteffect = Gift_6UpLevelEffects; break;
        }
        if (gifteffect == null) return;

        gifteffect.SetActive(true);//����Ч
        //���ݶ��黻��ɫ
        var giftEffectsPlaying = gifteffect.GetComponent<GiftEffectsPlaying>();
        if (giftEffectsPlaying != null)
        {
            giftEffectsPlaying.team = gift.m_team;
        }
        Is_EffectsPlaying = true;
        //���µ�ǰ���ڲ�������
        CurrentGiftPlaying = gift;



        //��������֪ͨ
        //PlayGiftNotif(in gift, in GiftMager, in spawn);
    }
    //�������֪ͨ
    public void AddGiftOntice(in GiftType gift)
    {
        switch (gift.m_team)
        {
            case layer.Team1: Team1_GiftNoticeEffectList.Add(gift); break;
            case layer.Team2: Team2_GiftNoticeEffectList.Add(gift); break;
        }
    }
    //��������֪ͨ����
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
    //��������֪ͨ
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

        GiftNotific.SetActive(true);//��ʼ����

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
    //����Ƿ�Ϊ�ɲ��ŵ�������Ч
    bool CheckGiftEffectsCanPlayed(in GiftType gift)
    {
        var gameRoot = GameRoot.Instance;
        bool b = true;
        var giftEffects = gift.m_Effects;
        if(giftEffects == "��Ů����Ч" || giftEffects == "��Ů��������Ч")
            b = gameRoot.Is_Gift1EffectsCanPlayed;
        else if (giftEffects == "����Ȧ��Ч" || giftEffects == "����Ȧ������Ч")
            b = gameRoot.Is_Gift2EffectsCanPlayed;
        else if (giftEffects == "���������Ч" || giftEffects == "�������������Ч")
            b = gameRoot.Is_Gift3EffectsCanPlayed;
        else if (giftEffects == "��ħը����Ч" || giftEffects == "��ħը��������Ч")
            b = gameRoot.Is_Gift4EffectsCanPlayed;
        else if (giftEffects == "���ؿ�Ͷ��Ч" || giftEffects == "���ؿ�Ͷ������Ч")
            b = gameRoot.Is_Gift5EffectsCanPlayed;
        else if (giftEffects == "����������Ч" || giftEffects == "��������������Ч")
            b = gameRoot.Is_Gift6EffectsCanPlayed;

        return b;
    }
    //�׳������ж�
    public void CheckFirstRechargeGift(in GiftType gift, Spawn spawn, EntityManager entiMager)
    {
        //�׳�����ﵽ���ƺ󣬲����׳䵥λ
        if (FirstRechargeDic.Count >= LimitPlayerNum)
            return;
        //����������Ѿ��͹��׳��ˣ����˳�
        if (FirstRechargeDic.ContainsKey(gift.m_OpneID))
            return;
        else//���û�������ң��͸�����һ���׳䵥λ
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
            FirstRechargeDic.Add(gift.m_OpneID, player);//�������֮�󲻻��ٻ���׳佱��
        }
        FirstRechargeText.text = $"���{FirstRechargeDic.Count}/{LimitPlayerNum}";
    }
    //����ͼ����ʾ
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
