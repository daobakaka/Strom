using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Main场景
public class MainScene : ScenesState
{
    readonly string sceneName = "GameScene";
    PanelManager panelManager;
    UIManager uiManager;
    public override void OnEnter()
    {
        panelManager = PanelManager.instance;//new PanelManager();
        //uiManager = new UIManager();
        //if (SceneManager.GetActiveScene().name != sceneName)
        //{
        //    Debug.Log($"添加一次MainScene场景开始监听  当前场景名字为：{SceneManager.GetActiveScene().name}");
        //    SceneManager.LoadScene(sceneName);
        //    panelManager.Push(new MainPanel());
        //    //SceneManager.sceneLoaded += SceneLoaded;
        //}
        //else

        //var uitype = new UIType("UI/Prefabs/MainPanel");
        //uiManager.GetSingleUI(uitype);
        panelManager.Push(new MainPanel());

    }

    public override void OnExit()
    {
        //SceneManager.sceneLoaded -= SceneLoaded;
        panelManager.PopAll();
    }

    //场景加载完毕后执行的方法
    void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        panelManager.Push(new MainPanel());
        Debug.Log($"{sceneName}场景加载完毕！");
    }
}
