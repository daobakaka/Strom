using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct InstantiateFlesh : IComponentData, IEnableableComponent
{
    public int TeamID;
}
