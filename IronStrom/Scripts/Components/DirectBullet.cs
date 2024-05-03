using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DirectBullet : IComponentData//直接做用的子弹
{
    public Entity BulletHit;//子弹击中特效
    public float StartLifetime;//子弹特效的粒子存活时间
    public float StartSpeed;//子弹特效的粒子速度
    public float StartOffset;//子弹特效的前后偏移

}
