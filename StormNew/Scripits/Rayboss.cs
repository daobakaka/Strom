using Games.Characters.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Subscriber;


public class Rayboss : MonoBehaviour
{
    // Start is called before the first frame update
    public string name;
    private Animator animator0;
    public static float ICEbossHP = 10000;
    public static float FIREbossHP = 10000;
    public float baseHp;
    /// <summary>
    /// boss health module
    /// </summary>
    public string enemy;
    public string enemyfire;
    public string enemybullet;
    public string enemyskill;
    public bool isICEboss;
    public string mytag;
    /// <summary>
    /// the VFX of Attack module
    /// </summary>
    public Transform hitPosition;
    public GameObject hitVFX;
    public GameObject vfx;
    /// <summary>
    /// the module of BOSS damage cut off 90%;
    /// </summary>
    public GameObject damageCutOff;
    public Vector3 damageCutOff_offset;
    private bool casticeskill;
    private bool castfireskill;
    /// <summary>
    /// the module of pubisher
    /// </summary>
    public EventHandler<MyCustomEventArgs> EMyEvent;
    void Start()
    {
        vfx = GameObject.FindWithTag("VFX");
        animator0 = GetComponent<Animator>();
        initBoss();
        // StartCoroutine("IEHealthupdate");//boss health update in  cycle
        Debug.Log("it is me !!!!!!!!!!!!!!!!!!!");
       // initBoss();
    }
    private void OnEnable()
    {
        casticeskill = true;
        castfireskill = true;
        //---

        //--
        StartCoroutine("IEBssSkill_Ef_Recover");
        //--
       if(isICEboss)
        StartCoroutine("IEHealthupdate");//boss health update in  cycle
        //
        StartCoroutine("IEBossSkillForDamageReduce");
        StartCoroutine("IESelfReduce");
        
    }
    private void OnDisable()
    {
        StopCoroutine("IEBossSkillForDamageReduce");
        //--
        StopCoroutine("IEBssSkill_Ef_Recover");
        //--
        StopCoroutine("IEHealthupdate");//boss health update in  cycle
        StopAllCoroutines();
    }
    public void initBoss()//******wjy code*****
    {
       // Debug.LogError("it is me !!!!!!!!!!!!!!!!!!!");
        Monsterins.instance.ifbooslive = true;
        baseHp = PlayerPrefs.GetInt(HttpRquest.BaseHpKey, HttpRquest.instance.globalBloodList[0]);
        ICEbossHP = baseHp;
        FIREbossHP = baseHp;
        UIManager.instance.UpdateHPBarValue(0, (long)ICEbossHP, (float)ICEbossHP / (float)baseHp,true);
        UIManager.instance.UpdateHPBarValue(3, (long)FIREbossHP, (float)FIREbossHP / (float)baseHp,true);

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("icebosshealth="+ICEbossHP);               
            }
        }
        if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("firebosshealth=" +FIREbossHP);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(name)) 
        {
           
            animator0.SetTrigger("Attack");
            // Debug.Log("fire" + "-----------------------------------------------------------------------------------------");
          //  TriggerEvent("kill the boss");
        }

        if (other.tag.Equals(enemy))
        {
            other.gameObject.TryGetComponent<Attack>(out Attack component);
           
                if (isICEboss)
                    ICEbossHP -= 0.5f * Monsterins.protectBossICE_assist;
                else
                    FIREbossHP -= 0.5f * Monsterins.protectBossICE_assist;
            
            UIManager.instance.UpdateHPBarValue(0, (long)ICEbossHP, (float)ICEbossHP / (float)baseHp);
            UIManager.instance.UpdateHPBarValue(3, (long)FIREbossHP, (float)FIREbossHP / (float)baseHp);
            //Debug.Log("收到攻击");
            GameManager.instance.AddSelfScoreAttakBoss(component);
        }

        if (other.tag.Equals(enemyfire))

        {
            other.gameObject.TryGetComponent<Attack>(out Attack component);
            if (component.body.GetComponent<Monstermove>().showtarget.tag == mytag)
            {
                if (isICEboss)
                    ICEbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;
                else
                    FIREbossHP -= component.attacktimes * Monsterins.protectBossFIRE_assist;
            }
            GameManager.instance.AddSelfScoreAttakBoss(component);

        }
        if ( other.tag.Equals(enemybullet))

        {
            other.gameObject.TryGetComponent<Attack>(out Attack component);
            
                if (isICEboss)
                    ICEbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;
                else
                    FIREbossHP -= component.attacktimes * Monsterins.protectBossFIRE_assist;
            
            GameManager.instance.AddSelfScoreAttakBoss(component);

        }
        if (other.tag.Equals(enemyskill))
        {
            other.gameObject.TryGetComponent<Attack>(out Attack component);
            if (isICEboss)
                ICEbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;
            else
                FIREbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;

            UIManager.instance.UpdateHPBarValue(0, (long)ICEbossHP, (float)ICEbossHP / (float)baseHp);
            UIManager.instance.UpdateHPBarValue(3, (long)FIREbossHP, (float)FIREbossHP / (float)baseHp);
            // Debug.Log("收到攻击");
            GameManager.instance.AddSelfScoreAttakBoss(component);
        }
        if (other.tag.Equals("WILD"))//when the wild enter into the boss field;
        {
            animator0.SetTrigger("Attack");
            if (isICEboss)
                ICEbossHP -= 0.5f * Monsterins.protectBossICE_assist;
            else
                FIREbossHP -= 0.5f * Monsterins.protectBossICE_assist;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(name)||other.tag.Equals("WILD"))
        {
            animator0.SetTrigger("Attack");
           // Debug.Log("精英怪出圈了");
            // Debug.Log("fire" + "-----------------------------------------------------------------------------------------");
        }
       

    }
    /// <summary>
    /// this is a coroutine for boss that its health update self
    /// </summary>
    /// <returns></returns>
    IEnumerator IEHealthupdate()
    {
        WaitForSeconds loop = new WaitForSeconds(Time.deltaTime);
        Debug.Log("start monitor");
        
        for (; ;)
        {

            if (ICEbossHP > 0 && FIREbossHP > 0)
            {
                UIManager.instance.UpdateHPBarValue(0, (long)ICEbossHP, (float)ICEbossHP / (float)baseHp);
                UIManager.instance.UpdateHPBarValue(3, (long)FIREbossHP, (float)FIREbossHP / (float)baseHp);
            }
            if (FIREbossHP < 50 && FIREbossHP >=20)
            {
                FIREbossHP -= 0.05f;
            }
            if (ICEbossHP < 50 && ICEbossHP >= 20)
            {
                ICEbossHP -= 0.05f;
            }
            if (FIREbossHP < 20 && FIREbossHP >0)
            {
                FIREbossHP -= 0.3f;
            }
            if (ICEbossHP < 20 && ICEbossHP >0)
            {
                ICEbossHP -= 0.3f;
            }
            if (ICEbossHP <=0 || FIREbossHP <= 0)
                break;
            yield return loop;
            
        }
    
    }
  public  void AnimationVFX()//boss attack effects;
    {
        Netpool.Getinstance().Insgameobj(hitVFX, hitPosition.position, Quaternion.identity, vfx.transform);
    }
    IEnumerator IEBossSkillForDamageReduce()//the IECorotine that to check the skill of last minute;
    {
        yield return new WaitForSeconds(3);
        WaitForSeconds loop = new WaitForSeconds(0.5f);
        Debug.Log("start protect time time time time  monitor!!!!!!!!!!!!!!!!!!!!");
        for (; ICEbossHP > 0;)
        { yield return loop;
            if (Monsterins.protectBossICE && isICEboss && casticeskill)
            {
                Netpool.Getinstance().Insgameobj(damageCutOff, transform.position + damageCutOff_offset, Quaternion.Euler(-90, 0, 0), vfx.transform);
                casticeskill = false;
                Debug.Log("its time to protect my king!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            else if (Monsterins.protectBossFIRE && !isICEboss && castfireskill)
            { Netpool.Getinstance().Insgameobj(damageCutOff, transform.position + damageCutOff_offset, Quaternion.Euler(-90, 0, 0), vfx.transform);
                castfireskill = false;
                Debug.Log("its time to protect my king!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

            if (FIREbossHP < 0)
                break;
        }   
    }
    IEnumerator IEBssSkill_Ef_Recover()
    {
        WaitForSeconds loop = new WaitForSeconds(5f);
      //  Debug.Log("start protect monitor!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"+ Monsterins.protectBossICE+ Monsterins.protectBossFIRE+ castfireskill);
        for (; ICEbossHP > 0;)////
        {
            yield return loop;
            casticeskill = true;
            castfireskill = true;
  //  Debug.Log("start protect monitor!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + Monsterins.protectBossICE + Monsterins.protectBossFIRE + castfireskill+ICEbossHP+"  "+FIREbossHP);
            if (FIREbossHP < 0)
                break;
        }
    }
    protected virtual void OnMyEvent(string messsage)
    {
        EMyEvent?.Invoke(this, new MyCustomEventArgs(messsage));
    }
    public void TriggerEvent(string message)
    {
        OnMyEvent(message);

    }
    IEnumerator IESelfReduce()
    {
        yield return new WaitForSeconds(10);
        ICEbossHP -= 0.1f;
        FIREbossHP -= 0.1f;
        UIManager.instance.UpdateHPBarValue(0, (long)ICEbossHP, (float)ICEbossHP / (float)baseHp);
        UIManager.instance.UpdateHPBarValue(3, (long)FIREbossHP, (float)FIREbossHP / (float)baseHp);

    }
}
