using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��Ҫ����ؼ���
/// </summary>
///

public class Pool : NoneMonoinstance<Pool>//�̳е���ģʽ��MOno����
{
    public Dictionary<string, List<GameObject>> pooldic = new Dictionary<string, List<GameObject>>();
    public GameObject Rootpoll;
    public GameObject Insgameobj(string name)//������������س�ʼ��
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
            gameObject0.name = name;//�Ѷ������ָ�Ϊ����س�������
            if (Rootpoll == null)//������������ ��Ϊ��¡��ڵ�
                Rootpoll = new GameObject("Rootpoll");
            gameObject0.transform.parent = Rootpoll.transform;
        }
        gameObject0.SetActive(true);
        return gameObject0;
    }

    public GameObject Insgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent)//��ͨ��������س�ʼ��
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0)//���������г������λ�ù�����仯λ�ü���
        {
            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;
            foreach (var item in pooldic)
            {

                Debug.Log("��λ��/����" + item.Key + "-----" + item.Value.Count);

            }
        }
        else//��������û�г������λ�ò�����������
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.name = obj.name;//�Ѷ������ָ�Ϊ����س�������
            if (pooldic.ContainsKey(obj.name))
            {
                pooldic[obj.name].Add(gameObject0);
            }
            else
            {
                pooldic.Add(obj.name, new List<GameObject>() { gameObject0 });
                Debug.Log("�����³���");
            }
            try
            { Debug.Log("û��λ��/����" + pooldic[obj.name] + pooldic[obj.name].Count); }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        gameObject0.SetActive(true);
        return gameObject0;

    }

    public GameObject NetInsgameobj(GameObject obj, Vector3 vector, Quaternion quaternion, Transform parent)//���絥������س�ʼ��
    {
        GameObject gameObject0 = null;
        if (pooldic.ContainsKey(obj.name) && pooldic[obj.name].Count > 0)//���������г������λ�ù�����仯λ�ü���
        {
            gameObject0 = pooldic[obj.name][0];
            pooldic[obj.name].RemoveAt(0);
            gameObject0.transform.position = vector;
            gameObject0.transform.rotation = quaternion;
            foreach (var item in pooldic)
            {

                Debug.Log("��λ��/����" + item.Key + "-----" + item.Value.Count);

            }
        }
        else//��������û�г������λ�ò�����������
        {
            gameObject0 = GameObject.Instantiate(obj, vector, quaternion, parent);
            gameObject0.name = obj.name;//�Ѷ������ָ�Ϊ����س�������
            if (pooldic.ContainsKey(obj.name))
            {
                pooldic[obj.name].Add(gameObject0);
            }
            else
            {
                pooldic.Add(obj.name, new List<GameObject>() { gameObject0 });
                Debug.Log("�����³���");
            }
            try
            { Debug.Log("û��λ��/����" + pooldic[obj.name] + pooldic[obj.name].Count); }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        gameObject0.SetActive(true);
        //if (!gameObject0.GetComponent<NetworkObject>().IsSpawned)
        //{
        //    gameObject0.GetComponent<NetworkObject>().Spawn();
        //    gameObject0.GetComponent<NetworkObject>().TrySetParent(parent);
        //}
        return gameObject0;

    }
    /// <summary>
    /// �Ŷ�����ʧ��
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void Pushobject(string name, GameObject obj)
    {
        // obj.GetComponent<NetworkObject>().Despawn();
        obj.SetActive(false);//ʧ��
        if (pooldic.ContainsKey(name))
        {
            pooldic[name].Add(obj);
        }
        else
        {
            pooldic.Add(name, new List<GameObject>() { obj });//�������obj gameobject
        }
    }

    public void NetPushobject(string name, GameObject obj)
    {
        obj.SetActive(false);//ʧ��
        if (pooldic.ContainsKey(name))
        {
            pooldic[name].Add(obj);
        }
        else
        {
            pooldic.Add(name, new List<GameObject>() { obj });//�������obj gameobject
        }




    }
    public void Clearpoll()//��������
    {

        pooldic.Clear();
        Rootpoll = null;



    }
}

