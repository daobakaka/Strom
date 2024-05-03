using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//��ʼ����
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

    //����������Ϻ�ִ�еķ���
    void SceneLoaded(Scene scene,LoadSceneMode load)
    {
        panelManager.Push(new DifficultyPanel());
        Debug.Log($"{sceneName}����������ϣ�");
    }

}
