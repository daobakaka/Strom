using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public struct DirectShoot : IComponentData//��ʾ������Ϊ Ϊֱ�����õĽ�ɫ
{
    public Entity DirectParticle_Parfab;//ֱ�ӹ�������Ч�����ӵ�ͬ����ʿ����λ�ù�ϵ
    public Entity DirectParticle_Entity;//��ʵ�������ӵ�
    public bool Is_CumulativeDamage;//�Ƿ�Ϊ�����˺�
    public float AT_Min;//�˺���С
    public float AT_Max;//�˺�������
    public float IntervalTime;//�˺��ۼӵ��ۼӼ��
    public float Cur_IntervalTime;//�˺��ۼӵ��ۼӼ��
    public Entity CD_ShootEntity;//�ۻ��˺���Ŀ�꣬���ڼ���Ƿ���ͬһ����λ����������ۻ����˺����¿�ʼ

    public bool Is_ShootEntityChanges;//����Ŀ���Ƿ����仯
}
