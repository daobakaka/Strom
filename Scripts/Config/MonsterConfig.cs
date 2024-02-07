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
    public string kaka = "kaka";
    public MonsterConfig1 monsterConfig1;
    public MonsterConfig2 monsterConfig2;
    public MonsterConfig3 monsterConfig3;
    public MonsterConfig4 monsterConfig4;
    public MonsterConfig5 monsterConfig5;
    public MonsterConfig6 monsterConfig6;
    public MonsterConfig7 monsterConfig7;
    public WildConfig1 wildConfig1;
    public WildConfig2 wildConfig2;
    public WildConfig3 wildConfig3;
    public WildConfig4 wildConfig4;
    [System.Serializable]
    public class MonsterConfig1
    {
        public float damage = 0.6f;
        public float monsterHealth = 150;
        public float healthCache = 150;
        public float monsterSpeed = 50;
        public float speedCache = 50;
    }

    [System.Serializable]
    public class MonsterConfig2
    {
        public float damage = 0.8f;
        public float monsterHealth=200;
        public float healthCache=200;
        public float monsterSpeed=70;
        public float speedCache=70;
    }

    [System.Serializable]
    public class MonsterConfig3
    {
        public float damage=1f;
        public float monsterHealth=300;
        public float healthCache=300;
        public float monsterSpeed=40;
        public float speedCache=40;
    }

    [System.Serializable]
    public class MonsterConfig4
    {
        public float damage=1.1f;
        public float monsterHealth=500;
        public float healthCache=500;
        public float monsterSpeed=60;
        public float speedCache=60;
    }
    [System.Serializable]
    public class MonsterConfig5
    {
        public float damage=1.2f;
        public float monsterHealth=800;
        public float healthCache=800;
        public float monsterSpeed=20;
        public float speedCache=20;
        public float mechaAttackDis=120000;
        public float bulletDamage = 6f;
    }
    [System.Serializable]
    public class MonsterConfig6
    {
        public float damage=1.5f;
        public float monsterHealth=1500;
        public float healthCache=1500;
        public float monsterSpeed=40;
        public float speedCache=40;
        public float skillDamage = 20;
    }
    [System.Serializable]
    public class MonsterConfig7
    {
        public float damage=2f;
        public float monsterHealth=2000;
        public float healthCache=2000;
        public float monsterSpeed=40;
        public float speedCache=40;
        public float skillDamage = 60;
    }
    [System.Serializable]
    public class WildConfig1
    {
        public float damage=3f;
        public float monsterHealth=1500;
        public float healthCache=1500;
        public float monsterSpeed=30;
        public float speedCache=30;
    }
    [System.Serializable]
    public class WildConfig2
    {
        public float damage=5f;
        public float monsterHealth=2000;
        public float healthCache=2000;
        public float monsterSpeed=30;
        public float speedCache=30;
    }
    [System.Serializable]
    public class WildConfig3
    {
        public float damage=7;
        public float monsterHealth=2500;
        public float healthCache=2500;
        public float monsterSpeed=40;
        public float speedCache=40;
    }
    [System.Serializable]
    public class WildConfig4
    {
        public float damage=9;
        public float monsterHealth=3000;
        public float healthCache=3000;
        public float monsterSpeed=40;
        public float speedCache=40;
    }
}