using com.unity.mgobe.src.Matcher;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class FindEnemy : MonoBehaviour
{
    private Rigidbody rigidbody0;
    public string targetboss;
    public string targetenemy;
    public string targetenemyMecha;
    public string mytag;
    public float speed = 100;
    public float speedcache;
    public float readytime = 2;
    private bool iffind = true;
    public GameObject realtarget = null;
    private GameObject cachetarget = null;
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
   // private bool ifParabola_x = false;
    void Start()
    {
        //if (FWD != null)
        //    FWD = GameObject.FindWithTag("VFX");
        

    }
    private void OnEnable()
    {
        rigidbody0 = this.GetComponent<Rigidbody>();
        GetComponent<TrailRenderer>().emitting = false;
        iffind = true;
        islookat = true;
        ifParabola = false;
        this.tag = "Untagged";
        ifshoot = false;
        rigidbody0.velocity = Vector3.zero * 0;
       if(GameManager.instance.isGameFighting)//the judge for wjy code
        StartCoroutine("IEFind");
        //---velocity module
        speed = speedcache;
        rotateRadius = Random.Range(0, 1f) > 0.5f ? Random.Range(0.2f, 1f) * 100: -Random.Range(0.2f, 1f) * 100;
        rotateRadius_che= Random.Range(0, 1f) > 0.5f ? Random.Range(0.2f, 1f) * 100 : -Random.Range(0.2f, 1f) * 100;
        rotateRate = Random.Range(0.2f, 0.7f);
        //-- velocity module
        cachetarget = null;//reset the value of cachetarget
    }
    private void OnDisable()
    {
        StopCoroutine("IEFind");
        GetComponent<TrailRenderer>().emitting = false;
        //---速度为零
        rigidbody0.velocity = Vector3.zero * 0;
        transform.rotation = quaternion.identity;
        transform.position = new Vector3(0, 0, 0);
        realtarget = null;
        // Debug.Log("cloese bullet---------------------------------------------------------------------------------");
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameFighting)
        {
            Parabola();
            SpeedChange();//the change of speed that mecha provide;
        }
    }

    IEnumerator IEFind() //find enemy tarfet,then shoot them
    {
        this.tag = mytag;
        rigidbody0.velocity=new Vector3(Random.Range(0,0.4f),1,Random.Range(0,0.4f))*75 ;//开始增加一个向上速度
        //yield return new WaitForSeconds(1);
        //rigidbody0.velocity = Vector3.up * 10;
        //---加速减速零
        if (GameObject.FindWithTag("WILD") != null && iffind)
        {
            GameObject target = null;
            float min;
            GameObject[] targets = GameObject.FindGameObjectsWithTag("WILD");
            min = Vector3.Distance(targets[0].transform.position, gameObject.transform.position);
            target = targets[0].gameObject;
            cachetarget = target;
            for (int i = 0; i < targets.Length; i++)
            {
                if (min > Vector3.Distance(targets[i].transform.position, this.transform.position))
                {
                    min = Vector3.Distance(targets[i].transform.position, this.transform.position);
                    target = targets[i].gameObject;
                    cachetarget = target;//cache a target for wild
                }
            }
        }

        if (GameObject.FindWithTag("WILD") != null && iffind && cachetarget != null && (transform.position - cachetarget.transform.position).sqrMagnitude < mindistance)
        {


            realtarget = cachetarget;
            transform.LookAt((realtarget.transform.position + new Vector3(0, 200, 0)));
            distance = (realtarget.transform.position - transform.position).magnitude;
            iffind = false;

        }
        else if (GameObject.FindWithTag(targetenemyMecha) != null && iffind)//this is the target of mecha preferential
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetenemyMecha);
            realtarget = targets[Random.Range(0, targets.Length)];
            if ((transform.position - realtarget.transform.position).sqrMagnitude < mindistance)
            {
                transform.LookAt((realtarget.transform.position + new Vector3(0, 200, 0)));
                distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
            }//if enemy is boss,aiming at bosss position which add 500 m;
            else
            {
                transform.LookAt(realtarget.transform);//if enemy is boss,and the distance >20000,it would ues oringinal target
                distance = (realtarget.transform.position - transform.position).magnitude;
            }
            iffind = false;
        }
        else if (GameObject.FindWithTag(targetenemy) != null && iffind)
        {

            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetenemy);
            realtarget = targets[Random.Range(0, targets.Length)];
            if ((transform.position - realtarget.transform.position).sqrMagnitude < mindistance)
            {
                transform.LookAt((realtarget.transform.position + new Vector3(0, 200, 0)));
                distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
            }//if enemy is boss,aiming at bosss position which add 500 m;
            else
            {
                transform.LookAt(realtarget.transform);//if enemy is boss,and the distance >20000,it would ues oringinal target
                distance = (realtarget.transform.position - transform.position).magnitude;
            }
            iffind = false;
        }
        else
           if (GameObject.FindWithTag(targetboss) != null && iffind)
        {


            realtarget = GameObject.FindWithTag(targetboss);
            //-----
            if ((transform.position - realtarget.transform.position).sqrMagnitude < mindistance)
            {
                transform.LookAt((realtarget.transform.position + new Vector3(0, 200, 0)));
                distance = (realtarget.transform.position + new Vector3(0, 200, 0) - transform.position).magnitude;
            }//if enemy is boss,aiming at bosss position which add 200 m;
            else
            {
                transform.LookAt(realtarget.transform);//if enemy is boss,and the distance >20000,it would ues oringinal target
                distance = (realtarget.transform.position - transform.position).magnitude;
            }
            ///---
            iffind = false;

        }
        ifParabola = true;
        yield return new WaitForSeconds(0.1f);
        GetComponent<TrailRenderer>().emitting = true;
        yield return new WaitForSeconds(8);
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);//强制8秒后失活



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetenemy) || other.tag.Equals(targetboss)||other.tag.Equals("WILD") && !ifParabola)//找到目标之后碰撞
        {
            StartCoroutine("IEDisappear");
        }
    }
    IEnumerator IEDisappear()
    {
        yield return new WaitForSeconds(0.1f);
        Netpool.Getinstance().Pushobject(this.gameObject.name, this.gameObject);//set itself disappear when it collsion other obj
        

    }

    void Parabola()//parabloa trajectory
    {
   
       if(ifParabola)
            transform.Translate(new Vector3(0,-0.7f*MathF.Sqrt(speed),0.5f*speed ) * Time.deltaTime, Space.Self);
        else
              if (ifshoot)//2秒之后
        {
            //rigidbody0.velocity = Vector3.zero * 0;
            //var direction = (realtarget.transform.position - this.transform.position);
            //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            //transform.Translate(new Vector3(0, -0.7f * MathF.Sqrt(speed), 1*speed) * Time.deltaTime, Space.Self);//sin 螺旋线运动
            // transform.Translate(new Vector3(rotateRadius*MathF.Cos(rotateRate*speed), rotateRadius_che*MathF.Sin(rotateRate * speed), 1 * speed) * Time.deltaTime, Space.Self);//sin 螺旋线运动
            transform.Translate(new Vector3(0.2f*rotateRadius*MathF.Sqrt(speed), -0.5f *  MathF.Sqrt(speed), speed) * Time.deltaTime, Space.Self);
          
            transform.LookAt(realtarget.transform);
            
            rigidbody0.AddRelativeForce(transform.forward, ForceMode.Acceleration);//add an acceleration to it
        }


    }
    void SpeedChange()//the distance judge
    {
        speed += 50*Time.deltaTime;
        if (ifParabola)
        { if ((realtarget.transform.position - transform.position).magnitude < 0.8f * distance|| (realtarget.transform.position - transform.position).magnitude > 2f * distance)
            {

                ifParabola = false;
                ifshoot = true;        
            }
        }
        if (transform.position.y < 0)
            Netpool.Getinstance().Pushobject(name, gameObject);

    }
}
