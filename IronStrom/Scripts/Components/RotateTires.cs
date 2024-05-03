using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct RotateTires : IComponentData//钢球的轮胎旋转
{
    public Entity Owner;//这个轮胎的拥有者
}
