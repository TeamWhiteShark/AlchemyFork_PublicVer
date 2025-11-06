using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    protected abstract bool isDestroy { get; }
    
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
                    _instance = go.AddComponent<T>();
                    if (!Application.isBatchMode)
                    {
                        // if (Application.isPlaying)
                        //     DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }
    
    protected virtual void Awake()
    {
        // === 중복 생성 방지 ===
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        if (!isDestroy)
            DontDestroyOnLoad(gameObject);
    }

    public static bool IsCreatedInstance()
    {
        return (_instance != null);
    }
}