using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BingYing : IComponentData
{
    public int TeamID;//�ķ�����
    public int PlayerID;//���ID
    public Entity FirePoint;//������
    public float CountDownTime;//��������ʱ
    public float Cur_CountDownTime;//��������ʱ
    public float InitSpeed;//��ʼ���ٶ�
}
