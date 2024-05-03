using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存储单个UI的信息
public class UIType
{
    //UI名字
    public string Name { get; private set; }
    //UI路径
    public string Path { get; private set; }

    public UIType(string path)
    {
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }

}
