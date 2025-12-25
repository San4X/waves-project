using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public Transform fishBullet;
    public Transform bubble;
    
    public static PrefabManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
