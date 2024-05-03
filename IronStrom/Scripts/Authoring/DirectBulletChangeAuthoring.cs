using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DirectBulletChangeAuthoring : MonoBehaviour
{
    //public GameObject Owner;
}

public partial class DirectBulletChangeBaker : Baker<DirectBulletChangeAuthoring>
{
    public override void Bake(DirectBulletChangeAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var dirBulletChange = new DirectBulletChange
        {
            //Owner = GetEntity(authoring.Owner.gameObject, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, dirBulletChange);
    }
}