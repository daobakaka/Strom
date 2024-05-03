using Games.Characters.EliteUnits;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class Attack : MonoBehaviour
{/// <summary>
/// �������ʼ�Ŀ��boss��Ŀ�����
/// </summary>
    public float attacktimes = 1;
    public float damage;
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
    /// ��Ч�����߼�
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
    public MonsterData monsterData;
    public bool ifTS;//certain the monster is TS
    /// <summary>
    /// the module of finally skill
    /// </summary>
    public float attackchache;
    /// <summary>
    /// module for test
    /// </summary>
   // private bool ifsword;
    public float swordDuration = 1;
    //public bool deathSword;
    public Transform body;
    private BoxCollider boxCollider;
    //�Ƿ���߼�������
    public bool isMaxDeath;


    private void Start()
    {
        if (ifvfx)
            monther = GameObject.FindWithTag("VFX").transform;
        boxCollider = GetComponent<BoxCollider>();
    }
    public void Init(Monstermove monstermove) 
    {
        attacktimes = monstermove.monsterConfigs.attackTimer;
        attackchache = monstermove.monsterConfigs.skillDamage;
        this.monsterData = monstermove.monsterData;
        this.damage = monstermove.monsterConfigs.damage;
    }
    public float GetDamge() 
    {
        return 30 * attacktimes;
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
        //if (ifvfx)
        //    StartCoroutine("IESwordShow");
    }

    private void OnDisable()
    {
        StopCoroutine("IEAttackmonitoer");
        attacktimes = attackchache;
        //if (ifvfx)
        //    StopCoroutine("IESwordShow");
    }
    /// <summary>
    /// ��Ϊ��ײ�����ͳһ ����TriggerEnter��ײ��ʱ�򲻻ᴥ��
    /// </summary>
    public void AttackOpen() 
    {
        boxCollider.enabled = false;
        boxCollider.enabled = true;
    }
    /// <summary>
    /// ���񹥻���Ч
    /// </summary>
    public void DeathAttackEffect(Transform pos) 
    {
        if(isMaxDeath)
        Netpool.Getinstance().Insgameobj(priticle, pos.position, pos.rotation, monther);
    }
    private void OnTriggerEnter(Collider other)//show the collison
    {
        if (ifvfx && Random.Range(0, 1f) < appearPI)
        {
            if (other.tag.Equals(targetboss) || other.tag.Equals(targetenemy)||other.tag.Equals("WILD"))
            {
                if (!isMaxDeath)
                    Netpool.Getinstance().Insgameobj(priticle, other.transform.position + offset + new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z)), Quaternion.Euler(rotate.x, rotate.y, rotate.z), monther);

            }
        }

    }
    public void AttackEffect(bool isDes=true)
    {
        float attackrange = 20;
        if (monsterData.monsterType == MonsterType.PUNISHER) 
        {
            attackrange = 40;
        }
        //��Entity �����������
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var tagentity = entityManager.CreateEntity();
            entityManager.AddComponentData<BulletSkill>(tagentity, new BulletSkill { team = monsterData.team,monsterType= monsterData.monsterType, range= attackrange, damage =Mathf.CeilToInt(damage), playerName= monsterData.uid });
            entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
            entityManager.SetName(tagentity, "BulletSkill");
        if (isDes)
        {
            //������Ч
            Netpool.Getinstance().Insgameobj(priticle, transform.position + offset + new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z)), Quaternion.Euler(rotate.x, rotate.y, rotate.z), monther);
            //����
            Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);
        }
    }
    //����ʱ��
    IEnumerator IEAttackmonitoer()//the Monitor of attacktime add  it despends on that if sides camp trriger finally skill
    {
        WaitForSeconds loop = new WaitForSeconds(1f);
        for (; ; )
        {
            yield return loop;
            if (Monsterins.ificecamp&&monsterData.team== EliteUnitPortalMan.Team.Human)
            {
                attacktimes += attacktimes * 0.5f;
               
                break;

            }
            else if (Monsterins.iffirecamp && monsterData.team == EliteUnitPortalMan.Team.Org)
            {

                attacktimes += attacktimes * 0.5f;
                break;
            }
        
        }
       // Debug.Log("add damage");
        yield return new WaitForSeconds(30f);//the method has just excute once
        attacktimes = attackchache;//let the attack recover
    
    }


    //IEnumerator IESwordShow()//a control for close the particle 
    //{
    //    WaitForSeconds loop = new WaitForSeconds(swordDuration);

    //    for (; ; )
    //    { yield return loop;
    //        ifsword = true;
    //    }
    //}
}


