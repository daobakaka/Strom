using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJAttack : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// loop time,the bullet numbers of shoot,the piont which parent get buffers
    /// 
    /// </summary>
    public float loop = 3;
    public GameObject bullet;
    public int num = 3;
    private GameObject mother;
    private GameObject vfx;
    void Start()
    {
        vfx = GameObject.FindWithTag("VFX");
       

    }
    private void OnEnable()
    {
        //mother = GameObject.FindWithTag("MOTHER");
        StartCoroutine("IEShoot");
       
    }
    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator IEShoot()//shoot mode
    {
        WaitForSeconds p = new WaitForSeconds(loop);
        for (; ; )
        {
            yield return p;
            for (int i = 0; i < num; i++)
            {
                Netpool.Getinstance().Insgameobj(bullet, transform.position, Quaternion.identity, vfx.transform);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    private void OnDisable()
    {
        StopCoroutine("IEShoot");
    }
}
