using System;
using UnityEngine;

public class PlayerTargeter : MonoBehaviour
{
    [SerializeField] private Transform objectToFollow;


    private void Start()
    {
        transform.position = objectToFollow.position;
    }

    private void Update()
    {
        transform.position = objectToFollow.position;
    }
}
