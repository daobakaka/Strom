using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public struct Parasitic : IComponentData//给寄生单位装状态组件(Idle,Move...)然后删除自身
{
    public bool Is_HaveActComponent;//有没有行为组件
    public Entity Owner;
}
