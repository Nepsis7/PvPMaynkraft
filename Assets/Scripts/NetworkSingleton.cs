using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    static T instance = null;
    public static T Instance => instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
    }
}
