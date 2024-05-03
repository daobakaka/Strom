using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : ScenesState
{
    readonly string sceneName = "LoadScene";
    PanelManager panelManager;
    public override void OnEnter()
    {
        panelManager = PanelManager.instance;
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
            panelManager.Push(new LoadPanel());
            //SceneManager.sceneLoaded += SceneLoaded;
        }
        else
            panelManager.Push(new LoadPanel());



    }

    public override void OnExit()
    {
        //SceneManager.sceneLoaded -= SceneLoaded;
        panelManager.PopAll();

    }

    //场景加载完毕后执行的方法
    void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        panelManager.Push(new LoadPanel());
        Debug.Log($"{sceneName}场景加载完毕！");
    }
}
