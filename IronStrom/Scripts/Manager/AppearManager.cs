using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AppearManager : MonoBehaviour//出场管理
{
    private static AppearManager _AppearManager;
    public static AppearManager instance { get { return _AppearManager; } }


    [Header("出场特效")]
    [Tooltip("霸主出场特效")]public GameObject BaZhuAppearAnim;



    private void Awake()
    {
        _AppearManager = this;


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunAppear(Entity entity, float3 Pos, quaternion Rot)
    {
        var entiMager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var appear = entiMager.GetComponentData<Appear>(entity);
        switch(appear.appearName)
        {
            case AppearName.BaZhuApper2:BaZhuAppear2(entity, entiMager, Pos, Rot); break;

        }

    }
    //霸主第二种出场方式
    private void BaZhuAppear2(Entity entity,EntityManager entiMager, float3 Pos, quaternion Rot)
    {
        //实例化这个出场动画
        var bazhuapperaAnim = GameObject.Instantiate(BaZhuAppearAnim);
        Pos.y += 10;
        bazhuapperaAnim.transform.position = Pos;
        bazhuapperaAnim.transform.rotation = Rot;
        //将这个士兵Etity给到出场动画obj
        var appearAnim = bazhuapperaAnim.GetComponent<AppearAnim>();
        if (appearAnim == null) return;
        appearAnim.entity = entity;
        //将实例化的霸主藏起来，等出场动画播放完后再出现
        var entiTransform = entiMager.GetComponentData<LocalTransform>(entity);
        entiTransform.Position.y = -100;
        entiMager.SetComponentData(entity, entiTransform);
    }



}
