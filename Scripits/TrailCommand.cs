using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCommand : MonoBehaviour
{
    // Start is called before the first frame update
    public float TrailPI = 0.5f;
    public Transform TrailTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// ÍÏÎ²¹ì¼£¶¯»­¿ØÖÆ
    /// </summary>
    /// <param name="num"></param>
    public void ToogleTrail(int num)
    {
        switch (num)
        {
            case 1:
                if (Random.Range(0, 1f) < TrailPI)
                    Debug.Log("kill");
               TrailTransform.GetComponent<TrailRenderer>().emitting = true;
                break;
            case 2:
                Debug.Log("dontkill");
                TrailTransform.GetComponent<TrailRenderer>().emitting = false;
                break;
        }
    }
}
