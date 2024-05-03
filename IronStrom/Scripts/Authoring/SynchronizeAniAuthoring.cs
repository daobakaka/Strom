using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SynchronizeAniAuthoring : MonoBehaviour
{
    [Tooltip("--Obj动画触发Event的时间_攻击动画")] public float ObjAniEventTime_Fire;
    [Tooltip("--Obj动画触发Event的时间_空中攻击动画")] public float ObjAniEventTime_FireAir;
    [Tooltip("--Obj攻击动画的总时间")] public float ObjAniTotalTime_Fire;
    [Tooltip("--Obj空中攻击动画的总时间")] public float ObjAniTotalTime_FireAir;
}

public class SynchronizeAniBaker : Baker<SynchronizeAniAuthoring>
{
    public override void Bake(SynchronizeAniAuthoring authoring)
    {
        var entitty = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var synchronizeAni = new SynchronizeAni 
        {
            ObjAniEventTime_Fire = authoring.ObjAniEventTime_Fire,
            ObjAniEventTime_FireAir = authoring.ObjAniEventTime_FireAir,
            ObjAniTotalTime_Fire = authoring.ObjAniTotalTime_Fire,
            ObjAniTotalTime_FireAir = authoring.ObjAniTotalTime_FireAir,
            Cur_ObjAniTotalTime_Fire = 0,
            EventKey = 0,
        };
        AddComponent(entitty, synchronizeAni);
    }
}