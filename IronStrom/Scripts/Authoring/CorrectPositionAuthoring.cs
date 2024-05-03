using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CorrectPositionAuthoring : MonoBehaviour
{
    public GameObject Owner;//ÓµÓÐÕß
}


public class CorrectPositionBaker : Baker<CorrectPositionAuthoring>
{
    public override void Bake(CorrectPositionAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var CorrectPos = new CorrectPosition
        {
            Owner = GetEntity(authoring.Owner.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, CorrectPos);
    }
}