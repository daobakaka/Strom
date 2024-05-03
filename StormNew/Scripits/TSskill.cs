using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TSskill : MonoBehaviour
{
    /// <summary>
    /// the module of mecha entity
    /// </summary>
    public bool isICE;
 
    // Start is called before the first frame update
    public float duration = 1;
    void Start()
    {
        
    }
    public void AttackEffect(Monstermove monstermove)
    {
        //在Entity 世界检测敌人用
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var tagentity = entityManager.CreateEntity();
        entityManager.AddComponentData<BulletSkill>(tagentity, new BulletSkill { team = monstermove.monsterData.team, monsterType = monstermove.monsterData.monsterType, range = 40, damage = Mathf.CeilToInt(monstermove.monsterConfigs.damage), playerName = monstermove.playerID });
        entityManager.AddComponentData<LocalTransform>(tagentity, LocalTransform.FromPositionRotation(this.transform.position, transform.rotation));
        entityManager.SetName(tagentity, "BulletSkill");
    }
    private void OnEnable()
    {
        StartCoroutine("IELoseme");
    }
    IEnumerator IELoseme()
    { 
    yield return new WaitForSeconds(duration);
        Netpool.Getinstance().Pushobject(this.name, gameObject);
    
    }
}
