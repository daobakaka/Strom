using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamrtaMove : MonoBehaviour
{
    private Rigidbody rigidbody0;
    public float speed = 10;
    public float rotatesp = 10;
    float horizontal;
    float vertical;
    private Vector3 position;
    public static bool shoot;
    void Start()
    {
        rigidbody0 = GetComponent<Rigidbody>();
        //StartCoroutine("IETestForDelegate");
        transform.position = new Vector3(-32.6f, 3.6f, -62.4f);
        transform.rotation = Quaternion.Euler(0, 41, 0);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.S))
            transform.position += -transform.forward * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += new Vector3(0,  speed * Time.deltaTime,0);
        }
        if (Input.GetKey(KeyCode.X))
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        }
        // rigidbody0.MovePosition(position);
        if (Input.GetKey(KeyCode.Q))
        {
            //  transform.localEulerAngles += new Vector3(0, rotatesp, 0);
            this.transform.rotation *= Quaternion.Euler(0, rotatesp, 0);


        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.rotation *= Quaternion.Euler(0, -rotatesp, 0);
        }
        if (Input.GetKey(KeyCode.R))
        {
            transform.rotation *= Quaternion.Euler(-rotatesp, 0, 0);

        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.rotation *= Quaternion.Euler(rotatesp, 0, 0);
        }
       // this.transform.position += new Vector3(position.x, position.y, position.z);

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    shoot = true;

        //}
        //if (Input.GetKeyUp(KeyCode.T))
        //{

        //    shoot = false;

        //}

    }

}

