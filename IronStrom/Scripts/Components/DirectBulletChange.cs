using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum DirectBullet_FirePoint
{
    Null,
    FirePoint_R,
    FirePoint_L,
    FirePoint_R2,
    FirePoint_L2,
}
public struct DirectBulletChange : IComponentData
{
    public Entity Owner;//����ӵ���ӵ����
    public DirectBullet_FirePoint DB_FirePoint;//�ӵ���Ч���ĸ������
}
