using Games.Characters.EliteUnits;
using Games.Characters.SDKLayer;
using Games.Characters.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterinsData
{
    public Team team;
    public MonsterType type;
    public string uid = "1";
}
public class Monsterins : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] spwanpoint;
    public GameObject monster1;
    public GameObject monster2;
    public GameObject monster3;
    public GameObject monster4;
    public GameObject[] monsterunilt;
    public Transform mother;
    public static bool ifclone = true;
    public float spawnerPosionRandom;
    public static Monsterins instance;
    //等待生成的步兵 骑兵
    public List<MonsterinsData> waitGenerateList = new List<MonsterinsData>();
    /// <summary>
    /// across
    /// </summary>
    public bool ifbooslive = true;
    /// <summary>
    /// module of the most right skill
    /// </summary>
    public static bool ifuseskill;
    public static bool ificecamp;
    public static bool iffirecamp;
    public static float bossDamageReduce = 1;
    public static bool protectBossICE;
    public static float protectBossICE_assist = 1;
    public static bool protectBossFIRE;
    public static float protectBossFIRE_assist = 1;
    private bool bossmonitor;
    /// <summary>
    /// count system
    /// </summary>
    public int objnum;
    public int vfxnum;
    public int objnum_assist;
    public int objnumcache;
    public int testnum;
    // public static int deathnum;
    public int excutetime;
    /// <summary>
    /// protect time for world
    /// </summary>
    private float iceHPcache, fireHPcache;
    /// <summary>
    /// the module of subscribe
    /// </summary>
    public Action<String> AActionForBoss;
    public EventHandler EEventHandler;
    public static int ifCheackCollison;
    private bool ifcheck = false;
    public GameManager gameManager0;
    /// <summary>
    /// module of world trasfer  false 在enity世界
    /// 自己有精英怪 对面没有精英怪 但有步兵骑兵 就进入enity世界
    /// </summary>
    public static bool IceObjWorld;
    public static bool FireObjWorld;
    void Start()
    {


        //-----
        Debug.Log("-----清算缓存池");
        Netpool.Getinstance().Clearpoll();//场景开始清除缓存
        transform.GetChild(0).gameObject.SetActive(true);
        //------
        StartCoroutine("IESpawnerPosition");// start the conroutine of the gameobject spawn positon
        //-----
        StartCoroutine("IEBossMonitorForUI");//start the coroutine of BOSS health monitor ,and it use to trriger the finally skill of tow sides camps;
        //--
        StartCoroutine("IEICEBossReduceDamage");//start the coroitine of boss emit golden light,the boss has 90% off damage;
        //---
        StartCoroutine("IEFIREBossReduceDamage");//start the coroitine of boss emit golden light,the boss has 90% off damage;
                                                 //  SkillParIns();//give the skill par ins
                                                 //---
        StartCoroutine("IECaculateObjCount");//start the IEcoroutine
        ActionModule();
        AActionForBoss("it is an Aciton for boss");
        //gameManager0 = GameObject.FindWithTag("GAMEMANAGER").GetComponent<GameManager>();
        //Subscribe(gameManager0);
        //Debug.Log("start time to test");

        CoroutineManager.Instance.StartManagedCoroutineHashSet("IETestCoroutine", IETestCoroutine());

    }
    private void Awake()
    {
        instance = this;
    }
    private Monsterins() { }
    public void StartGameForWorld()
    {
        Netpool.Getinstance().Clearpoll();//场景开始清除缓存
        SkillParIns();
        StopCoroutine("IESpawnerPosition");
        StopCoroutine("IEBossMonitorForUI");
        StopCoroutine("IEICEBossReduceDamage");
        StopCoroutine("IEICEBossReduceDamage");
        StopCoroutine("IECaculateObjCount");
        //-------------------------------------------------
        StartCoroutine("IESpawnerPosition");// start the conroutine of the gameobject spawn positon
        //-----
        StartCoroutine("IEBossMonitorForUI");//start the coroutine of BOSS health monitor ,and it use to trriger the finally skill of tow sides camps;
        //--
        StartCoroutine("IEICEBossReduceDamage");//start the coroitine of boss emit golden light,the boss has 90% off damage;
        //---
        StartCoroutine("IEFIREBossReduceDamage");//start the coroitine of boss emit golden light,the boss has 90% off damage;
        //---
        StartCoroutine("IECaculateObjCount");//start the IEcoroutine
        //
        StartCoroutine("IECacheBossHp");
        //
        StartCoroutine("IEIfcheck");

        gameManager0 = GameManager.instance;
        gameManager0.EMyEvent = null;
        Subscribe(gameManager0);
        Debug.Log("start time to test");
        IceObjWorld = true;
        FireObjWorld = true;

    }
    private void ActionForBoss(string word)
    {

        Debug.Log(word);


    }
    private void ActionModule()
    {
        AActionForBoss = ActionForBoss;

    }
    IEnumerator IECacheBossHp()
    {
        yield return new WaitForSeconds(6);

        iceHPcache = Rayboss.ICEbossHP;
        fireHPcache = Rayboss.ICEbossHP;
        Debug.Log($"-----------ice={iceHPcache}and the rayboss={Rayboss.ICEbossHP}---fire={fireHPcache}and the rayboss={Rayboss.FIREbossHP}---------------");

    }

    private void OnEnable()
    {
        // SkillParIns();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(IEins(13));
                GameManager.instance.addFightScorePool(MonsterType.WILD1);
            }
        }
        if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(IEins(14));
                GameManager.instance.addFightScorePool(MonsterType.WILD2);//*****wjy code******
            }
        }
        if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(IEins(15));
                GameManager.instance.addFightScorePool(MonsterType.WILD3);
            }
        }
        if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(IEins(16));
                GameManager.instance.addFightScorePool(MonsterType.WILD4);//*****wjy code******
            }
        }
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    Debug.Log(this.transform.GetChild(0).childCount);

        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        StartCoroutine(IEins(17));
        //        GameManager.instance.addFightScorePool(MonsterType.PUNISHER);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        StartCoroutine(IEins(18));
        //        GameManager.instance.addFightScorePool(MonsterType.PUNISHER);
        //    }
        //}
        //---clear for clear pool--
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        if (ifclone)
        //            SkillParIns();
        //        ifclone = false;
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        if (ifclone)
        //            CaculateCount();
        //        ifclone = false;
        //    }
        //}
        //------------clear for  clear pool
        ClearUpAll();//BOSS death
        //-----
        KillTheWild("kaka06");//control to kill wild

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    Debug.Log(Netpool.Getinstance().monsterStruct["kaka0"].monsterIntegral);
        //}

        if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (ifclone)
                    PublisherTest();
                ifcheck = false;
            }
        }
        if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (ifclone)
                    ifCheackCollison *= -1;
                ifcheck = false;
                Debug.Log($"has start to checkcollison,and the checking is:    {ifCheackCollison}");
            }
        }

        if (Input.GetKey(KeyCode.F5))//&&Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CoroutineManager.Instance.LogActiveCoroutines();
                CoroutineManager.Instance.StopManagedCoroutineHashSet("IETestCoroutine");

            }
        }
    }
    IEnumerator IESpawnerPosition()
    {
        WaitForSeconds loop = new WaitForSeconds(Time.deltaTime);
        for (; ; )
        {

            spawnerPosionRandom = Random.Range(-249, 249);

            yield return loop;
        }


    }
    IEnumerator IEins(int choose, string uid = "1")
    {
        GameObject target;
        if (ifclone)
        {
            // Debug.Log("开始召唤");
            switch (choose)
            {
                case 1:
                    target = Netpool.Getinstance().Insgameobj(monster1, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka0", MonsterType.infantry);
                    break;
                case 2:
                    target = Netpool.Getinstance().Insgameobj(monster2, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka1", MonsterType.infantry);
                    break;
                case 3:
                    target = Netpool.Getinstance().Insgameobj(monster3, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka2", MonsterType.cavalry);
                    break;
                case 4:
                    target = Netpool.Getinstance().Insgameobj(monster4, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka3", MonsterType.cavalry);
                    break;
                case 5:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[0], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.guard);
                    //  setLabel(target.transform, team, uid, MonsterType.guard);
                    break;
                case 6:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[1], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.guard);
                    //  setLabel(target.transform, team, uid, MonsterType.guard);
                    break;
                case 7:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[2], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.FemaleHunter);
                    //  setLabel(target.transform, team, uid, MonsterType.FemaleHunter);
                    break;
                case 8:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[3], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.FemaleHunter);
                    //  setLabel(target.transform, team, uid, MonsterType.FemaleHunter);
                    break;
                case 9:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[4], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka8", MonsterType.Mecha);
                    //  setLabel(target.transform, team, "uid_dsfsefewr654164", MonsterType.Mecha);
                    break;
                case 10:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[5], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka9", MonsterType.Mecha);
                    // setLabel(target.transform, team, "uid_dsfsefewr654164", MonsterType.Mecha);
                    break;
                case 11:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka10", MonsterType.PUNISHER);
                    //  setLabel(target.transform, team, "uid_dsfsefewr654164", MonsterType.PUNISHER);
                    break;
                case 12:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[7], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka11", MonsterType.PUNISHER);
                    // setLabel(target.transform, team, "uid_dsfsefewr654164", MonsterType.PUNISHER);
                    break;
                case 13:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[8], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_bear", MonsterType.WILD1);
                    break;
                case 14:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[9], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_sprider", MonsterType.WILD2);
                    break;
                case 15:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[10], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_turtle", MonsterType.WILD3);
                    break;
                case 16:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[11], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_hermit", MonsterType.WILD4);
                    break;
                //---test
                case 17:
                    if (Netpool.Getinstance().monsterStruct.ContainsKey("kaka11"))
                    {
                        if (Netpool.Getinstance().monsterStruct["kaka11"].num < 2)
                        {

                            target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka11", MonsterType.PUNISHER);

                        }
                        else

                            target = Netpool.Getinstance().Insgameobj(monsterunilt[12], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka11", MonsterType.PUNISHER);

                    }
                    else
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, "kaka11", MonsterType.PUNISHER);
                    break;
                //---test
                case 18:
                    if (Netpool.Getinstance().monsterStruct.ContainsKey("kaka12"))
                    {
                        if (Netpool.Getinstance().monsterStruct["kaka12"].num < 2)
                        {

                            target = Netpool.Getinstance().Insgameobj(monsterunilt[7], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka12", MonsterType.PUNISHER);

                        }
                        else

                            target = Netpool.Getinstance().Insgameobj(monsterunilt[13], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka12", MonsterType.PUNISHER);

                    }
                    else
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[7], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, "kaka12", MonsterType.PUNISHER);
                    break;


            }
            //  ifclone = false;

        }
        yield return null;
        ifclone = true;
        //spwanpoint[Random.Range(0, spwanpoint.Length)
    }

    #region top man
    public List<Transform> topMainList = new List<Transform>();
    public void initTopMain()
    {
        topMainList.Add(transform.Find("FWARD/AncientGodStatue_LOD0/point"));
        topMainList.Add(transform.Find("FWARD/Angel_LOD0/point"));
        topMainList.Add(transform.Find("FWARD/TuongAngel_LOD0/point"));
    }


    #endregion

    #region call wild

    public void CallWild(WildRandomData wildRandomData)
    {
        int type = 13;
        switch (wildRandomData.type)
        {
            case 1:
                type = 13;
                break;
            case 2:
                type = 14;
                break;
            case 3:
                type = 15;
                break;
            case 4:
                type = 16;
                break;
        }
        for (int i = 0; i < wildRandomData.count; i++)
        {
            StartCoroutine(IEins(type));
        }
    }
    int index = 12;
    public void CallTestWild()
    {
        index++;
        StartCoroutine(IEins(index));
        if (index == 16)
            index = 12;

    }
    #endregion


    #region call monsters

    public Dictionary<string, int> DeathGodList = new Dictionary<string, int>();

    public bool AddDeathGodList(string key)
    {
        if (DeathGodList.ContainsKey(key))
            DeathGodList[key] = DeathGodList[key] + 1;
        else
            DeathGodList.Add(key, 1);
        int count = DeathGodList[key];
        if (count >= 3)
            return true;
        else
            return false;


    }

    /// <summary>
    /// 召唤怪物
    /// </summary>
    /// <param name="team"></param>
    /// <param name="type"></param>
    /// <param name="uid"></param>
    public void CallMonsters(Team team, MonsterType type, bool isGroupFirst = false, string uid = "1")
    {
        StartCoroutine(IEins(team, type, isGroupFirst, uid));

        bool isDeathGod = false;
        if (MonsterType.PUNISHER == type)
            isDeathGod = AddDeathGodList(uid);
        GameManager.instance.addFightScorePool(type, uid, isDeathGod);

    }

    public void WaitCallMonsters(Team team, MonsterType type, int count, string uid = "1")
    {
        StartCoroutine(_WaitCallMonsters(team, type, count, uid));
    }
    public void initConfigValue()
    {
        string str = HttpRquest.instance.get_global_config_value(100028);
        string[] strs = str.Split(',');
        callCount = int.Parse(strs[0]);
        waitTime = float.Parse(strs[1]);
    }

    private float waitTime = 0.5f;
    private int callCount = 5;
    private IEnumerator _WaitCallMonsters(Team team, MonsterType type, int count, string uid = "1")
    {
        WaitForSeconds loop = new WaitForSeconds(0.5f * Time.deltaTime);
        int currentCount = 0;
        //按组刷的
        if (type == MonsterType.cavalry || type == MonsterType.infantry)
        {
            count = count / GameManager.instance.GetMonsterCount(type);
        }
        while (currentCount != count)
        {
            if (Rayboss.FIREbossHP > 0 && Rayboss.ICEbossHP > 0)
            {
                for (int j = 0; j < callCount; j++)
                {
                    if (currentCount >= count)
                        break;
                    CallMonsters(team, type, currentCount == 0, uid);
                    currentCount++;
                    yield return loop;
                    //Debug.Log("call troops in queue");
                }
            }
            else
            {
                Debug.Log("quit the game");
                ClearAll();
                break;

            }
        }
    }

    #endregion
    IEnumerator IEins(Team team, MonsterType type, bool isGroupFirst = false, string uid = "1")
    {
        GameObject target = null;
        ifclone = true;
        if (ifclone)
        {
            // Debug.Log("开始召唤");
            switch (type)
            {
                case MonsterType.infantry:
                    waitGenerateList.Add(new MonsterinsData()
                    {
                        team = team,
                        type = type,
                        uid = uid
                    });
                    //if (team == Team.BLUE)
                    //    target = Netpool.Getinstance().Insgameobj(monster1, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.infantry);
                    //else
                    //    target = Netpool.Getinstance().Insgameobj(monster2, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.infantry);
                    break;
                case MonsterType.cavalry:
                    waitGenerateList.Add(new MonsterinsData()
                    {
                        team = team,
                        type = type,
                        uid = uid
                    });
                    //if (team == Team.BLUE)
                    //    target = Netpool.Getinstance().Insgameobj(monster3, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.cavalry);
                    //else
                    //    target = Netpool.Getinstance().Insgameobj(monster4, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.cavalry);
                    break;
                case MonsterType.guard:
                    if (team == Team.BLUE)
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[0], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.guard);
                    else
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[1], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.guard);
                    break;
                case MonsterType.FemaleHunter:
                    if (team == Team.BLUE)
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[2], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.FemaleHunter);
                    else
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[3], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.FemaleHunter);
                    break;
                case MonsterType.Mecha:
                    if (team == Team.BLUE)
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[4], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.Mecha);
                    else
                        target = Netpool.Getinstance().Insgameobj(monsterunilt[5], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.Mecha);
                    break;
                case MonsterType.PUNISHER://give the thrid time to ins deathMan way
                    if (team == Team.BLUE)
                    {
                       // target = Netpool.Getinstance().Insgameobj(monsterunilt[12], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                        if (Netpool.Getinstance().monsterStruct.ContainsKey(uid))
                        {
                            if (Netpool.Getinstance().monsterStruct[uid].num < 2)
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                            else
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[12], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                        }
                        else
                            target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                    }
                    else
                    {
                        // target = Netpool.Getinstance().Insgameobj(monsterunilt[13], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                        if (Netpool.Getinstance().monsterStruct.ContainsKey(uid))
                        {
                            if (Netpool.Getinstance().monsterStruct[uid].num < 2)
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[7], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                            else
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[13], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                        }
                        else
                            target = Netpool.Getinstance().Insgameobj(monsterunilt[7], new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                    }
                    break;
                case MonsterType.WILD1:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[8], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_bear", MonsterType.WILD1);
                    break;
                case MonsterType.WILD2:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[9], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_sprider", MonsterType.WILD2);
                    break;
                case MonsterType.WILD3:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[10], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_turtle", MonsterType.WILD3);
                    break;
                case MonsterType.WILD4:
                    target = Netpool.Getinstance().Insgameobj(monsterunilt[11], new Vector3(17 + spawnerPosionRandom, 5, -75 + spawnerPosionRandom), Quaternion.identity, mother, "wild_hermit", MonsterType.WILD4);
                    break;
            }
            if (team != Team.WILD && target != null)
            {
                SetLabel(target.transform, team, uid, type, isGroupFirst);
            }

            //  ifclone = false;

        }
        yield return new WaitForSeconds(0);
        ifclone = true;

    }




    public void TestAnimation()
    {

        Debug.Log("its an animation test!!!!!!");


    }
    public void ClearUpAll()
    {
        if (!GameManager.instance.isGameFighting) return;
        //Debug.LogError("6666666666666666"+ Rayboss.ICEbossHP+""+ Rayboss.FIREbossHP+""+ ifbooslive);
        if ((Rayboss.ICEbossHP <= 0 || Rayboss.FIREbossHP <= 0) && ifbooslive)
        {
            Debug.LogError("游戏结束");
            UIManager.instance.ClearGame(Rayboss.ICEbossHP, Rayboss.FIREbossHP);

            ifbooslive = false;
            // transform.GetChild(0).transform.gameObject.SetActive(false);
            Netpool.Getinstance().Clearpoll();//--clear up pool;
            StopCoroutine("IESpawnerPosition");//close the coroutine of obj spawn
            for (int i = 0; i < transform.GetChild(0).childCount; i++)//set active fasle of all gameobject
                                                                      //{ if (mother.transform.GetChild(i).gameObject.activeSelf==true)
                                                                      //    {
                                                                      //        mother.transform.GetChild(i).gameObject.SetActive(false);
                                                                      //    }
                Destroy(transform.GetChild(0).GetChild(i).gameObject);//destory all gameobjects    
        }
    }
    public void ClearAll()
    {
        Netpool.Getinstance().Clearpoll();//--clear up pool;
        StopCoroutine("IESpawnerPosition");//close the coroutine of obj spawn
        StopAllCoroutines();
        allMonsterList.Clear();
        allMonsters.Clear();
        if (transform.childCount > 1)
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)//set active fasle of all gameobject
            {
                Destroy(transform.GetChild(0).GetChild(i).gameObject);//destory all gameobjects 
            }
            for (int j = 0; j < transform.GetChild(2).childCount; j++)
            {
                Destroy(transform.GetChild(2).GetChild(j).gameObject);//destory all gameobjects which is vfx


            }
        }

    }

    /// <summary>
    /// wjy code
    /// </summary>
    /// <param name="target"></param>
    /// <param name="team"></param>
    /// <param name="uid"></param>
    /// <param name="type"></param>
    public void SetLabel(Transform target, Team team, string uid, MonsterType type, bool isGroupFirst = false)
    {
        Monstermove _Monstermove = target.GetComponent<Monstermove>();
        _Monstermove.initMonsterData(team, uid, type);
        if (isGroupFirst)
            UIManager.instance.showNamePanel(team, uid, type, _Monstermove);

        AddEliteMonster(_Monstermove);
    }



    public void SkillParIns()//the skill o camp use .and initialization,do it

    {
        bossDamageReduce = 1;
        ifuseskill = false;
        iffirecamp = false;
        ificecamp = false;
        IceObjWorld = true;
        FireObjWorld = true;
        protectBossICE = false;
        protectBossICE_assist = 1;
        protectBossFIRE = false;
        protectBossFIRE_assist = 1;
        ifCheackCollison = -1;
        Netpool.Getinstance().Clearpoll();
        Debug.Log("clear up all mache of static+........................................................");
        StopCoroutine("IEICEBossReduceDamage");
        StopCoroutine("IEFIREBossReduceDamage");
        StopCoroutine("IECounterattackTimeReduce");
        Publisher.Getinstance().DisposeEvent();
        iceHPcache = 0;
        fireHPcache = 0;



    }
    IEnumerator IEICEBossReduceDamage()//if the skill has trrigered,then boss would emit golden light,and it recover after 20 seconds;
    {
        WaitForSeconds LOOP = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(5);//the boss start the recycle in 5 miniutes later
        for (; ; )
        {

            if (Rayboss.ICEbossHP < 0.05f * iceHPcache && Rayboss.ICEbossHP > 0)
            {
                protectBossICE = true;
                protectBossICE_assist = 0.1f;
                Debug.Log("start protecting!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                break;
            }

            yield return LOOP;
        }
        yield return new WaitForSeconds(30);//the time of finally skill continus 30 secends;
        protectBossICE = false;
        protectBossICE_assist = 1f;

    }

    IEnumerator IEFIREBossReduceDamage()//if the skill has trrigered,then boss would emit golden light,and it recover after 20 seconds;
    {
        WaitForSeconds LOOP = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(5);//the boss start the recycle in 5 miniutes later
        for (; ; )
        {
            if (Rayboss.FIREbossHP < 0.05f * fireHPcache && Rayboss.FIREbossHP > 0)
            {
                protectBossFIRE = true;
                protectBossFIRE_assist = 0.1f;
                Debug.Log("start protecting!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                break;
            }

            yield return LOOP;
        }
        yield return new WaitForSeconds(30);//the time of finally skill continus 30 secends;
        protectBossFIRE = false;
        protectBossFIRE_assist = 1f;

    }
    /// <summary>
    /// module of Counterattack moment
    /// </summary>
    /// <returns></returns>
    IEnumerator IEBossMonitor()
    {
        WaitForSeconds LOOP = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(5);//the boss start the recycle in 5 miniutes later
        for (; ; )
        {

            if (Rayboss.ICEbossHP < 7000)
            {
                ifuseskill = true;
                ificecamp = true;
                bossDamageReduce = 1.5f;//the damage has reduced
                Debug.Log("start killing!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                break;
            }
            else if (Rayboss.FIREbossHP < 7000)
            {
                ifuseskill = true;
                iffirecamp = true;
                bossDamageReduce = 1.5f;//the damage has reduced
                Debug.Log("start killing!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                break;
            }

            yield return LOOP;
        }
        yield return new WaitForSeconds(30);//the time of finally skill continus 30 secends;
        ifuseskill = false;
        ificecamp = false;
        iffirecamp = false;
        bossDamageReduce = 1;
    }


    IEnumerator IEBossMonitorForUI()
    {
        WaitForSeconds LOOP = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(5);//the boss start the recycle in 5 miniutes later
        for (; ; )
        {
            if (ifuseskill)
                break;
            yield return LOOP;


        }
        yield return new WaitForSeconds(30);//the time of finally skill continus 30 secends;
        ifuseskill = false;
        ificecamp = false;
        iffirecamp = false;
        bossDamageReduce = 1;
    }
    /// <summary>
    /// kill code for wild
    /// </summary>
    /// <param name="target"></param>
    /// <param name="team"></param>
    /// <param name="uid"></param>
    /// <param name="type"></param>
    /// 
    public void KillTheWild(string playerID)//the method use to command the monster who calling up that go to lock the target wild,and the playerID is the ID of player who join the game
    {

        if (Input.GetKeyDown(KeyCode.K))
        {

            if (ifclone)
            {
                for (int i = 0; i < mother.childCount; i++)
                {
                    mother.transform.GetChild(i).TryGetComponent<Monstermove>(out Monstermove component);
                    if (component.playerID.Equals(playerID))
                        component.controlToKillWild = true;
                }

                ifclone = false;
                Debug.Log("Start kill WILD");

            }
        }
    }

    public void KillTheWild(string playerID, bool net)//the method use to command the monster who calling up that go to lock the target wild,and the playerID is the ID of player who join the game
    {
        if (net)
        {
            for (int i = 0; i < mother.childCount; i++)
            {
                mother.transform.GetChild(i).TryGetComponent<Monstermove>(out Monstermove component);
                if (component.playerID.Equals(playerID))
                    component.controlToKillWild = true;
            }

            Debug.Log("Start kill WILD");
        }
    }
    public void OnApplicationQuit()
    {
        SkillParIns();
    }

    public void CaculateCount()//caculate for wangbiao 
    {

        objnum = 0;
        vfxnum = 0;
        for (int i = 0; i < transform.GetChild(0).childCount; i++)

        {
            if (transform.GetChild(0).GetChild(i).gameObject.activeSelf == true)
            {
                objnum++;

            }

        }
        for (int j = 0; j < transform.GetChild(2).childCount; j++)
        {
            if (transform.GetChild(2).GetChild(j).gameObject.activeSelf == true)
            {


                vfxnum++;

            }


        }
        Debug.Log("objnum= " + objnum + "           vfxnum= " + vfxnum);

    }
    /// <summary>
    /// module of change the vfx of obj
    /// </summary>
    IEnumerator IECaculateObjCount()
    {
        WaitForSeconds loop = new WaitForSeconds(5);
        for (; ; )
        {
            objnum_assist = 0;
            yield return loop;

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                if (transform.GetChild(0).GetChild(i).gameObject.activeSelf == true)
                {
                    objnum_assist++;

                }
            }
            objnumcache = objnum_assist;

        }
    }

    void PublisherTest()
    {
        Publisher.Getinstance().TriggerEvent(EventArgs.Empty);
        Debug.Log("the message has been published");
    }
    IEnumerator IEIfcheck()
    {
        WaitForSeconds loop = new WaitForSeconds(0.5f);
        yield return loop;
        ifcheck = true;

    }

    private void Subscribe(GameManager gameManager)
    {
        //var handel = Publisher.Getinstance().GetPublisherEvent();
        //handel += OnMyEventHappen;
        gameManager.EMyEvent += OnMyEventHappen;
        Debug.Log("has subscribe");
    }

    private void OnMyEventHappen(object sender, EventArgs e)
    {
        Debug.Log("counterattack has happend,and the time continus 30 seconds");
        StartCoroutine("IECounterattackTimeReduce");
    }
    IEnumerator IECounterattackTimeReduce()
    {
        yield return new WaitForSeconds(30);
        bossDamageReduce = 1;
        ifuseskill = false;
        iffirecamp = false;
        ificecamp = false;
    }
    IEnumerator IETestCoroutine()
    {

        Debug.Log("it is a test for CoroutineManager");
        yield return null;


    }
    #region 精英怪管理器
    public Dictionary<MonsterType, List<Monstermove>> allMonsterList = new Dictionary<MonsterType, List<Monstermove>>();
    private List<Monstermove> allMonsters = new List<Monstermove>();
    public void AddEliteMonster(Monstermove monstermove)
    {
        if (!allMonsterList.ContainsKey(monstermove.monsterType))
        {
            allMonsterList[monstermove.monsterType] = new List<Monstermove>();
        }
        allMonsterList[monstermove.monsterType].Add(monstermove);
        allMonsters.Add(monstermove);
    }
    public void RemoveEliteMonster(Monstermove monstermove)
    {
        allMonsters.Remove(monstermove);
        if (!allMonsterList.ContainsKey(monstermove.monsterType)) return;
        allMonsterList[monstermove.monsterType].Remove(monstermove);
    }
    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject GetLastTarget(Vector3 pos, EliteUnitPortalMan.Team team) 
    {
        GameObject Target =null;
        float dis = int.MaxValue;
        float temp;
        foreach (var item in allMonsters)
        {
            if (item.monsterData.team != team&& !item.isDie)
            {
                temp = Vector3.Distance(item.transform.position, pos);
                if (temp < dis)
                {
                    Target = item.gameObject;
                    dis = temp;
                }
            }
        }
        return Target;
    }
    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Monstermove GetLastTargetMove(Vector3 pos, EliteUnitPortalMan.Team team)
    {
        Monstermove Target = null;
        float dis = int.MaxValue;
        float temp;
        foreach (var item in allMonsters)
        {
            if (item.monsterData.team != team && !item.isDie)
            {
                temp = Vector3.Distance(item.transform.position, pos);
                if (temp < dis)
                {
                    Target = item;
                    dis = temp;
                }
            }
        }
        return Target;
    }
    /// <summary>
    /// 获取某个类型怪物数量
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns></returns>
    public int GetEliteMonsterNumner(MonsterType monsterType)
    {
        if (!allMonsterList.ContainsKey(monsterType))
            return 0;
        return allMonsterList[monsterType].Count;
    }
    /// <summary>
    /// 获取某个类型所有怪物
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns></returns>
    public List<Monstermove> GetEliteMonsterList(MonsterType monsterType)
    {
        if (!allMonsterList.ContainsKey(monsterType))
            return null;
        return allMonsterList[monsterType];
    }
    /// <summary>
    /// 获取所有野怪
    /// </summary>
    /// <returns></returns>
    public List<Monstermove> GetEliteMonsterList_WILD()
    {
        List<Monstermove> list = new List<Monstermove>();
        if (allMonsterList.ContainsKey(MonsterType.WILD1)) 
        {
            list.AddRange(allMonsterList[MonsterType.WILD1]);
        }
        if (allMonsterList.ContainsKey(MonsterType.WILD2))
        {
            list.AddRange(allMonsterList[MonsterType.WILD2]);
        }
        if (allMonsterList.ContainsKey(MonsterType.WILD3))
        {
            list.AddRange(allMonsterList[MonsterType.WILD3]);
        }
        if (allMonsterList.ContainsKey(MonsterType.WILD4))
        {
            list.AddRange(allMonsterList[MonsterType.WILD4]);
        }
        return list;
    }
    /// <summary>
    /// 枚举转换
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static EliteUnitPortalMan.Team TurnTeam(Team team) 
    {
        switch (team)
        {
            case Team.BLUE:
                return EliteUnitPortalMan.Team.Human;
            case Team.RED:
                return EliteUnitPortalMan.Team.Org;
            case Team.WILD:
                return EliteUnitPortalMan.Team.Monster;
        }
        return EliteUnitPortalMan.Team.Monster;
    } 
    #endregion
}
