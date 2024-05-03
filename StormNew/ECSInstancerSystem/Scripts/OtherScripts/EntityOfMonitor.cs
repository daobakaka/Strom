using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
public class EntityOfMonitor : MonoBehaviour
{
    public static EntityOfMonitor Instance;
    //怪物初始生成距离
    public Vector2 fireInitVector2 = new Vector2(0, -315);
    public Vector2 iceInitVector2=new Vector2(-1000,500);
    public List<Transform> fireInitList = new List<Transform>();
    public List<Transform> iceInitList = new List<Transform>();
    //初始旋转
    public Vector3 fireInitRotate;
    public Vector3 iceInitRotate;
    public bool ifclone;
    public int entitiesNum;
    public int entitiesIceNum;
    public int entitiesFireNum;
    public float IceTimes;
    public float FireTimes;
    public int floor;
    //怪物离Boss多近的时候 开始朝着Boss走
    public float findingDistanceBoss= 150000;
    //怪物检测小怪的范围
    public float findingDistanceLittleMonster= 25000;
    //怪物攻击距离
    public float entityDamageDis;
    public float entityEliteDamageDis;
    public float entityBossDamageDis;
    public int count;
    public int row;
    public int countlevelTwo;
    public int rowLevelTwo;
    public Dictionary<string, BlobAssetReference<PlayerID>> entityIntegral = new Dictionary<string, BlobAssetReference<PlayerID>>();//the intergral of only string

    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI numberText;
    public Transform textBox;

    private float deltaTime = 0.0f;
    private void Awake()
    {
        Instance = this;
      
    }
    void Start()
    {
        StartCoroutine("IEifclone");
        //GameObject gameObject = GameObject.FindWithTag("TEST");



    }

    // Update is called once per frame
    void Update()
    {
        numberText.text =$"数量={entitiesNum}";


        // 计算时间间隔
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // 计算FPS
        float fps = 1.0f / deltaTime;
        // 更新UI Text元素
        fpsText.text = $"帧数{fps}";

        GetTheInstanceOfSystemByFight();
    }
    //同步步兵 骑兵 战斗 产生的积分
    float tempUpTimer;
    void GetTheInstanceOfSystemByFight()//the the instance of 
    {
        tempUpTimer -= Time.deltaTime;
        if (tempUpTimer < 0) 
        {
            tempUpTimer = 2.0f;
            var mySystemHandle = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GpuMonsterSystem>();
            var PlayerInergral = World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<GpuMonsterSystem>(mySystemHandle);
            foreach (var item in PlayerInergral.entityIntegral)
            {
                //Debug.LogError($"name is :{ PlayerInergral.TransferBlobAsset(item.Key)},and the intergral is{ PlayerInergral.entityIntegral[item.Key]}");
                GameManager.instance.SetPlayerData(PlayerInergral.TransferBlobAsset(item.Key), PlayerInergral.entityIntegral[item.Key]);
                item.Value = 0;
                //GameManager.instance.KillMonsterAddScore();
            }
        }
    }
    //同步步兵 骑兵 存活 产生的积分
    public void GetTheInstanceOfSystemBySurvival()//the the instance of 
    {
        //获取存活兵的积分
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // 创建一个实体查询描述
        var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Unit1Component>());
        // 使用实体查询来获取实体数组
        var entities = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

        float addScore = 0;
       var CallMonsterDataList = HttpRquest.instance.CallMonsterDataList;
        // 遍历所有实体
        foreach (var entity in entities)
        {
            // 这里可以对每一个实体进行操作
            // 比如获取组件数据
            var someComponentData = entityManager.GetComponentData<Unit1Component>(entity);

            // 然后对组件数据进行一些操作
            if (someComponentData.order == 61 || someComponentData.order == 62)
            {
                var data = CallMonsterDataList[100018];
                addScore += data.nextTurnAddPool;
            }
            else if (someComponentData.order == 63 || someComponentData.order == 64)
            {
                var data = CallMonsterDataList[100019];
                addScore += data.nextTurnAddPool;
            }
        }
        GameManager.instance.AddNextScorePool(addScore);
        // 释放实体数组占用的资源
        entities.Dispose();
    }
    /// <summary>
    /// module of get instance;
    /// </summary>
    /// <returns></returns>
    private EntityOfMonitor() { }
   private static EntityOfMonitor GetInstance()
    {
        if (Instance == null)
            Instance = new();
        return Instance;
    
    
    }
    IEnumerator IEifclone()
    {
        WaitForSeconds loop = new WaitForSeconds(1f);
        for (; ; )
        {
            ifclone = true;
            yield return loop;
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
