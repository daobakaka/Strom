using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Die : IComponentData,IEnableableComponent
{
    public Entity DeadParticle;//������Ч
    public Entity DeadPoint;//������Чλ��
    public Entity DeadLanguage;//����Ч��
    public Entity DeadLanguage2;//����Ч��
    public bool Is_DieDirectly;//�Ƿ�ֱ����
    public bool Is_LanguageNoBullet;//�����ӵ���
}
