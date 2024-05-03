using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//场景状态
public abstract class ScenesState
{
    //场景进入时执行的操作
    public abstract void OnEnter();
    //场景退出时执行的操作
    public abstract void OnExit();

}
