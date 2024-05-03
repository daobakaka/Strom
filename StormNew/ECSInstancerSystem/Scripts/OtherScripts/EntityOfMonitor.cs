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
    //�����ʼ���ɾ���
    public Vector2 fireInitVector2 = new Vector2(0, -315);
    public Vector2 iceInitVector2=new Vector2(-1000,500);
    public List<Transform> fireInitList = new List<Transform>();
    public List<Transform> iceInitList = new List<Transform>();
    //��ʼ��ת
    public Vector3 fireInitRotate;
    public Vector3 iceInitRotate;
    public bool ifclone;
    public int entitiesNum;
    public int entitiesIceNum;
    public int entitiesFireNum;
    public float IceTimes;
    public float FireTimes;
    public int floor;
    //������Boss�����ʱ�� ��ʼ����Boss��
    public float findingDistanceBoss= 150000;
    //������С�ֵķ�Χ
    public float findingDistanceLittleMonster= 25000;
    //���﹥������
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
        numberText.text =$"����={entitiesNum}";


        // ����ʱ����
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // ����FPS
        float fps = 1.0f / deltaTime;
        // ����UI TextԪ��
        fpsText.text = $"֡��{fps}";

        GetTheInstanceOfSystemByFight();
    }
    //ͬ������ ��� ս�� �����Ļ���
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
    //ͬ������ ��� ��� �����Ļ���
    public void GetTheInstanceOfSystemBySurvival()//the the instance of 
    {
        //��ȡ�����Ļ���
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // ����һ��ʵ���ѯ����
        var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Unit1Component>());
        // ʹ��ʵ���ѯ����ȡʵ������
        var entities = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

        float addScore = 0;
       var CallMonsterDataList = HttpRquest.instance.CallMonsterDataList;
        // ��������ʵ��
        foreach (var entity in entities)
        {
            // ������Զ�ÿһ��ʵ����в���
            // �����ȡ�������
            var someComponentData = entityManager.GetComponentData<Unit1Component>(entity);

            // Ȼ���������ݽ���һЩ����
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
        // �ͷ�ʵ������ռ�õ���Դ
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
