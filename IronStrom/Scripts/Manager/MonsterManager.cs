using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterData
{
    public Dictionary<Entity, float> PlayerATDic;//存储每个玩家对我的伤害<玩家的EntityID，玩家的伤害>
    public MonsterData()
    {
        PlayerATDic = new Dictionary<Entity, float>();
    }
}

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _MonsterManager;
    public static MonsterManager instance { get { return _MonsterManager; } }


    public Dictionary<Entity, MonsterData> MonsterDic;//<怪物的Entity，怪物的数据>

    private void Awake()
    {
        _MonsterManager = this;
        MonsterDic = new Dictionary<Entity, MonsterData>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //var teamMager = TeamManager.teamManager;
        //foreach(var pair in MonsterDic)
        //{
        //    var playerATDic = pair.Value.PlayerATDic;
        //    foreach(var pair1 in playerATDic)
        //    {
        //        if (!teamMager._Dic_Team1OpenID.ContainsKey(pair1.Key)) continue;
        //        var PlayerOpenID = teamMager._Dic_Team1OpenID[pair1.Key];
        //        if (!teamMager._Dic_Team1.ContainsKey(PlayerOpenID)) continue;
        //        var playerData = teamMager._Dic_Team1[PlayerOpenID];
        //        Debug.Log($"  怪物：{pair.Key}. 被攻击玩家{playerData.m_Nick}.攻击 伤害为：{pair1.Value}");
        //    }
        //}
    }

    //统计怪物受到的伤害最后计算伤害最高的玩家获得怪物掉落物
    public void MonsterRecordPlayerAT(EntityOpenID PlayerentiID, Entity monster, float AT, EntityManager EntiMager)
    {
        if (!EntiMager.Exists(monster)) return;
        //有这个怪物，获得这个怪物的数据
        if (!MonsterDic.ContainsKey(monster)) return;
        var monsterData = MonsterDic[monster];
        //怪物的被打记录里面有没有这个玩家的攻击记录
        if (monsterData.PlayerATDic.ContainsKey(PlayerentiID.PlayerEntiyID))//有这个玩家的攻击记录,加上
        {
            monsterData.PlayerATDic[PlayerentiID.PlayerEntiyID] += AT;
        }
        else//没有这个玩家的记录，记上这个玩家
        {
            monsterData.PlayerATDic.Add(PlayerentiID.PlayerEntiyID, AT);
        }
    }
    
    //怪物死亡的时候调用，计算谁攻击我的伤害最多
    public Entity MonsterDeadCountPlayerAT(Entity monsterEnti, EntityManager entiMager)
    {
        if (!entiMager.Exists(monsterEnti)) return Entity.Null;
        if (!MonsterDic.ContainsKey(monsterEnti)) return Entity.Null;

        var monsterData = MonsterDic[monsterEnti];
        if (monsterData.PlayerATDic.Count <= 0) return Entity.Null;

        var maxATPlayer = monsterData.PlayerATDic.OrderByDescending(pair => pair.Value).FirstOrDefault();
        return maxATPlayer.Key;
    }



}
