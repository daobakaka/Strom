using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Queries for all Spawner components. Uses RefRW because this system wants
        // to read from and write to the component. If the system only needed read-only
        // access, it would use RefRO instead.
        foreach (RefRW<Spawner0> spawner in SystemAPI.Query<RefRW<Spawner0>>())
        {
            ProcessSpawner(ref state, spawner);
            //if (Input.GetKeyDown(KeyCode.J))//查询后生成
            //{
            //    Genclone( ref state, spawner);
            //    Debug.Log("生成一次Entity");
            //}

           
        }
    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner0> spawner)
    {
        // If the next spawn time has passed.
        if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            // Spawns a new entity and positions it at the spawner.
            //Debug.Log("生成一次Entity");
            for (int i = 0; i < 10; i++)
            {
                Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRW.Prefab);
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(new float3(-226 + Random.Range(-200, 200), 5, 216 + Random.Range(-50, 50))));

            }
            // Resets the next spawn time.
            spawner.ValueRW.NextSpawnTime = (float)(SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate);
            // Debug.Log("¿ªÊ¼¸´ÖÆentity");

        }

    }
    private void Genclone(ref SystemState state, RefRW<Spawner0> spawner)
    {
        for (int i = 0; i < 3; i++)
        {
            Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRW.Prefab);
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(new float3(-226 + Random.Range(-200, 200), 5, 216 + Random.Range(-50, 50))));


        }
    }
}
/////
////using Unity.Collections;
////using Unity.Entities;
////using Unity.Transforms;
////using Unity.Burst;
////using UnityEngine;

////[BurstCompile]
////public partial struct OptimizedSpawnerSystem : ISystem
////{
////    public void OnCreate(ref SystemState state) { }

////    public void OnDestroy(ref SystemState state) { }

////    [BurstCompile]
////    public void OnUpdate(ref SystemState state)
////    {
////        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

////        // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
////        new ProcessSpawnerJob
////        {
////            ElapsedTime = SystemAPI.Time.ElapsedTime,
////            Ecb = ecb
////        }.ScheduleParallel();
////    }

////    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
////    {
////        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
////        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
////        return ecb.AsParallelWriter();
////    }
////}

////[BurstCompile]
////public partial struct ProcessSpawnerJob : IJobEntity
////{
////    public EntityCommandBuffer.ParallelWriter Ecb;
////    public double ElapsedTime;

////    // IJobEntity generates a component data query based on the parameters of its `Execute` method.
////    // This example queries for all Spawner components and uses `ref` to specify that the operation
////    // requires read and write access. Unity processes `Execute` for each entity that matches the
////    // component data query.
////    private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
////    {
////        // If the next spawn time has passed.
////        if (spawner.NextSpawnTime < ElapsedTime)
////        {
////            // Spawns a new entity and positions it at the spawner.
////            Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
////            Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

////            // Resets the next spawn time.
////            spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
////        }
////    }
////}

