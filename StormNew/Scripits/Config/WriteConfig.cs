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
        // 读取JSON文件
        string jsonText = System.IO.File.ReadAllText("Assets\\Other\\Scripits\\Config\\MonsterConfigs.json");

        // 反序列化JSON数据为配置类对象
        gameConfig = JsonUtility.FromJson<MonsterConfig>(jsonText);


        ///---读取使用unity生成的格式语言

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

