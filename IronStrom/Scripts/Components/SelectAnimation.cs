using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
public enum AnimationID
{
    NUll,
    AnimationID_ChangGong,
    AnimationID_BaoLei,
}
public struct SelectAnimation : IComponentData
{
    public AnimationID AnimID;
}
