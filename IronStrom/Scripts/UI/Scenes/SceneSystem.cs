using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������״̬����ϵͳ
public class SceneSystem
{
    //����״̬��
    ScenesState sceneState;

    //���õ�ǰ���������뵱ǰ����
    public void SetScene(ScenesState state)
    {
        sceneState?.OnExit();
        sceneState = state;
        sceneState?.OnEnter();
    }

}
