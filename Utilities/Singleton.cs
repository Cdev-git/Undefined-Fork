using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Undefined.Utilities;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.Obliterate();

            return;
        }

        Instance = this as T;
    }
}