using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSettingPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/MainSettingPanel";
    static readonly string EntityNumpath = "UI/Prefabs/EntityNumberDisplay";
    //private GameObject EntityNumPanel;
    public MainSettingPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        //按下面板关闭按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_Exit").onClick.AddListener(() =>
        {
            Pop();
        });

        //按下显示EtityNum的按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_EntityNum").onClick.AddListener(ShowEntityNum);

        //按下重新开始的按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_Restart").onClick.AddListener(() =>
        {
            GameRoot.Instance._SceneSystem.SetScene(new LoadScene());
        });

        //按下显示头像按钮
        _UITool.GetOrAddComponentInChildren<Button>("Button_AvatarName").onClick.AddListener(() =>
        {
            if (EntityUIManager.Instance.Is_DisplayAvatarName == true)
                EntityUIManager.Instance.Is_DisplayAvatarName = false;
            else if(EntityUIManager.Instance.Is_DisplayAvatarName == false)
                EntityUIManager.Instance.Is_DisplayAvatarName = true;
        });
    }


    //显示场上有多少Entity单位的方法
    void ShowEntityNum()
    {
        GameObject parent = GameObject.Find("Canvas");
        if (!parent)
        {
            Debug.LogError("        Canvas不存在");
            return;
        }
        var EntityNumPanel = _UITool.FindChildGameObject(parent, "EntityNumberDisplay(Clone)");
        if (EntityNumPanel)
        {
            GameObject.Destroy(EntityNumPanel);
        }
        else
        {
            EntityNumPanel = GameObject.Instantiate(Resources.Load<GameObject>(EntityNumpath), _UITool.GetGameObject().transform.parent.transform);
        }
    }
}
