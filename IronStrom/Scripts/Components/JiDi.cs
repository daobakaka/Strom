using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct JiDi : IComponentData
{
    public Entity FirePoint;//������
    public Entity WalkPoint;//������ص�λ��
    public Entity shieldPoint;//���ػ��ܵ�λ��
    public Entity JidiShield;//���صĻ���
}
