using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSskill : MonoBehaviour
{
    // Start is called before the first frame update
    public float duration = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
