using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitySynchronizeObjAuthroing : MonoBehaviour
{

}

public class EntitySynchronizeObjBaker : Baker<EntitySynchronizeObjAuthroing>
{
    public override void Bake(EntitySynchronizeObjAuthroing authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var EntiSynObj = new EntitySynchronizeObj();
        AddComponent(entity, EntiSynObj);
    }
}
