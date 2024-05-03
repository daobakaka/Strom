using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AttackAuthoring : MonoBehaviour
{
    public int order;
    public float speed;
    public float health;
    public float damage;
    public bool ifIce;


    class AttackBaker : Baker<AttackAuthoring>
    {
        public override void Bake(AttackAuthoring authoring)
        {
            var newentity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(newentity, new EntityAttack
            {
                collisionHealth = authoring.health,
                ifIce=authoring.ifIce,


            }); 
            //AddComponent(newentity, new UIPortalNameComponent
            //{


            //});
        }

    }
}
