using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class RemoteBulid : MonoBehaviour
{
    // Start is called before the first frame update
    public string address;
    public TextAsset jsonFile;
    public static RemoteBulid instance;
    public string jsonUrl;
    public Text configFile;
    public MonsterConfig monsterConfig;
    public MonsterConfig testConfig;
    public int xx = 1;

    void Start()
    {
    }
    private void Awake()
    {
       // LoadConfig(address);
        StartCoroutine(GetJsonData());
        LoadConfig();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LoadConfig(string address)
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(address);

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                jsonFile = op.Result;
                MonsterConfig config = JsonUtility.FromJson<MonsterConfig>(configFile.text);
                Debug.Log("Player Speed: " + config.monsterConfig1.damage);
                Debug.Log("Enemy Speed: " + config.monsterConfig2.damage);
                Debug.Log("Player Lives: " + config.wildConfig1.monsterSpeed);
            }
            else
            {
                Debug.LogError("1 Failed to load game config.------------------------------------------------------------------------------------------");
            }
        };
    }
    void LoadConfig()
    { 
      
        Addressables.LoadAssetsAsync<TextAsset>("config", OnAssetLoaded).Completed += OnAllAssetsLoaded;


    }

    void OnAssetLoaded(TextAsset aa)
    {

        jsonFile = aa;
        Debug.Log("it has cast the aa");
    
    
    }
    void OnAllAssetsLoaded(AsyncOperationHandle<IList<TextAsset>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            testConfig= JsonUtility.FromJson<MonsterConfig>(jsonFile.text);
            Debug.Log("Player Speed: " + testConfig.monsterConfig1.damage);
            Debug.Log("Enemy Speed: " + testConfig.monsterConfig2.damage);
            Debug.Log("Player Lives: " + testConfig.wildConfig1.monsterSpeed);
        }
        else
        {
            Debug.LogError("Failed to load assets with label '");




        }
    }
    IEnumerator GetJsonData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(jsonUrl))
        {
            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("2 Error:-------------------------------------------------------- " + webRequest.error+"------------------------------------------------------------------------------");
            }
            else
            {
                // 使用下载的数据（webRequest.downloadHandler.text）
                monsterConfig = JsonUtility.FromJson<MonsterConfig>(webRequest.downloadHandler.text);
                var jsonString = webRequest.downloadHandler.text;
               // jsonFile= JsonUtility.FromJson<TextAsset>(jsonString);
                //string jsonToSave = JsonUtility.ToJson(jsonFile, true);
                SaveJsonToFile(jsonString);
                // 现在你可以使用 userData 中的数据了
                //Debug.Log("Player Speed----------------------: " + config.monsterConfig1.damage);
                //Debug.Log("Enemy Speed------------------: " + config.monsterConfig2.damage);
                //Debug.Log("Player Lives----------------: " + config.wildConfig1.monsterSpeed);
                Debug.Log("it has read the config successfully!!!!!!!!!!");
                Debug.Log(Application.persistentDataPath);
                LoadJsonFromFile();
                Loadfortest();
                
            }
        }
    }
    void SaveJsonToFile(string jsonText)
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "data.json");
        System.IO.File.WriteAllText(filePath, jsonText);
        Debug.Log("Saved data to " + filePath);
    }
 void LoadJsonFromFile()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "data.json");

        if (System.IO.File.Exists(filePath))
        {
            string jsonText = System.IO.File.ReadAllText(filePath);
           monsterConfig=JsonUtility.FromJson<MonsterConfig>(jsonText);
        }
        else
        {
            Debug.LogError("Cannot find file " + filePath);
        }
    }
    public MonsterConfig LoadJsonFromFileRemote()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "data.json");

        if (System.IO.File.Exists(filePath))
        {
            string jsonText = System.IO.File.ReadAllText(filePath);
            // 假设你有一个对应JSON结构的C#类
            //jsonFile = JsonUtility.FromJson<TextAsset>(jsonText);

            var kaka = JsonUtility.FromJson<MonsterConfig>(jsonText);
            return kaka;

            // 现在你可以使用data中的数据了
        }
        else
        { Debug.Log("cant find path--------------------------"); 
        return null;
        }
    }

    void Loadfortest()
    {

        Debug.Log("start to test!!!!!!!!!!!!!!!!!!!!!");
        testConfig = monsterConfig;
        Debug.Log("it has save the data sucessfully!!!"+testConfig.wildConfig1.speedCache);
    
    
    
    }
}
