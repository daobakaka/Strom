using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�洢����UI����Ϣ
public class UIType
{
    //UI����
    public string Name { get; private set; }
    //UI·��
    public string Path { get; private set; }

    public UIType(string path)
    {
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }

}