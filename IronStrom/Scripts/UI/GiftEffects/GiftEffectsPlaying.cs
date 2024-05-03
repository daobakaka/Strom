using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftEffectsPlaying : MonoBehaviour
{
    [HideInInspector] public layer team;//哪个队伍播放礼物特效
    public Material Team1Mate;//队伍1的材质球
    public Material Team2Mate;//队伍2的材质球

    public SkinnedMeshRenderer[] ShiBingMesh;
    [Tooltip("是否为礼物特效")]public bool Is_GiftEffects;
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
        //换颜色
        ChangeMeshRenderer();



    }

    //换颜色
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


    //礼物特效关闭自己
    public void Gift_1Effects()
    {
        GiftManager.Instance.Is_EffectsPlaying = false;
        gameObject.SetActive(false);
        Is_GiftEffects = true;
    }
    //通知特效关闭自己
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
    //关闭音浪通知
    public void CloseVoiceWaveNotice()
    {
        SceneBombManager.instance.Is_VoiceWaveNoticePlaying = false;
        gameObject.SetActive(false);
    }

    public void GiftAnimPlayIdle()//动画播放闲置动画
    {
        animator.Play("Gift_Idle");
    }
    public void GiftAnimPlayWalk()//动画播放行走动画
    {
        animator.Play("Gift_Walk");
    }
    public void GiftAnimPlayFire()//动画播放攻击动画
    {
        animator.Play("Gift_Fire");
    }

    public void GiftAnimPlayParticle_1()//播放特效
    {
        particle_1.Play();
    }
    public void GiftAnimPlayParticle_2()//播放特效
    {
        particle_2.Play();
    }
    public void GfitAnimStopParticle_1()//关闭特效
    {
        particle_1.Stop();
    }
    public void GfitAnimStopParticle_2()//关闭特效
    {
        particle_2.Stop();
    }

}
