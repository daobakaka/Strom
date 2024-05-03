using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct AddShield : IComponentData
{
    public Entity ShieldParent;//¸¸

    public float ShieldScale;//»¤¶Ü°ë¾¶
    public float ShieldHP;//»¤¶ÜÖµ
    public float ShieldExpandSpeed;
}
