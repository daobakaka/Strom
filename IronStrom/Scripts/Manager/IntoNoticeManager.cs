using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntoNoticeManager : MonoBehaviour//入场通知管理器
{
    private static IntoNoticeManager _IntoNoticeManager;
    public static IntoNoticeManager Instance { get { return _IntoNoticeManager; } }

    public GameObject IntoNFrameObj;//入场通知框
    public VideoPlayer IntoNoticeVideo;//入场视频播放器
    public RawImage rawImage;//显示纹理
    //入场播放的视频
    public VideoClip IntoN1_3;
    public VideoClip IntoN4_10;
    public VideoClip IntoN11_30;
    public VideoClip IntoN31_50;


    [HideInInspector] public List<PlayerData> IntoNoticeQueueList;//入场通知队伍
    [HideInInspector] public bool Is_IntoNoticePlaying;//是否正在播放进场特效
    private IEnumerator currentCoroutine; // 用来存储当前运行的协程的引用
    private void Awake()
    {
        _IntoNoticeManager = this;
        IntoNoticeQueueList = new List<PlayerData>();
        Is_IntoNoticePlaying = false;
        IntoNoticeVideo.SetDirectAudioMute(0, true);//入场视频静音
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //播放入场通知链表
        PlayIntoNoticeQueue();
    }

    //播放入场通知链表
    void PlayIntoNoticeQueue()
    {
        if (IntoNoticeQueueList.Count <= 0 || Is_IntoNoticePlaying)//如果正在播放入场特效
            return;
        // 如果当前有协程运行，停止它
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        //打开特效
        IntoNFrameObj.SetActive(true);
        Is_IntoNoticePlaying = true;//正在播放入场特效
        IntoNoticeFrame IntoNFrame  = IntoNFrameObj.GetComponent<IntoNoticeFrame>();
        //拿到表里面的第一个出场玩家
        var player = IntoNoticeQueueList.First();

        float startAtSeconds = 0;//视频开始时间
        float stopAtSeconds = 0;//视频结束的时间
        //将玩家数据赋值给特效Obj
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
            // 启动协程并存储其引用
            currentCoroutine = PlayVideoSegment(startAtSeconds, stopAtSeconds, rawImage);
            StartCoroutine(currentCoroutine);
            //StartCoroutine(PlayVideoSegment(startAtSeconds, stopAtSeconds, rawImage));
        }

        IntoNFrame.PlayerHeadshot.sprite = Resources.Load<Sprite>(player.m_Avatar);
        IntoNFrame.playerName.text = $"<color=#fccb09>世界排名{player.m_Rank}</color><color=#00fff0>{player.m_Nick}";
        string team = player.m_Team == layer.Team1 ? "橙队" : "蓝队";
        IntoNFrame.IntoTeam.text = $"<color=#FF0000>加入了</color>{team}";
        IntoNoticeQueueList.RemoveAt(0);
    }

    private IEnumerator PlayVideoSegment(float startAtSeconds, float stopAtSeconds, RawImage rawImage)//播放视频段
    {
        //IntoNoticeVideo.time = startAtSeconds; // 跳转到视频的特定时间点
        //IntoNoticeVideo.Play(); // 开始播放视频
        //rawImage.enabled = true;
        //Debug.Log($"  播放的视频为:{IntoNoticeVideo.clip}  RawImage是否打开:{rawImage.enabled}");

        //while (IntoNoticeVideo.time < stopAtSeconds)//视频到了指定结束时间，就结束
        //    yield return null;

        //IntoNoticeVideo.Stop();
        //rawImage.enabled = false;
        //Debug.Log($"  视频关");





        // 清除RenderTexture以去除上一个视频的残留帧
        if (IntoNoticeVideo.targetTexture != null)
        {
            ClearRenderTexture(IntoNoticeVideo.targetTexture);
        }
        rawImage.enabled = true;
        IntoNoticeVideo.time = startAtSeconds;
        IntoNoticeVideo.Play();

        // 等待视频准备就绪
        yield return new WaitUntil(() => IntoNoticeVideo.isPrepared);
        // 等待视频开始播放
        yield return new WaitForSeconds(0.1f);
        // 确保视频播放，并检查是否到达了结束时间
        while (IntoNoticeVideo.isPlaying && IntoNoticeVideo.time < stopAtSeconds)
        {
            yield return null; // 等待下一帧
        }
        // 等待直到达到或超过stopAtSeconds，防止提前结束
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
