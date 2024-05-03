using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AppearManager : MonoBehaviour//��������
{
    private static AppearManager _AppearManager;
    public static AppearManager instance { get { return _AppearManager; } }


    [Header("������Ч")]
    [Tooltip("����������Ч")]public GameObject BaZhuAppearAnim;



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
    //�����ڶ��ֳ�����ʽ
    private void BaZhuAppear2(Entity entity,EntityManager entiMager, float3 Pos, quaternion Rot)
    {
        //ʵ���������������
        var bazhuapperaAnim = GameObject.Instantiate(BaZhuAppearAnim);
        Pos.y += 10;
        bazhuapperaAnim.transform.position = Pos;
        bazhuapperaAnim.transform.rotation = Rot;
        //�����ʿ��Etity������������obj
        var appearAnim = bazhuapperaAnim.GetComponent<AppearAnim>();
        if (appearAnim == null) return;
        appearAnim.entity = entity;
        //��ʵ�����İ������������ȳ���������������ٳ���
        var entiTransform = entiMager.GetComponentData<LocalTransform>(entity);
        entiTransform.Position.y = -100;
        entiMager.SetComponentData(entity, entiTransform);
    }



}
