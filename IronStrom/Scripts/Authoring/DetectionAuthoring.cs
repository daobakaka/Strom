using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DetectionAuthoring : MonoBehaviour
{

}

public class DetectionBaker : Baker<DetectionAuthoring>
{
    public override void Bake(DetectionAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var aetect = new Detection();
        
        AddComponent(entity, aetect);
    }

}

