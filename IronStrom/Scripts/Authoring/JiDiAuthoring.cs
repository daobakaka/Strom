using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class JiDiAuthoring : MonoBehaviour
{
    public GameObject FirePoint;
    public GameObject WalkPoint;
    public GameObject shieldPoint;
    public List<GameObject> JiDiPoint;//基地被攻击位置
}

public class JiDiBaker : Baker<JiDiAuthoring>
{
    public override void Bake(JiDiAuthoring authoring)
    {
        //设置基地组件
        var entity = GetEntity(authoring.gameObject,TransformUsageFlags.Dynamic);
        var jidi = new JiDi
        {
            FirePoint = GetEntity(authoring.FirePoint.gameObject, TransformUsageFlags.Dynamic),
            WalkPoint = GetEntity(authoring.WalkPoint.gameObject, TransformUsageFlags.Dynamic),
            shieldPoint = GetEntity(authoring.shieldPoint.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, jidi);

        //设置基地被攻击点buffer组件
        var jidipointBuffer = AddBuffer<JiDiPointBuffer>(entity);
        foreach (var point in authoring.JiDiPoint)
        {
            jidipointBuffer.Add(new JiDiPointBuffer
            {
                PointEntity = GetEntity(point, TransformUsageFlags.Dynamic),
            });
        }
    }
}