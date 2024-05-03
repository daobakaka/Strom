using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SelectAnimationAuthoring : MonoBehaviour
{
    public AnimationID AnimID;
}

public class SelectAnimationBaker : Baker<SelectAnimationAuthoring>
{
    public override void Bake(SelectAnimationAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var selectAnim = new SelectAnimation
        {
            AnimID = authoring.AnimID,
        };
        AddComponent(entity, selectAnim);
    }
}

