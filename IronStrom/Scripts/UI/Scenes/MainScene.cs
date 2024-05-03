using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Main����
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
        //    Debug.Log($"���һ��MainScene������ʼ����  ��ǰ��������Ϊ��{SceneManager.GetActiveScene().name}");
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

    //����������Ϻ�ִ�еķ���
    void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        panelManager.Push(new MainPanel());
        Debug.Log($"{sceneName}����������ϣ�");
    }
}
