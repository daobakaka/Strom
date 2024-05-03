using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ObjTransfer))]
public partial class YAxisRotationSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<UnitSpawner>();
    }

    protected override void OnUpdate()
    {

        Entities.ForEach((ref LocalTransform transform, in Unit1Component tag) =>
        {
            if (tag.health > 0)
            {
                transform.Rotation.value.x = 0;
                transform.Rotation.value.z = 0;
            }
        }).Schedule();
       
        
    }
}

public struct YAxisRotationSpeed : IComponentData
{
    public float RadiansPerSecond;
}
