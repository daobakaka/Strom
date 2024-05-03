using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ParticleAuthoring : MonoBehaviour
{
    public float DieTime;//À¿Õˆ ±º‰
}

public class ParticleBaker : Baker<ParticleAuthoring>
{
    public override void Bake(ParticleAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var par = new ParticleComponent
        {
            DieTime = authoring.DieTime,
        };
        AddComponent(entity, par);
    }
}