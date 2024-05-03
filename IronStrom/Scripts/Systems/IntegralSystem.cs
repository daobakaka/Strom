using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class IntegralSystem : SystemBase//����ϵͳ
{
    ComponentLookup<Integral> m_Integral;


    void UpDataComponentLookup()
    {
        m_Integral.Update(this);
    }

    protected override void OnCreate()
    {
        m_Integral = GetComponentLookup<Integral>(true);
    }
    protected override void OnUpdate()
    {
        UpDataComponentLookup();
        //ÿ��ʿ���ѻ����Ͻ��������ڵ����
        HandInIntegral();

    }

    //ÿ��ʿ���ѻ����Ͻ��������ڵ����
    void HandInIntegral()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        TeamIntegral(ref teamMager._Dic_Team1);
        TeamIntegral(ref teamMager._Dic_Team2);
    }
    //��ȡ����
    void TeamIntegral(ref Dictionary<string, PlayerData> team)
    {
        foreach (KeyValuePair<string, PlayerData> pair in team)
        {
            foreach (Entity enti in pair.Value.m_GiftShiBingList)
            {
                if (EntityManager.HasComponent<Integral>(enti))
                {
                    var shibingInte = EntityManager.GetComponentData<Integral>(enti);
                    pair.Value.m_ATScore += shibingInte.ATIntegral;
                    shibingInte.ATIntegral = 0;
                    EntityManager.SetComponentData(enti, shibingInte);

                }
            }
            foreach (Entity enti in pair.Value.m_LikeShiBingList)
            {
                if (EntityManager.HasComponent<Integral>(enti))
                {
                    var shibingInte = EntityManager.GetComponentData<Integral>(enti);
                    pair.Value.m_ATScore += shibingInte.ATIntegral;
                    shibingInte.ATIntegral = 0;
                    EntityManager.SetComponentData(enti, shibingInte);

                }
            }

            //Debug.Log($"  ���{pair.Value.m_Nick}. �Ĺ�������Ϊ��{pair.Value.m_ATScore}. �������Ϊ��{pair.Value.m_GiftScore}.");
        }
    }



}
