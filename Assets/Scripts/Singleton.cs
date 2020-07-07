using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public abstract IEnumerator Initialize();
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        Instance = FindObjectOfType<T>();
    }
}
