using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public struct Parasitic : IComponentData//��������λװ״̬���(Idle,Move...)Ȼ��ɾ������
{
    public bool Is_HaveActComponent;//��û����Ϊ���
    public Entity Owner;
}
