using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TOWER : IComponentData
{
    public bool Is_HaveActComponent;//防御塔是否拥有行为

    public Entity GunBarrel;//炮管

}
