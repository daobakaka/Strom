using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterData
{
    public Dictionary<Entity, float> PlayerATDic;//�洢ÿ����Ҷ��ҵ��˺�<��ҵ�EntityID����ҵ��˺�>
    public MonsterData()
    {
        PlayerATDic = new Dictionary<Entity, float>();
    }
}

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _MonsterManager;
    public static MonsterManager instance { get { return _MonsterManager; } }


    public Dictionary<Entity, MonsterData> MonsterDic;//<�����Entity�����������>

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
        //        Debug.Log($"  ���{pair.Key}. ���������{playerData.m_Nick}.���� �˺�Ϊ��{pair1.Value}");
        //    }
        //}
    }

    //ͳ�ƹ����ܵ����˺��������˺���ߵ���һ�ù��������
    public void MonsterRecordPlayerAT(EntityOpenID PlayerentiID, Entity monster, float AT, EntityManager EntiMager)
    {
        if (!EntiMager.Exists(monster)) return;
        //�����������������������
        if (!MonsterDic.ContainsKey(monster)) return;
        var monsterData = MonsterDic[monster];
        //����ı����¼������û�������ҵĹ�����¼
        if (monsterData.PlayerATDic.ContainsKey(PlayerentiID.PlayerEntiyID))//�������ҵĹ�����¼,����
        {
            monsterData.PlayerATDic[PlayerentiID.PlayerEntiyID] += AT;
        }
        else//û�������ҵļ�¼������������
        {
            monsterData.PlayerATDic.Add(PlayerentiID.PlayerEntiyID, AT);
        }
    }
    
    //����������ʱ����ã�����˭�����ҵ��˺����
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
