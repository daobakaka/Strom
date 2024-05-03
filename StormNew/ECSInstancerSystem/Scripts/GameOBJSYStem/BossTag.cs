using Games.Characters.EliteUnits;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.WebCam;

public class BossTag : MonoBehaviour
{
    public EliteUnitPortalMan.Team team;
    Entity tagentity;
    EntityManager entityManager;
    private UnitSprint myUnit1;
    void Start()
    {
       

    }
    private void Awake()
    {
       
    }
    private void OnEnable()
    {
        InsObj();
    }
    private void OnDisable()
    {
        DestoryEntityObj();
    }
    void DestoryEntityObj()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        commandBuffer.DestroyEntity(tagentity);
        if(entityManager!=null)
        commandBuffer.Playback(entityManager);
        commandBuffer.Dispose();


    }
    void InsObj()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        tagentity = entityManager.CreateEntity();
        entityManager.AddComponentData<unitTagBoss>(tagentity, new unitTagBoss { team = team, health = 0 });
        entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
    }
    // Update is called once per frame
    void Update()
    {     
       // FacedEenemy();
        SyncPosition();      
    }
    void SyncPosition()//the reduce of boss's health 
    {
        entityManager.SetComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, this.transform.rotation));
        var healthReduce = entityManager.GetComponentData<unitTagBoss>(tagentity);
        if (team== EliteUnitPortalMan.Team.Human)
        {
            Rayboss.ICEbossHP -= healthReduce.health;
        }
        else
        {
            Rayboss.FIREbossHP -= healthReduce.health;
        }
        healthReduce.health = 0;
        entityManager.SetComponentData<unitTagBoss>(tagentity, healthReduce);
    }
}
