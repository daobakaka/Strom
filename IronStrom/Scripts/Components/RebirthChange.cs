using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct RebirthChange : IComponentData
{
    public bool Is_UpSkill;//�Ƿ��������ĵ�λ
    public Entity PlayerEntityID;//��˵����EntityID
}
