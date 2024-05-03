using GPUECSAnimationBaker.Engine.AnimatorSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _eventManager;
    public static EventManager eventManager { get { return _eventManager; } }
    private void Awake()
    {
        _eventManager = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
