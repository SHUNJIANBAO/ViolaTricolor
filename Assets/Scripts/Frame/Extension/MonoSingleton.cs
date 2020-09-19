using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T>:MonoBehaviour where T : MonoBehaviour 
{
    static object obj = new object();
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance==null)
            {
                lock (obj)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance==null)
                    {
                        _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
            }
            return _instance;
        }
    }

    public static T GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (GetInstance()!=null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

}
