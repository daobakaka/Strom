 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI管理工具，获取某个子对象组件的操作
public class UITool
{
    //当前活动面板
    GameObject activePanel;

    public UITool(GameObject panel)
    {
        activePanel = panel;
    }


    //获取自己的GameObject
    public GameObject GetGameObject()
    {
        return activePanel;
    }

    //给当前活动面板获取或者添加一个组件
    public T GetOrAddComponent<T>() where T : Component
    {
        if (activePanel == null)
        {
            Debug.Log($"activePanel为:{activePanel}");
            return null;
        }
            
        if (activePanel.GetComponent<T>() == null)
            activePanel.AddComponent<T>();

        return activePanel.GetComponent<T>();
    }

    //根据名称查找一个子对象
    public GameObject FindChildGameObject(string name)
    {
        Transform[] trans = activePanel.GetComponentsInChildren<Transform>();
        foreach(Transform item in trans)
        {
            if (item.name == name)
                return item.gameObject;
        }
        Debug.Log($"{activePanel.name}里找不到名为{name}的子对象");
        return null;
    }
    public GameObject FindChildGameObject(GameObject obj,string name)
    {
        Transform[] trans = obj.GetComponentsInChildren<Transform>();
        foreach (Transform item in trans)
        {
            if (item.name == name)
                return item.gameObject;
        }
        Debug.Log($"{obj.name}里找不到名为{name}的子对象");
        return null;
    }

    //根据名称获取一个子对象的组件
    public T GetOrAddComponentInChildren<T>(string name) where T : Component
    {
        GameObject child = FindChildGameObject(name);
        if(child)
        {
            if (child.GetComponent<T>() == null)
                child.AddComponent<T>();

            return child.GetComponent<T>();
        }

        return null;
    }

}
