using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;
/// <summary>
/// ���绺���
/// </summary>
///

public class Netpool : NoneMonoinstance<Netpool>
{
    public Dictionary<string, List<GameObject>> pooldic = new Dictionary<string, List<GameObject>>();
    public GameObject Rootpoll;
    public Dictionary<string,MonsterStruct> monsterStruct= new Dictionary<string,MonsterStruct>();//the pool of integral
    public GameObject Insgameobj(string name)

   
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(name) && pooldic[name].Count > 0)
        {
            gameObject0 = pooldic[name][0];
            pooldic[name].RemoveAt(0);
        }
        else
        {
            gameObject0 = GameObject.Instantiate(Resources.Load<GameObject>(name));
            gameObject0.name = name;//
            if (Rootpoll == null)//
                Rootpoll = new GameObject("Rootpoll");
            gameObject0.transform.parent = Rootpoll.transform;
        }
        gameObject0.SetActive(true);
        return gameObject0;
    }

    public GameObject Insgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent)//
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0)//
        {
         
            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;
            //Debug.Log("����");
           
        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.name = obj.name;
        
        }
        try
        {
            gameObject0.SetActive(true);
         
        }
        catch (Exception e)
        { Debug.Log(e); }
        return gameObject0;

    }
    public GameObject Insgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent,bool ts)//
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0)//
        {

            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            // gameObject0.transform.position = vector;
            if (ts)
                gameObject0.transform.position = new Vector3(vector.x, 7, vector.z);
            gameObject0.transform.rotation = quaternion;
            //Debug.Log("����");

        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.name = obj.name;

        }
        try
        {
            gameObject0.SetActive(true);

        }
        catch (Exception e)
        { Debug.Log(e); }
        return gameObject0;

    }
    /// <summary>
    /// add a method for change the scale
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="vector"></param>
    /// <param name="quaternion"></param>
    /// <param name="parent"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public GameObject Insgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent,float scale)//
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0)//
        {

            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;
            gameObject0.transform.localScale = new Vector3(1, 1, 1);//recover the scale of the object;
            gameObject0.transform.localScale *= scale;//pss the parameter to object
            //Debug.Log("����");

        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.transform.localScale *=scale;//pss the parameter to object
            gameObject0.name = obj.name;

        }
        try
        {
            gameObject0.SetActive(true);

        }
        catch (Exception e)
        { Debug.Log(e); }
        return gameObject0;

    }

    /// <summary>
    /// ����ID�����ط���
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="vector"></param>
    /// <param name="quaternion"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Insgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent, string ID,MonsterType monsterType)//
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0&& pooldic[obj.name][0].activeSelf == false)//
        {
            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;

            
        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.name = obj.name;
           
        }
            try
        {
            gameObject0.SetActive(true);
          
        }

        catch (Exception e)
        { Debug.Log(e); }
        //-----------�����ɵ��������ϴ���IDֵ-------------------------------------------
        if (monsterStruct.ContainsKey(ID))//����Ѱ��������
        { monsterStruct[ID].monsterIntegral += 5;
            monsterStruct[ID].num++;
        }//�ٻ������5����,����ٻ���������һ��numֵ
        else//���δ������ң���Ϊ��Ҵ�����ID���Ҵ���monsterStruct�С�
        {
            monsterStruct.Add(ID, new MonsterStruct { ID = ID,num =1}); //û��ID����Ϊ�ֵ�����ID����,ΪnumȡֵΪ1

        }
        gameObject0.TryGetComponent<Monstermove>(out Monstermove component);//ȡ����obj��IDֵ��ΪID��ֵ����Ӧ
        if (component.ifIntegral == true)//�Ƿ������ֳؿ���
        {
            component.playerID = ID;//Ϊ������ǩ
            component.IDtransform.TryGetComponent<Attack>(out Attack attack);//ȡ���������������˺�����
            attack.ID = ID;//Ϊ���﹥�������ǩ
            attack.monsterType = monsterType;//Ϊ����������ǩ
                           //----------------------------------------------------�������ֳ�ϵͳ
        }
        return gameObject0;

    }
    public GameObject NetInsgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent)//
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name + "(Clone)") && pooldic[obj.name + "(Clone)"].Count > 0)//
        {
            gameObject0 = pooldic[obj.name + "(Clone)"][0];
            pooldic[obj.name + "(Clone)"].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;
            foreach (var item in pooldic)
            {

                Debug.Log("��λ��/����" + item.Key + "-----" + item.Value.Count);

            }
        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
          
            if (pooldic.ContainsKey(obj.name + "(Clone)"))
            {
                pooldic[obj.name + "(Clone)"].Add(obj);
                Debug.Log("λ�ò���������λ��");
            }
            else
            {
                pooldic.Add(obj.name + "(Clone)", new List<GameObject>() { gameObject0 });
                try
                {
                    foreach (var item in pooldic)
                    {

                        Debug.Log("���벻��,�����³��뵱ǰ" + item.Key + "++++++++" + item.Value.Count);

                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        Debug.Log("����������������������״̬");//------������汾ʧ��
        return gameObject0;
    }
    /// <summary>
    /// �Ŷ�����ʧ��
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void Pushobject(string name, GameObject obj)
    {

        if (pooldic.ContainsKey(name))
        {
            pooldic[name].Add(obj);
          
        }
        else
        {
         
            pooldic.Add(name, new List<GameObject>() { obj });
    
        }
        if (obj.activeSelf)
            obj.SetActive(false);//
    }

    public void NetPushobject(bool booler, GameObject obj)
    {


        if (obj.activeSelf)
            obj.SetActive(booler);//ʧ��
    
        if (pooldic.ContainsKey(obj.name))
        {
            pooldic[obj.name].Add(obj);
            
        }
        else
        {
            pooldic.Add(obj.name, new List<GameObject>() { obj });
        
            
        }


    }
    public void Clearpoll()//��������
    {

        pooldic.Clear();
        monsterStruct.Clear();
        Rootpoll = null;



    }
}
