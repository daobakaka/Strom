using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct BaZhu : IComponentData
{
    public Entity FirePoint_1;
    public Entity FirePoint_2;
    public Entity MainGunBullet;//主炮子弹
    public Entity MainGunMuzzle;//主炮枪口
    //public float MainGunShootTime;//主炮攻击时间
    //public float Cur_MainGunShootTime;
    public float MainGunPointOfTime;//主炮开炮时间点
    public float MainGunShootNum;//主炮攻击次数
    public bool Is_MainGunShoot;//是否让主炮攻击
}
