using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioType
{
    [HideInInspector] public AudioSource Source;//��ƵԴ
    public AudioClip Clip;//��ƵƬ��
    //public AudioMixerGroup Group;//��Ƶ���

    public string Name;//��Ƶ����
    //public float Volume;//����
    public float Pitch;//�����ٶ�
    public bool Loop;//�Ƿ�ѭ������

}
