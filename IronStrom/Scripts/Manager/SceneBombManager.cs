using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceWaveNotice//��������֪ͨ
{
    //ΨһID
    public string m_Open_ID;
    //�ǳ�
    public string m_Nick;
    //ͷ��
    public string m_Avatar;
    //��������
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

    [Tooltip("���˻���")] public int VoiceWave_Shield;
    [Tooltip("���˺�ը")] public int VoiceWave_Missile;
    [Tooltip("���˼���")] public int VoiceWave_Laser;
    [Tooltip("����ԭ�ӵ�")] public int VoiceWave_NucleaBomb;

    [Tooltip("���˼������ӵĻ���ֵ")] public int ShieldValue;

    public List<VoiceWaveNotice> VoiceWaveNoticeList;//����֪ͨ����

    public GameObject VoiceWaveNoticeObj;// ����֪ͨ
    [HideInInspector] public bool Is_VoiceWaveNoticePlaying;//�Ƿ����ڲ�������֪ͨ



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
        //��������֪ͨ����
        RunVoicWaveNotice();


    }

    //�������֪ͨ
    public void AddVoicWaveNotice(in PlayerData player)
    {
        VoiceWaveNoticeList.Add(new VoiceWaveNotice(in player));
    }

    //��������֪ͨ����
    void RunVoicWaveNotice()
    {
        if (VoiceWaveNoticeList.Count > 0 && !Is_VoiceWaveNoticePlaying)
        {
            var voiceWave = VoiceWaveNoticeList.FirstOrDefault();
            PlayVoicWaveNotice(in voiceWave);
            VoiceWaveNoticeList.RemoveAt(0);
        }
    }
    //��������֪ͨ
    void PlayVoicWaveNotice(in VoiceWaveNotice voiceWave)
    {
        var TeamMager = TeamManager.teamManager;
        if (TeamMager == null) return;


        var voiceWaveNotifData = VoiceWaveNoticeObj.GetComponent<VoiceWave_NotificationData>();
        VoiceWaveNoticeObj.SetActive(true);//��ʼ����

        var teamMager = TeamManager.teamManager;
        if (voiceWaveNotifData == null || teamMager == null) return;

        voiceWaveNotifData.Headshot.sprite = Resources.Load<Sprite>(voiceWave.m_Avatar);
        voiceWaveNotifData.PlayerName.text = voiceWave.m_Nick;
        voiceWaveNotifData.VoiceWaveName.text = BySceneBombType(in voiceWave.m_sceneBomb);
        voiceWaveNotifData.VoiceWaveValue.text = $"���˴ﵽ<color=#FFC90E>{BySceneBombTypeGetValue(in voiceWave.m_sceneBomb)}</color>";

        Is_VoiceWaveNoticePlaying = true;
    }
    //ͨ���������ͻ������
    string BySceneBombType(in SceneBombType scenebombType)
    {
        string type = "";

        switch (scenebombType)
        {
            case SceneBombType.Shile:type = "���⻤��"; break;
            case SceneBombType.Laser:type = "����ɨ��"; break;
            case SceneBombType.MissileAirStrike:type = "ȫͼ��ը"; break;
            case SceneBombType.NucleaBomb:type = "����ը��"; break;
        }

        return type;
    }
    //ͨ���������ͻ����������
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
