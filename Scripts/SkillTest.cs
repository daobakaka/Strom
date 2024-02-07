using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] vfx;
    public Vector3 offset;
    public Vector3 angle_offset;
    public Transform mother;
    public Vector3 random;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {

            InsSkill();


        }
    }

    void InsSkill()
    {



        GameObject clone =Netpool.Getinstance().Insgameobj(vfx[0], transform.position + offset+new Vector3(Random.Range(-100,100)*random.x,Random.Range(-100,100)*random.y,Random.Range(-100,100)*random.z), Quaternion.Euler(angle_offset.x,angle_offset.y,angle_offset.z),mother);

    }
}
