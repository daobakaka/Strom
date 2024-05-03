using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioType
{
    [HideInInspector] public AudioSource Source;//音频源
    public AudioClip Clip;//音频片段
    //public AudioMixerGroup Group;//音频轨道

    public string Name;//音频名字
    //public float Volume;//音量
    public float Pitch;//播放速度
    public bool Loop;//是否循环播放

}
