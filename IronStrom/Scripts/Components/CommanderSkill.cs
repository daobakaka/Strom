using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
public enum SceneBombType
{
    NUll,
    NucleaBomb,//ԭ�ӵ�
    MissileAirStrike,//������Ϯ
    Laser,//����
    Shile,//����

}

public struct CommanderSkill : IComponentData//ָ�ӹټ���Ҳ���Ǵﵽ���˵ĳ���ը��
{
    public SceneBombType SceneBombtype;
    public float InitTime;//ָ�ӹټ���ǰҡʱ��
    public float Cur_InitTime;//ָ�ӹټ���ǰҡʱ��
    public layer TeamID;
    public float IntervalTime;//���ʱ��
    public float Cur_IntervalTime;//���ʱ��
    public float SustainTime;//����ʱ��
    public float Cur_SustainTime;//����ʱ��
    public int MissileNum;//��������
    public float Speed;
    public Entity Laser;

}