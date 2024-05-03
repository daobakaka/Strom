using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntoNoticeManager : MonoBehaviour//�볡֪ͨ������
{
    private static IntoNoticeManager _IntoNoticeManager;
    public static IntoNoticeManager Instance { get { return _IntoNoticeManager; } }

    public GameObject IntoNFrameObj;//�볡֪ͨ��
    public VideoPlayer IntoNoticeVideo;//�볡��Ƶ������
    public RawImage rawImage;//��ʾ����
    //�볡���ŵ���Ƶ
    public VideoClip IntoN1_3;
    public VideoClip IntoN4_10;
    public VideoClip IntoN11_30;
    public VideoClip IntoN31_50;


    [HideInInspector] public List<PlayerData> IntoNoticeQueueList;//�볡֪ͨ����
    [HideInInspector] public bool Is_IntoNoticePlaying;//�Ƿ����ڲ��Ž�����Ч
    private IEnumerator currentCoroutine; // �����洢��ǰ���е�Э�̵�����
    private void Awake()
    {
        _IntoNoticeManager = this;
        IntoNoticeQueueList = new List<PlayerData>();
        Is_IntoNoticePlaying = false;
        IntoNoticeVideo.SetDirectAudioMute(0, true);//�볡��Ƶ����
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�����볡֪ͨ����
        PlayIntoNoticeQueue();
    }

    //�����볡֪ͨ����
    void PlayIntoNoticeQueue()
    {
        if (IntoNoticeQueueList.Count <= 0 || Is_IntoNoticePlaying)//������ڲ����볡��Ч
            return;
        // �����ǰ��Э�����У�ֹͣ��
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        //����Ч
        IntoNFrameObj.SetActive(true);
        Is_IntoNoticePlaying = true;//���ڲ����볡��Ч
        IntoNoticeFrame IntoNFrame  = IntoNFrameObj.GetComponent<IntoNoticeFrame>();
        //�õ�������ĵ�һ���������
        var player = IntoNoticeQueueList.First();

        float startAtSeconds = 0;//��Ƶ��ʼʱ��
        float stopAtSeconds = 0;//��Ƶ������ʱ��
        //��������ݸ�ֵ����ЧObj
        if (player.m_Rank < 4)
        {
            IntoNFrame.IntoNoticeFrame1_3.SetActive(true);
            IntoNoticeVideo.clip = IntoN1_3;
            startAtSeconds = 2;
            stopAtSeconds = 7;
            //startAtSeconds = 8;
            //stopAtSeconds = 13;
            IntoNoticeVideo.SetDirectAudioMute(0, true);
        }
        else if (player.m_Rank <= 10)
        {
            IntoNFrame.IntoNoticeFrame4_10.SetActive(true);
            IntoNoticeVideo.clip = IntoN4_10;
            startAtSeconds = 8;
            stopAtSeconds = 13;
            //startAtSeconds = 1;
            //stopAtSeconds = 6;
            IntoNoticeVideo.SetDirectAudioMute(0, true);
        }
        else if (player.m_Rank <= 30)
        {
            IntoNFrame.IntoNoticeFrame11_30.SetActive(true);
            IntoNoticeVideo.clip = IntoN11_30;
            //IntoNoticeVideo.clip = IntoN4_10;
            startAtSeconds = 7;
            stopAtSeconds = 12;
            IntoNoticeVideo.SetDirectAudioMute(0, true);
        }
        else if (player.m_Rank <= 50)
        {
            IntoNFrame.IntoNoticeFrame31_50.SetActive(true);
            IntoNoticeVideo.clip = IntoN31_50;
            startAtSeconds = 0;
            stopAtSeconds = 5;
            //startAtSeconds = 5;
            //stopAtSeconds = 10;
            IntoNoticeVideo.SetDirectAudioMute(0, true);
        }
        if(stopAtSeconds > 0)
        {
            // ����Э�̲��洢������
            currentCoroutine = PlayVideoSegment(startAtSeconds, stopAtSeconds, rawImage);
            StartCoroutine(currentCoroutine);
            //StartCoroutine(PlayVideoSegment(startAtSeconds, stopAtSeconds, rawImage));
        }

        IntoNFrame.PlayerHeadshot.sprite = Resources.Load<Sprite>(player.m_Avatar);
        IntoNFrame.playerName.text = $"<color=#fccb09>��������{player.m_Rank}</color><color=#00fff0>{player.m_Nick}";
        string team = player.m_Team == layer.Team1 ? "�ȶ�" : "����";
        IntoNFrame.IntoTeam.text = $"<color=#FF0000>������</color>{team}";
        IntoNoticeQueueList.RemoveAt(0);
    }

    private IEnumerator PlayVideoSegment(float startAtSeconds, float stopAtSeconds, RawImage rawImage)//������Ƶ��
    {
        //IntoNoticeVideo.time = startAtSeconds; // ��ת����Ƶ���ض�ʱ���
        //IntoNoticeVideo.Play(); // ��ʼ������Ƶ
        //rawImage.enabled = true;
        //Debug.Log($"  ���ŵ���ƵΪ:{IntoNoticeVideo.clip}  RawImage�Ƿ��:{rawImage.enabled}");

        //while (IntoNoticeVideo.time < stopAtSeconds)//��Ƶ����ָ������ʱ�䣬�ͽ���
        //    yield return null;

        //IntoNoticeVideo.Stop();
        //rawImage.enabled = false;
        //Debug.Log($"  ��Ƶ��");





        // ���RenderTexture��ȥ����һ����Ƶ�Ĳ���֡
        if (IntoNoticeVideo.targetTexture != null)
        {
            ClearRenderTexture(IntoNoticeVideo.targetTexture);
        }
        rawImage.enabled = true;
        IntoNoticeVideo.time = startAtSeconds;
        IntoNoticeVideo.Play();

        // �ȴ���Ƶ׼������
        yield return new WaitUntil(() => IntoNoticeVideo.isPrepared);
        // �ȴ���Ƶ��ʼ����
        yield return new WaitForSeconds(0.1f);
        // ȷ����Ƶ���ţ�������Ƿ񵽴��˽���ʱ��
        while (IntoNoticeVideo.isPlaying && IntoNoticeVideo.time < stopAtSeconds)
        {
            yield return null; // �ȴ���һ֡
        }
        // �ȴ�ֱ���ﵽ�򳬹�stopAtSeconds����ֹ��ǰ����
        yield return new WaitUntil(() => IntoNoticeVideo.time >= stopAtSeconds);
        IntoNoticeVideo.Stop();
        rawImage.enabled = false;

    }
    void ClearRenderTexture(RenderTexture rt)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = currentRT;
    }
}
