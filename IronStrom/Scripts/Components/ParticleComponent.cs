using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct ParticleComponent : IComponentData
{
    public float DieTime;//死亡计时
    public bool Is_Follow;//是否要跟随目标
    public Entity FollowPoint;
    public bool Is_CasterDead;//施法者是否死亡
    public Entity Caster;//施法者
    public float ParScale;//特效的大小
    public bool Is_NoDieTime;//不要倒计时
}
