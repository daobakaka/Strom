using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameDifficulty
{
    NUll,
    Easy,//��
    Normal,//��ͨ
    Hard,//����
}
public enum GameTime
{
    Null,
    ShortTerm,//��ʱ��
    MediumTerm,//��ʱ��
    LongTerm,//��ʱ��
}

//����ȫ��
public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }
    //����������
    public SceneSystem _SceneSystem { get; private set; }
    //��ʾһ�����
    public UnityAction<BasePanel> Push { get; private set; }


    //��Ϸ���׶�
    [HideInInspector] public GameDifficulty gameDifficulty;
    //��Ϸʱ��
    [HideInInspector] public GameTime gameTime;
    //��ͬ�Ѷȵȼ���
    [HideInInspector] public float JiDiHP;//����Ѫ��
    [HideInInspector] public int LikeNum;//���������ﵽ�����Ұ��
    [HideInInspector] public int Cur_LikeNum;
    [HideInInspector] public int MonsterNum;//��������
    [HideInInspector] public Dictionary<ShiBingName, int> AwardShiBingNumDic;//����ʿ������


    //������Ч�Ƿ���Բ���
    [HideInInspector] public bool Is_Gift1EffectsCanPlayed;//�Ƿ��ܲ�����Ů����Ч
    [HideInInspector] public bool Is_Gift2EffectsCanPlayed;//�Ƿ��ܲ�������Ȧ��Ч
    [HideInInspector] public bool Is_Gift3EffectsCanPlayed;//�Ƿ��ܲ������������Ч
    [HideInInspector] public bool Is_Gift4EffectsCanPlayed;//�Ƿ��ܲ��Ŷ�ħը����Ч
    [HideInInspector] public bool Is_Gift5EffectsCanPlayed;//�Ƿ��ܲ������ؿ�Ͷ��Ч
    [HideInInspector]  public bool Is_Gift6EffectsCanPlayed;//�Ƿ��ܲ��ų���������Ч


    //����ͼ״̬
    [HideInInspector] public GiftImage giftImage;//����ͼ���Ų�


    //��F12��ʵ���������ʵ����ʿ��֮���л�
    [HideInInspector] public bool GiftOrShiBing;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        _SceneSystem = new SceneSystem();

        DontDestroyOnLoad(gameObject);
        Debug.Log("GameRootAwake");

        AwardShiBingNumDic = new Dictionary<ShiBingName, int>();
        gameDifficulty = GameDifficulty.Normal;
        gameTime = GameTime.MediumTerm;
        Cur_LikeNum = 0;


        Is_Gift1EffectsCanPlayed = true;
        Is_Gift2EffectsCanPlayed = true;
        Is_Gift3EffectsCanPlayed = true;
        Is_Gift4EffectsCanPlayed = true;
        Is_Gift5EffectsCanPlayed = true;
        Is_Gift6EffectsCanPlayed = true;


        giftImage = GiftImage.VErtical;


        GiftOrShiBing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _SceneSystem.SetScene(new StartScenes());
    }

    // Update is called once per frame
    void Update()
    {
        //�Ƿ��Ѿ�����������
        LoadMainScene();
        //��F12��ʵ���������ʵ����ʿ��֮������л�
        HoldF12();
    }

    //���֮������Push
    public void SetAction(UnityAction<BasePanel> push)
    {
        Push = push;
    }

    //�Ƿ��Ѿ�����������
    void LoadMainScene()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
            return;

        GameObject parent = GameObject.Find("Canvas");
        Transform[] trans = parent.GetComponentsInChildren<Transform>();
        foreach (Transform item in trans)
        {
            if (item.name == "MainPanel")
                return;
        }

        Instance._SceneSystem.SetScene(new MainScene());
    }

    //��ͣ��Ϸ
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    //��ʼ��Ϸ
    public void PlayGame()
    {
        Time.timeScale = 1;
    }

    //��F12��ʵ���������ʵ����ʿ��֮������л�
    void HoldF12()
    {
        if(Input.GetKeyDown(KeyCode.F12))
            GiftOrShiBing = GiftOrShiBing == true ? false : true;
    }

}
