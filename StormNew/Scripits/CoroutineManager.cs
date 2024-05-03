using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CoroutineManager:MonoBehaviour
    {
    public static CoroutineManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private CoroutineManager() { }
    // public List<Coroutine> activeCoroutines = new List<Coroutine>();
    public Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
    public HashSet<string> activeCoroutinesHash = new HashSet<string>();
    private void OnDestroy()
    {
      
    }

    public Coroutine StartManagedCoroutine(string name, IEnumerator coroutine)
        {
            var coro = StartCoroutine(coroutine);
            activeCoroutines.Add(name,coro);
            return coro;
        }
        public void StopManagedCoroutine(string name)
        {
            if (activeCoroutines.ContainsKey(name))
            {
                StopCoroutine(name);
                activeCoroutines.Remove(name);
            }
        }
        public void LogActiveCoroutines()
        {
          Debug.Log("start to foreach");
        foreach (var coro in activeCoroutines)
        {
                Debug.Log("Active coroutine: " + activeCoroutines.Count+"name is"+coro.Key);
        }
        foreach (var hash in activeCoroutinesHash)
        {

            Debug.Log("Active coroutineHash: " + activeCoroutinesHash.Count + "name is" + hash);

        }


        }
    public void StartManagedCoroutineHashSet(string name, IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
        activeCoroutinesHash.Add(name);

    }
    public void StopManagedCoroutineHashSet(string name)
    {
        if (activeCoroutinesHash.Contains(name))
        {
            StopCoroutine(name);
            activeCoroutinesHash.Remove(name);
        }
    }


}
