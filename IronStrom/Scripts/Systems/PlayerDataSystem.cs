using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


public partial class PlayerDataSystem : SystemBase
{
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocalToWorld;
    ComponentLookup<ShiBing> m_ShiBing;
    List<Entity> destroyEnti;
    void UpDataComponentLookup()
    {
        m_transform.Update(this);
        m_LocalToWorld.Update(this);
        m_ShiBing.Update(this);
    }

    protected override void OnCreate()
    {

        m_transform = GetComponentLookup<LocalTransform>(true);
        m_LocalToWorld = GetComponentLookup<LocalToWorld>(true);
        m_ShiBing = GetComponentLookup<ShiBing>(true);
    }
    protected override void OnUpdate()
    {
        var EntiUIManger = EntityUIManager.Instance;
        var teamManger = TeamManager.teamManager;
        if (EntiUIManger == null || teamManger == null)
            return;
        UpDataComponentLookup();

        //ˢ�¶�Ӧͷ���ֵ䣬ȥ������ֵ�ļ�
        UpdataAvatarName(ref EntiUIManger);
        //ˢ�¶�Ӧ����UI���ֵ䣬ȥ�������ļ�
        UpdataShieldUI(ref EntiUIManger);
        //ˢ�����������ֵ����ÿһ��������µ�ʿ����ȥ�������ĵ�λ
        UpdataTeamDic(ref teamManger);

        //ÿ����AvatarName�����Entityʿ�����Լ�ͬ���Լ�ͷ���λ��
        Entities.ForEach((Entity entity, AvatarName avaname) =>
        {
            if (!EntiUIManger.AvatarNameDic.ContainsKey(entity)||
                !EntityManager.HasComponent<LocalTransform>(entity) ||
                !EntityManager.Exists(entity))
                return;

            var avatarnamePoint = m_ShiBing[entity].AvatarNamePoint;
            if (!EntityManager.Exists(avatarnamePoint))
            {
                avatarnamePoint = entity;
            }
            var avatarnamePointPos = m_LocalToWorld[avatarnamePoint].Position;
            //ͬ��ͷ��λ��
            EntiUIManger.AvatarSynEntityPos(entity, avatarnamePointPos);
            //ͬ��HPSlider
            EntiUIManger.SynEntityShiBingHP(in entity, EntityManager);
            //ͬ���߷�Slider
            EntiUIManger.SynMutinySlider(entity, EntityManager);

        }).WithBurst().WithStructuralChanges().Run();


        //ÿ����ShieldUI�����Entity���Լ�ͬ���Լ��ͻ���UI��λ��
        Entities.ForEach((Entity entity, Shield shield, ShieldUI shieldUI) =>
        {
            if (!EntiUIManger.ShieldSliderDic.ContainsKey(entity) ||
               !EntityManager.Exists(entity))
                return;

            var shieldPos = m_LocalToWorld[entity].Position;
            shieldPos.y += shield.ShieldScale / 2;

            //ͬ������UI
            EntiUIManger.SynShieldUI(entity, shieldPos, EntityManager);

        }).WithBurst().WithStructuralChanges().Run();

    }

    //ˢ�¶�Ӧͷ���ֵ䣬ȥ������ֵ�ļ�
    void UpdataAvatarName(ref EntityUIManager EntiUIManger)
    {
        destroyEnti = new List<Entity>();
        //���Entityʿ������ɾ����Ӧ��ͷ��
        foreach (KeyValuePair<Entity, GameObject> pair in EntiUIManger.AvatarNameDic)
        {
            if (!EntityManager.Exists(pair.Key))
            {
                GameObject.Destroy(pair.Value);
                destroyEnti.Add(pair.Key);
                //EntityUIManager.Instance.AvatarNameDic.Remove(pair.Key);
            }

            //�Ƿ���ʾͷ������
            if(EntityUIManager.Instance.Is_DisplayAvatarName)
                pair.Value.SetActive(true);
            else
                pair.Value.SetActive(false);
        }
        foreach (Entity enit in destroyEnti)
        {
            EntiUIManger.AvatarNameDic.Remove(enit);
        }
        destroyEnti.Clear();
    }
    //ˢ�¶�Ӧ����UI���ֵ䣬ȥ�������ļ�
    void UpdataShieldUI(ref EntityUIManager EntiUIManger)
    {
        destroyEnti = new List<Entity>();
        foreach (KeyValuePair<Entity, GameObject> pair in EntiUIManger.ShieldSliderDic)
        {
            if (!EntityManager.Exists(pair.Key))
            {
                GameObject.Destroy(pair.Value);
                destroyEnti.Add(pair.Key);
            }
        }
        foreach (Entity enit in destroyEnti)
        {
            EntiUIManger.AvatarNameDic.Remove(enit);
        }
        destroyEnti.Clear();
    }

    //ˢ�����������ֵ����ÿһ��������µ�ʿ����ȥ�������ĵ�λ
    void UpdataTeamDic(ref TeamManager teamMager)
    {
        foreach(KeyValuePair<string,PlayerData> pair in teamMager._Dic_Team1)
        {
            var entiGiftList = pair.Value.m_GiftShiBingList;
            entiGiftList.RemoveAll(enti => !EntityManager.Exists(enti));

            var entiLikeList = pair.Value.m_LikeShiBingList;
            entiLikeList.RemoveAll(enti => !EntityManager.Exists(enti));
        }
        foreach (KeyValuePair<string, PlayerData> pair in teamMager._Dic_Team2)
        {
            var entiList = pair.Value.m_GiftShiBingList;
            entiList.RemoveAll(enti => !EntityManager.Exists(enti));

            var entiLikeList = pair.Value.m_LikeShiBingList;
            entiLikeList.RemoveAll(enti => !EntityManager.Exists(enti));
        }
    }


}
