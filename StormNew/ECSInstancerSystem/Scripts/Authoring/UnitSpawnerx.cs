using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitSpawnerx : MonoBehaviour
{
    public GameObject unit1;
    public GameObject unit2;
    public int insnum;
    public float spawnRate;
    public float lengthRate;
    public float nextSpawnerTime;
    public GameObject killer;
    public GameObject GpuMonsterIce;
    public GameObject GpuMonsterFire;
    public GameObject bullet;
    public GameObject deadBodyice1;
    public GameObject deadBodyice2;
    public GameObject deadBodyfire1;
    public GameObject deadBodyfire2;

    class UnitBaker : Baker<UnitSpawnerx>
    {
        public override void Bake(UnitSpawnerx authoring)
        {
            var newentity = GetEntity(TransformUsageFlags.None);
            AddComponent(newentity, new UnitSpawner
            {
                entity1 = GetEntity(authoring.unit1, TransformUsageFlags.Dynamic),
                entity2 = GetEntity(authoring.unit2, TransformUsageFlags.Dynamic),
                insnum = authoring.insnum,
                spawnRate=authoring.spawnRate,
                lengthRate=authoring.lengthRate,
                nextSpawnerTime=authoring.nextSpawnerTime,
                killer=GetEntity(authoring.killer,TransformUsageFlags.Dynamic),
                bullet=GetEntity(authoring.bullet,TransformUsageFlags.Dynamic),
                GpuMonsterIce=GetEntity(authoring.GpuMonsterIce,TransformUsageFlags.Dynamic),
                GpuMonsterFire=GetEntity(authoring.GpuMonsterFire,TransformUsageFlags.Dynamic),
                deadBodyice1=GetEntity(authoring.deadBodyice1,TransformUsageFlags.Dynamic),
                deadBodyice2=GetEntity(authoring.deadBodyice2,TransformUsageFlags.Dynamic),
                deadBodyfire1=GetEntity(authoring.deadBodyfire1,TransformUsageFlags.Dynamic),
                deadBodyfire2=GetEntity(authoring.deadBodyfire2,TransformUsageFlags.Dynamic),
            });
        }
    }
}
