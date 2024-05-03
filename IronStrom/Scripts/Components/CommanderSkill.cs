using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
public enum SceneBombType
{
    NUll,
    NucleaBomb,//原子弹
    MissileAirStrike,//导弹空袭
    Laser,//激光
    Shile,//护盾

}

public struct CommanderSkill : IComponentData//指挥官技能也就是达到音浪的场景炸弹
{
    public SceneBombType SceneBombtype;
    public float InitTime;//指挥官技能前摇时间
    public float Cur_InitTime;//指挥官技能前摇时间
    public layer TeamID;
    public float IntervalTime;//间隔时间
    public float Cur_IntervalTime;//间隔时间
    public float SustainTime;//持续时间
    public float Cur_SustainTime;//持续时间
    public int MissileNum;//导弹个数
    public float Speed;
    public Entity Laser;

}