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
    private float spawnerPosionRandom;
    public static Monsterins instance;
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
    void Start()
    {


        //-----
        instance = this;
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
    }

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
        //if (Input.GetKey(KeyCode.F3))//蓝
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        StartCoroutine(IEins(1));
        //        //    StartCoroutine(IEins(Team.BLUE,MonsterType.infantry));
        //        GameManager.instance.addFightScorePool(MonsterType.infantry);
        //    }

        //}
        //if (Input.GetKey(KeyCode.F4))//红
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        StartCoroutine(IEins(2));
        //        GameManager.instance.addFightScorePool(MonsterType.infantry);
        //    }

        //}
        //if (Input.GetKey(KeyCode.F3))
        //{
        //    if (Input.GetKeyDown(KeyCode.O))
        //    {
        //        StartCoroutine(IEins(3));
        //        GameManager.instance.addFightScorePool(MonsterType.cavalry);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))
        //{
        //    if (Input.GetKeyDown(KeyCode.O))
        //    {
        //        StartCoroutine(IEins(4));
        //        GameManager.instance.addFightScorePool(MonsterType.cavalry);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha7))
        //    {
        //        StartCoroutine(IEins(5));
        //        GameManager.instance.addFightScorePool(MonsterType.guard);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha7))
        //    {
        //        StartCoroutine(IEins(6));
        //        GameManager.instance.addFightScorePool(MonsterType.guard);

        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha8))
        //    {
        //        StartCoroutine(IEins(7));
        //        GameManager.instance.addFightScorePool(MonsterType.FemaleHunter);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha8))
        //    {
        //        StartCoroutine(IEins(8));
        //        GameManager.instance.addFightScorePool(MonsterType.FemaleHunter);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha9))
        //    {
        //        StartCoroutine(IEins(9));
        //        GameManager.instance.addFightScorePool(MonsterType.Mecha);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha9))
        //    {
        //        StartCoroutine(IEins(10));
        //        GameManager.instance.addFightScorePool(MonsterType.Mecha);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha0))
        //    {
        //        StartCoroutine(IEins(11));
        //        GameManager.instance.addFightScorePool(MonsterType.PUNISHER);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha0))
        //    {
        //        StartCoroutine(IEins(12));
        //        GameManager.instance.addFightScorePool(MonsterType.PUNISHER);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        StartCoroutine(IEins(13));
        //        GameManager.instance.addFightScorePool(MonsterType.WILD1);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        StartCoroutine(IEins(14));
        //        GameManager.instance.addFightScorePool(MonsterType.WILD2);//*****wjy code******
        //    }
        //}
        //if (Input.GetKey(KeyCode.F3))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.K))
        //    {
        //        StartCoroutine(IEins(15));
        //        GameManager.instance.addFightScorePool(MonsterType.WILD3);
        //    }
        //}
        //if (Input.GetKey(KeyCode.F4))//&&Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKeyDown(KeyCode.K))
        //    {
        //        StartCoroutine(IEins(16));
        //        GameManager.instance.addFightScorePool(MonsterType.WILD4);//*****wjy code******
        //    }
        //}
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
    int index = 13;
    public void CallTestWild()
    {
        index++;
        StartCoroutine(IEins(index));
        if (index == 16)
            index = 12;

    }
    #endregion


    #region call monsters

    public Dictionary<string,int> DeathGodList=new Dictionary<string, int>();

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
    public void CallMonsters(Team team, MonsterType type, string uid = "1")
    {
        StartCoroutine(IEins(team, type, uid));
       
            bool isDeathGod = false;
        if (MonsterType.PUNISHER == type)
            isDeathGod= AddDeathGodList(uid);
        GameManager.instance.addFightScorePool(type, uid,isDeathGod);

    }

    public void WaitCallMonsters(Team team, MonsterType type, int count, string uid = "1")
    {
        StartCoroutine(_WaitCallMonsters(team, type, count, uid));
    }
    public void StopWaitCallMonster(Team team, MonsterType type, int count, string uid = "1")
    {

        StopCoroutine(_WaitCallMonsters(team, type, count, uid));


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
        WaitForSeconds loop = new WaitForSeconds(0.5f*Time.deltaTime);
        int currentCount = 0;
        while (currentCount != count)
        {
            if (Rayboss.FIREbossHP > 0 && Rayboss.ICEbossHP > 0)
                for (int j = 0; j < callCount; j++)
                {
                    if (currentCount >= count)
                        break;
                    currentCount++;
                    CallMonsters(team, type, uid);
                    yield return loop;
                    //Debug.Log("call troops in queue");
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

    IEnumerator IEins(Team team, MonsterType type, string uid = "1")
    {
        GameObject target = null;
        ifclone = true;
        if (ifclone)
        {
            // Debug.Log("开始召唤");
            switch (type)
            {
                case MonsterType.infantry:
                    if (team == Team.BLUE)
                        target = Netpool.Getinstance().Insgameobj(monster1, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.infantry);
                    else
                        target = Netpool.Getinstance().Insgameobj(monster2, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.infantry);
                    break;
                case MonsterType.cavalry:
                    if (team == Team.BLUE)
                        target = Netpool.Getinstance().Insgameobj(monster3, new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.cavalry);
                    else
                        target = Netpool.Getinstance().Insgameobj(monster4, new Vector3(226 + spawnerPosionRandom, 5, -315 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.cavalry);
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

                        if (Netpool.Getinstance().monsterStruct.ContainsKey(uid))
                        {
                            if (Netpool.Getinstance().monsterStruct[uid].num <2)
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                            else
                                target = Netpool.Getinstance().Insgameobj(monsterunilt[12], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                        }
                        else
                            target = Netpool.Getinstance().Insgameobj(monsterunilt[6], new Vector3(-226 + spawnerPosionRandom, 5, 225 + spawnerPosionRandom), Quaternion.identity, mother, uid, MonsterType.PUNISHER);
                    }
                    else
                    {
                        if (Netpool.Getinstance().monsterStruct.ContainsKey(uid))
                        {
                            if (Netpool.Getinstance().monsterStruct[uid].num <2)
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
            if (team != Team.WILD)
            {
                SetLabel(target.transform, team, uid, type);
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
    public void SetLabel(Transform target, Team team, string uid, MonsterType type)
    {
        Monstermove _Monstermove = target.GetComponent<Monstermove>();
        _Monstermove.initMonsterData(team, uid, type);
        UIManager.instance.showNamePanel(team, uid, type, _Monstermove);
    }



    public void SkillParIns()//the skill o camp use .and initialization,do it

    {
        bossDamageReduce = 1;
        ifuseskill = false;
        iffirecamp = false;
        ificecamp = false;
        protectBossICE = false;
        protectBossICE_assist = 1;
        protectBossFIRE = false;
        protectBossFIRE_assist = 1;
        ifCheackCollison = -1;
        Netpool.Getinstance().Clearpoll();
        Debug.Log("clear up all mache of static+........................................................");
        StopCoroutine("IEICEBossReduceDamage");
        StopCoroutine("IEFIREBossReduceDamage");
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

            if (Rayboss.ICEbossHP < 0.05f*iceHPcache&&Rayboss.ICEbossHP>0)
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
            if (Rayboss.FIREbossHP < 0.05f*fireHPcache&&Rayboss.FIREbossHP>0)
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

    public void KillTheWild(string playerID,bool net)//the method use to command the monster who calling up that go to lock the target wild,and the playerID is the ID of player who join the game
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

        { if (transform.GetChild(0).GetChild(i).gameObject.activeSelf == true)
            {
                objnum++;
            
            }
        
        }
        for (int j = 0; j < transform.GetChild(2).childCount; j++)
        { if (transform.GetChild(2).GetChild(j).gameObject.activeSelf == true)
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
    { WaitForSeconds loop = new WaitForSeconds(5);
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
            objnumcache= objnum_assist;
          
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
}
