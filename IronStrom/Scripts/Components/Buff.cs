using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
public enum BuffType//Buff的类型
{
    Null,
    buffAT,//攻击力buff
    buffDP,//防御力buff
    buffDB,//格挡值buff
    buffHP,//HP增加buff
    buffRampage,//暴走buff

    buffSpeed,//移动buff
    buffShootTime,//攻速buff
    buffNotMove,//禁锢buff
    buffMutiny,//策反buff
    AddHPbuff,//增加血量buff
    RecoverHPbuff,//永久回血buff
}
public enum BuffAct//Buff的阶段
{
    Init,//开始阶段
    Run,//执行阶段
    End,//结束阶段
    Delete,//删除
}
public struct Buff : IBufferElementData
{
    public BuffType buffType;//Buff的类型
    public BuffAct buffAct;//Buff的阶段

    public float BuffProportion;//buff的比例
    public float BuffChangeValue;//buff改变的数值
    public float BuffChangeValue1;//buff改变的数值
    public float BuffTime;//buff的持续时间
    public float IntervalTime;//buff效果的间隔时间
    public float Cur_IntervalTime;//buff效果的间隔时间
    public bool Is_deBuff;//是否为负面效果
    public Entity MyAttacker;//攻击我的人
    //public Entity BuffParticle;//buff特效

}
