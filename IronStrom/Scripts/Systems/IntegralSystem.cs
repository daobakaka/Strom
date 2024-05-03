using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class IntegralSystem : SystemBase//积分系统
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
        //每个士兵把积分上交给隶属于的玩家
        HandInIntegral();

    }

    //每个士兵把积分上交给隶属于的玩家
    void HandInIntegral()
    {
        var teamMager = TeamManager.teamManager;
        if (teamMager == null) return;
        TeamIntegral(ref teamMager._Dic_Team1);
        TeamIntegral(ref teamMager._Dic_Team2);
    }
    //获取积分
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

            //Debug.Log($"  玩家{pair.Value.m_Nick}. 的攻击积分为：{pair.Value.m_ATScore}. 礼物积分为：{pair.Value.m_GiftScore}.");
        }
    }



}
