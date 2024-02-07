using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using ProjectDawn.Navigation.Sample.Mass;

class SpawnerAuthoring : MonoBehaviour//entity�ĺ決
{
    public GameObject Prefab;
    public float SpawnRate;
    public int MonoNum;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner0
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate,
            MyNum = authoring.MonoNum,

        }); 

        //var entity2 = GetEntity(TransformUsageFlags.None);
        //AddComponent(entity2, new Spawner
        //{
        //    Prefab = GetEntity(authoring.Plane, TransformUsageFlags.Dynamic),
        //    SpawnPosition = authoring.transform.position-new Vector3(0,1150,0),
        //    NextSpawnTime = 0.0f,

        //});
    }
}
