using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct BaZhu : IComponentData
{
    public Entity FirePoint_1;
    public Entity FirePoint_2;
    public Entity MainGunBullet;//�����ӵ�
    public Entity MainGunMuzzle;//����ǹ��
    //public float MainGunShootTime;//���ڹ���ʱ��
    //public float Cur_MainGunShootTime;
    public float MainGunPointOfTime;//���ڿ���ʱ���
    public float MainGunShootNum;//���ڹ�������
    public bool Is_MainGunShoot;//�Ƿ������ڹ���
}
