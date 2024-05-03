using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class CameraSystem : SystemBase
{
    ComponentLookup<MyLayer> m_Layer;
    ComponentLookup<ShiBing> m_ShiBing;
    ComponentLookup<LocalTransform> m_transform;
    ComponentLookup<LocalToWorld> m_LocalToWorld;


    Entity MouseClickOnce;//鼠标点击一次的Entity
    Entity MouseClickTwo;//鼠标点击两次的Entity
    float ClickUpTime = 1f;//鼠标点击的刷新时间
    float Cur_ClickUpTime = 1f;//鼠标点击的刷新时间
    void UpDataComponentLookup()
    {
        m_Layer.Update(this);
        m_ShiBing.Update(this);
        m_transform.Update(this);
        m_LocalToWorld.Update(this);
    }
    protected override void OnCreate()
    {
        m_Layer = GetComponentLookup<MyLayer>();
        m_ShiBing = GetComponentLookup<ShiBing>();
        m_transform = GetComponentLookup<LocalTransform>();
        m_LocalToWorld = GetComponentLookup<LocalToWorld>();
        MouseClickOnce = Entity.Null;
        MouseClickTwo = Entity.Null;
    }
    protected override void OnUpdate()
    {
        if (CameraCtrl.cameraCtrl == null)
            return;

        UpDataComponentLookup();
        Dependency.Complete();// 确保所有写入LocalToWorld组件的作业已经完成

        Cur_ClickUpTime -= SystemAPI.Time.DeltaTime;
        if (Cur_ClickUpTime <= 0)
        {
            Cur_ClickUpTime = ClickUpTime;
            if (CameraCtrl.cameraCtrl.ChosenEntityCameraPoint == Entity.Null)
            {
                MouseClickOnce = Entity.Null;
                MouseClickTwo = Entity.Null;
            }
        }

        if(CameraCtrl.cameraCtrl.Is_QuitEntityPoint)//如果退出了EntityPoint就不要同步了
        {
            MouseClickTwo = Entity.Null;
        }


        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//创建一个碰撞检测需要的单例
        if (Input.GetMouseButtonDown(0)) // 检查鼠标左键是否被点击
        {
            uint collidesWithMask = 0;
            collidesWithMask = (1u << (int)layer.Team1 | 1u << (int)layer.Team2);
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = ray.origin,
                End = ray.origin + ray.direction * 10000,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = collidesWithMask,
                    GroupIndex = 0,
                }
            };
            NativeList<Unity.Physics.RaycastHit> allHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp); // 用于存储所有碰撞
            ////设置击中的第一个单位
            //bool hit = physicsWorld.CastRay(raycastInput, out hitInfo);
            //if (hit)
            //{
            //    Entity hitEntity = physicsWorld.Bodies[hitInfo.RigidBodyIndex].Entity;
            //    Debug.Log("   检测到的士兵为：" + hitEntity);
            //    if (!m_ShiBing.TryGetComponent(hitEntity, out ShiBing sb))
            //        return;
            //    if (!m_transform.TryGetComponent(m_ShiBing[hitEntity].CameraPoint, out LocalTransform ltf))
            //        return;
            //    Debug.Log("   的摄像机是：" + m_ShiBing[hitEntity].CameraPoint);
            //}

            //设置击中的所有单位
            if (physicsWorld.CastRay(raycastInput, ref allHits))
            {
                //if(MouseClickOnce != Entity.Null && MouseClickOnce != MouseClickTwo)
                //{
                //    MouseClickOnce = Entity.Null;
                //    MouseClickTwo = Entity.Null;
                //}

                foreach (var hit in allHits)
                {
                    Entity hitEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    if (!m_ShiBing.TryGetComponent(hitEntity, out ShiBing sb))
                        return;
                    if (!m_transform.TryGetComponent(m_ShiBing[hitEntity].CameraPoint, out LocalTransform ltf))
                        return;
                    if (MouseClickOnce == Entity.Null)
                    {
                        MouseClickOnce = hitEntity;
                        //Debug.Log("    鼠标第一次击中的单位为：" + MouseClickOnce);
                        return;
                    }
                    else
                    {
                        MouseClickTwo = hitEntity;
                        CameraCtrl.cameraCtrl.Is_QuitEntityPoint = false;
                        //Debug.Log("    鼠标第二次击中的单位为：" + MouseClickTwo);
                    }
                    return;
                }
            }
            allHits.Dispose(); // 释放NativeList占用的内存
        }
        KeyFollowTheSoldiers(ref MouseClickOnce, ref MouseClickTwo);
        //如果鼠标两次选中的都是同一个单位，就让镜头跟随他
        if (MouseClickOnce == MouseClickTwo && MouseClickTwo != Entity.Null && MouseClickOnce != Entity.Null)
        {
            if (!EntityManager.Exists(MouseClickTwo))
            {
                MouseClickOnce = Entity.Null;
                MouseClickTwo = Entity.Null;
                var Enti = ReplaceEntityCameraPoint();
                MouseClickOnce = Enti;
                MouseClickTwo = Enti;
                return;
            }
            if (!EntityManager.Exists(m_ShiBing[MouseClickTwo].CameraPoint))
                return;

            //记录选中士兵摄像机之前的位置
            if (CameraCtrl.cameraCtrl.CameraAct_ != CameraAct.In_EntityPoint)
            {
                CameraCtrl.cameraCtrl.preMoveCameraPoint = CameraCtrl.cameraCtrl.transform.position;
                CameraCtrl.cameraCtrl.preMoveCameraQuaternion = CameraCtrl.cameraCtrl.transform.rotation;
                CameraCtrl.cameraCtrl.CameraAct_ = CameraAct.In_EntityPoint;
            }

            CameraCtrl.cameraCtrl.ChosenEntityCameraPoint = MouseClickTwo;
            CameraCtrl.cameraCtrl.EntityCameraPointV3 = m_LocalToWorld[m_ShiBing[MouseClickTwo].CameraPoint].Position;
            CameraCtrl.cameraCtrl.EntityCameraPointQuaternion = m_LocalToWorld[m_ShiBing[MouseClickTwo].CameraPoint].Rotation;

        }
    }

    //Entity单位死了，选择随机一个其他Entity单位跟随

    Entity ReplaceEntityCameraPoint()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<ShiBing>());
        using (NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob))
        {
            foreach (var entity in entities)
            {
                ShiBing shiBing = EntityManager.GetComponentData<ShiBing>(entity);
                if (shiBing.Name == ShiBingName.BaZhu)
                    return entity;// 一旦找到匹配的实体，立即返回它
                else if(shiBing.Name == ShiBingName.ZhanZhengGongChang)
                    return entity;
                else if (shiBing.Name == ShiBingName.BaoLei)
                    return entity;
                else if (shiBing.Name == ShiBingName.RongDian)
                    return entity;
                else if (shiBing.Name == ShiBingName.HuoShen)
                    return entity;
                else if (shiBing.Name == ShiBingName.HaiKe)
                    return entity;
                else if (shiBing.Name == ShiBingName.XiNiu)
                    return entity;
                else if (shiBing.Name == ShiBingName.TieChui)
                    return entity;
                else if (shiBing.Name == ShiBingName.FengHuang)
                    return entity;
                else if (shiBing.Name == ShiBingName.ChangGong)
                    return entity;
                else if (shiBing.Name == ShiBingName.BaoYu)
                    return entity;
                else if (shiBing.Name == ShiBingName.GangQiu)
                    return entity;
                else if (shiBing.Name == ShiBingName.BingFeng)
                    return entity;
                else if (shiBing.Name == ShiBingName.HuGuang)
                    return entity;
                else if (shiBing.Name == ShiBingName.PaChong)
                    return entity;
                else if (shiBing.Name == ShiBingName.JianYa)
                    return entity;

            }
        }
        // 如果没有找到匹配的实体，返回Entity.Null
        return Entity.Null;

    }

    //键盘按键跟随士兵
    void KeyFollowTheSoldiers(ref Entity MouseOnce, ref Entity MouseTwo)
    {
        var teamMager = TeamManager.teamManager;
        bool b = false;
        if (Input.GetKeyDown(KeyCode.Keypad8) && Input.GetKey(KeyCode.F4))
        {
            if (teamMager._Dic_Team1.Count <= 0)
                return;
            Entity shibing = teamMager._Dic_Team1.FirstOrDefault().Value.m_GiftShiBingList.FirstOrDefault();
            if (!EntityManager.Exists(shibing))
                shibing = teamMager._Dic_Team1.FirstOrDefault().Value.m_LikeShiBingList.FirstOrDefault();
            if(EntityManager.Exists(shibing))
            {
                MouseOnce = shibing;
                MouseTwo = shibing;
                b = true;
            }

        }
        else if (Input.GetKeyDown(KeyCode.Keypad9) && Input.GetKey(KeyCode.F4))
        {
            if (teamMager._Dic_Team2.Count <= 0)
                return;
            Entity shibing = teamMager._Dic_Team2.FirstOrDefault().Value.m_GiftShiBingList.FirstOrDefault();
            if (!EntityManager.Exists(shibing))
                shibing = teamMager._Dic_Team2.FirstOrDefault().Value.m_LikeShiBingList.FirstOrDefault();
            if (EntityManager.Exists(shibing))
            {
                MouseOnce = shibing;
                MouseTwo = shibing;
                b = true;
            }
        }
        if(b)
        {
            if (!EntityManager.Exists(m_ShiBing[MouseTwo].CameraPoint))
                return;
            //记录选中士兵摄像机之前的位置
            if (CameraCtrl.cameraCtrl.CameraAct_ != CameraAct.In_EntityPoint)
            {
                CameraCtrl.cameraCtrl.preMoveCameraPoint = CameraCtrl.cameraCtrl.transform.position;
                CameraCtrl.cameraCtrl.preMoveCameraQuaternion = CameraCtrl.cameraCtrl.transform.rotation;
                CameraCtrl.cameraCtrl.CameraAct_ = CameraAct.In_EntityPoint;
            }

            CameraCtrl.cameraCtrl.ChosenEntityCameraPoint = MouseTwo;
            CameraCtrl.cameraCtrl.EntityCameraPointV3 = m_LocalToWorld[m_ShiBing[MouseTwo].CameraPoint].Position;
            CameraCtrl.cameraCtrl.EntityCameraPointQuaternion = m_LocalToWorld[m_ShiBing[MouseTwo].CameraPoint].Rotation;
            //CameraCtrl.cameraCtrl.CameraAct_ = CameraAct.In_EntityPoint;
            CameraCtrl.cameraCtrl.Is_QuitEntityPoint = false;
            CameraCtrl.cameraCtrl.Is_Click = false;
        }
    }
}
