using com.unity.mgobe.src.Util.Def;
using Games.Characters.UI;
using MagicalFX;
using ProjectDawn.Navigation.Sample.Zerg;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using static Subscriber;
using Random = UnityEngine.Random;


public class Monstermove : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// ������������룬Ŀ��boss�����ˡ���Ӫ
    /// 
    /// </summary>
    [Header("BASE")]
    private Rigidbody rigidbody0;
    private CapsuleCollider capsuleCollider0;
    private Animator animator0;
    public float movespeed;
    public float speedchache;
    public float distance;
    public string targetboss;
    public string targetenemy;
    public bool isICE;
    private bool ifuseskill;

    /// <summary>
    /// ������ǩ���ӵ���ǩ
    /// </summary>
    [Header("TAG")]
    public string enemyfire;
    public string enemybullet;
    public string boss;
    public string mytag;
    public string enemyTS;
    public string innerreduce;
    /// <summary>
    /// ��������������棬boss�����ж�
    /// </summary>
    [Header("HEALTH")]
    public float monsterhealth;
    public float healthcache;
    private bool inbossarea;
    public float pirandom = 0.3f;//obj attack field
    private GameObject realtarget = null;
    private Vector3 realdirection;
    private GameObject mother;
    private GameObject vfx;
    public bool UIdeath;
    //�������
    /// <summary>
    ///���Լ������ֿ���
    /// </summary>
    [Header("MONITOR")]
    private bool iflookat = true;
    private bool iffind = true;
    private bool arealdyfind = true;
    private int intmonitor;
    private int intmoitor_change;
    private string appearmonstername;
    private string disappearmonstername;
    private bool realattack = true;
    private bool realwildattack = true;
    private bool ifmove = true;
    public GameObject showtarget = null;
    private GameObject subscribBoss = null;
    //��������
    [Header("ANIMATION")]
    public bool iftwoattack;
    private float animatorpara;
    public bool ismechea;//mecha start the special skii
    private bool mecheaattack;
    public float mechabulletdis = 1000000;
    /// <summary>
    /// ������β��Ч����
    /// </summary>
    [Header("TRAIL")]
    public float TrailPI = 0.5f;
    public Transform TrailTransform;
    /// <summary>
    /// mecha Attack module
    /// </summary>
    public GameObject mechabullet;
    public float mechaorTSattackfrequency = 5;
    /// <summary>
    /// TS module
    /// </summary>
    public bool isTS;//base on mecha skill ,start specail skill of TS;
    public GameObject TSskillVFX;
    public GameObject TSskillVFX_shield;
    public float specialskillPI = 0.1f;
    /// <summary>
    ///shader for dissolve EFX
    /// </summary>
    [Header("RENDER SETTING")]
    [Tooltip("its the render of the monster's shape")]
    public Renderer[] shaders;
    public float deadDissolveDuration;
    private int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private MaterialPropertyBlock block;
    public bool ifdissovle;
    private bool dissovlerecover;
    /// <summary>
    /// so on
    /// </summary>
    [Header("MONSTERTYPE")]
    [Tooltip("its  the monsterType")]
    public MonsterType monsterType;
    public MonsterData monsterData;
    /// <summary>
    /// wild monster module
    /// </summary>
    /// <param name="team"></param>
    /// <param name="uid"></param>
    /// <param name="type"></param>
    [Header("WILD")]
    public bool ifkillwild;
    public bool controlToKillWild;
    public string enemytag;
    private string enemytag_ast;
    public bool ifWILD;
    public string[] wildenemy;
    /// <summary>
    /// interal static
    /// </summary>
    /// <param name="team"></param>
    /// <param name="uid"></param>
    /// <param name="type"></param>
    [Header("INTEGRAL")]
    private Dictionary<string, float> killer = new Dictionary<string, float>();
    private float maxValue;
    public string maxKey;
    /// <summary>
    /// player ID module
    /// </summary>
    public string playerID;
    public Transform IDtransform;
    public bool ifIntegral;
    public GameObject killtime;
    public bool amicecamp;
    public Blood blood;
    public bool showNameTag=false;
    /// <summary>
    /// TS change its body module
    /// </summary>
    [Header("DEATH")]
    public bool isdeath;
    public GameObject[] deathSkill;
    public Vector3 deathSkillOffset;
    public Vector3 deathSkillOffset_assisit;
    public Vector3 range;
    /// <summary>
    /// golden time
    /// </summary>
    private int emission = Shader.PropertyToID("_EmissionColor");
    public bool emissiontest;
    private MaterialPropertyBlock blockemi;
    public Material material;
    /// <summary>
    /// under attack module
    /// </summary>
    [Header("UNDERATTACK")]
    public bool ifunderattack;
    public GameObject underattackVFX;
    public Vector3 directOffset;
    public Vector3 underattackVFXOffset;
    public Vector3 underattackVFXRotate;
    public float appearProbability = 1;
    public float underattackScale = 1;
    public float appearProbabilityCache;
    /// <summary>
    /// read config
    /// </summary>
    [Header("CONFIG")]
    public MonsterConfig monsterConfig;
    public float damage;
    public float skilldamage;
    public int monsterLevel;
    private bool amIRead = true;
    // public float timeTodisslove = 0;
    /// <summary>
    /// config module
    /// </summary>
    public TextAsset monsterFile;
    void ReadConfig(int level)
    {
        if (amIRead)
        { LoadConfig();
            switch (level)
            {
                case 1:
                    monsterhealth = monsterConfig.monsterConfig1.monsterHealth;
                    healthcache = monsterConfig.monsterConfig1.healthCache;
                    damage = monsterConfig.monsterConfig1.damage;
                    movespeed = monsterConfig.monsterConfig1.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig1.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.attacktimes = damage;
                    component1.attackchache = damage;
                    component1.appearPICache = component1.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 2:
                    monsterhealth = monsterConfig.monsterConfig2.monsterHealth;
                    healthcache = monsterConfig.monsterConfig2.healthCache;
                    damage = monsterConfig.monsterConfig2.damage;
                    movespeed = monsterConfig.monsterConfig2.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig2.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component2);
                    component2.attacktimes = damage;
                    component2.attackchache = damage;
                    component2.appearPICache = component2.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 3:
                    monsterhealth = monsterConfig.monsterConfig3.monsterHealth;
                    healthcache = monsterConfig.monsterConfig3.healthCache;
                    damage = monsterConfig.monsterConfig3.damage;
                    movespeed = monsterConfig.monsterConfig3.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig3.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component3);
                    component3.attacktimes = damage;
                    component3.attackchache = damage;
                    component3.appearPICache = component3.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 4:
                    monsterhealth = monsterConfig.monsterConfig4.monsterHealth;
                    healthcache = monsterConfig.monsterConfig4.healthCache;
                    damage = monsterConfig.monsterConfig4.damage;
                    movespeed = monsterConfig.monsterConfig4.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig4.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component4);
                    component4.attacktimes = damage;
                    component4.attackchache = damage;
                    component4.appearPICache = component4.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 5:
                    monsterhealth = monsterConfig.monsterConfig5.monsterHealth;
                    healthcache = monsterConfig.monsterConfig5.healthCache;
                    damage = monsterConfig.monsterConfig5.damage;
                    movespeed = monsterConfig.monsterConfig5.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig5.speedCache;
                    skilldamage = monsterConfig.monsterConfig5.bulletDamage;
                    mechabulletdis = monsterConfig.monsterConfig5.mechaAttackDis;
                    IDtransform.TryGetComponent<Attack>(out Attack component5);
                    component5.attacktimes = damage;
                    component5.attackchache = damage;
                    component5.appearPICache = component5.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 6:
                    monsterhealth = monsterConfig.monsterConfig6.monsterHealth;
                    healthcache = monsterConfig.monsterConfig6.healthCache;
                    damage = monsterConfig.monsterConfig6.damage;
                    movespeed = monsterConfig.monsterConfig6.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig6.speedCache;
                    skilldamage = monsterConfig.monsterConfig6.skillDamage;
                    IDtransform.TryGetComponent<Attack>(out Attack component6);
                    component6.attacktimes = damage;
                    component6.attackchache = damage;
                    component6.appearPICache = component6.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 7:
                    monsterhealth = monsterConfig.monsterConfig7.monsterHealth;
                    healthcache = monsterConfig.monsterConfig7.healthCache;
                    damage = monsterConfig.monsterConfig7.damage;
                    movespeed = monsterConfig.monsterConfig7.monsterSpeed;
                    speedchache = monsterConfig.monsterConfig7.speedCache;
                    skilldamage = monsterConfig.monsterConfig7.skillDamage;
                    IDtransform.TryGetComponent<Attack>(out Attack component7);
                    component7.attacktimes = damage;
                    component7.attackchache = damage;
                    component7.appearPICache = component7.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 8://���ع�
                    monsterhealth = monsterConfig.wildConfig1.monsterHealth;
                    healthcache = monsterConfig.wildConfig1.healthCache;
                    damage = monsterConfig.wildConfig1.damage;
                    movespeed = monsterConfig.wildConfig1.monsterSpeed;
                    speedchache = monsterConfig.wildConfig1.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component8);
                    component8.attacktimes = damage;
                    component8.attackchache = damage;
                    component8.appearPICache = component8.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 9://�ľ�з
                    monsterhealth = monsterConfig.wildConfig2.monsterHealth;
                    healthcache = monsterConfig.wildConfig2.healthCache;
                    damage = monsterConfig.wildConfig2.damage;
                    movespeed = monsterConfig.wildConfig2.monsterSpeed;
                    speedchache = monsterConfig.wildConfig2.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component9);
                    component9.attacktimes = damage;
                    component9.attackchache = damage;
                    component9.appearPICache = component9.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 10://֩��
                    monsterhealth = monsterConfig.wildConfig3.monsterHealth;
                    healthcache = monsterConfig.wildConfig3.healthCache;
                    damage = monsterConfig.wildConfig3.damage;
                    movespeed = monsterConfig.wildConfig3.monsterSpeed;
                    speedchache = monsterConfig.wildConfig3.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component10);
                    component10.attacktimes = damage;
                    component10.attackchache = damage;
                    component10.appearPICache = component10.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 11://��ԭ��
                    monsterhealth = monsterConfig.wildConfig4.monsterHealth;
                    healthcache = monsterConfig.wildConfig4.healthCache;
                    damage = monsterConfig.wildConfig4.damage;
                    movespeed = monsterConfig.wildConfig4.monsterSpeed;
                    speedchache = monsterConfig.wildConfig4.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component11);
                    component11.attacktimes = damage;
                    component11.attackchache = damage;
                    component11.appearPICache = component11.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
            }
            amIRead = false;
        }

    }
    void ReadConfigForAA(int level)
    {
        if (amIRead)
        {
           // LoadConfigFromAA();
            switch (level)
            {
                case 1:
                    monsterhealth =RemoteBulid.instance. monsterConfig.monsterConfig1.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig1.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig1.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig1.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig1.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.attacktimes = damage;
                    component1.attackchache = damage;
                    component1.appearPICache = component1.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 2:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig2.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig2.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig2.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig2.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig2.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component2);
                    component2.attacktimes = damage;
                    component2.attackchache = damage;
                    component2.appearPICache = component2.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 3:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig3.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig3.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig3.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig3.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig3.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component3);
                    component3.attacktimes = damage;
                    component3.attackchache = damage;
                    component3.appearPICache = component3.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 4:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig4.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig4.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig4.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig4.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig4.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component4);
                    component4.attacktimes = damage;
                    component4.attackchache = damage;
                    component4.appearPICache = component4.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 5:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig5.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig5.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig5.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig5.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig5.speedCache;
                    skilldamage = RemoteBulid.instance.monsterConfig.monsterConfig5.bulletDamage;
                    mechabulletdis = RemoteBulid.instance.monsterConfig.monsterConfig5.mechaAttackDis;
                    IDtransform.TryGetComponent<Attack>(out Attack component5);
                    component5.attacktimes = damage;
                    component5.attackchache = damage;
                    component5.appearPICache = component5.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 6:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig6.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig6.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig6.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig6.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig6.speedCache;
                    skilldamage = RemoteBulid.instance.monsterConfig.monsterConfig6.skillDamage;
                    IDtransform.TryGetComponent<Attack>(out Attack component6);
                    component6.attacktimes = damage;
                    component6.attackchache = damage;
                    component6.appearPICache = component6.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 7:
                    monsterhealth = RemoteBulid.instance.monsterConfig.monsterConfig7.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.monsterConfig7.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.monsterConfig7.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.monsterConfig7.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.monsterConfig7.speedCache;
                    skilldamage = RemoteBulid.instance.monsterConfig.monsterConfig7.skillDamage;
                    IDtransform.TryGetComponent<Attack>(out Attack component7);
                    component7.attacktimes = damage;
                    component7.attackchache = damage;
                    component7.appearPICache = component7.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 8://���ع�
                    monsterhealth = RemoteBulid.instance.monsterConfig.wildConfig1.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.wildConfig1.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.wildConfig1.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.wildConfig1.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.wildConfig1.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component8);
                    component8.attacktimes = damage;
                    component8.attackchache = damage;
                    component8.appearPICache = component8.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 9://�ľ�з
                    monsterhealth = RemoteBulid.instance.monsterConfig.wildConfig2.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.wildConfig2.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.wildConfig2.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.wildConfig2.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.wildConfig2.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component9);
                    component9.attacktimes = damage;
                    component9.attackchache = damage;
                    component9.appearPICache = component9.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 10://֩��
                    monsterhealth = RemoteBulid.instance.monsterConfig.wildConfig3.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.wildConfig3.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.wildConfig3.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.wildConfig3.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.wildConfig3.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component10);
                    component10.attacktimes = damage;
                    component10.attackchache = damage;
                    component10.appearPICache = component10.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
                case 11://��ԭ��
                    monsterhealth = RemoteBulid.instance.monsterConfig.wildConfig4.monsterHealth;
                    healthcache = RemoteBulid.instance.monsterConfig.wildConfig4.healthCache;
                    damage = RemoteBulid.instance.monsterConfig.wildConfig4.damage;
                    movespeed = RemoteBulid.instance.monsterConfig.wildConfig4.monsterSpeed;
                    speedchache = RemoteBulid.instance.monsterConfig.wildConfig4.speedCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component11);
                    component11.attacktimes = damage;
                    component11.attackchache = damage;
                    component11.appearPICache = component11.appearPI;
                    appearProbabilityCache = appearProbability;
                    break;
            }
            amIRead = false;
        }



    }
    void LoadConfig()
    {
        //#if UNITY_EDITOR
        //        string jsonText = System.IO.File.ReadAllText("Assets\\Other\\Scripits\\Config\\MonsterConfigs.json");
        //        monsterConfig = JsonUtility.FromJson<MonsterConfig>(jsonText);
        //#else
        //                        TextAsset jsonText = Resources.Load<TextAsset>("MonsterConfigs");
        //                        monsterConfig = JsonUtility.FromJson<MonsterConfig>(jsonText.text);
        //#endif
#if SYSTEM1

        LoadJsonFromFile();

#endif
#if SYSTEM

        LoadConfigFromAA();

#endif
    }
    void LoadJsonFromFile()//a function for read the json of monsterconfig for Xlua under the path that Application.persistentDataPath and"data.json"
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "data.json");

        if (System.IO.File.Exists(filePath))
        {
            string jsonText = System.IO.File.ReadAllText(filePath);
            monsterConfig = JsonUtility.FromJson<MonsterConfig>(jsonText);
        }
        else
        {
            Debug.LogError("Cannot find file " + filePath);
        }
    }
    void LoadConfigFromAA()
    {
        GameObject config = GameObject.FindWithTag("AA");
        // monsterFile = config.GetComponent<RemoteBulid>().jsonFile;
        //Debug.Log(config.GetComponent<RemoteBulid>().xx);
        //Debug.Log(config.GetComponent<RemoteBulid>().testConfig.monsterConfig1.damage);
        monsterConfig = config.GetComponent<RemoteBulid>().testConfig;
   


    }
    //-----------
    IEnumerator IEChangeMyCollisonExculde()//the mecthod for change the collider;
    {
        yield return new WaitForSeconds(5);
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; monsterhealth > 0;)
        {
            yield return loop;
            if (Random.Range(0, 1f) > 0.1f)
            {
                GameObject[] objectsToExclude = GameObject.FindGameObjectsWithTag(mytag);

                foreach (var obj in objectsToExclude)
                {
         
                    Collider colliderToExclude = obj.GetComponent<Collider>();

            
                    Physics.IgnoreCollision(capsuleCollider0, colliderToExclude, true);
                }
                yield return loop;
                foreach (var obj in objectsToExclude)
                {
        
                    Collider colliderToExclude = obj.GetComponent<Collider>();

                    Physics.IgnoreCollision(capsuleCollider0, colliderToExclude, false);
                }

            }
        }
    }
    IEnumerator IEChangeMyLayer()//the method to change the layer of obj,to caculate collsion;
    {
        WaitForSeconds loop = new WaitForSeconds(5);
        WaitForSeconds loopRecover= new WaitForSeconds(5);
        for (; monsterhealth > 0;)
        {
            yield return loop;
            {
                if (Monsterins.ifCheackCollison == 1)
                    if (Random.Range(0, 1f) > 0.7f)
                    {
                        gameObject.layer = LayerMask.NameToLayer("LayerMan");
                        //Debug.Log("the layer has changed");
                        yield return loopRecover;
                        gameObject.layer = LayerMask.NameToLayer("Default");
                    }
                    else
                    {if(gameObject.layer!=LayerMask.NameToLayer("Default"))
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        yield return loopRecover;
                    }
            }
        }
    }
    void Start()
    {
        rigidbody0 = GetComponent<Rigidbody>();
        animator0 = GetComponent<Animator>();
        capsuleCollider0 = GetComponent<CapsuleCollider>();
        //---choose the parent
        if (isICE)
            mother = GameObject.FindWithTag("MOTHER_AS");
        else
            mother = GameObject.FindWithTag("MOTHER");

        vfx = GameObject.FindWithTag("VFX");
        subscribBoss = subscribBoss ?? GameObject.FindWithTag("ICE");
       // Subscribe(Publisher.Getinstance());//the player of subscrib
       // SubscribeBossMessage(subscribBoss.GetComponent<Rayboss>());
        //--choose the parent
        // Findenemy();
        // Debug.Log("its has started monstermove");
    }
    public void initMonsterData(Team team, string uid, MonsterType type)//****wjy code****
    {
        monsterData = new MonsterData();
        monsterData.team = team;
        monsterData.uid = uid;
        monsterData.monsterType = type;
        monsterType = type;
        GameManager.instance.allMonster.Add(this);
      
    }
    private void OnEnable()
    {
        monsterhealth = healthcache;
        enemytag = null;
        movespeed = speedchache;
        controlToKillWild = false;
        dissovlerecover = true;
        ifuseskill = true;
        iflookat = true;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;//recover the collsion of obj
        gameObject.tag = mytag;
        mecheaattack = false;
        UIdeath= false;
        inbossarea = false;
        ReadConfig(monsterLevel);//read the config
       // ReadConfigForAA(monsterLevel);//read the config for aa
        if (ifdissovle)
        {
            if (block == null)
                block = new MaterialPropertyBlock();//spwaner a block for use dissolve VFX
            block.SetFloat(DissolveAmount, 1);// the one which act on has starting invisible 
            StartCoroutine("IEStartDissolve");//start dissolve render
        }
        //----
        if (ifWILD)
        StartCoroutine("IEBearecycle");
        StartCoroutine("IEiffand");
        StartCoroutine("IEiflookat");
        StartCoroutine("IEAnimatorPlay");//attack state control
        StartCoroutine("IEMecheaAttack");//start mecha attack coroutine
       // StartCoroutine("ClearGameobj");//this IE use to check the gameobj which has not disappear when its health less than zero;
        if (ismechea && !isTS)
            StartCoroutine("IEMechaVFX");
        ///----finaly skill
        StartCoroutine("IEFinallySkill");//start finally monitor
        StartCoroutine("IEFindWild");//start the recycle of find wild
        if (emissiontest)
        {
            if (blockemi == null)
                blockemi = new MaterialPropertyBlock();
            // blockemi.SetColor(emission, new Color(0.8f, 0.5f, 0.1f, 1));
            ////blockemi.SetFloat(emission, 0.5f);
            StartCoroutine("IEEmissionTime");
        }
        //start coroutine  of that to adjust the vfx density
        StartCoroutine("IEStartToAdjustDensity");
        StartCoroutine("IECheckTheY");
        // StartCoroutine("IEChangeMyCollisonExculde");
        StartCoroutine("IEChangeMyLayer");
    }

    private void OnDisable()
    {
        StopCoroutine("IEiffand");
        StopCoroutine("IEiflookat");
        StopCoroutine("IEMecheaAttack");//stop mecha coroutine
        StopCoroutine("IEMechaShoot");//stop mecha coroutine
        StopCoroutine("IETSskill");//close Coroutine of TS skill
       // StopCoroutine("ClearGameobj");
        StopCoroutine("IEMechaVFX");
        StopCoroutine("IEFinallySkill");//stop finally monitor
        StopCoroutine("IEDeathDissolve");
        StopCoroutine("IEFindWild");//
        StopCoroutine("IEBearecycle");//
        StopCoroutine("IEStartToAdjustDensity");
        StopCoroutine("IEreduce");
        ifdissovle = true;
        StopCoroutine("IECheckTheY");
        StopCoroutine("IEChangeMyLayer");
        gameObject.layer = LayerMask.NameToLayer("Default");
        // StopCoroutine("IEChangeMyCollisonExculde");
        //if (ifdissovle)
        //    MonsterDeathBefore();
        //if (isdeath)
        //    StopCoroutine("IEDeathskill");//

        // Debug.LogError("death " +playerID);
        //  Debug.Log("monster death");    }
    }
    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.isGameFighting)
        {
            if (!ifWILD)
            {
                UnityEngine.Profiling.Profiler.BeginSample("MyMethod Realmove");
                Realmove();
                UnityEngine.Profiling.Profiler.EndSample();
            }
            else
            {
                UnityEngine.Profiling.Profiler.BeginSample("MyMethod Wildmove");
                WildMove();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        //---
        //camp skill monitoer

        // StartDissolve(ifdissovle);
        //UnityEngine.Profiling.Profiler.BeginSample("MyMethod Realmove");
        //Realmove();
        //UnityEngine.Profiling.Profiler.EndSample();
    }
    /// <summary>
    /// dissolve Coroutine;
    /// </summary>
    IEnumerator IEStartDissolve()
    {
        WaitForSeconds loop = new WaitForSeconds(Time.deltaTime);
       // block.SetFloat(DissolveAmount, 1);
        for (float t = 0; t < deadDissolveDuration;)
        {
            t += Time.deltaTime;
            yield return loop;
            var alpha = 1.0f - Mathf.Clamp01(t / deadDissolveDuration);
            block.SetFloat(DissolveAmount, alpha);
            foreach (var r in shaders)
                r.SetPropertyBlock(block);
           // Debug.Log("step"+block.GetFloat((DissolveAmount)));
            yield return null;
            
        }
        block.SetFloat(DissolveAmount, 0);
        //Debug.Log("has excute");

    }

    IEnumerator IEDeathDissolve()
    {
        WaitForSeconds loop = new WaitForSeconds(Time.deltaTime);
        for (float t = deadDissolveDuration; t > 0;)
        {
            t -= Time.deltaTime;
            yield return loop;
            var alpha = 1 - Mathf.Clamp01(t / deadDissolveDuration);
            block.SetFloat(DissolveAmount, alpha);
            foreach (var r in shaders)
                r.SetPropertyBlock(block);
            yield return null;
            //foreach (var r in shaders)
            //    r.SetPropertyBlock(null);
        }
        foreach (var r in shaders)
            r.SetPropertyBlock(null);
        // block.SetFloat(DissolveAmount, 1);


    }
    IEnumerator IEEmissionTime()//the emission time for goleden skill,howerver its doesnot work
    {
        //WaitForSeconds loop = new WaitForSeconds(Time.deltaTime);
        yield return new WaitForSeconds(5f);

        //  blockemi.SetColor(emission, new Color(1f, 0.5f, 0.1f, 1));
        block.SetColor(emission, Color.red);
        //material.SetColor(emission, Color.red);
        foreach (var r in shaders)
        {
            r.SetPropertyBlock(block);
            //MaterialGlobalIlluminationFlags flags = r.material.globalIlluminationFlags;
            //flags = MaterialGlobalIlluminationFlags.AnyEmissive;
            //material.globalIlluminationFlags = flags;
        }
        //---
        // blockemi.SetColor(emission, new Color(0.8f, 0.5f, 0.1f, 1));
        //for (float t = 0; t < 2;)
        //{
        //    t += Time.deltaTime;
        //    yield return loop;

        //    blockemi.SetColor(emission, new Color(t, 0.5f, 0.1f, 1));
        //    foreach (var r in shaders)
        //    {
        //        r.SetPropertyBlock(blockemi);
        //        //MaterialGlobalIlluminationFlags flags = r.material.globalIlluminationFlags;
        //        //flags = MaterialGlobalIlluminationFlags.AnyEmissive;
        //        //material.globalIlluminationFlags = flags;
        //    }

        //    yield return null;
        //}
        //--

        /*foreach (var r in shaders)
        {
            *//*Material material = r.material;
            material.SetColor("_EmissionColor", new Color(0.8f, 0.5f, 0.1f, 1));
            material.SetFloat("_Metallic", 0.5f);
            r.material = material;
            r.material.SetPass(0);*//*
            r.SetPropertyBlock(blockemi);
        }*/
        Debug.Log("start golen body" + emission + blockemi.isEmpty);
        // material.SetColor(emission, new Color(0.8f, 0.6f, 0.1f));
        //Debug.Log("has set color");
    }
    /// <summary>
    /// find WILD module
    /// </summary>
    /// <returns></returns>
    IEnumerator IEFindWild()//the IE of start find WILD, per  half sceosnd qurey once 
    {
        WaitForSeconds loop = new WaitForSeconds(0.5f);

        for (; monsterhealth > 0;)
        {
            yield return loop;
            if (!controlToKillWild)
            {
                if (GameObject.FindWithTag("WILD") != null)
                {
                    GameObject target = null;
                    float min;

                    if (iflookat)
                    { GameObject[] targets = GameObject.FindGameObjectsWithTag("WILD");
                        min = (targets[0].transform.position - gameObject.transform.position).sqrMagnitude;
                        target = targets[0].gameObject;
                        showtarget = target;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            if (min > (targets[i].transform.position - this.transform.position).sqrMagnitude + 3000)
                            {
                                min = (targets[i].transform.position - this.transform.position).sqrMagnitude;
                                target = targets[i].gameObject;
                                showtarget = target;
                            }
                        }
                        //iflookat = false;
                    }
                    //judge the min distance of the wild;

                    if ((showtarget.transform.position - transform.position).sqrMagnitude < 10000)
                    {

                        ifkillwild = true;
                    }
                    else
                    {


                        ifkillwild = false;

                    }

                }
            }
            else
            {
                ifkillwild = true;
            }
        }
    }
    IEnumerator ClearGameobj() //this IE use to check the gameobj which has not disappear when its health less than zero ,and it is the test code
    {
        WaitForSeconds loop = new WaitForSeconds(3);
        for (; ; )
        {
            if (monsterhealth < 0)
            {
                Netpool.Getinstance().Pushobject(name, gameObject);

                break;
            }
            yield return loop;
        }

    }
    IEnumerator IEiffand()//���Ƶ���Ѱ��
    {
        WaitForSeconds loop = new WaitForSeconds(0.3f);
        for (; monsterhealth > 0;)
        {
            yield return loop;
            iffind = true;

        }

    }
    IEnumerator IEAnimatorPlay()//���ƶ�������
    {
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; monsterhealth > 0;)
        {
            animatorpara = Random.Range(0, 1f);

            yield return loop;

        }

    }
    /// <summary>
    /// ������ʱ����
    /// </summary>
    public void MonsterDeath()
    {
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);     
    }
    public void WildMonsterDeath()
    {
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);
       // Debug.Log("i call mechod here,  name" + gameObject.name);
    }
    /// <summary>
    /// ��ɫ�����¼�ִ������
    /// </summary>
    public void MonsterDeathBefore()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        if (dissovlerecover)//disdissovel
        {
            StartCoroutine("IEDeathDissolve");
            dissovlerecover = false;
        }
        UIdeath = true;//----monster death for wjy
        if (GameManager.instance.allMonster.Contains(this))
        {
            if (blood != null&& blood.gameObject.activeSelf&& blood.target==transform)
            {
                blood.DestroyBlood();
            }
            GameManager.instance.allMonster.Remove(this);
            GameManager.instance.CheckShowName(0);
            showNameTag = false;
      
        } 
   
        GameManager.instance.KillMonsterAddScore(monsterType, maxKey, 1, isdeath);
        GameManager.instance.SetPlayerKillData(maxKey);
    }

    /// <summary>
    /// mechce IE
    /// </summary>
    /// 
    IEnumerator IEMechaShoot()//the module of mecha shoot
    {
        WaitForSeconds p = new WaitForSeconds(0.2f);
        for (; monsterhealth > 0;)
        {

            for (int i = 0; i < 3; i++)
            {
                yield return p;
                GameObject gameObject0 = Netpool.Getinstance().Insgameobj(mechabullet, this.transform.GetChild(1).position, Quaternion.identity, vfx.transform);
                //  Debug.Log(this.gameObject.name);
                gameObject0.TryGetComponent<Attack>(out Attack attack);
                attack.ID = playerID;//go to the mecha module and add the ID for bullet
                attack.attacktimes = skilldamage;
                attack.attackchache = skilldamage;
            }
            break;
            // Debug.Log("��ʼ����");
        }
    }
    IEnumerator IEMecheaAttack()//control mechas attack f//now you can control TS
    {
        WaitForSeconds loop = new WaitForSeconds(mechaorTSattackfrequency);
        for (; monsterhealth > 0;)
        {

            yield return loop;
            if (Random.Range(0, 1f) > (1 - specialskillPI))//control the requency of skill which appear in the scence;
            {
                if (showtarget != null)
                {
                    if ((showtarget.transform.position - transform.position).sqrMagnitude < mechabulletdis)
                        mecheaattack = true;
                }
                // Debug.Log("OnAttack����������������������������");

            }
            else
                mecheaattack = false;


        }

    }
    IEnumerator IEMechaVFX()
    {
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; monsterhealth > 0;)
        {
            if (Random.Range(0, 1f) > 0.6f)
            {
                transform.GetChild(2).GetChild(1).gameObject.SetActive(true);


            }
            else
                transform.GetChild(2).GetChild(1).gameObject.SetActive(false);

            yield return loop;
        }



    }
    /// <summary>
    /// TS IE
    /// </summary>
    /// <param name="num"></param>
    IEnumerator IETSskill()//TS skill,x aix rotate -90
    {
        if (isTS)
            Netpool.Getinstance().Insgameobj(TSskillVFX_shield, this.transform.position + new Vector3(0, 25, 0), Quaternion.identity, vfx.transform);//creat the vfx of TS who has casting skills;
        WaitForSeconds p = new WaitForSeconds(0.3f);
        for (; monsterhealth > 0;)
        {

            for (int i = 0; i < 3; i++)
            {
                yield return p;
                GameObject gameObject0 = Netpool.Getinstance().Insgameobj(TSskillVFX, new Vector3(transform.position.x, 7, transform.position.z) + transform.forward * 80 * (i + 1), Quaternion.Euler(-90, Random.Range(0, 350), 0), vfx.transform, true);
                gameObject0.TryGetComponent<Attack>(out Attack attack);
                attack.ID = playerID;
                attack.attacktimes = skilldamage;
                attack.attackchache = skilldamage;
                //  Debug.Log(this.gameObject.name);
            }
            break;
            // Debug.Log("��ʼ����");
        }
    }
    public void AnimationEventMechea(int num)//the mecha recover walk after attack
    {
        switch (num)
        {
            case 1:
                StartCoroutine("IEMechaShoot");
                break;
            case 2:
                mecheaattack = false;
                animator0.SetBool("walking", true);
                // Debug.Log("Over Attack����������������������������");
                break;
        }
    }
    public void AnimationEventTS(int num)//TS recover move AnimationEvent
    {
        switch (num)
        {
            case 1:
                StartCoroutine("IETSskill");
                break;
            case 2:
                mecheaattack = false;
                animator0.SetBool("walking", true);
                // Debug.Log("Over Attack����������������������������");
                break;
        }
    }
    /// <summary>
    /// module of death skill
    /// </summary>
    public void AnimationEventDeath()//give a dudge for Death Skill;
    {

        if (monsterhealth > 0)
            //  StartCoroutine("IEDeathskill");
            DeathSkill();
    }
    IEnumerator IEDeathskill()
    {
        WaitForSeconds loop = new WaitForSeconds(0.2f);
        if (monsterhealth > 0)
            Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 1; i++)
        {
            yield return loop;
            if (monsterhealth > 0)
                Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx

        }
    }

    void DeathSkill()
    {

        if (monsterhealth > 0)
        {
            // Debug.Log("has start the death's Skill");

            //Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis
            if (showtarget != null)
            {
                Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis


                if (showtarget.TryGetComponent<Monstermove>(out Monstermove component2))
                { if (component2.monsterhealth > 0)
                    {
                        GameObject clone = Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx
                        clone.TryGetComponent<FX_LifeTime>(out FX_LifeTime component);
                        clone.TryGetComponent<Attack>(out Attack component1);
                        component1.ID = playerID;
                        component1.attacktimes = skilldamage;
                        component1.attackchache = skilldamage;
                        component.showtarget = showtarget.gameObject;//pass the forward transform for rockfire effects;  // Debug.Log("has generate a fire ball!!!!!!!");
                    }

                }
                else
                {
                    if (showtarget.tag.Equals("ICE") || showtarget.tag.Equals("FIRE"))
                    {
                        GameObject clone = Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx
                        clone.TryGetComponent<FX_LifeTime>(out FX_LifeTime component);
                        clone.TryGetComponent<Attack>(out Attack component1);
                        component1.ID = playerID;
                        component1.attacktimes = skilldamage;
                        component1.attackchache = skilldamage;
                        component.showtarget = showtarget.gameObject;//pass the forward transform for rockfire effects
                    }
                }
            }

        }
    }

    /// <summary>
    /// the main monster logic
    /// </summary>
    void Realmove()
    {
        if (!ifkillwild)
        {
            if (ismechea && mecheaattack)
            {
                if (monsterhealth > 0)
                {
                    animator0.SetTrigger("hit");
                    animator0.SetBool("walking", false);
                }
                else
                {
                    animator0.SetBool("death", true);
                    animator0.SetBool("walking", false);
                    this.gameObject.tag = "Untagged";
                }
            }
            else
            {
                if (monsterhealth > 0)
                {
                    float min;
                    GameObject target = null;
                    if (GameObject.FindWithTag(targetenemy) != null)//
                    {

                        if (iflookat)
                        { GameObject[] targets = GameObject.FindGameObjectsWithTag(targetenemy);
                            min = (targets[0].transform.position - gameObject.transform.position).sqrMagnitude;
                            target = targets[0].gameObject;
                            showtarget = target;
                            for (int i = 0; i < targets.Length; i++)
                            {
                                if (min > (targets[i].transform.position - this.transform.position).sqrMagnitude + 3000)
                                {
                                    min = (targets[i].transform.position - this.transform.position).sqrMagnitude;
                                    target = targets[i].gameObject;
                                    showtarget = target;//
                                }
                            }
                            // iflookat = false;
                        }
                        {
                            var direction = (showtarget.transform.position - this.transform.position);//
                            if (showtarget != null)
                                this.transform.LookAt(showtarget.transform.position, Vector3.up);
                            //iflookat = false;
                            if (direction.sqrMagnitude > 10f)
                            {
                                if (direction.sqrMagnitude > pirandom * distance)
                                {
                                    animator0.SetBool("walking", true);
                                    transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                                }
                                else
                                {
                                    if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))//
                                    {
                                        if (component.monsterhealth > 0)
                                        {
                                            if (iftwoattack && animatorpara > 0.7f)
                                            {

                                                animator0.SetTrigger("attackhand");


                                            }
                                            else
                                            {

                                                animator0.SetTrigger("attack");

                                            }

                                            animator0.SetBool("walking", false);
                                        }
                                        else
                                        {
                                            animator0.SetBool("walking", true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GameObject.FindWithTag(targetboss) != null)
                        {
                            target = GameObject.FindWithTag(targetboss);
                            showtarget = target;
                            var direction = (showtarget.transform.position - this.transform.position);

                            this.transform.LookAt(showtarget.transform.position, Vector3.up);
                            //iflookat = false;

                            if (direction.sqrMagnitude > 10.1f)
                            {

                                if (direction.sqrMagnitude >1.3f* distance)
                                {
                                    transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                                    animator0.SetBool("walking", true);
                                }
                                else
                                {
                                    if (iftwoattack && animatorpara > 0.7f)
                                    {
                                        animator0.SetTrigger("attackhand");

                                    }
                                    else
                                    {

                                        animator0.SetTrigger("attack");
                                    }

                                    animator0.SetBool("walking", false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    animator0.SetBool("death", true);
                    animator0.SetBool("walking", false);
                    this.gameObject.tag = "Untagged";
                    //if (dissovlerecover)//disdissovel
                    //{
                    //    StartCoroutine("IEDeathDissolve");
                    //    dissovlerecover = false;
                    //}
                }
            }
        }
        else
        {
            if (ismechea && mecheaattack&&monsterhealth>0)
            {
                animator0.SetTrigger("hit");
                animator0.SetBool("walking", false);
            }

           else if (GameObject.FindWithTag("WILD") != null && monsterhealth > 0)
            { 
                    float min;
                    GameObject target = null;
                    if (iflookat)
                    {
                        GameObject[] targets = GameObject.FindGameObjectsWithTag("WILD");
                        min = (targets[0].transform.position - gameObject.transform.position).sqrMagnitude;
                        target = targets[0].gameObject;
                        showtarget = target;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            if (min > (targets[i].transform.position - this.transform.position).sqrMagnitude + 3000)
                            {
                                min = (targets[i].transform.position - this.transform.position).sqrMagnitude;
                                target = targets[i].gameObject;

                                showtarget = target;//the target show in the scense

                            }
                        }
                        // iflookat = false;
                    }
                    var direction = (showtarget.transform.position - this.transform.position);
                    if (showtarget != null)
                        this.transform.LookAt(showtarget.transform.position, Vector3.up);
                    //iflookat = false;

                    if (direction.sqrMagnitude > 10.1f)
                    {

                        if (direction.sqrMagnitude > distance)
                        {
                            transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                            animator0.SetBool("walking", true);
                        }
                        else
                        {
                            if (iftwoattack && animatorpara > 0.7f)
                            {
                                animator0.SetTrigger("attackhand");
                            }
                            else
                            {

                                animator0.SetTrigger("attack");
                            }

                            animator0.SetBool("walking", false);
                        }
                    }
                }
                 else if (monsterhealth <= 0)
                {
                    animator0.SetBool("death", true);
                    animator0.SetBool("walking", false);
                    this.gameObject.tag = "Untagged";
                    //if (dissovlerecover)//disdissovel
                    //{
                    //    StartCoroutine("IEDeathDissolve");
                    //    dissovlerecover = false;
                    //}
                }
                else
                {

                    ifkillwild = false;
                }

            } 
    }
    /// <summary>
    /// the main logic of Wild animals
    /// </summary>
    void WildMove()
    {
        if (monsterhealth > 0)
        {
            float min;
            GameObject target = null;
            if (GameObject.FindWithTag(enemytag) != null)
            {
                if (iflookat)
                { GameObject[] targets = GameObject.FindGameObjectsWithTag(enemytag);
                    min = (targets[0].transform.position - gameObject.transform.position).sqrMagnitude;
                    target = targets[0].gameObject;
                    showtarget = target;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (min > (targets[i].transform.position - this.transform.position).sqrMagnitude + 3000)
                        {
                            min = (targets[i].transform.position - this.transform.position).sqrMagnitude;
                            target = targets[i].gameObject;
                            showtarget = target;
                        }
                    }
                    //iflookat= false;
                }

                {
                    var direction = (showtarget.transform.position - this.transform.position);
                    if (showtarget != null)
                        this.transform.LookAt(showtarget.transform.position, Vector3.up);
                    // iflookat = false;

                    if (direction.sqrMagnitude > 10f)
                    {
                        if (direction.sqrMagnitude > pirandom * distance)
                        {
                            animator0.SetBool("walking", true);
                            transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                        }
                        else
                        {
                            if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))//judge the health of enemy that is it zero;
                            {
                                if (component.monsterhealth > 0)
                                {//attack 
                                    if (iftwoattack && animatorpara > 0.7f)
                                    {

                                        animator0.SetTrigger("attackhand");
                                    }
                                    else
                                    {
                                        animator0.SetTrigger("attack");
                                    }
                                    //attack
                                    animator0.SetBool("walking", false);
                                }
                                else
                                {
                                    animator0.SetBool("walking", true);
                                }
                            }
                        }
                    }
                }
            }
            else if (GameObject.FindWithTag(enemytag_ast) != null)//find enemy,and identify who close to you
            {




                if (iflookat)
                { GameObject[] targets = GameObject.FindGameObjectsWithTag(enemytag_ast);
                    min = (targets[0].transform.position - gameObject.transform.position).sqrMagnitude;
                    target = targets[0].gameObject;
                    showtarget = target;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (min > (targets[i].transform.position - this.transform.position).sqrMagnitude + 3000)
                        {
                            min = (targets[i].transform.position - this.transform.position).sqrMagnitude;
                            target = targets[i].gameObject;
                            showtarget = target;//the target show in the scense
                        }
                    }
                    // iflookat = false;
                }
                {
                    var direction = (showtarget.transform.position - this.transform.position);//Two point directional vector

                    if (showtarget != null)
                        this.transform.LookAt(showtarget.transform.position, Vector3.up);
                    // iflookat = false;

                    if (direction.sqrMagnitude > 10f)
                    {
                        if (direction.sqrMagnitude > pirandom * distance)
                        {
                            animator0.SetBool("walking", true);
                            transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                        }
                        else
                        {
                            if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))
                            {
                                if (component.monsterhealth > 0)
                                {
                                    if (iftwoattack && animatorpara > 0.7f)
                                    {

                                        animator0.SetTrigger("attackhand");
                                    }
                                    else
                                    {
                                        animator0.SetTrigger("attack");
                                    }

                                    animator0.SetBool("walking", false);
                                }
                                else
                                {
                                    animator0.SetBool("walking", true);
                                }
                            }
                        }
                    }

                }

            }
            else
            {
                if (GameObject.FindWithTag(targetboss) != null)
                {
                    target = GameObject.FindWithTag(targetboss);
                    showtarget = target;
                    var direction = (showtarget.transform.position - this.transform.position);
                    this.transform.LookAt(showtarget.transform.position, Vector3.up);

                    if (direction.sqrMagnitude > 10.1f)
                    {

                        if (direction.sqrMagnitude > 1.3f*distance)
                        {
                            transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                            animator0.SetBool("walking", true);
                        }
                        else
                        {
                            if (iftwoattack && animatorpara > 0.7f)
                            {
                                animator0.SetTrigger("attackhand");
                            }
                            else
                            {

                                animator0.SetTrigger("attack");
                            }

                            animator0.SetBool("walking", false);
                        }
                    }
                }
            }
        }
        else
        {
            animator0.SetBool("death", true);
            animator0.SetBool("walking", false);
            this.gameObject.tag = "Untagged";
            //if (dissovlerecover)//disdissovel
            //{
            //    StartCoroutine("IEDeathDissolve");
            //    dissovlerecover = false;
            //}
        }
    }
    /// <summary>
    /// certain the obj which monsterhealth is less than zero that has been recovery
    /// </summary>
    /// <returns></returns>
    IEnumerator IEBearecycle()
    {
        WaitForSeconds loop = new WaitForSeconds(30);
        for (; monsterhealth > 0;)
        {
            //  Debug.Log("wild has start find enemy");
            if (Random.Range(0, 1f) > 0.5f)
            {
                enemytag = wildenemy[0];
                enemytag_ast = wildenemy[1];
            }
            else
            { enemytag = wildenemy[1];
                enemytag_ast = wildenemy[0];
            }
            if (Random.Range(0, 1f) > 0.5f)
                targetboss = "ICE";
            else
                targetboss = "FIRE";
            yield return loop;
        }

    }
    public void Isattackable()//animation event
    {
        animator0.SetBool("Isattack", true);
    }
    IEnumerator IEiflookat()//the monster trun around of lookAT frequency
    {
        WaitForSeconds loop = new WaitForSeconds(3f);
        for (; monsterhealth > 0;)
        {if (gameObject.activeSelf)
            {
                yield return loop;
                //if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))
                //    if (component.monsterhealth <= 0)
                        iflookat = true;
            }
        }
    }
    public void Reducehealth(int num)//�����¼�����ʱ����
    {
        switch (num)
        {
            case 1:
                if (this.tag.Equals("ICEMAN"))
                {
                    transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.tag = "ICEFIRE";
                }
                else
                {
                    transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.tag = "FIREFIRE";
                }
                break;
            case 2:
                transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.tag = "NONE";
                break;
        }
    }

    private void OnTriggerEnter(Collider collision)//add wild monster logic
    {
        if (!ifWILD)
        {
            if (collision.tag.Equals(enemyfire) && realattack)
            {
                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;

                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.ID].monsterIntegral += component.attacktimes;//Integral increase
                                                                                                               //#if SYSTEM


                    if (killer.ContainsKey(component.ID))
                    {

                        killer[component.ID] += 3 * component.attacktimes;
                        // Debug.Log("contains"+component.ID + "add hurt");

                    }
                    else
                    {
                        killer.Add(component.ID, 3 * component.attacktimes);

                        //  Debug.Log("not contains"+component.ID + "add hurt");
                    }

                    if (monsterhealth < component.attacktimes)
                    {
                        // Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag

                        foreach (var item in killer)
                        {
                            if (item.Value > maxValue)
                            {
                                maxValue = item.Value;
                                maxKey = item.Key;
                            }
                            // Debug.Log(maxKey + "----" + maxValue);
                        }
                        // Debug.Log("who damage me most hurt " + maxKey);
                    }
                    //#endif
                }
                // Debug.Log("������");
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)//its the judge of that if the monster has an effect of undering attack;
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);
            }
            if (collision.tag.Equals(boss)||collision.tag.Equals(innerreduce))
            {
               if(!inbossarea)
                StartCoroutine("IEreduce");
            }
            if (collision.tag.Equals(enemybullet))//mecha hurt
            {

                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;
                    Netpool.Getinstance().monsterStruct[component.ID].monsterIntegral += component.attacktimes;
                    //Integral increase
                    //Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    if (monsterhealth < component.attacktimes)
                    {
                        //  Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag

                    }
                    //Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                }
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);
            }
            if (collision.tag.Equals(enemyTS))//the skill of TS can Cause 30 damage points;
            {
                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;
                    Netpool.Getinstance().monsterStruct[component.ID].monsterIntegral += component.attacktimes;//Integral increase
                    if (monsterhealth < component.attacktimes)
                    {
                        // Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    }
                }
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);

            }
            if (collision.tag.Equals("WILDATC") && realwildattack)//the skill of TS can Cause 30 damage points;
            {
                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;
                    realwildattack = false;
                    if (monsterhealth < component.attacktimes)
                    {
                        // Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    }
                }
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);

            }

        }
        else
        {
            if ((collision.tag.Equals(wildenemy[4]) || collision.tag.Equals(wildenemy[5])) && realattack)//normal attack
            {
                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;
                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.ID].monsterIntegral += component.attacktimes;//Integral increase
                    if (monsterhealth < component.attacktimes)
                    {
                        //Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    }
                    //Debug.Log("wild under attack");
                    GameManager.instance.AddSelfScoreAttakWild(component);
                }
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);
            }
            if (collision.tag.Equals(wildenemy[6]) || collision.tag.Equals(wildenemy[7]) || collision.tag.Equals(wildenemy[8]) || collision.tag.Equals(wildenemy[9]))
            {
                if (collision.gameObject.TryGetComponent<Attack>(out Attack component) && ifIntegral)//get out the attacktimes of the obj
                {
                    this.monsterhealth -= 3 * component.attacktimes;
                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.ID].monsterIntegral += component.attacktimes;//Integral increase
                    if (monsterhealth < component.attacktimes)
                    {
                        //Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    }
                    // Debug.Log("wild under attack");
                    GameManager.instance.AddSelfScoreAttakWild(component);
                }
                if (ifunderattack && Random.Range(0, 1f) < appearProbability)
                    Netpool.Getinstance().Insgameobj(underattackVFX, transform.position + directOffset + new Vector3(Random.Range(-1, 1f) * underattackVFXOffset.x, Random.Range(-1, 1f) * underattackVFXOffset.y + Random.Range(-1, 1f) * underattackVFXOffset.y),
                        Quaternion.Euler(underattackVFXRotate.x, underattackVFXRotate.y, underattackVFXRotate.z), vfx.transform, underattackScale);
            }
            if (collision.tag.Equals(wildenemy[2]) || collision.tag.Equals(wildenemy[3])||collision.tag.Equals(innerreduce))//if the monster run into BOSS field;
            {if(!inbossarea)
                StartCoroutine("IEreduce");
                // GameManager.instance.AddSelfScoreAttakWild(component);
            }

        }

        // Debug.Log("��ײ��");
    }
    private void OnTriggerExit(Collider other)//lock the damage of hrut

    {
        if (!ifWILD)
        {
            if (other.tag.Equals(boss))
            {
                if (inbossarea)
                { StopCoroutine("IEreduce");
                  inbossarea = false;
                }
            }
            if (other.tag.Equals(enemyfire))
            {
                realattack = true;
            }
            if (other.tag.Equals("WILDATC"))
            {

                realwildattack = true;

            }
        }
        else
        {
            if (other.tag.Equals(wildenemy[2]) || other.tag.Equals(wildenemy[3]))
            {
                if (inbossarea)
                {
                    StopCoroutine("IEreduce");
                    inbossarea = false;
                }
            }
            if (other.tag.Equals(wildenemy[4]) || other.tag.Equals(wildenemy[5]))
            {
                realattack = true;
            }
        }
    }
    IEnumerator IEreduce()//which goes into the field and its blood has being reduce
    {
        WaitForSeconds loop = new WaitForSeconds(1);
        inbossarea = true;
            for (; monsterhealth > 0;)
            {
                yield return loop;
                monsterhealth -= 1;
            }
        
    }

    /// <summary>
    /// trail control
    /// </summary>
    /// <param name="num"></param>
    public void ToogleTrail(int num)
    {
        switch (num)
        {
            case 1:
                if (Random.Range(0, 1f) < TrailPI)
                    TrailTransform.GetComponent<TrailRenderer>().emitting = true;
                break;
            case 2:
                TrailTransform.GetComponent<TrailRenderer>().emitting = false;
                break;
        }
    }
    /// <summary>
    ///finally skii moudle
    /// </summary>
    /// <param name="num"></param>
    /// 

    IEnumerator IEFinallySkill()//camp skill of zhe player
    {
        WaitForSeconds loop = new WaitForSeconds(1);
        for (; ; )

        {
            yield return loop;
            if (Monsterins.ifuseskill && ifuseskill)//Monster skill
            {

                if (Monsterins.ificecamp && amicecamp)
                {
                    monsterhealth += 0.5f * healthcache;//Monster's maximum health increased by 50%
                    movespeed += movespeed * 0.4f;//Monster's maximum velocity increased by 50%
                                                  //Monsterins.icecampdamage = 1.4f;//Monster's maximum damage increased by 50%
                                                  // this.transform.localScale += new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 0.3f;//Monster's maximum scale increased by 50%
                                                  //  Debug.Log("ice camp is  very strong!!!!!!!!!!!!!!!!!!!!!!!!health and spped");
                    ifuseskill = false;//you can use the finally skill once
                    StartCoroutine("IEFinllySkillState");//Start the coroutine of fillay skill
                    if (!ifWILD)
                        Netpool.Getinstance().Insgameobj(killtime, transform.position, Quaternion.identity, vfx.transform);
                    break;
                }
               
                else if (Monsterins.iffirecamp && !amicecamp)
                {

                    monsterhealth += 0.5f * healthcache;//Monster's maximum health increased by 50%
                    movespeed += movespeed * 0.4f;//Monster's maximum velocity increased by 50%
                                                  // Monsterins.icecampdamage = 1.4f;//Monster's maximum damage increased by 50%
                                                  // this.transform.localScale += new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 0.3f;//Monster's maximum scale increased by 50%
                                                  // Debug.Log("fire camp is  very strong!!!!!!!!!!!!!!!!!!!!!!!!health and spped");
                    ifuseskill = false;//you can use the finally skill once
                    StartCoroutine("IEFinllySkillState");//Start the coroutine of fillay skill
                    if (!ifWILD)
                        Netpool.Getinstance().Insgameobj(killtime, transform.position, Quaternion.identity, vfx.transform);
                    break;
                }
            }
        }
    }
    /// <summary>
    ///finally skill IE
    /// </summary>
    /// <param name="num"></param>
    /// 
    IEnumerator IEFinllySkillState()//the coroutines of skill
    {
        yield return new WaitForSeconds(30);
        if (Monsterins.ificecamp)
        {
            movespeed = speedchache; ;//recover the speed;
            //this.transform.localScale += new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 0.3f;//Monster's maximum scale increased by 50%

        }
        else if (Monsterins.iffirecamp)
        {
            movespeed = speedchache;//recover the speed;
        }
        //Debug.Log("strong time has done.enjony your fun");
    }

    public void AnimationEventIflook(int num)
    { switch (num)

        { case 1:
                iflookat = false;
                break;

            case 2:
                iflookat = true;
                break;
        }



    }
    /// <summary>
    ///module of the density of  vfx
    /// </summary>
    /// <param name="num"></param>
    /// 
    IEnumerator IEStartToAdjustDensity()
    {
        WaitForSeconds loop = new WaitForSeconds(1);
        for (; monsterhealth > 0;)
        {
            yield return loop;

            StartToAdjustDensity();

        }

    }
    void StartToAdjustDensity(int level)
    {
        switch (level)
        {
            case 1:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 2:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;

            case 3:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 4:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 5:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 6:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 7:
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 8://���ع�
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 9://�ľ�з
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 10://֩��
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
            case 11://��ԭ��
                if (Monsterins.instance.objnumcache <= 50)
                {
                    appearProbability = appearProbabilityCache;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache;

                }
                else if (Monsterins.instance.objnumcache > 50 || Monsterins.instance.objnumcache <= 100)
                {
                    appearProbability = appearProbabilityCache * 0.7f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.7f;

                }
                else if (Monsterins.instance.objnumcache > 100 || Monsterins.instance.objnumcache <= 150)
                {
                    appearProbability = appearProbabilityCache * 0.5f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.5f;

                }
                else if (Monsterins.instance.objnumcache > 150 || Monsterins.instance.objnumcache <= 200)
                {
                    appearProbability = appearProbabilityCache * 0.3f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.3f;

                }
                else if (Monsterins.instance.objnumcache > 200 || Monsterins.instance.objnumcache <= 250)
                {
                    appearProbability = appearProbabilityCache * 0.2f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.2f;

                }
                else if (Monsterins.instance.objnumcache > 250)
                {
                    appearProbability = appearProbabilityCache * 0.1f;
                    IDtransform.TryGetComponent<Attack>(out Attack component1);
                    component1.appearPI = component1.appearPICache * 0.1f;

                }
                break;
        }
    }
    void StartToAdjustDensity()
    {
        if (Monsterins.instance.objnumcache <= 50)
        {
            appearProbability = appearProbabilityCache;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache;

        }
        else if (Monsterins.instance.objnumcache > 50 && Monsterins.instance.objnumcache <= 100)
        {
            appearProbability = appearProbabilityCache * 0.7f;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache * 0.7f;

        }
        else if (Monsterins.instance.objnumcache > 100 && Monsterins.instance.objnumcache <= 150)
        {
            appearProbability = appearProbabilityCache * 0.5f;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache * 0.5f;
            
        }
        else if (Monsterins.instance.objnumcache > 150 &&Monsterins.instance.objnumcache <= 200)
        {
            appearProbability = appearProbabilityCache * 0.3f;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache * 0.3f;
            

        }
        else if (Monsterins.instance.objnumcache > 200 && Monsterins.instance.objnumcache <= 250)
        {
            appearProbability = appearProbabilityCache * 0.2f;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache * 0.2f;

        }
        else if (Monsterins.instance.objnumcache > 250)
        {
            appearProbability = appearProbabilityCache * 0.1f;
            IDtransform.TryGetComponent<Attack>(out Attack component1);
            component1.appearPI = component1.appearPICache * 0.1f;

        }
    }
    /// <summary>
    ///module of the adjust the scene
    /// </summary>
    /// <param name="num"></param>
    /// 
    IEnumerator IECheckTheY()
    {
        WaitForSeconds loop = new WaitForSeconds(0.5f);
        for (; monsterhealth > 0;)
        { if (transform.position.y > 13)
            {
                GetComponent<CapsuleCollider>().isTrigger = true;

            }
            else if (transform.position.y <= 5)
            {

                GetComponent<CapsuleCollider>().isTrigger = false;

            }
            yield return loop;
        
        }
    }
    /// <summary>
    ///module of Observer mode
    /// </summary>
    /// <param name="num"></param>
    /// 
    private void Subscribe(Publisher publisher)
    {
        //var handel = Publisher.Getinstance().GetPublisherEvent();
        //handel += OnMyEventHappen;
        publisher.EmyEvent += OnMyEventHappen;
        //Debug.Log("has subscribe");
    }

    private void OnMyEventHappen(object sender, EventArgs e)
    {
        Debug.Log("Event has been triggered.");
    }

    private void SubscribeBossMessage(Rayboss rayboss)
    {
        rayboss.EMyEvent += OnBossEventHappen;
    }
    private void OnBossEventHappen(object sender, MyCustomEventArgs e)
    {

        Debug.Log(e.Message);
    
    }
        
    
}

