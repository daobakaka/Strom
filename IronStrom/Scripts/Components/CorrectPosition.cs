using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct CorrectPosition : IComponentData//某个物体修正自己和拥有者的位置
{
    public Entity Owner;//拥有者
}
