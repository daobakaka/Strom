using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AppearAnim : MonoBehaviour
{
    public Entity entity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BaZhu_AnimPlayisOver()
    {
        //�������
        var entiMager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var appear = entiMager.GetComponentData<Appear>(entity);
        appear.IsOver_AppearAnimPlay = true;
        //���Լ�������λ�ô��ݸ����entityʿ��
        appear.AppearPos = transform.position;
        appear.AppearRot = transform.rotation;
        entiMager.SetComponentData(entity, appear);

    }
    public void DestroyObj()
    {
        //ɾ���Լ�(��������obj)
        GameObject.Destroy(gameObject);
    }
}
