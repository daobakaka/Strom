using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    private static AudioManager _AudioManager;
    public static AudioManager Instance { get { return _AudioManager; } }

    [Header("音乐")]
    public List<AudioClip> MusicAudioClipList;//音乐
    public AudioSource MusicSource;//音乐源
    public AudioMixerGroup musicMixerGroup;//音乐组
    public int currentTrackIndex = 0; // 当前播放的音轨索引
    [Header("音效")]
    public List<AudioType> EffectAudioTypeList;//音效
    public AudioMixerGroup EffectMixerGroup;//音效组
    public float EffectVolume;//所有音效的音量


    private void Awake()
    {
        if (_AudioManager == null)
            _AudioManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InitAudio();
        //MusicSource.volume = 0.5f;
    }

    private void Start()
    {


    }

    public void InitAudio()
    {
        //初始化音乐 音效
        //foreach (var audio in MusicAudioTypeList)
        //{
        //    audio.Source = gameObject.AddComponent<AudioSource>();

        //    audio.Source.clip = audio.Clip;
        //    audio.Source.name = audio.Name;
        //    audio.Source.volume = audio.Volume;
        //    audio.Source.pitch = audio.Pitch;
        //    audio.Source.loop = audio.Loop;
        //    if (musicMixerGroup != null)
        //        audio.Source.outputAudioMixerGroup = musicMixerGroup;
        //}
        foreach (var audio in EffectAudioTypeList)
        {
            audio.Source = gameObject.AddComponent<AudioSource>();

            audio.Source.clip = audio.Clip;
            audio.Source.name = audio.Name;
            audio.Source.volume = EffectVolume;
            audio.Source.pitch = audio.Pitch;
            audio.Source.loop = audio.Loop;
            if (EffectMixerGroup != null)
                audio.Source.outputAudioMixerGroup = EffectMixerGroup;
        }
    }

    //循环播放所有BGM
    public void PlayMusic()
    {
        // 如果播放列表中有曲子
        if (MusicAudioClipList.Count > 0)
        {
            // 设置AudioSource的clip为下一首曲子
            MusicSource.clip = MusicAudioClipList[currentTrackIndex];

            // 播放当前音轨
            MusicSource.Play();

            // 更新索引以播放下一首曲子
            currentTrackIndex = (currentTrackIndex + 1) % MusicAudioClipList.Count;
        }
    }

    public void PlayEffect(string name)//播放音效
    {
        foreach(var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Play();
                return;
            }
        }
        Debug.Log($"没有找到名为{name}的音乐");
    }
    public void PauseEffect(string name)//暂停音效
    {
        foreach (var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Pause();
                return;
            }
        }
        Debug.Log($"没有找到名为{name}的音乐");
    }
    public void StopEffect(string name)//停止音效
    {
        foreach (var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Stop();
                return;
            }
        }
        Debug.Log($"没有找到名为{name}的音乐");
    }

    public void UpdateAllEffectVolume(float volume)//刷新所有音效的音量
    {
        foreach(var type in EffectAudioTypeList)
        {
            type.Source.volume = volume;
        }
    }

}
