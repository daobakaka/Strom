using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

public struct Bullet : IComponentData
{
    public float Speed;
    public float Radius;//胶囊体半径
    public float Height;//胶囊体高度

    public Entity CannonHit;//子弹击中特效
    public Entity DeadLanguage;//亡语效果
    public Entity DeadLanguage2;//亡语效果
    public Entity TarGet;//子弹攻击目标
    public Entity DeadParticle;//子弹死亡效果

    public Entity CenterPoint;//中心点的位置

    public bool Is_NOAttack;//是否为不攻击的子弹
}
