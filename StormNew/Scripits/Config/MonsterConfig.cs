using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �����ļ���Ϣ
/// </summary>

[System.Serializable]//��ʱ��Ӱ��
public class MonsterConfig
{
    public string title;
    public MonsterConfigs[] monsterConfigs;

}
[System.Serializable]
public class MonsterConfigs
{
    /// <summary>
    /// ����
    /// </summary>
    public string id;
    public float damage = 0.6f;
    public float monsterHealth = 150;
    public float monsterSpeed = 50;
    /// <summary>
    /// �����˺�
    /// </summary>
    public float skillDamage;
    /// <summary>
    /// ��������ʱ�䣨��Ҫ�Ͷ���ʱ��ƥ�䣩
    /// </summary>
    public float attackTimer;
}