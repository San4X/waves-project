using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlobBody : MonoBehaviour
{
    [SerializeField] private Transform segment;
    [SerializeField] private float minScale, maxScale;
    [SerializeField] private float spawnVectorAngle;
    [SerializeField] private float spawnVectorLength;
    [SerializeField] private int bodyAmount;
    [SerializeField] private float segmentSpawnAnimTime;
    
    [Header("Weapon")]
    [SerializeField] private Transform thorn;
    [SerializeField] private int thornsAmountOnEach;
    [SerializeField] private float attackAngle;
    [SerializeField] private float thornHolderFindingRange = 0.5f; // if starting object is sphere it equals to radius

    private Transform _centralChild;


    private void Awake()
    {
        _centralChild = transform.GetChild(0);
    }

    private void Start()
    {
        StartCoroutine(CreateBody());
    }

    private IEnumerator CreateBody()
    {
        for (int i = 0; i < bodyAmount; i++)
        {
            SpawnSegment();
            yield return new WaitForSeconds(segmentSpawnAnimTime);
        }
        SpawnThorns();
    }

    private readonly List<Transform> _spawnedSegments = new();
    private void SpawnSegment()
    {
        float angle = Random.Range(-spawnVectorAngle / 2, spawnVectorAngle / 2);
        Vector3 rayOrigin = Quaternion.Euler(0, angle, 0) * Vector3.forward * spawnVectorLength;
        Vector3 rayDirection = transform.position - rayOrigin;
        var newSegmentPosition = PerformRaycast(rayOrigin, rayDirection);
        
        var parameters = new InstantiateParameters
        {
            worldSpace = true,
            parent = transform
        };
            
        var segmentTransform = Instantiate(
            segment, 
            newSegmentPosition, 
            Quaternion.identity,
            parameters);
        
        // Scale
        // float randScale = Random.Range(minScale, maxScale);
        // segmentTransform.localScale = Vector3.one * randScale;
        _spawnedSegments.Add(segmentTransform);
        segmentTransform.localScale = Vector3.one * (_lastSegmentScale * 0.8f);
        Physics.SyncTransforms();
        
        AnimateSegmentSpawn(segmentTransform);
    }

    private float _lastSegmentScale = 1;
    private Vector3 PerformRaycast(Vector3 position, Vector3 direction)
    {
        if (Physics.Raycast(position, direction, out RaycastHit hit, direction.magnitude))
        {
            _lastSegmentScale = hit.transform.localScale.x;
            
            var hitPosition = hit.point;
            return hitPosition;
        }
        return Vector3.zero;
    }

    private void AnimateSegmentSpawn(Transform animatable)
    {
        if(segmentSpawnAnimTime <= 0) return;
        
        float to = animatable.localScale.x;
        animatable.localScale = Vector3.zero;
        
        Tween.Scale(animatable, to, segmentSpawnAnimTime, Ease.OutExpo);
    }

    // Thorns
    private readonly List<Transform> _weaponHolders = new();
    private void FindWeaponHolders()
    {
        int sides = 4;
        float angleStep = 360f / sides;
        
        for (int i = 0; i < sides; i++)
        {
            // знаходить точку на краю колайдера зі сторони кута
            float angle = i * angleStep;
            Vector3 sidePosition = Quaternion.Euler(0f, angle, 0f) * (Vector3.forward * thornHolderFindingRange);
            sidePosition = _centralChild.TransformPoint(sidePosition);
            
            // знаходить найближчий об'єкт до точки із заспавнених 
            Transform closestSegment = _spawnedSegments[0];
            foreach (var t in _spawnedSegments)
            {
                if (Vector3.Distance(sidePosition, t.position) < Vector3.Distance(sidePosition, closestSegment.position))
                {
                    closestSegment = t;
                }
            }
            _weaponHolders.Add(closestSegment);
        }
    }

    private List<Transform> _thorns = new();
    private void SpawnThorns()
    {
        FindWeaponHolders();
        
        foreach (var holder in _weaponHolders)
        {
            // find attack direction
            Vector3 attackDir = (holder.position - _centralChild.position).normalized;
            
            // angle between each thorn
            float angleStep = attackAngle / (thornsAmountOnEach-1);
            for (int i = 0; i < thornsAmountOnEach; i++)
            {
                // individual angle
                float orderAngle = i * angleStep;
                float finalAngle = -attackAngle / 2f + orderAngle; // add offset
                
                Vector3 thornDir = attackDir;
                // convert angle to direction with offset
                if(thornsAmountOnEach>1) thornDir = Quaternion.Euler(0f, finalAngle, 0f) * attackDir;
           
                Quaternion rotation = Quaternion.LookRotation(thornDir, Vector3.up);
                
                // spawn
                var spawned = Instantiate(thorn, holder.position, rotation, holder);
                _thorns.Add(spawned);
            }
        }
    }
}
