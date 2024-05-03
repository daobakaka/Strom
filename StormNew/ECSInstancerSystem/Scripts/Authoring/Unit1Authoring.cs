using Games.Characters.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Unit1Authoring : MonoBehaviour
{
    public int order;
    public float speed;
    public float health;
    public float damage;
    public GameObject prefab;
    public bool ifMove;
    public GameObject bulletRuntime;

    class Unit1Baker : Baker<Unit1Authoring>
    {
        public override void Bake(Unit1Authoring authoring)
        {
            var newentity = GetEntity( TransformUsageFlags.Dynamic);
            AddComponent(newentity, new Unit1Component
            {
                order = authoring.order,
                speed = authoring.speed,
                health = authoring.health,
                damage = authoring.damage,
              //  prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),

            });
            //AddComponent(newentity, new UIPortalNameComponent
            //{


            //});
        }
    }

}
