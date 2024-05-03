using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BingYingAuthoring : MonoBehaviour
{
    //private int playerID;
    //public int PlayerID { get { return playerID; } set { playerID = value; } }
    private int teamID;
    public int TeamID { get { return teamID; } set { teamID = value; } }
    public Transform FirePoint;
    public float CountDownTime;
    [System.NonSerialized]public float Cur_CountDownTime;
    public float InitSpeed;


}

public class BingYingBaker : Baker<BingYingAuthoring>
{
    public override void Bake(BingYingAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var bingying = new BingYing
        {
            TeamID = authoring.TeamID,
            FirePoint = GetEntity(authoring.FirePoint.gameObject, TransformUsageFlags.Dynamic),
            CountDownTime = authoring.CountDownTime,
            Cur_CountDownTime = authoring.CountDownTime,
            InitSpeed = authoring.InitSpeed
        };
        AddComponent(entity, bingying);
    }
}