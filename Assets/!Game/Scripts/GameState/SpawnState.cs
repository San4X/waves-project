using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class SpawnState : IState
{
    private Transform _objectToSpawn;
    private int _quantity;
    private ObjectSpawner _objectSpawner;
    private Transform _player;
    private float _timeCountBeforeSpawn;
    private float _minRandOffset, _maxRandOffset;
    private List<Transform> _spawnedObjects = new();


    public SpawnState(Transform objectToSpawn, ObjectSpawner objectSpawner, int quantity, float timeCountBeforeSpawn,
        Transform player, float minRandOffset, float maxRandOffset)
    {
        _objectToSpawn = objectToSpawn;
        _objectSpawner = objectSpawner;
        _quantity = quantity;
        _timeCountBeforeSpawn = timeCountBeforeSpawn;
        _player = player;
        _minRandOffset = minRandOffset;
        _maxRandOffset = maxRandOffset;
    }
    
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        Sequence.Create(cycles: _quantity)
            .ChainDelay(_timeCountBeforeSpawn)
            .ChainCallback(Spawn);
    }

    public void OnExit()
    {
        _spawnedObjects = new List<Transform>();
    }

    private void Spawn()
    {
        var spawned = _objectSpawner.SpawnObject(_objectToSpawn, _player.position, _minRandOffset, _maxRandOffset);
        _spawnedObjects.Add(spawned);
    }

    public bool IsSpawnedGone()
    {
        if (_spawnedObjects.Count < _quantity) return false;
        
        foreach (var obj in _spawnedObjects)
        {
            if (obj != null) return false;
        }
        
        return true;
    }
}
