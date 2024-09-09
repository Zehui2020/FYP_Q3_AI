using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();
    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("UnityMainThreadDispatcher");
                _instance = gameObject.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    private void Update()
    {
        while (_executionQueue.Count > 0)
        {
            System.Action action = _executionQueue.Dequeue();
            action();
        }
    }

    public static void Enqueue(System.Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}
