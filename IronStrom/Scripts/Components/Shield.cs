using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Shield : IComponentData//护盾
{
                               //可能需要一个枚举来辨别是什么护盾
    public Entity ShieldParent;
    public float ShieldScale;//护盾半径
    public float ShieldExpandSpeed;

}
