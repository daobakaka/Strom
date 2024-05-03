using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//开始场景
public class StartScenes : ScenesState
{
    readonly string sceneName = "UIStart";
    PanelManager panelManager;

    public override void OnEnter()
    {
        panelManager = PanelManager.instance;
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SceneManager.sceneLoaded += SceneLoaded;
        }
        else
            panelManager.Push(new DifficultyPanel());

    }

    public override void OnExit()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        panelManager.PopAll();
    }

    //场景加载完毕后执行的方法
    void SceneLoaded(Scene scene,LoadSceneMode load)
    {
        panelManager.Push(new DifficultyPanel());
        Debug.Log($"{sceneName}场景加载完毕！");
    }

}
