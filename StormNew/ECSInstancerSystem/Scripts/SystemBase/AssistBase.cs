using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class AssistBase : SystemBase
{
    EntityQuery entityQuery;
    EntityQuery entityQueryIce;
    int totalEntityCount;
    int totalEntityCountIce;
    int totalEntityCountFire;
    float nextSpawnerTime;
    protected override void OnCreate()
    {
        RequireForUpdate<UnitSpawner>();
        base.OnCreate();

      
    }
    protected override void OnStartRunning()
    {
        
        entityQuery = EntityManager.CreateEntityQuery(typeof(Unit1Component));
        entityQueryIce = EntityManager.CreateEntityQuery(typeof(IceCampDistinguish));
        nextSpawnerTime = 0;
        
    }
    protected override void OnUpdate()
    {
        #region the base to foreach entities
        //Entities.ForEach((ref UnitSpawner unitspawn) =>
        //{

        //    if (unitspawn.nextSpawnerTime < SystemAPI.Time.ElapsedTime)
        //    {


        //    }


        //}).Schedule();
        #endregion

        if (nextSpawnerTime < SystemAPI.Time.ElapsedTime)
        { 
            nextSpawnerTime = SystemAPI.GetSingleton<UnitSpawner>().spawnRate + (float)SystemAPI.Time.ElapsedTime;
            totalEntityCount = entityQuery.CalculateEntityCount();
            totalEntityCountIce = entityQueryIce.CalculateEntityCount();
            totalEntityCountFire = totalEntityCount - totalEntityCountIce;
            EntityOfMonitor.Instance.entitiesNum = totalEntityCount;
            EntityOfMonitor.Instance.entitiesIceNum = totalEntityCountIce;
            EntityOfMonitor.Instance.entitiesFireNum = totalEntityCountFire;
            EntityOfMonitor.Instance.floor = (int)math.floor(totalEntityCount / 50);
            float firetime =(float) (totalEntityCountIce / (1 + totalEntityCountFire));
            float icetime = (float)(totalEntityCountFire / (1 + totalEntityCountIce));
            if (totalEntityCountIce - totalEntityCountFire >= 0) 
            {
                EntityOfMonitor.Instance.FireTimes = 1 + firetime/ (totalEntityCount/100+1);
                EntityOfMonitor.Instance.IceTimes = 1;
               // Debug.Log($"ice time,and the firetimes is:{firetime} floor is{(EntityOfMonitor.Instance.floor + 1)},and the total is{firetime / (EntityOfMonitor.Instance.floor + 1)}");
            }
            else
            {
                EntityOfMonitor.Instance.IceTimes = 1+  icetime / (totalEntityCount /100 + 1);
                EntityOfMonitor.Instance.FireTimes = 1;
               // Debug.Log($"fire time,and the icetimes is:{icetime} floor is{ (EntityOfMonitor.Instance.floor + 1)},and the total is{icetime / (EntityOfMonitor.Instance.floor + 1)}");
            }
           
        }//the corotinue of handle that to caculate all of the entities in the world
    }
}
