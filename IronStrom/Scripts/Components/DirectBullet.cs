using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DirectBullet : IComponentData//ֱ�����õ��ӵ�
{
    public Entity BulletHit;//�ӵ�������Ч
    public float StartLifetime;//�ӵ���Ч�����Ӵ��ʱ��
    public float StartSpeed;//�ӵ���Ч�������ٶ�
    public float StartOffset;//�ӵ���Ч��ǰ��ƫ��

}
