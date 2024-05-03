using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class JiDiComponentAuthoring : MonoBehaviour
{

}

public class JiDiCompentBaker : Baker<JiDiComponentAuthoring>
{
    public override void Bake(JiDiComponentAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var jidicompent = new JiDiComponent();
        AddComponent(entity, jidicompent);
    }
}
