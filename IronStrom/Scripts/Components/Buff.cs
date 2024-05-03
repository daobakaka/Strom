using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
public enum BuffType//Buff������
{
    Null,
    buffAT,//������buff
    buffDP,//������buff
    buffDB,//��ֵbuff
    buffHP,//HP����buff
    buffRampage,//����buff

    buffSpeed,//�ƶ�buff
    buffShootTime,//����buff
    buffNotMove,//����buff
    buffMutiny,//�߷�buff
    AddHPbuff,//����Ѫ��buff
    RecoverHPbuff,//���û�Ѫbuff
}
public enum BuffAct//Buff�Ľ׶�
{
    Init,//��ʼ�׶�
    Run,//ִ�н׶�
    End,//�����׶�
    Delete,//ɾ��
}
public struct Buff : IBufferElementData
{
    public BuffType buffType;//Buff������
    public BuffAct buffAct;//Buff�Ľ׶�

    public float BuffProportion;//buff�ı���
    public float BuffChangeValue;//buff�ı����ֵ
    public float BuffChangeValue1;//buff�ı����ֵ
    public float BuffTime;//buff�ĳ���ʱ��
    public float IntervalTime;//buffЧ���ļ��ʱ��
    public float Cur_IntervalTime;//buffЧ���ļ��ʱ��
    public bool Is_deBuff;//�Ƿ�Ϊ����Ч��
    public Entity MyAttacker;//�����ҵ���
    //public Entity BuffParticle;//buff��Ч

}
