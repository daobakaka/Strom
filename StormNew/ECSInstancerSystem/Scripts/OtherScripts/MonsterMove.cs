using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterMove : MonoBehaviour
{
    public bool isICE;
    Entity tagentity;
    EntityManager entityManager;
    public string targetBoss;
    public GameObject showTargrt;
    public float distance;
    public float moveSpeed;
    private Animator animator0;
    public float monsterHealth;
    public float monsterHealthTotal;
    private UnitSprint myUnit1;
    void Start()
    {

        animator0 = GetComponent<Animator>();
    }
    private void Awake()
    {
      
    }
    private void OnEnable()
    {
        InsEntityObj();
    }
    private void OnDisable()
    {
        DestoryEntityObj();
    }
    void InsEntityObj()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        tagentity = entityManager.CreateEntity();
        //if (isICE)
        //{
        //    entityManager.AddComponentData<Unit1Component>(tagentity, new Unit1Component { order = 101, health=0,});
        //    entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
        //    entityManager.AddComponentData<UnitSprint>(tagentity, myUnit1 = new UnitSprint {sprint=false,order=101 });
        //    entityManager.SetName(tagentity, "unitMonsterIce");
        //}
        //else
        //{
        //    entityManager.AddComponentData<Unit1Component>(tagentity, new Unit1Component { order = 101, health = 0, });
        //    entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
        //    entityManager.AddComponentData<UnitSprint>(tagentity, myUnit1 = new UnitSprint { sprint = false, order = 102 });
        //    entityManager.SetName(tagentity, "unitMonsterFire");
        //}
    }
    // Update is called once per frame
    void Update()
    {
       // FacedEenemy();
        SyncPosition();
        RealMove();
    }
    void DestoryEntityObj()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        commandBuffer.DestroyEntity(tagentity);
        commandBuffer.Playback(entityManager);
        commandBuffer.Dispose();


    }
    void SyncPosition()
    {
        entityManager.SetComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, this.transform.rotation));
        var healthReduce = entityManager.GetComponentData<Unit1Component>(tagentity);
        monsterHealthTotal = monsterHealth + healthReduce.health;
    }
    void FacedEenemy()
    {
        if (isICE)
        {
            var rotation = entityManager.GetComponentData<LocalTransform>(tagentity);
            this.transform.rotation = rotation.Rotation;
            // Debug.Log($"rotate is :{rotation.Rotation}");
        }
        else
        {
            var rotation = entityManager.GetComponentData<LocalTransform>(tagentity);
            this.transform.rotation = rotation.Rotation;
        }
    }

    void RealMove()
    {
        if (monsterHealthTotal > 0)
        {
           
            if (GameObject.FindWithTag(targetBoss) != null)
            {
                showTargrt = GameObject.FindWithTag(targetBoss);

                var direction = (showTargrt.transform.position - this.transform.position);

                this.transform.LookAt(new Vector3(showTargrt.transform.position.x, 0, showTargrt.transform.position.z), Vector3.up);
                //iflookat = false;

                if (direction.sqrMagnitude > 10.1f)
                {

                    if (direction.sqrMagnitude > 1.3f * distance)
                    {
                        transform.Translate(new Vector3(direction.normalized.x * moveSpeed * Time.deltaTime, 0, direction.normalized.z * moveSpeed * Time.deltaTime), Space.World);
                        animator0.SetBool("walking", true);
                        myUnit1.sprint = true;
                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);
                     


                    }
                    else
                    {
                        animator0.SetTrigger("attack");
                        animator0.SetBool("walking", false);
                        myUnit1.sprint = false;
                        entityManager.SetComponentData<UnitSprint>(tagentity, myUnit1);

                    }
                }
            }
        }
        else
        {
            animator0.SetBool("death", true);
            animator0.SetBool("walking", false);
            transform.gameObject.SetActive(false);

        }
    }
}
