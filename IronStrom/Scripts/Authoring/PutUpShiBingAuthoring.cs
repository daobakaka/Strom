using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Animations;

public class PutUpShiBingAuthoring : MonoBehaviour
{
    public GameObject GroundSpawnPoint;//���������
    public GameObject AirSpawnPoint;//���г�����
    [Tooltip("--ָ��ʿ��----")]public GameObject SpecifyShiBing;//ָ����λ
    [Tooltip("--��������ʱ--")]public float PutUpTime;//��������ʱ
}

public class PutUpShiBingBaker : Baker<PutUpShiBingAuthoring>
{
    public override void Bake(PutUpShiBingAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var putupshibing = new PutUpShiBing
        {
            GroundSpawnPoint = GetEntity(authoring.GroundSpawnPoint.gameObject, TransformUsageFlags.Dynamic),
            AirSpawnPoint = GetEntity(authoring.AirSpawnPoint.gameObject, TransformUsageFlags.Dynamic),
            SpecifyShiBing = GetEntity(authoring.SpecifyShiBing.gameObject, TransformUsageFlags.Dynamic),
            PutUpTime = authoring.PutUpTime,
            Cur_PutUpTime = authoring.PutUpTime,
            PutUpNum = 1,
        };
        AddComponent(entity, putupshibing);
    }
}
