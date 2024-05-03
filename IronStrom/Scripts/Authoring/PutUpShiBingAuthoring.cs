using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Animations;

public class PutUpShiBingAuthoring : MonoBehaviour
{
    public GameObject GroundSpawnPoint;//地面出生点
    public GameObject AirSpawnPoint;//空中出生点
    [Tooltip("--指定士兵----")]public GameObject SpecifyShiBing;//指定单位
    [Tooltip("--出兵倒计时--")]public float PutUpTime;//出兵倒计时
}

public class PutUpShiBingBaker : Baker<PutUpShiBingAuthoring>
{
    public override void Bake(PutUpShiBingAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var putupshibing = new PutUpShiBing
        {
            GroundSpawnPoint = GetEntity(authoring.GroundSpawnPoint.gameObject, TransformUsageFlags.Dynamic),
            AirSpawnPoint = GetEntity(authoring.AirSpawnPoint.gameObject, TransformUsageFlags.Dynamic),
            SpecifyShiBing = GetEntity(authoring.SpecifyShiBing.gameObject, TransformUsageFlags.Dynamic),
            PutUpTime = authoring.PutUpTime,
            Cur_PutUpTime = authoring.PutUpTime,
            PutUpNum = 1,
        };
        AddComponent(entity, putupshibing);
    }
}
