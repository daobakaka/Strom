using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(EntityCtrlEventSystem))]//在个System之前执行
public partial class SynchronizeAniSystem : SystemBase
{
    ComponentLookup<ShiBing> m_shibing;
    ComponentLookup<ShiBingChange> m_shibingChange;
    ComponentLookup<MyLayer> m_MyLayer;
    ComponentLookup<SX> m_SX;

    void UpDateComponentLookup()
    {
        m_shibing.Update(this);
        m_shibingChange.Update(this);
        m_MyLayer.Update(this);
        m_SX.Update(this);
    }
    protected override void OnCreate()
    {
        m_shibing = GetComponentLookup<ShiBing>(true);
        m_shibingChange = GetComponentLookup<ShiBingChange>(true);
        m_MyLayer = GetComponentLookup<MyLayer>(true);
        m_SX = GetComponentLookup<SX>(true);
    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();

        //为entity生成对应的obj肉体
        Entities.ForEach((Entity entity, EntitySynchronizeObj esobj) =>
        {
            UpDateComponentLookup();
            int teamID = 0;
            if (m_MyLayer[entity].BelongsTo == layer.Team1)
                teamID = 1;
            else if (m_MyLayer[entity].BelongsTo == layer.Team2)
                teamID = 2;
            else if (m_MyLayer[entity].BelongsTo == layer.Neutral)
                teamID = 3;

            var obj = SynchronizeManager.synchronizeManager.InstanCorrespondObj(m_shibing[entity].Name, teamID);
            SynchronizeManager.synchronizeManager.SynAniDic[entity] = obj;
            EntityManager.RemoveComponent<EntitySynchronizeObj>(entity);

        }).WithBurst().WithStructuralChanges().Run();

        //同步entity和obj之间的位置
        Entities.ForEach((Entity entity,ref SynchronizeAni synAni) =>
        {
            if(SynchronizeManager.synchronizeManager.SynAniDic.TryGetValue(entity,out GameObject obj))
            {
                UpDateComponentLookup();
                //同步位置
                var pos = EntityManager.GetComponentData<LocalTransform>(entity).Position;
                var rot = EntityManager.GetComponentData<LocalTransform>(entity).Rotation;
                obj.transform.position = pos;
                obj.transform.rotation = rot;

                var act = m_shibingChange[entity].Act;
                var name = m_shibing[entity].Name;

                //攻击的是否为空中单位
                if(EntityManager.Exists(m_shibing[entity].ShootEntity))
                {
                    if (EntityManager.HasComponent<SX>(m_shibing[entity].ShootEntity))
                    {
                        if (m_SX[m_shibing[entity].ShootEntity].Is_AirForce)
                            synAni.ShootEnitIs_Air = true;
                        else
                            synAni.ShootEnitIs_Air = false;
                    }
                }

                //同步行为动画
                var SynObj = obj.GetComponent<SynchronizeGameObj>();
                if(SynObj != null)
                {
                    SynObj.PlayAni(act, name, m_SX[entity].Cur_AinWalkSpeed, synAni.ShootEnitIs_Air);

                    if (synAni.Is_DotsEventFire)//DOTS返回过来的已经完成了Event逻辑的触发
                    {
                        SynObj.Is_EventFire_1 = synAni.Is_EventFire_1;
                        SynObj.Is_EventFire_2 = synAni.Is_EventFire_2;
                        synAni.Is_DotsEventFire = false;
                    }
                    else
                    {
                        synAni.Is_EventFire_1 = SynObj.Is_EventFire_1;
                        synAni.Is_EventFire_2 = SynObj.Is_EventFire_2;
                    }


                    if (synAni.Is_DotsPlayingAniFire)
                    {
                        SynObj.Is_PlayingAniFire = synAni.Is_PlayingAniFire;
                        synAni.Is_DotsPlayingAniFire = false;
                    }
                    else
                        synAni.Is_PlayingAniFire = SynObj.Is_PlayingAniFire;

                }

            }

        }).WithoutBurst().WithStructuralChanges().Run();
        
    }
}
