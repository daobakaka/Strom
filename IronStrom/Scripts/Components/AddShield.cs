using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct AddShield : IComponentData
{
    public Entity ShieldParent;//��

    public float ShieldScale;//���ܰ뾶
    public float ShieldHP;//����ֵ
    public float ShieldExpandSpeed;
}
