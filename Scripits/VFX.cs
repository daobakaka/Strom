using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    public Transform mother;
    public float duration = 0.5f;
    public bool ifVFX = false;
  //  public bool[] bools;
    void Start()
    {
       // Cheakact();//判断使用哪个组件
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        if (ifVFX)
        {
            mother = GameObject.FindWithTag("VFX").transform;
           // Cheakact();
            StartCoroutine("IEpriticle");
        }
    }
    IEnumerator IEpriticle()
    {

        yield return new WaitForSeconds(duration);

        Netpool.Getinstance().Pushobject(this.name, gameObject);
    }
    //void Cheakact()
    //{
    //    transform.GetChild(0).gameObject.SetActive(bools[0]);
    //    transform.GetChild(1).gameObject.SetActive(bools[1]);
    //    transform.GetChild(2).gameObject.SetActive(bools[2]);
    
    //}
}
