using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�洢����UI��Ϣ�������Դ���������������UI
public class UIManager
{
    private Dictionary<UIType, GameObject> DicUI;

    public UIManager()
    {
        DicUI = new Dictionary<UIType, GameObject>();
    }

    //��ȡһ��UI����
    public GameObject GetSingleUI(UIType type)
    {
        GameObject parent = GameObject.Find("Canvas");
        if(!parent)
        {
            Debug.LogError("        Canvas������");
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
    //�������ֻ�ȡһ��UI����
    public GameObject GetSingleUI(string UIname)
    {
        GameObject parent = GameObject.Find("Canvas");
        if (!parent)
        {
            Debug.LogError("        Canvas������");
            return null;
        }
        foreach(var pair in DicUI)
        {
            if (pair.Key.Name == UIname)
                return pair.Value;
        }
        return null;
    }

    //����һ��UI����
    public void DestroyUI(UIType type)
    {
        if(DicUI.ContainsKey(type))
        {
            GameObject.Destroy(DicUI[type]);
            DicUI.Remove(type);
        }
    }

}
