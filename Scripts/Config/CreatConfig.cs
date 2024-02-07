using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 创建配置文件
/// </summary>
public class CreatConfig : MonoBehaviour
{
    // Start is called before the first frame update
    public MonsterConfig monsterConfig;
    public string[] configname;
    public bool ifgenerate;

    void Start()
    {
        // 创建一个配置类实例
        //configTest= new ConfigTest
        //{
        //    playerName = "Player1",
        //    playerScore = 100,
        //    isSoundEnabled = true,
        //    musicVolume = 0.8f,
        //};
       // Creat();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F6))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {

                Creat();

            }
        
        
        
        }
    }
    void Creat()
    {
        if (ifgenerate)
        {
            monsterConfig = new MonsterConfig();
            //Debug.Log(monsterConfig.monsterConfig1.damage);
           
           
           
            //创建一个需要配置的文件对象
            string jsonText = JsonUtility.ToJson(monsterConfig);

            // 将JSON字符串保存到文件
            System.IO.File.WriteAllText("Assets/Other/Scripits/Config/MonsterConfigs.json", jsonText);

            Debug.Log("Config file created and saved.");
            ifgenerate = false;
        }
        
    }
    void CreatTest()
    {
        //if (ifgenerate)
        //{

            
        //   configTest = new ConfigTest
        //   {
        //       playerName = "Player1",
        //       playerScore = 100,
        //       isSoundEnabled = true,
        //       musicVolume = 0.8f,
        //   };
          
        //    string jsonText = JsonUtility.ToJson(monsterConfig);

        //    // 将JSON字符串保存到文件
        //    System.IO.File.WriteAllText("Assets/Other/Scripits/Config/configTest.json", jsonText);

        //    Debug.Log("Config file created and saved.");
        //    ifgenerate = false;
        //}




    }
}
