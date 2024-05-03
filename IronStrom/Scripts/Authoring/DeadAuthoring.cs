using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DeadAuthoring : MonoBehaviour
{

}

public class DeadBaker : Baker<DeadAuthoring>
{
    public override void Bake(DeadAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var die = new Die();
        AddComponent(entity, die);
    }
}
