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
        //播放完毕
        var entiMager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var appear = entiMager.GetComponentData<Appear>(entity);
        appear.IsOver_AppearAnimPlay = true;
        //将自己的最终位置传递给这个entity士兵
        appear.AppearPos = transform.position;
        appear.AppearRot = transform.rotation;
        entiMager.SetComponentData(entity, appear);

    }
    public void DestroyObj()
    {
        //删除自己(出场动画obj)
        GameObject.Destroy(gameObject);
    }
}
