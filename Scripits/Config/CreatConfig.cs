using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���������ļ�
/// </summary>
public class CreatConfig : MonoBehaviour
{
    // Start is called before the first frame update
    public MonsterConfig monsterConfig;
    public string[] configname;
    public bool ifgenerate;

    void Start()
    {
        // ����һ��������ʵ��
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
           
           
           
            //����һ����Ҫ���õ��ļ�����
            string jsonText = JsonUtility.ToJson(monsterConfig);

            // ��JSON�ַ������浽�ļ�
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

        //    // ��JSON�ַ������浽�ļ�
        //    System.IO.File.WriteAllText("Assets/Other/Scripits/Config/configTest.json", jsonText);

        //    Debug.Log("Config file created and saved.");
        //    ifgenerate = false;
        //}




    }
}
