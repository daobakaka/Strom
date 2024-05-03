using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SynchronizeGameObj : MonoBehaviour
{
    Animator animator;


    [System.NonSerialized] public bool Is_EventFire_1 = false;
    [System.NonSerialized] public bool Is_EventFire_2 = false;
    [System.NonSerialized] public bool Is_PlayingAniFire = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 代表第一个层

        

    }



    public void PlayAni(ActState actstate,ShiBingName name,float AniSpeed,bool Is_Air)
    {
        switch(name)
        {
            case ShiBingName.HuoShen : HuoShenAni(actstate, AniSpeed); break;
            case ShiBingName.RongDian: RongDianAni(actstate, AniSpeed); break;
            case ShiBingName.Monster_1: Monster1Ani(actstate); break;
            case ShiBingName.Monster_3: Monster3Ani(actstate, Is_Air); break;
            case ShiBingName.Monster_4: Monster4Ani(actstate); break;
            case ShiBingName.Monster_5: Monster5Ani(actstate); break;
            case ShiBingName.Monster_6: Monster6Ani(actstate); break;
            case ShiBingName.Monster_7: Monster7Ani(actstate); break;
        }
    }
    //火神的动画
    void HuoShenAni(ActState actstate, float AniSpeed)
    {
        if (animator == null)
            return;
        animator.speed = AniSpeed;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("battle_idle");break;
            case ActState.Walk: animator.Play("walk_d1"); break;
            case ActState.Move: animator.Play("walk_d1"); break;
            case ActState.Ready: animator.Play("battle_idle"); break;
            case ActState.Fire: animator.Play("battle_idle"); break;
        }
    }
    //熔点的动画
    void RongDianAni(ActState actstate, float AniSpeed)
    {
        if (animator == null)
            return;
        animator.speed = AniSpeed;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("Idle"); break;
            case ActState.Walk: animator.Play("Legs_Spider_Med_Walk"); break;
            case ActState.Move: animator.Play("Legs_Spider_Med_Walk"); break;
            case ActState.Ready: animator.Play("Idle"); break;
            case ActState.Fire: animator.Play("Idle"); break;
        }
    }
    //怪物1的动画
    void Monster1Ani(ActState actstate)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("Idle"); break;
            case ActState.Walk: animator.Play("Walk"); break;
            case ActState.Move: animator.Play("Walk"); break;
            case ActState.Ready: animator.Play("Idle"); break;
            case ActState.Fire: animator.Play("SmashAttack"); break;
            case ActState.Appear: animator.Play("Walk"); break;
        }
    }
    //怪物3的动画
    void Monster3Ani(ActState actstate, bool Is_Air)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("IdleBreathe"); break;
            case ActState.Walk: animator.Play("Walk"); break;
            case ActState.Move: animator.Play("Walk"); break;
            case ActState.Ready: animator.Play("IdleBreathe"); break;
            case ActState.Appear: animator.Play("Walk"); break;
        }
        if(actstate == ActState.Fire)
        {
            if (Is_Air)
                animator.Play("TailAttack");
            else
                animator.Play("BiteAttack");
        }
    }
    //怪物4的动画
    void Monster4Ani(ActState actstate)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("Idle_1"); break;
            case ActState.Walk: animator.Play("Walk_1"); break;
            case ActState.Move: animator.Play("Walk_1"); break;
            case ActState.Ready: animator.Play("Idle_1"); break;
            case ActState.Fire: animator.Play("BiteAttack_1"); break;
            case ActState.Appear: animator.Play("Walk"); break;
        }
    }
    //怪物5的动画
    void Monster5Ani(ActState actstate)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("Idle"); break;
            case ActState.Walk: animator.Play("Walk"); break;
            case ActState.Move: animator.Play("Walk"); break;
            case ActState.Ready: animator.Play("Idle"); break;
            case ActState.Fire: animator.Play("2HitComboAttack_1"); break;
            case ActState.Appear: animator.Play("Walk"); break;
        }
    }
    //怪物6的动画
    void Monster6Ani(ActState actstate)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("IdleBreathe_1"); break;
            case ActState.Walk: animator.Play("Walk_1"); break;
            case ActState.Move: animator.Play("Walk_1"); break;
            case ActState.Ready: animator.Play("IdleBreathe_1"); break;
            case ActState.Fire: animator.Play("SmashAttack_1"); break;
            case ActState.Appear: animator.Play("Walk_1"); break;
        }
    }
    //怪物7的动画
    void Monster7Ani(ActState actstate)
    {
        if (animator == null)
            return;
        switch (actstate)
        {
            case ActState.Idle: animator.Play("FlyForward"); break;
            case ActState.Walk: animator.Play("FlyForward"); break;
            case ActState.Move: animator.Play("FlyForward"); break;
            case ActState.Ready: animator.Play("FlyForward"); break;
            case ActState.Fire: animator.Play("FlyNormalGetHit"); break;
            case ActState.Appear: animator.Play("FlyForward"); break;
        }
    }


    void MonsterEvent_Fire_1()
    {
        Is_EventFire_1 = true;
    }
    void MonsterEvent_Fire_2()
    {
        Is_EventFire_2 = true;
    }
    void AniPlayingFier()
    {
        Is_PlayingAniFire = true;
    }

}
