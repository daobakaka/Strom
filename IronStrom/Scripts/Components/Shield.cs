using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Shield : IComponentData//����
{
                               //������Ҫһ��ö���������ʲô����
    public Entity ShieldParent;
    public float ShieldScale;//���ܰ뾶
    public float ShieldExpandSpeed;

}
