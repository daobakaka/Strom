using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Die : IComponentData,IEnableableComponent
{
    public Entity DeadParticle;//死亡特效
    public Entity DeadPoint;//死亡特效位置
    public Entity DeadLanguage;//亡语效果
    public Entity DeadLanguage2;//亡语效果
    public bool Is_DieDirectly;//是否直接死
    public bool Is_LanguageNoBullet;//不是子弹？
}
