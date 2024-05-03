using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 配置文件信息
/// </summary>

[System.Serializable]//暂时无影响
public class MonsterConfig
{
    public string title;
    public MonsterConfigs[] monsterConfigs;

}
[System.Serializable]
public class MonsterConfigs
{
    /// <summary>
    /// 类型
    /// </summary>
    public string id;
    public float damage = 0.6f;
    public float monsterHealth = 150;
    public float monsterSpeed = 50;
    /// <summary>
    /// 技能伤害
    /// </summary>
    public float skillDamage;
    /// <summary>
    /// 攻击动画时间（需要和动画时间匹配）
    /// </summary>
    public float attackTimer;
}