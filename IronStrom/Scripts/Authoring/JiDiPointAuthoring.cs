using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class JiDiPointAuthoring : MonoBehaviour
{
    public GameObject Jidi;
}

public class JiDiPointBaker : Baker<JiDiPointAuthoring>
{
    public override void Bake(JiDiPointAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var jidiPoint = new JiDiPoint
        {
            Jidi = GetEntity(authoring.Jidi.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, jidiPoint);
    }
}