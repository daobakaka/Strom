using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景的状态管理系统
public class SceneSystem
{
    //场景状态类
    ScenesState sceneState;

    //设置当前场景并进入当前场景
    public void SetScene(ScenesState state)
    {
        sceneState?.OnExit();
        sceneState = state;
        sceneState?.OnEnter();
    }

}
