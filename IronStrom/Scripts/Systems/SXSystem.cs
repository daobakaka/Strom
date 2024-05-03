using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;


[BurstCompile]
public partial class SXSystem : SystemBase
{
    ComponentLookup<EntityCtrl> m_entiCtrl;

    void UpDateComponentLookup()
    {
        m_entiCtrl.Update(this);
    }

    protected override void OnCreate()
    {
        m_entiCtrl = GetComponentLookup<EntityCtrl>(true);
    }
    protected override void OnUpdate()
    {
        UpDateComponentLookup();


        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sxjob = new SXJob()
        {
            ECB = ecb.AsParallelWriter(),
            entityCtrl = m_entiCtrl,
        };
        Dependency = sxjob.ScheduleParallel(Dependency);

        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();


    }
}
[BurstCompile]
public partial struct SXJob : IJobEntity//ֻ������GpuECSCtrl�ĵ�λ�Ķ����ٶȵ���
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<EntityCtrl> entityCtrl;
    void Execute(Entity entity,ref SX sx)
    {
        //�����ƶ������Ե�λ(��̨������...) || ��ǰ�ƶ��ٶ��Ƿ����һ֡һ������һ���ͼ��㶯�������ٶ�
        if (sx.Speed == 0 || sx.Speed == sx.Record_Speed || sx.AinWalkSpeed == 0)
            return;

        sx.Cur_AinWalkSpeed = sx.Speed / sx.Init_Speed * sx.AinWalkSpeed;
        sx.Record_Speed = sx.Speed;

        //��ͳ������Entity�Ͳ���sx.Is_ChangedAinWalkSpeed Ϊ true
        if (entityCtrl.TryGetComponent(entity,out EntityCtrl ec))
        {
            if (entityCtrl[entity].Is_TraditionalAnimation)
                return;
        }

        sx.Is_ChangedAinWalkSpeed = true;
        
    }
}
