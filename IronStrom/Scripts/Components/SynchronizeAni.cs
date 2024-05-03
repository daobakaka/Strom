using Unity.Entities;
using UnityEngine;

public struct SynchronizeAni : IComponentData
{
    public float ObjAniEventTime_Fire;
    public float ObjAniEventTime_FireAir;
    public float ObjAniTotalTime_Fire;
    public float ObjAniTotalTime_FireAir;
    public float Cur_ObjAniTotalTime_Fire;
    public int EventKey;

    public bool ShootEnitIs_Air;

    public bool Is_EventFire_1;//��������Event
    public bool Is_EventFire_2;//��������Event
    public bool Is_PlayingAniFire;//���������Ƿ����

    public bool Is_DotsEventFire;
    public bool Is_DotsPlayingAniFire;
}
