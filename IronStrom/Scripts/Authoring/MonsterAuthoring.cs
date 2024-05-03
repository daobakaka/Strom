using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterAuthoring : MonoBehaviour
{

}

public class MonsterBaker : Baker<MonsterAuthoring>
{
    public override void Bake(MonsterAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var monster = new Monster();
        AddComponent(entity, monster);
    }
}