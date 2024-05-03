using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftEffectsPlaying : MonoBehaviour
{
    [HideInInspector] public layer team;//�ĸ����鲥��������Ч
    public Material Team1Mate;//����1�Ĳ�����
    public Material Team2Mate;//����2�Ĳ�����

    public SkinnedMeshRenderer[] ShiBingMesh;
    [Tooltip("�Ƿ�Ϊ������Ч")]public bool Is_GiftEffects;
    public Animator animator;
    public ParticleSystem particle_1;
    public ParticleSystem particle_2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //����ɫ
        ChangeMeshRenderer();



    }

    //����ɫ
    void ChangeMeshRenderer()
    {
        if (ShiBingMesh.Length <= 0) return;
        if(Is_GiftEffects)
        {
            if(team == layer.Team1 && Team1Mate != null)
            {
                foreach (var mesh in ShiBingMesh)
                    mesh.material = Team1Mate;
            }
            else if(team == layer.Team2 && Team2Mate != null)
            {
                foreach (var mesh in ShiBingMesh)
                    mesh.material = Team2Mate;
            }
            Is_GiftEffects = false;
        }
    }


    //������Ч�ر��Լ�
    public void Gift_1Effects()
    {
        GiftManager.Instance.Is_EffectsPlaying = false;
        gameObject.SetActive(false);
        Is_GiftEffects = true;
    }
    //֪ͨ��Ч�ر��Լ�
    public void CloseTeam1GiftNotice()
    {
        GiftManager.Instance.Is_Team1_EffectsNoticePlaying = false;
        gameObject.SetActive(false);
    }
    public void CloseTeam2GiftNotice()
    {
        GiftManager.Instance.Is_Team2_EffectsNoticePlaying = false;
        gameObject.SetActive(false);
    }
    //�ر�����֪ͨ
    public void CloseVoiceWaveNotice()
    {
        SceneBombManager.instance.Is_VoiceWaveNoticePlaying = false;
        gameObject.SetActive(false);
    }

    public void GiftAnimPlayIdle()//�����������ö���
    {
        animator.Play("Gift_Idle");
    }
    public void GiftAnimPlayWalk()//�����������߶���
    {
        animator.Play("Gift_Walk");
    }
    public void GiftAnimPlayFire()//�������Ź�������
    {
        animator.Play("Gift_Fire");
    }

    public void GiftAnimPlayParticle_1()//������Ч
    {
        particle_1.Play();
    }
    public void GiftAnimPlayParticle_2()//������Ч
    {
        particle_2.Play();
    }
    public void GfitAnimStopParticle_1()//�ر���Ч
    {
        particle_1.Stop();
    }
    public void GfitAnimStopParticle_2()//�ر���Ч
    {
        particle_2.Stop();
    }

}
