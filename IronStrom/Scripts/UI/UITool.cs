 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI�����ߣ���ȡĳ���Ӷ�������Ĳ���
public class UITool
{
    //��ǰ����
    GameObject activePanel;

    public UITool(GameObject panel)
    {
        activePanel = panel;
    }


    //��ȡ�Լ���GameObject
    public GameObject GetGameObject()
    {
        return activePanel;
    }

    //����ǰ�����ȡ�������һ�����
    public T GetOrAddComponent<T>() where T : Component
    {
        if (activePanel == null)
        {
            Debug.Log($"activePanelΪ:{activePanel}");
            return null;
        }
            
        if (activePanel.GetComponent<T>() == null)
            activePanel.AddComponent<T>();

        return activePanel.GetComponent<T>();
    }

    //�������Ʋ���һ���Ӷ���
    public GameObject FindChildGameObject(string name)
    {
        Transform[] trans = activePanel.GetComponentsInChildren<Transform>();
        foreach(Transform item in trans)
        {
            if (item.name == name)
                return item.gameObject;
        }
        Debug.Log($"{activePanel.name}���Ҳ�����Ϊ{name}���Ӷ���");
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
        Debug.Log($"{obj.name}���Ҳ�����Ϊ{name}���Ӷ���");
        return null;
    }

    //�������ƻ�ȡһ���Ӷ�������
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
