using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static MonsterConfig;

public class WriteCongif : MonoBehaviour
{
    public MonsterConfig gameConfig;
    public bool ifread;

    void Start()
    {
        //LoadConfig();
        //ApplyConfig();
    }

    void LoadConfig()
    {
        // ��ȡJSON�ļ�
        string jsonText = System.IO.File.ReadAllText("Assets\\Other\\Scripits\\Config\\MonsterConfigs.json");

        // �����л�JSON����Ϊ���������
        gameConfig = JsonUtility.FromJson<MonsterConfig>(jsonText);


        ///---��ȡʹ��unity���ɵĸ�ʽ����

    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.F6))
    //    {
    //        if (Input.GetKeyDown(KeyCode.I))
    //        {
    //            if (ifread)

    //            {
    //                LoadConfig();
    //                ApplyConfig();

    //            }
    //            ifread= false;
    //        }



    //    }
    //}
    void ApplyConfig()
    {

    }
}

