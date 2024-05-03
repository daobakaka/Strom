using Unity.Entities;
using UnityEngine;

public class ShieldAuthoring : MonoBehaviour
{
    //public GameObject ShieldParent;
}

public class ShieldBaker : Baker<ShieldAuthoring>
{
    public override void Bake(ShieldAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var shield = new Shield
        {
            //ShieldParent = GetEntity(authoring.ShieldParent, TransformUsageFlags.Dynamic),
        };
        AddComponent(entity, shield);
    }
}