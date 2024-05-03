using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceCampAuthoring : MonoBehaviour
{
    public int value;
    class IceCampBaker : Baker<IceCampAuthoring>

    {
        public override void Bake(IceCampAuthoring authoring)
        {
            var newentity = GetEntity(TransformUsageFlags.Dynamic);
            AddSharedComponent(newentity, new IceCampDistinguish
            {
                value = authoring.value
            }); 
        }
    }
}
