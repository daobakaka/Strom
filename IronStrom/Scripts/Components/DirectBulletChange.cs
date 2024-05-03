using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum DirectBullet_FirePoint
{
    Null,
    FirePoint_R,
    FirePoint_L,
    FirePoint_R2,
    FirePoint_L2,
}
public struct DirectBulletChange : IComponentData
{
    public Entity Owner;//这个子弹的拥有者
    public DirectBullet_FirePoint DB_FirePoint;//子弹特效是哪个发射口
}
