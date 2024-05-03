using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EntityOpenID : IComponentData
{
    public Entity PlayerEntiyID;//士兵拿到所属玩家的EntityID对应玩家的OpenID
}
