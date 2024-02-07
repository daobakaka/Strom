using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class Attack : MonoBehaviour
{/// <summary>
/// 攻击倍率及目标boss，目标敌人
/// </summary>
    public float attacktimes = 1;
    public GameObject priticle;
    /// <summary>
    /// its a magic control
    /// </summary>
    [Header("setting for magic")]
    [Tooltip("its a magic control,that if you add vfx,you may use control")]
    public string targetboss;
    public string targetenemy;
    /// <summary>
    /// other
    /// </summary>
    public Transform monther;
    /// <summary>
    /// 特效处理逻辑
    /// </summary>
    public bool ifvfx = false;
    public Vector3 offset;
    public Vector3 range;
    public Vector3 rotate;
    public float appearPI = 1;
    public float appearPICache;
    /// <summary>
    /// player ID modul
    /// </summary>
    public string ID;
    public MonsterType monsterType;
    public bool ifTS;//certain the monster is TS
    /// <summary>
    /// the module of finally skill
    /// </summary>
    public float attackchache;
    public bool amicecamp;
    /// <summary>
    /// module for test
    /// </summary>
    private bool ifsword;
    public float swordDuration = 1;
    public bool deathSword;
    
    private void Start()
    {
        if (ifvfx)
            monther = GameObject.FindWithTag("VFX").transform;

    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        if (ifvfx&&monther==null)
            monther = GameObject.FindWithTag("VFX").transform;
        attackchache = attacktimes;//save the chache of attacktimes
        StartCoroutine("IEAttackmonitoer");//start the monitor of damage add
        if (ifvfx)
            StartCoroutine("IESwordShow");
    }

    private void OnDisable()
    {
        StopCoroutine("IEAttackmonitoer");
        attacktimes = attackchache;
        if (ifvfx)
            StopCoroutine("IESwordShow");
    }
    private void OnTriggerEnter(Collider other)//show the collison
    {
        if (ifvfx && Random.Range(0, 1f) < appearPI)
        {
            if (other.tag.Equals(targetboss) || other.tag.Equals(targetenemy)||other.tag.Equals("WILD"))
            {


                if (ifsword && deathSword)
                {
                    GameObject clone = Netpool.Getinstance().Insgameobj(priticle, transform.position + offset + new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z)), Quaternion.Euler(rotate.x, rotate.y, rotate.z), monther);
                    ifsword = false;
                }
                else if (!deathSword)
                    Netpool.Getinstance().Insgameobj(priticle, transform.position + offset + new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z)), Quaternion.Euler(rotate.x, rotate.y, rotate.z), monther);

                //if(iftest)
                //Debug.Log("please look the sword shadow ,"+"name:  "+clone.name);

            }
        }

    }
    IEnumerator IEAttackmonitoer()//the Monitor of attacktime add  it despends on that if sides camp trriger finally skill
    {
        WaitForSeconds loop = new WaitForSeconds(1f);
        for (; ; )
        {
            yield return loop;
            if (Monsterins.ificecamp&&amicecamp)
            {
                attacktimes += attacktimes * 0.5f;
               
                break;

            }
            else if (Monsterins.iffirecamp&&!amicecamp)
            {


                attacktimes += attacktimes * 0.5f;
                break;
            }
        
        }
       // Debug.Log("add damage");
        yield return new WaitForSeconds(30f);//the method has just excute once
        attacktimes = attackchache;//let the attack recover
    
    }


    IEnumerator IESwordShow()//a control for close the particle 
    {
        WaitForSeconds loop = new WaitForSeconds(swordDuration);

        for (; ; )
        { yield return loop;
            ifsword = true;
        }
    }
}


