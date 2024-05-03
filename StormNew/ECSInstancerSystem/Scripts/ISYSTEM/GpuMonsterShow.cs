using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct GpuMonsterShow : ISystem, ISystemStartStop
{
    public void OnStartRunning(ref SystemState state)
    {
        //var handle = World.ActiveWorld.GetSystemBaseHandle<T>;
        //ref var system = ref World.ActiveWorld.GetSystem<T>(handle);
        //var handle = World.
    }

    public void OnStopRunning(ref SystemState state)
    {
        
    }

    void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<UnitSpawner>();

    }

    void OnDestroy(ref SystemState state) { }

    void OnUpdate(ref SystemState state) { }
}
