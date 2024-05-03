using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotateSpeed = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        gameObject.transform.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);
    }
}
