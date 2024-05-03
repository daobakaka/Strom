using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct RebirthChange : IComponentData
{
    public bool Is_UpSkill;//是否是升级的单位
    public Entity PlayerEntityID;//凤凰的玩家EntityID
}
