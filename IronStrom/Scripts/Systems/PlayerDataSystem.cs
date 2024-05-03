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

        //刷新对应头像字典，去除死亡值的键
        UpdataAvatarName(ref EntiUIManger);
        //刷新对应护盾UI的字典，去除死亡的键
        UpdataShieldUI(ref EntiUIManger);
        //刷新两个队伍字典里的每一个玩家旗下的士兵，去除死亡的单位
        UpdataTeamDic(ref teamManger);

        //每个有AvatarName组件的Entity士兵，自己同步自己头像的位置
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
            //同步头像位子
            EntiUIManger.AvatarSynEntityPos(entity, avatarnamePointPos);
            //同步HPSlider
            EntiUIManger.SynEntityShiBingHP(in entity, EntityManager);
            //同步策反Slider
            EntiUIManger.SynMutinySlider(entity, EntityManager);

        }).WithBurst().WithStructuralChanges().Run();


        //每个有ShieldUI组件的Entity，自己同步自己和护盾UI的位置
        Entities.ForEach((Entity entity, Shield shield, ShieldUI shieldUI) =>
        {
            if (!EntiUIManger.ShieldSliderDic.ContainsKey(entity) ||
               !EntityManager.Exists(entity))
                return;

            var shieldPos = m_LocalToWorld[entity].Position;
            shieldPos.y += shield.ShieldScale / 2;

            //同步护盾UI
            EntiUIManger.SynShieldUI(entity, shieldPos, EntityManager);

        }).WithBurst().WithStructuralChanges().Run();

    }

    //刷新对应头像字典，去除死亡值的键
    void UpdataAvatarName(ref EntityUIManager EntiUIManger)
    {
        destroyEnti = new List<Entity>();
        //如果Entity士兵死亡删除对应的头像
        foreach (KeyValuePair<Entity, GameObject> pair in EntiUIManger.AvatarNameDic)
        {
            if (!EntityManager.Exists(pair.Key))
            {
                GameObject.Destroy(pair.Value);
                destroyEnti.Add(pair.Key);
                //EntityUIManager.Instance.AvatarNameDic.Remove(pair.Key);
            }

            //是否显示头像名字
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
    //刷新对应护盾UI的字典，去除死亡的键
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

    //刷新两个队伍字典里的每一个玩家旗下的士兵，去除死亡的单位
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
