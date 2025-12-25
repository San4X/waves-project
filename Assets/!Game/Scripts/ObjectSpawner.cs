using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner Instance { get; private set; }
    
    [Header("Bubble")]
    public float bubbleOffsetMaxRange;
    public int bubbleQuantity;
    public Transform bubbleParent;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // private void Update()
    // {
    //     Health[] targets = FindObjectsByType<Health>(FindObjectsSortMode.None);
    //     if(targets.Length < quantity) SpawnObject(FindRandomOffset());
    // }

    private Vector3 FindRandomOffset(float minRandOffset, float maxRandOffset)
    {
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);
        float randRadius = Random.Range(minRandOffset, maxRandOffset);
        Vector2 randomPosition = new Vector2(randX, randY).normalized * randRadius;

        return new Vector3(randomPosition.x, 0f, randomPosition.y);
    }
    
    public Transform SpawnObject(Transform objectToSpawn, Vector3 position, float minRandOffset, float maxRandOffset)
    {
        Vector3 offset = FindRandomOffset(minRandOffset, maxRandOffset);
        return Instantiate(objectToSpawn, position + offset, Quaternion.identity);
    }
}
