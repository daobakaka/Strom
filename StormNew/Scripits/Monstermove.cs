using com.unity.mgobe.src.Util.Def;
using Games.Characters.EliteUnits;
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
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
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
    private Attack attack;
    public float movespeed;
    public float distance;

    public string targetboss;
    public string targetenemy;
    //是否使用过反击时刻
    private bool ifuseskill;
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool isDie;

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
    public float monsterHealthTotal;
    public float monsterhealth;
    private bool inbossarea;
    public float pirandom = 0.3f;//obj attack field
    private GameObject vfx;
    public bool UIdeath;
    //�������
    /// <summary>
    ///���Լ������ֿ���
    /// </summary>
    [Header("MONITOR")]
    private bool iflookat = true;
    private bool realattack = true;
    private bool realwildattack = true;
    //精英怪攻击的目标
    public GameObject showtarget = null;
    //是否攻击的Boss
    public bool isAttackBoss;

    private GameObject subscribBoss = null;
    //��������
    [Header("ANIMATION")]
    //是否有两个攻击动画
    public bool iftwoattack;
    private float animatorpara;
    /// <summary>
    ///是否机甲
    /// </summary>
    public bool ismechea;
    public float mecheaAttackCD1=3;
    private float tempmecheaAttackCD;
    private bool iscontinueMove;
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
    public MonsterConfigs monsterConfigs;
    public MonsterType monsterType;
    public MonsterData monsterData;
    public float damage;
    public float skilldamage;
    public GameObject counterattackMoment = null;
    public bool ifcounterattackMoment = false;
    [Header("WILD")]
    //是否去追击野怪
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
    //取最后一个造成伤害得人
    public string maxKey;
    /// <summary>
    /// player ID module
    /// </summary>
    public string playerID;
    public Transform IDtransform;
    public bool ifIntegral;
    public GameObject killtime;
    public Blood blood;
    public bool showNameTag=false;
    /// <summary>
    /// TS change its body module 死神不是死亡
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
    [Header("EntityTransfer")]
    public EntityManager entityManager;
    public Entity tagentity;
    public UnitSprint myUnit1;
    public Transform showTargetEntity;
    [SerializeField]//false 在enity世界
    private bool ObjWorld;
    void InsEntityObj()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        tagentity = entityManager.CreateEntity();
        monsterHealthTotal = monsterhealth;
        if (monsterData.team==  Games.Characters.EliteUnits.EliteUnitPortalMan.Team.Human)
        {
            entityManager.AddComponentData<Unit1Component>(tagentity, new Unit1Component { order = 101, health = 0,damage=damage,speed=movespeed, monsterType = monsterData.monsterType, team = monsterData.team });
            entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
            entityManager.AddComponentData<UnitSprint>(tagentity, myUnit1 = new UnitSprint { sprint = false, monsterType=monsterData.monsterType,team= monsterData.team} );
            entityManager.SetName(tagentity, "unitIceMonster");
        }
        else
        {
            entityManager.AddComponentData<Unit1Component>(tagentity, new Unit1Component { order = 102, health = 0, damage = damage, speed = movespeed, monsterType = monsterData.monsterType, team = monsterData.team });
            entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
            entityManager.AddComponentData<UnitSprint>(tagentity, myUnit1 = new UnitSprint { sprint = false, monsterType = monsterData.monsterType, team = monsterData.team });
            entityManager.SetName(tagentity, "unitFireMonster");
        }
    }
    void DestoryEntityObj()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        if(entityManager.Exists(tagentity))
        commandBuffer.DestroyEntity(tagentity);
        if(entityManager!=null)
        commandBuffer.Playback(entityManager);
        commandBuffer.Dispose();


    }
    void SyncPosition()
    {
        if (monsterData.team == Games.Characters.EliteUnits.EliteUnitPortalMan.Team.Human)
            ObjWorld = Monsterins.IceObjWorld;
        else
            ObjWorld = Monsterins.FireObjWorld;
        // entityManager.SetComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, this.transform.rotation));
        if (entityManager.Exists(tagentity)&& entityManager.HasComponent<Unit1Component>(tagentity))
        {
            var healthReduce = entityManager.GetComponentData<Unit1Component>(tagentity);
            healthReduce.isAttackCD = tempmecheaAttackCD > 0;
            healthReduce.iscontinueMove = iscontinueMove;
            //现实中剩余血量+Entity世界扣的血量
            monsterHealthTotal = monsterhealth + healthReduce.health;

            entityManager.SetComponentData<Unit1Component>(tagentity, healthReduce);
        }
        FacedAndGoToEenemy();

    }
    void FacedAndGoToEenemy()
    {
        var entityTransform = entityManager.GetComponentData<LocalTransform>(tagentity);
        var entityAnimation = entityManager.GetComponentData<UnitSprint>(tagentity);
        if (!ObjWorld)
        {
            this.transform.rotation = entityTransform.Rotation;
            this.transform.position = entityTransform.Position;
            if (entityAnimation.attack == true)
            {
                if (ismechea)
                {
                    //导弹攻击
                    MecheaAttack();
                }
                else
                {
                    //普通攻击
                    if (iftwoattack && animatorpara > 0.1f)
                    {
                        animator0.SetTrigger("attackhand");
                    }
                    else
                    {

                        animator0.SetTrigger("attack");
                    }
                }
                animator0.SetBool("walking", false);

            }
            else
            {
                animator0.SetBool("walking", true);
            }
        }
        else
        {
            entityTransform.Rotation = this.transform.rotation;
            entityTransform.Position = this.transform.position;
            entityManager.SetComponentData<LocalTransform>(tagentity, entityTransform);
          //  Debug.Log("it is obj time");
        }
 
    }
    void ReadConfig()
    {
        monsterConfigs = HttpRquest.instance.GetMonsterConfigs(monsterData.monsterType);
        monsterHealthTotal= monsterhealth = monsterConfigs.monsterHealth;
        damage = monsterConfigs.damage;
        movespeed = monsterConfigs.monsterSpeed;
        skilldamage = monsterConfigs.skillDamage;
        IDtransform.TryGetComponent<Attack>(out Attack component1);
        //component1.attacktimes = damage;
        //component1.attackchache = damage;
        component1.appearPICache = component1.appearPI;
        appearProbabilityCache = appearProbability;
    }

    //-----------
    IEnumerator IEChangeMyCollisonExculde()//the mecthod for change the collider;
    {
        yield return new WaitForSeconds(5);
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; monsterHealthTotal > 0;)
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
        for (; monsterHealthTotal> 0;)
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
        monsterData.team =Monsterins.TurnTeam(team);
        monsterData.uid = uid;
        monsterData.monsterType = type;
        monsterType = type;

        Init();
        GameManager.instance.allMonster.Add(this);
      
    }
    private void Init()
    {
        showtarget = null;
        isAttackBoss = false;
        isDie = false;
        enemytag = null;
        controlToKillWild = false;
        dissovlerecover = true;
        ifuseskill = true;
        iflookat = true;
        showtarget = null;
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = true;
        capsuleCollider.isTrigger = false;//recover the collsion of obj
        gameObject.tag = mytag;
        UIdeath= false;
        inbossarea = false;
        ObjWorld = true;
        ReadConfig();//read the config
                     //初始化攻击
        attack = transform.GetComponentInChildren<Attack>();
        attack?.Init(this);

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
        StartCoroutine("IEiflookat");
       // StartCoroutine("ClearGameobj");//this IE use to check the gameobj which has not disappear when its health less than zero;
        if (ismechea && !isTS)
            StartCoroutine("IEMechaVFX");
        ///----finaly skill
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
        InsEntityObj();
    }

    private void OnDisable()
    {
        DestoryEntityObj();
        StopCoroutine("IEiflookat");
        StopCoroutine("IEMechaShoot");//stop mecha coroutine
        StopCoroutine("IETSskill");//close Coroutine of TS skill
       // StopCoroutine("ClearGameobj");
        StopCoroutine("IEMechaVFX");
        StopCoroutine("IEDeathDissolve");
        StopCoroutine("IEFindWild");//
        StopCoroutine("IEBearecycle");//
        StopCoroutine("IEStartToAdjustDensity");
        StopCoroutine("IEreduce");
        ifdissovle = true;
        StopCoroutine("IECheckTheY");
        StopCoroutine("IEChangeMyLayer");
        gameObject.layer = LayerMask.NameToLayer("Default");//set the layer
       if(counterattackMoment!=null)
        counterattackMoment.SetActive(false);

   

        // StopCoroutine("IEChangeMyCollisonExculde");
        //if (ifdissovle)
        //    MonsterDeathBefore();
        //if (isdeath)
        //    StopCoroutine("IEDeathskill");//

        // Debug.LogError("death " +playerID);
        //  Debug.Log("monster death");    }
    }
    // Update is called once per frame
    float tempTimer=0;
    void Update()
    {
        if (isDie) return;
        tempTimer += Time.deltaTime;
        if (GameManager.instance.isGameFighting)
        {
            if (!ifWILD)
            {
                if (ismechea)
                {
                    MecheaMove();
                }
                else
                {
                    Realmove();
                }
            }
            else
            {
               // UnityEngine.Profiling.Profiler.BeginSample("MyMethod Wildmove");
                WildMove();
               // UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        transform.localPosition = new Vector3(transform.localPosition.x,0, transform.localPosition.z);
        SyncPosition();//the method of entity
        CheckCounterattack();
        if (tempTimer > 5)
        {
            tempTimer = 0;

            RandomAtackType();
        }
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

        for (; monsterHealthTotal > 0;)
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
    /// <summary>
    /// mechce IE
    /// </summary>
    /// 
    IEnumerator IEMechaShoot()//the module of mecha shoot
    {
        WaitForSeconds p = new WaitForSeconds(0.2f);
        for (; monsterHealthTotal > 0;)
        {

            for (int i = 0; i < 3; i++)
            {
                yield return p;
                GameObject gameObject0 = Netpool.Getinstance().Insgameobj(mechabullet, this.transform.GetChild(1).position, Quaternion.identity, vfx.transform);
                //  Debug.Log(this.gameObject.name);
                gameObject0.GetComponent<FindEnemy>().Init(this);
                gameObject0.TryGetComponent<Attack>(out Attack attack);
                attack.Init(this);
            }
            break;
            // Debug.Log("��ʼ����");
        }
    }
    IEnumerator IEMechaVFX()
    {
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; monsterHealthTotal > 0;)
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
        for (; monsterHealthTotal > 0;)
        {

            for (int i = 0; i < 3; i++)
            {
                yield return p;
                GameObject gameObject0 = Netpool.Getinstance().Insgameobj(TSskillVFX, new Vector3(transform.position.x, 7, transform.position.z) + transform.forward * 80 * (i + 1), Quaternion.Euler(-90, Random.Range(0, 350), 0), vfx.transform, true);
                gameObject0.TryGetComponent<Attack>(out Attack attack);
                attack.Init(this);
                //在Entity产生伤害
                attack.AttackEffect(false);
                //  Debug.Log(this.gameObject.name);
            }
            break;
            // Debug.Log("��ʼ����");
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

        if (monsterHealthTotal > 0)
            //  StartCoroutine("IEDeathskill");
            DeathSkill();
    }
    IEnumerator IEDeathskill()
    {
        WaitForSeconds loop = new WaitForSeconds(0.2f);
        if (monsterHealthTotal > 0)
            Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 1; i++)
        {
            yield return loop;
            if (monsterHealthTotal > 0)
                Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx

        }
    }

    void DeathSkill()
    {

        if (monsterHealthTotal > 0)
        {
            // Debug.Log("has start the death's Skill");

            //Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis
            if (showtarget != null)
            {
                Netpool.Getinstance().Insgameobj(deathSkill[0], transform.position + deathSkillOffset_assisit, Quaternion.identity, vfx.transform);//the skill of death's halo,and it rotate -90 by x_axis


                if (showtarget.TryGetComponent<Monstermove>(out Monstermove component2)) //攻击精英怪
                { if (component2.monsterHealthTotal > 0)
                    {
                        GameObject clone = Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx
                        clone.TryGetComponent<FX_LifeTime>(out FX_LifeTime component);
                        clone.TryGetComponent<Attack>(out Attack component1);
                        component1.Init(this);
                        component.monstermove = this;
                        component.showtarget = showtarget.gameObject;//pass the forward transform for rockfire effects;  // Debug.Log("has generate a fire ball!!!!!!!");

                    }

                }
                else
                {
                    if (showtarget.tag.Equals("ICE") || showtarget.tag.Equals("FIRE")) //boss
                    {
                        GameObject clone = Netpool.Getinstance().Insgameobj(deathSkill[1], transform.position + deathSkillOffset + new Vector3(Random.Range(-1, 1f) * range.x, 0, Random.Range(-1, 1f) * range.z), Quaternion.identity, vfx.transform);//the skill of death's vfx
                        clone.TryGetComponent<FX_LifeTime>(out FX_LifeTime component);
                        clone.TryGetComponent<Attack>(out Attack component1);
                        component1.Init(this);
                        component.monstermove = this;
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
            if (ObjWorld)
            {
                if (monsterHealthTotal > 0)
                {
                    if (!isAttackBoss && showtarget != null && showtarget.tag != "Untagged")
                    {
                        //不需要重新找目标
                    }
                    else
                    {
                        showtarget = Monsterins.instance.GetLastTarget(transform.position, monsterData.team);
                    }
                    if (showtarget != null)//
                    {
                        isAttackBoss = false;
                        var direction = (showtarget.transform.position - this.transform.position);//
                        if (showtarget != null)
                            this.transform.LookAt(showtarget.transform.position, Vector3.up);
                        //iflookat = false;
                        if (direction.sqrMagnitude > 10f)
                        {
                            if (direction.sqrMagnitude > pirandom * distance)
                            {
                                animator0.SetBool("walking", true);
                                direction.y = 0;
                                transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                                myUnit1.sprint = true;
                                entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                            }
                            else
                            {
                                if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))//
                                {
                                    if (component.monsterHealthTotal > 0)
                                    {
                                        AttackAnim();

                                        animator0.SetBool("walking", false);
                                        myUnit1.sprint = false;
                                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                                    }
                                    else
                                    {
                                        animator0.SetBool("walking", true);
                                        myUnit1.sprint = true;
                                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        showtarget = GameObject.FindWithTag(targetboss);
                        if (showtarget != null)
                        {
                            isAttackBoss = true;
                            var direction = (showtarget.transform.position - this.transform.position);

                            this.transform.LookAt(showtarget.transform.position, Vector3.up);
                            //iflookat = false;

                            if (direction.sqrMagnitude > 10.1f)
                            {

                                if (direction.sqrMagnitude > 1.3f * distance)
                                {
                                    direction.y = 0;
                                    transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                                    animator0.SetBool("walking", true);
                                    myUnit1.sprint = true;
                                    entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                                }
                                else
                                {
                                    AttackAnim();

                                    animator0.SetBool("walking", false);
                                    myUnit1.sprint = false;
                                    entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Death();
                }
            }
        }
        else
        {
            if (GameObject.FindWithTag("WILD") != null && monsterHealthTotal > 0)
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
                        direction.y = 0;
                        transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                        animator0.SetBool("walking", true);
                        myUnit1.sprint = true;
                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                    }
                    else
                    {
                        AttackAnim();

                        animator0.SetBool("walking", false);
                        myUnit1.sprint = false;
                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);

                    }
                }
            }
            else if (monsterHealthTotal <= 0)
            {
                Death();
            }
            else
            {

                ifkillwild = false;
            }

        }
    }
    #region 机甲逻辑
    /// <summary>
    /// 机甲的移动方式
    /// </summary>
    public void MecheaMove()
    {
        tempmecheaAttackCD -= Time.deltaTime;

        if (monsterHealthTotal > 0)
        {
            if (ObjWorld)
            {
                if (!isAttackBoss && showtarget != null && showtarget.tag!= "Untagged")
                {
                    //不需要重新找目标
                }
                else
                {
                    showtarget = Monsterins.instance.GetLastTarget(transform.position, monsterData.team);
                    if (showtarget == null)
                    {
                        //寻找Boss
                        showtarget = GameObject.FindWithTag(targetboss);
                        isAttackBoss = true;
                    }
                    else 
                    {
                        isAttackBoss = false;
                    }
                }
                if (showtarget != null)//
                {
                    var direction = (showtarget.transform.position - this.transform.position);//
                    direction.y = 0;
                    this.transform.LookAt(Vector3.Lerp(this.transform.position, showtarget.transform.position, 0.1f), Vector3.up);
                    // 攻击CD CD间隙要移动
                    if (direction.sqrMagnitude > EntityOfMonitor.Instance.entityEliteDamageDis * 10)
                    {
                        iscontinueMove = false;
                        animator0.SetBool("walking", true);
                        transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        MecheaAttack();
                        if (iscontinueMove)
                        {
                            animator0.SetBool("walking", true);
                            transform.Translate(direction.normalized * movespeed * Time.deltaTime, Space.World);
                        }
                    }
                }
            }
        }
        else
        {
            Death();
        }

    }
    //机甲CD中 判断是否移动
    public void MecheaMove2()
    {
        if (tempmecheaAttackCD < 0) return;
        //攻击CD 继续移动
        var direction = (showtarget.transform.position - this.transform.position);//
        if (direction.sqrMagnitude > EntityOfMonitor.Instance.entityEliteDamageDis)
        {
            iscontinueMove = true;
        }
        else
        {
            //进入待机
            iscontinueMove = false;
        }
    }
    public bool MecheaAttack()
    {
        animator0.SetBool("walking", false);
        if (tempmecheaAttackCD < 0)
        {
            tempmecheaAttackCD = mecheaAttackCD1;
            animator0.SetTrigger("hit");
            iscontinueMove = false;
            Debug.LogError("机甲攻击了");
            return true;
        }
        else 
        {
            return false;
        }
    }
    #endregion
    /// <summary>
    /// the main logic of Wild animals
    /// </summary>
    void WildMove()
    {
        if (monsterHealthTotal > 0)
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
                    direction.y = 0;
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
                                if (component.monsterHealthTotal > 0)
                                {//attack 
                                    //attack
                                    AttackAnim();
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
                                if (component.monsterHealthTotal > 0)
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
            Death();
        }
    }
    /// <summary>
    /// certain the obj which monsterHealthTotal is less than zero that has been recovery
    /// </summary>
    /// <returns></returns>
    IEnumerator IEBearecycle()
    {
        WaitForSeconds loop = new WaitForSeconds(30);
        for (; monsterHealthTotal > 0;)
        {
            RandomTarge();
                        yield return loop;
        }

    }
    /// <summary>
    /// 随机查找目标
    /// </summary>
    public void RandomTarge() 
    {
        if (Random.Range(0, 1f) > 0.5f)
        {
            enemytag = wildenemy[0];
            enemytag_ast = wildenemy[1];
        }
        else
        {
            enemytag = wildenemy[1];
            enemytag_ast = wildenemy[0];
        }
        if (Random.Range(0, 1f) > 0.5f)
            targetboss = "ICE";
        else
            targetboss = "FIRE";
    }
    public void Isattackable()//animation event
    {
        animator0.SetBool("Isattack", true);
    }
    IEnumerator IEiflookat()//the monster trun around of lookAT frequency
    {
        WaitForSeconds loop = new WaitForSeconds(3f);
        for (; monsterHealthTotal > 0;)
        {if (gameObject.activeSelf)
            {
                yield return loop;
                //if (showtarget.TryGetComponent<Monstermove>(out Monstermove component))
                //    if (component.monsterHealthTotal <= 0)
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
                    this.monsterhealth -= component.GetDamge();

                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += component.attacktimes;//Integral increase
                                                                                                               //#if SYSTEM


                    if (killer.ContainsKey(component.monsterData.uid))
                    {

                        killer[component.monsterData.uid] += 3 * component.attacktimes;
                        // Debug.Log("contains"+component.ID + "add hurt");

                    }
                    else
                    {
                        killer.Add(component.monsterData.uid, 3 * component.attacktimes);

                        //  Debug.Log("not contains"+component.ID + "add hurt");
                    }

                    if (monsterHealthTotal < component.attacktimes)
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
                    this.monsterhealth -= component.GetDamge();
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += component.attacktimes;
                    //Integral increase
                    //Debug.Log(component.ID + "kill" + playerID + "the point add " + component.attacktimes + "score, and the type of monster is" + type);//give the one who get last damage a tag
                    if (monsterHealthTotal < component.attacktimes)
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
                    this.monsterhealth -= component.GetDamge();
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += component.attacktimes;//Integral increase
                    if (monsterHealthTotal < component.attacktimes)
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
                    this.monsterhealth -= component.GetDamge();
                    realwildattack = false;
                    if (monsterHealthTotal < component.attacktimes)
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
                    this.monsterhealth -= component.GetDamge();
                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += component.attacktimes;//Integral increase
                    if (monsterHealthTotal < component.attacktimes)
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
                    this.monsterhealth -= component.GetDamge();
                    realattack = false;
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += component.attacktimes;//Integral increase
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
            {
                if (!inbossarea)
                {
                    //Boss扣精英怪的血
                    StartCoroutine("IEreduce");
                }
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
            for (; monsterHealthTotal > 0;)
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
    /// 机甲第二种攻击的动画回调
    /// </summary>
    /// <param name="num"></param>
    public void AnimationEventMechea(int num)//the mecha recover walk after attack
    {
        switch (num)
        {
            case 1:
                Debug.LogError("机甲开始攻击");
                StartCoroutine("IEMechaShoot"); //攻击
                break;
            case 2:
                //animator0.SetBool("walking", true); //行走
                Debug.LogError("机甲结束攻击");
                MecheaMove2();
                break;
        }
    }
    //攻击动画开始 动画结束
    public void AnimationEventIflook(int num)
    { switch (num)

        { case 1:
                iflookat = false;
                //通知Enity 我攻击了
                var entityAnimation = entityManager.GetComponentData<UnitSprint>(tagentity);
                entityAnimation.generalattackBack = true;
                entityManager.SetComponentData<UnitSprint>(tagentity, entityAnimation);
                //
                if (ifWILD)
                attack?.AttackOpen();
                Debug.LogError("攻击开始");
                break;

            case 2:
                iflookat = true;
                Debug.LogError("攻击结束");
                break;
            case 3:
                Debug.LogError("最高级死神攻击中间段");
                Transform aaa = transform.Find("EffectPos");
                attack.DeathAttackEffect(aaa);
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
        for (; monsterHealthTotal > 0;)
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
        for (; monsterHealthTotal > 0;)
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
    private void OnBossEventHappen(object sender, Subscriber.MyCustomEventArgs e)
    {

        Debug.Log(e.Message);
    
    }
    #region 反击时刻
    float CheckCounterattackTimer=1;
    /// <summary>
    /// 检测反击时刻
    /// </summary>
    public void CheckCounterattack() 
    {
        if (!ifWILD && Monsterins.ifuseskill)
        {
            if ((Monsterins.ificecamp && monsterData.team == EliteUnitPortalMan.Team.Human)||(Monsterins.iffirecamp && monsterData.team == EliteUnitPortalMan.Team.Org))
            {
                if (ifuseskill)
                {
                    CheckCounterattackTimer -= Time.deltaTime;
                    if (CheckCounterattackTimer < 0)
                    {
                        CheckCounterattackTimer = 30;
                        ifuseskill = false;
                        monsterhealth += 0.5f * monsterConfigs.monsterHealth;
                        movespeed += movespeed * 0.4f;

                        StartCoroutine("IEFinllySkillState");
                        if (!ifWILD && !ifcounterattackMoment)
                            Netpool.Getinstance().Insgameobj(killtime, transform.position, Quaternion.identity, vfx.transform);
                        else if (ifcounterattackMoment && !ifWILD)
                            counterattackMoment.SetActive(true);
                    }
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
        if (!ifWILD)
        {
            yield return new WaitForSeconds(30);
            movespeed = monsterConfigs.monsterSpeed;
            counterattackMoment.SetActive(false);
            Debug.Log("技能结束");
        }
    }
    #endregion
    #region 攻击相关

    /// <summary>
    /// 随机攻击方式
    /// </summary>
    public void RandomAtackType()
    {
        animatorpara = Random.Range(0, 1f);
    }
    public void AttackAnim()
    {
        if (iftwoattack && animatorpara > 0.5f)
        {
            animator0.SetTrigger("attackhand");
        }
        else
        {
            animator0.SetTrigger("attack");
        }
    }
    #endregion
    #region 死亡相关
    /// <summary>
    /// 死亡
    /// </summary>
    public void Death()
    {
        isDie = true;
        if (ObjWorld)
        {
            myUnit1.sprint = false;
            entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
        }
        animator0.SetBool("death", true);
        animator0.SetBool("walking", false);
        this.gameObject.tag = "Untagged";
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;
        Monsterins.instance.RemoveEliteMonster(this);
    }
    /// <summary>
    /// 死亡动画（播放结束回调）
    /// </summary>
    public void MonsterDeath()
    {
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);
    }
    //public void WildMonsterDeath()
    //{
    //    Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);
    //   // Debug.Log("i call mechod here,  name" + gameObject.name);
    //}
    /// <summary>
    ///死亡动画（开始播放回调）
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
            if (blood != null && blood.gameObject.activeSelf && blood.target == transform)
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
    #endregion
}

