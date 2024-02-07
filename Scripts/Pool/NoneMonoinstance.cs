using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 非继承MONO 单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class NoneMonoinstance<T> where T : new()

{
    private static T instance;

    public static T Getinstance()
    {
        if (instance == null)
        {
            instance = new T();

        }

        return instance;


    }
}

public class Hostinfo : NoneMonoinstance<Hostinfo>

{
    public string name;
    public int level;
    public string ID;
}

