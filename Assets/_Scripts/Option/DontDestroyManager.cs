using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DontDestroyManager : MonoBehaviour
{
    public static GameObject Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = gameObject;
        DontDestroyOnLoad(gameObject);
    }
}
