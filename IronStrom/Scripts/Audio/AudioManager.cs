using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    private static AudioManager _AudioManager;
    public static AudioManager Instance { get { return _AudioManager; } }

    [Header("����")]
    public List<AudioClip> MusicAudioClipList;//����
    public AudioSource MusicSource;//����Դ
    public AudioMixerGroup musicMixerGroup;//������
    public int currentTrackIndex = 0; // ��ǰ���ŵ���������
    [Header("��Ч")]
    public List<AudioType> EffectAudioTypeList;//��Ч
    public AudioMixerGroup EffectMixerGroup;//��Ч��
    public float EffectVolume;//������Ч������


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
        //��ʼ������ ��Ч
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

    //ѭ����������BGM
    public void PlayMusic()
    {
        // ��������б���������
        if (MusicAudioClipList.Count > 0)
        {
            // ����AudioSource��clipΪ��һ������
            MusicSource.clip = MusicAudioClipList[currentTrackIndex];

            // ���ŵ�ǰ����
            MusicSource.Play();

            // ���������Բ�����һ������
            currentTrackIndex = (currentTrackIndex + 1) % MusicAudioClipList.Count;
        }
    }

    public void PlayEffect(string name)//������Ч
    {
        foreach(var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Play();
                return;
            }
        }
        Debug.Log($"û���ҵ���Ϊ{name}������");
    }
    public void PauseEffect(string name)//��ͣ��Ч
    {
        foreach (var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Pause();
                return;
            }
        }
        Debug.Log($"û���ҵ���Ϊ{name}������");
    }
    public void StopEffect(string name)//ֹͣ��Ч
    {
        foreach (var type in EffectAudioTypeList)
        {
            if (type.Name == name)
            {
                type.Source.Stop();
                return;
            }
        }
        Debug.Log($"û���ҵ���Ϊ{name}������");
    }

    public void UpdateAllEffectVolume(float volume)//ˢ��������Ч������
    {
        foreach(var type in EffectAudioTypeList)
        {
            type.Source.volume = volume;
        }
    }

}
