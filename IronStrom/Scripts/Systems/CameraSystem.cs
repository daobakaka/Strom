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


    Entity MouseClickOnce;//�����һ�ε�Entity
    Entity MouseClickTwo;//��������ε�Entity
    float ClickUpTime = 1f;//�������ˢ��ʱ��
    float Cur_ClickUpTime = 1f;//�������ˢ��ʱ��
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
        Dependency.Complete();// ȷ������д��LocalToWorld�������ҵ�Ѿ����

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

        if(CameraCtrl.cameraCtrl.Is_QuitEntityPoint)//����˳���EntityPoint�Ͳ�Ҫͬ����
        {
            MouseClickTwo = Entity.Null;
        }


        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();//����һ����ײ�����Ҫ�ĵ���
        if (Input.GetMouseButtonDown(0)) // ����������Ƿ񱻵��
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
            NativeList<Unity.Physics.RaycastHit> allHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp); // ���ڴ洢������ײ
            ////���û��еĵ�һ����λ
            //bool hit = physicsWorld.CastRay(raycastInput, out hitInfo);
            //if (hit)
            //{
            //    Entity hitEntity = physicsWorld.Bodies[hitInfo.RigidBodyIndex].Entity;
            //    Debug.Log("   ��⵽��ʿ��Ϊ��" + hitEntity);
            //    if (!m_ShiBing.TryGetComponent(hitEntity, out ShiBing sb))
            //        return;
            //    if (!m_transform.TryGetComponent(m_ShiBing[hitEntity].CameraPoint, out LocalTransform ltf))
            //        return;
            //    Debug.Log("   ��������ǣ�" + m_ShiBing[hitEntity].CameraPoint);
            //}

            //���û��е����е�λ
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
                        //Debug.Log("    ����һ�λ��еĵ�λΪ��" + MouseClickOnce);
                        return;
                    }
                    else
                    {
                        MouseClickTwo = hitEntity;
                        CameraCtrl.cameraCtrl.Is_QuitEntityPoint = false;
                        //Debug.Log("    ���ڶ��λ��еĵ�λΪ��" + MouseClickTwo);
                    }
                    return;
                }
            }
            allHits.Dispose(); // �ͷ�NativeListռ�õ��ڴ�
        }
        KeyFollowTheSoldiers(ref MouseClickOnce, ref MouseClickTwo);
        //����������ѡ�еĶ���ͬһ����λ�����þ�ͷ������
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

            //��¼ѡ��ʿ�������֮ǰ��λ��
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

    //Entity��λ���ˣ�ѡ�����һ������Entity��λ����

    Entity ReplaceEntityCameraPoint()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<ShiBing>());
        using (NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob))
        {
            foreach (var entity in entities)
            {
                ShiBing shiBing = EntityManager.GetComponentData<ShiBing>(entity);
                if (shiBing.Name == ShiBingName.BaZhu)
                    return entity;// һ���ҵ�ƥ���ʵ�壬����������
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
        // ���û���ҵ�ƥ���ʵ�壬����Entity.Null
        return Entity.Null;

    }

    //���̰�������ʿ��
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
            //��¼ѡ��ʿ�������֮ǰ��λ��
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
