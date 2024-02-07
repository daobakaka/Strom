using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;
/// <summary>
/// 网络缓存池
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
            //Debug.Log("结束");
           
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
            //Debug.Log("结束");

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
            //Debug.Log("结束");

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
    /// 增加ID的重载方法
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
        //-----------从生成的物体身上传入ID值-------------------------------------------
        if (monsterStruct.ContainsKey(ID))//如果已包含该玩家
        { monsterStruct[ID].monsterIntegral += 5;
            monsterStruct[ID].num++;
        }//召唤怪物加5积分,玩家召唤怪物增加一次num值
        else//如果未包含玩家，则为玩家创建新ID，且传入monsterStruct中。
        {
            monsterStruct.Add(ID, new MonsterStruct { ID = ID,num =1}); //没有ID，则为字典增加ID对象,为num取值为1

        }
        gameObject0.TryGetComponent<Monstermove>(out Monstermove component);//取出该obj的ID值并为ID赋值，对应
        if (component.ifIntegral == true)//是否开启积分池开关
        {
            component.playerID = ID;//为怪物打标签
            component.IDtransform.TryGetComponent<Attack>(out Attack attack);//取出关联攻击力的伤害对象
            attack.ID = ID;//为怪物攻击力打标签
            attack.monsterType = monsterType;//为怪物种类打标签
                           //----------------------------------------------------新增积分池系统
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

                Debug.Log("有位置/抽屉" + item.Key + "-----" + item.Value.Count);

            }
        }
        else//
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
          
            if (pooldic.ContainsKey(obj.name + "(Clone)"))
            {
                pooldic[obj.name + "(Clone)"].Add(obj);
                Debug.Log("位置不够创造新位置");
            }
            else
            {
                pooldic.Add(obj.name + "(Clone)", new List<GameObject>() { gameObject0 });
                try
                {
                    foreach (var item in pooldic)
                    {

                        Debug.Log("抽屉不够,创造新抽屉当前" + item.Key + "++++++++" + item.Value.Count);

                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        Debug.Log("服务器调动方法设置物体状态");//------非网络版本失活
        return gameObject0;
    }
    /// <summary>
    /// 放东西，失活
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
            obj.SetActive(booler);//失活
    
        if (pooldic.ContainsKey(obj.name))
        {
            pooldic[obj.name].Add(obj);
            
        }
        else
        {
            pooldic.Add(obj.name, new List<GameObject>() { obj });
        
            
        }


    }
    public void Clearpoll()//过场清算
    {

        pooldic.Clear();
        monsterStruct.Clear();
        Rootpoll = null;



    }
}
