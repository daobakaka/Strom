using com.unity.mgobe.src.Matcher;
using Games.Characters.EliteUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class FindEnemy : MonoBehaviour
{
    public Monstermove monstermove;
    private Rigidbody rigidbody0;
    public SphereCollider sphereCollider;
    public EliteUnitPortalMan.Team team;
//是否攻击的步兵 小兵
    public bool isAttackEnity;
    public string targetboss;
    public string targetenemy;
    public string mytag;
    public float speed = 100;
    public float speedcache;
    public float readytime = 2;
    private bool iffind = true;
    public GameObject realtarget = null;
    public Entity entity;
    private bool ifshoot = false;
    private bool ifParabola = false;
    public bool cheakice;
    // public GameObject FWD;
    private float distance;
    /// <summary>
    /// trajectory variable of bullet
    /// </summary>
    private float rotateRadius;
    private float rotateRadius_che;
    private float rotateRate;
    private bool islookat;
    public float mindistance = 30000;
    int count = 2;
    // private bool ifParabola_x = false;

    void Start()
    {
        //if (FWD != null)
        //    FWD = GameObject.FindWithTag("VFX");

    }
    public void OnEnable()
    {
        iffind = true;
    }
    public void Init(Monstermove monstermove)
    {
        count = 2;
        this.monstermove = monstermove;
        team = monstermove.monsterData.team;
        isAttackEnity = false;
        rigidbody0 = this.GetComponent<Rigidbody>();
        sphereCollider = this.GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
        GetComponent<TrailRenderer>().emitting = false;
        iffind = true;
        islookat = true;
        ifParabola = false;
        this.tag = "Untagged";
        ifshoot = false;
        rigidbody0.velocity = Vector3.zero * 0;
        if (GameManager.instance.isGameFighting)//the judge for wjy code
            StartCoroutine("IEFind");
        //---velocity module
        speed = speedcache;
        rotateRadius = Random.Range(0, 1f) > 0.5f ? Random.Range(0.2f, 1f) * 100 : -Random.Range(0.2f, 1f) * 100;
        rotateRadius_che = Random.Range(0, 1f) > 0.5f ? Random.Range(0.2f, 1f) * 100 : -Random.Range(0.2f, 1f) * 100;
        rotateRate = Random.Range(0.2f, 0.7f);
        //-- velocity module
        //InsEntityObj();
    }
    private void OnDisable()
    {
        StopCoroutine("IEFind");
        GetComponent<TrailRenderer>().emitting = false;
        //---速度为零
        if(rigidbody0!=null)
        rigidbody0.velocity = Vector3.zero * 0;
        transform.rotation = quaternion.identity;
        transform.position = new Vector3(0, 0, 0);
        realtarget = null;
        //DestoryEntityObj();
        // Debug.Log("cloese bullet---------------------------------------------------------------------------------");
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameFighting&& !iffind)
        {
            Parabola();
            SpeedChange();//the change of speed that mecha provide;
           // SyncPosition();
        }
    }
    public void Find() 
    {
        iffind = true;
        this.tag = mytag;
        rigidbody0.velocity = new Vector3(Random.Range(0, 0.4f), 1, Random.Range(0, 0.4f)) * 75;//开始增加一个向上速度
        //yield return new WaitForSeconds(1);
        //rigidbody0.velocity = Vector3.up * 10;
        //---加速减速零
        //查找是否有合适的野怪
        var yeguaiList = Monsterins.instance.GetEliteMonsterList_WILD();
        if (yeguaiList != null && yeguaiList.Count > 0 && iffind)
        {
            foreach (var item in yeguaiList)
            {
                if ((transform.position - item.transform.position).sqrMagnitude < mindistance)
                {
                    //找到合适的攻击目标
                    realtarget = item.gameObject;
                    transform.LookAt((realtarget.transform.position + new Vector3(0, 300, 0)));
                    distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
                    iffind = false;
                    break;
                }
            }
        }
        //查找精英怪
        if (iffind)
        {
            List<Monstermove> list = new List<Monstermove>();
            var jijiaList = Monsterins.instance.GetEliteMonsterList(MonsterType.Mecha);
            if (jijiaList != null)
                list.AddRange(jijiaList);
            jijiaList = Monsterins.instance.GetEliteMonsterList(MonsterType.FemaleHunter);
            if (jijiaList != null)
                list.AddRange(jijiaList);
            jijiaList = Monsterins.instance.GetEliteMonsterList(MonsterType.guard);
            if (jijiaList != null)
                list.AddRange(jijiaList);
            jijiaList = Monsterins.instance.GetEliteMonsterList(MonsterType.PUNISHER);
            if (jijiaList != null)
                list.AddRange(jijiaList);

            foreach (var item in list)
            {
                if (item.monsterData.team != team)
                {
                    if ((transform.position - item.transform.position).sqrMagnitude < EntityOfMonitor.Instance.entityEliteDamageDis*11)
                    {
                        //找到合适的攻击目标
                        realtarget = item.gameObject;
                        transform.LookAt((realtarget.transform.position + new Vector3(0, 300, 0)));
                        distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
                        iffind = false;
                        break;
                    }
                }
            }
        }
        //查找步兵 小兵 骑兵
        if (iffind)
        {
            var entityAnimation = monstermove.entityManager.GetComponentData<UnitSprint>(monstermove.tagentity);
            // if (entityManager.HasComponent<UnitSprint>(monstermove.tagentity))
            {
                if (monstermove.entityManager.Exists(entityAnimation.targetEntity))
                {
                    entity = entityAnimation.targetEntity;
                    var tran = monstermove.entityManager.GetComponentData<LocalToWorld>(entity);
                    distance = (new Vector3(tran.Position.x, 200 + tran.Position.y, tran.Position.z) - transform.position).magnitude;
                    transform.LookAt(new Vector3(tran.Position.x, 200 + tran.Position.y, tran.Position.z));
                    iffind = false;
                }
            }
        }
        //查找Boss
        if (iffind && GameObject.FindWithTag(targetboss) != null)
        {
            realtarget = GameObject.FindWithTag(targetboss);
            //-----
            //if ((transform.position - realtarget.transform.position).sqrMagnitude < mindistance)
            //{
            //    transform.LookAt((realtarget.transform.position + new Vector3(0, 200, 0)));
            //    distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
            //}//if enemy is boss,aiming at bosss position which add 200 m;
            //else
            //{
            //    transform.LookAt(realtarget.transform);//if enemy is boss,and the distance >20000,it would ues oringinal target
            //    distance = (realtarget.transform.position - transform.position).magnitude;
            //}
            transform.LookAt((realtarget.transform.position + new Vector3(0, 300, 0)));
            distance = (realtarget.transform.position + new Vector3(0, 300, 0) - transform.position).magnitude;
            ///---
            iffind = false;
        }

        ifParabola = true;
    }
    IEnumerator IEFind() //find enemy tarfet,then shoot them
    {
        Find();
          yield return new WaitForSeconds(0.1f);
        GetComponent<TrailRenderer>().emitting = true;
        yield return new WaitForSeconds(8);
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);//强制8秒后失活



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetenemy) || other.tag.Equals(targetboss) || other.tag.Equals("WILD") && !ifParabola)//找到目标之后碰撞
        {
            Attack attack = transform.GetComponent<Attack>();
            attack.AttackEffect();
        }
    }
    IEnumerator IEDisappear()
    {
        yield return new WaitForSeconds(0.1f);
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);//set itself disappear when it collsion other obj


    }
    void Parabola()//parabloa trajectory
    {

        if (ifParabola)
            transform.Translate(new Vector3(0, -0.7f * MathF.Sqrt(speed), 0.5f * speed) * Time.deltaTime, Space.Self);
        else
        {
            if (ifshoot)//2秒之后
            {
                //rigidbody0.velocity = Vector3.zero * 0;
                //var direction = (realtarget.transform.position - this.transform.position);
                //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
                //transform.Translate(new Vector3(0, -0.7f * MathF.Sqrt(speed), 1*speed) * Time.deltaTime, Space.Self);//sin 螺旋线运动
                // transform.Translate(new Vector3(rotateRadius*MathF.Cos(rotateRate*speed), rotateRadius_che*MathF.Sin(rotateRate * speed), 1 * speed) * Time.deltaTime, Space.Self);//sin 螺旋线运动
                transform.Translate(new Vector3(0.2f * rotateRadius * MathF.Sqrt(speed), -0.5f * MathF.Sqrt(speed), speed) * Time.deltaTime, Space.Self);

                rigidbody0.AddRelativeForce(transform.forward, ForceMode.Acceleration);//add an acceleration to it
                if (realtarget != null)
                {
                    sphereCollider.enabled = true;
                    transform.LookAt(realtarget.transform);
                }
                else 
                {
                    if (monstermove.entityManager.Exists(entity))
                    {
                        var tran = monstermove.entityManager.GetComponentData<LocalToWorld>(entity);
                        transform.LookAt(tran.Position);

                        if (Vector3.Distance((Vector3)tran.Position, transform.position) < 20)
                        {
                            entity = Entity.Null;

                            Attack attack = transform.GetComponent<Attack>();
                            attack.AttackEffect();
                        }
                    }
                    else 
                    {
                        if (count > 0)
                        {
                            count--;
                            var entityAnimation = monstermove.entityManager.GetComponentData<UnitSprint>(monstermove.tagentity);
                            if (monstermove.entityManager.Exists(entityAnimation.targetEntity))
                            {
                                entity = entityAnimation.targetEntity;
                            }
                        }
                        else 
                        {
                            Find();
                        }
                    }
                }
            }
        }
    }
    void SpeedChange()//the distance judge
    {
        speed += 50 * Time.deltaTime;
        if (ifParabola)
        {
            if (realtarget != null)
            {
                if (/*(realtarget.transform.position - transform.position).magnitude < 0.8f * distance ||*/ (realtarget.transform.position - transform.position).magnitude > /*2f **/ distance)
                {

                    ifParabola = false;
                    ifshoot = true;
                }
            }
            else 
            {
                if (monstermove.entityManager.Exists(entity))
                {
                    var tran = monstermove.entityManager.GetComponentData<LocalToWorld>(entity);
                   
                    if (/*(Vector3)tran.Position - transform.position).magnitude < 0.8f * distance ||*/ ((Vector3)tran.Position - transform.position).magnitude > distance)
                    {

                        ifParabola = false;
                        ifshoot = true;
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        count--;
                        var entityAnimation = monstermove.entityManager.GetComponentData<UnitSprint>(monstermove.tagentity);
                        if (monstermove.entityManager.Exists(entityAnimation.targetEntity))
                        {
                            entity = entityAnimation.targetEntity;
                        }
                    }
                    else
                    {
                        Find();
                    }
                }
            }
        }
        if (transform.position.y < 0)
            Netpool.Getinstance().Pushobject(name, gameObject);

    }
}
