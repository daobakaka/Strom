using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Games.Characters.UI;

public class RaybossRoot : MonoBehaviour
{
    public string enemyfire;
    public string mytag;
    public bool isICEboss;
    private bool ifattack = true;
    public bool ifIntegral;
    public Transform mybody;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag.Equals(enemyfire)&&ifattack)
        {
            if (other.gameObject.TryGetComponent<Attack>(out Attack component))
            {
               if(ifIntegral)
                Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral +=2* component.attacktimes;//if attack boss,the integral times two
                if (component.body.GetComponent<Monstermove>().showtarget.tag == mytag)
                {
                    if (isICEboss)
                        Rayboss.ICEbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;//boss health reduce
                    else
                        Rayboss.FIREbossHP -= component.attacktimes * Monsterins.protectBossFIRE_assist;
                }
            }
            mybody.GetComponent<Animator>().SetTrigger("Attack");
            ifattack = false;
           // Debug.Log("damaged by enemy");
        }
        if (other.tag.Equals("WILDATC"))
        {
            if (other.gameObject.TryGetComponent<Attack>(out Attack component))
            {
                if (ifIntegral)
                    Netpool.Getinstance().monsterStruct[component.monsterData.uid].monsterIntegral += 2 * component.attacktimes;//if attack boss,the integral times two

                if (isICEboss)
                    Rayboss.ICEbossHP -= component.attacktimes * Monsterins.protectBossICE_assist;//boss health reduce
                else
                    Rayboss.FIREbossHP -= component.attacktimes * Monsterins.protectBossFIRE_assist;
            }
            mybody.GetComponent<Animator>().SetTrigger("Attack");
            ifattack = false;
            
        }
    }
   
    /// <summary>
    /// hurt lock
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if ((other.tag.Equals(enemyfire)||other.tag.Equals("WILDATC") && !ifattack))
        {

            ifattack = true;
        }
    }

}
