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

    private void Update()
    {
        if (Input.GetKey(KeyCode.F6))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (ifread)

                {
                    LoadConfig();
                    ApplyConfig();

                }
                ifread= false;
            }



        }
    }
    void ApplyConfig()
    {
        // 应用配置到游戏中
        Debug.Log("Player Name: " + gameConfig.monsterConfig1.damage);
        Debug.Log("Player Score: " + gameConfig.monsterConfig2.damage);
        Debug.Log("Sound Enabled: " + gameConfig.monsterConfig3.damage);
        Debug.Log("Music Volume: " + gameConfig.monsterConfig4.damage);
        //-------
        Debug.Log("start tead configADD----------------------");
        //Debug.Log("gamesetting: " +)

        //Debug.Log("kaka: " + gameconfigAdd.kaka);
        //Debug.Log("Game Difficulty: " + gameconfigAdd.configData.gameSettings.difficulty);
        //Debug.Log("Graphics Resolution: " + gameconfigAdd.configData.gameSettings.graphics.resolution);
        //Debug.Log("Player Name: " + gameconfigAdd.configData.playerSettings.playerName);
        //Debug.Log("Mouse Sensitivity: " + gameconfigAdd.configData.soundSettings.effectsVolume);
        //Debug.Log("Gamestting:gra=" + gameconfigAdd.gameSetting.graphics);
        //Debug.Log("Control:con=" + gameconfigAdd.playerSettings.controls.keyboard[0] )

       

    }
}

