using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存储所有UI信息，并可以创建或者销毁所有UI
public class UIManager
{
    private Dictionary<UIType, GameObject> DicUI;

    public UIManager()
    {
        DicUI = new Dictionary<UIType, GameObject>();
    }

    //获取一个UI对象
    public GameObject GetSingleUI(UIType type)
    {
        GameObject parent = GameObject.Find("Canvas");
        if(!parent)
        {
            Debug.LogError("        Canvas不存在");
            return null;
        }
        if (DicUI.ContainsKey(type))
            return DicUI[type];

        GameObject ui = null;
        ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent.transform);
        ui.name = type.Name;
        DicUI.Add(type, ui);

        return ui; 
    }
    //更具名字获取一个UI对象
    public GameObject GetSingleUI(string UIname)
    {
        GameObject parent = GameObject.Find("Canvas");
        if (!parent)
        {
            Debug.LogError("        Canvas不存在");
            return null;
        }
        foreach(var pair in DicUI)
        {
            if (pair.Key.Name == UIname)
                return pair.Value;
        }
        return null;
    }

    //销毁一个UI对象
    public void DestroyUI(UIType type)
    {
        if(DicUI.ContainsKey(type))
        {
            GameObject.Destroy(DicUI[type]);
            DicUI.Remove(type);
        }
    }

}
