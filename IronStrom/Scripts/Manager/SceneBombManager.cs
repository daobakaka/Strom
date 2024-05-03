using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceWaveNotice//音浪礼物通知
{
    //唯一ID
    public string m_Open_ID;
    //昵称
    public string m_Nick;
    //头像
    public string m_Avatar;
    //音浪类型
    public SceneBombType m_sceneBomb;

    public VoiceWaveNotice(in PlayerData player)
    {
        m_Open_ID = player.m_Open_ID;
        m_Nick = player.m_Nick;
        m_Avatar = player.m_Avatar;
        m_sceneBomb = player.m_sceneBombType;
    }
}

public class SceneBombManager : MonoBehaviour
{
    private static SceneBombManager _SceneBombManager;
    public static SceneBombManager instance { get { return _SceneBombManager; } }

    [Tooltip("音浪护盾")] public int VoiceWave_Shield;
    [Tooltip("音浪轰炸")] public int VoiceWave_Missile;
    [Tooltip("音浪激光")] public int VoiceWave_Laser;
    [Tooltip("音浪原子弹")] public int VoiceWave_NucleaBomb;

    [Tooltip("音浪技能增加的护盾值")] public int ShieldValue;

    public List<VoiceWaveNotice> VoiceWaveNoticeList;//音浪通知链表

    public GameObject VoiceWaveNoticeObj;// 音浪通知
    [HideInInspector] public bool Is_VoiceWaveNoticePlaying;//是否正在播放音浪通知



    private void Awake()
    {
        _SceneBombManager = this;

        VoiceWaveNoticeList = new List<VoiceWaveNotice>();
        Is_VoiceWaveNoticePlaying = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //播放礼物通知链表
        RunVoicWaveNotice();


    }

    //添加音浪通知
    public void AddVoicWaveNotice(in PlayerData player)
    {
        VoiceWaveNoticeList.Add(new VoiceWaveNotice(in player));
    }

    //播放礼物通知链表
    void RunVoicWaveNotice()
    {
        if (VoiceWaveNoticeList.Count > 0 && !Is_VoiceWaveNoticePlaying)
        {
            var voiceWave = VoiceWaveNoticeList.FirstOrDefault();
            PlayVoicWaveNotice(in voiceWave);
            VoiceWaveNoticeList.RemoveAt(0);
        }
    }
    //播放礼物通知
    void PlayVoicWaveNotice(in VoiceWaveNotice voiceWave)
    {
        var TeamMager = TeamManager.teamManager;
        if (TeamMager == null) return;


        var voiceWaveNotifData = VoiceWaveNoticeObj.GetComponent<VoiceWave_NotificationData>();
        VoiceWaveNoticeObj.SetActive(true);//开始播放

        var teamMager = TeamManager.teamManager;
        if (voiceWaveNotifData == null || teamMager == null) return;

        voiceWaveNotifData.Headshot.sprite = Resources.Load<Sprite>(voiceWave.m_Avatar);
        voiceWaveNotifData.PlayerName.text = voiceWave.m_Nick;
        voiceWaveNotifData.VoiceWaveName.text = BySceneBombType(in voiceWave.m_sceneBomb);
        voiceWaveNotifData.VoiceWaveValue.text = $"音浪达到<color=#FFC90E>{BySceneBombTypeGetValue(in voiceWave.m_sceneBomb)}</color>";

        Is_VoiceWaveNoticePlaying = true;
    }
    //通过音浪类型获得名字
    string BySceneBombType(in SceneBombType scenebombType)
    {
        string type = "";

        switch (scenebombType)
        {
            case SceneBombType.Shile:type = "激光护盾"; break;
            case SceneBombType.Laser:type = "激光扫射"; break;
            case SceneBombType.MissileAirStrike:type = "全图轰炸"; break;
            case SceneBombType.NucleaBomb:type = "重型炸弹"; break;
        }

        return type;
    }
    //通过音浪类型获得类型音浪
    int BySceneBombTypeGetValue(in SceneBombType scenebombType)
    {
        int value = 0;
        switch (scenebombType)
        {
            case SceneBombType.Shile: value = VoiceWave_Shield; break;
            case SceneBombType.Laser: value = VoiceWave_Laser; break;
            case SceneBombType.MissileAirStrike: value = VoiceWave_Missile; break;
            case SceneBombType.NucleaBomb: value = VoiceWave_NucleaBomb; break;
        }
        return value;
    }
}
