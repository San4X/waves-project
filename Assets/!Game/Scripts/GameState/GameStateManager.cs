using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private Transform healBubble;
    [SerializeField] private Transform easyEel;
    [SerializeField] private Transform hardEel;
    private StateMachine _stateMachine;
    private ObjectSpawner _objectSpawner;
    private Transform _player;


    private void Awake()
    {
        _stateMachine = new StateMachine();
        _objectSpawner = FindAnyObjectByType<ObjectSpawner>();
        _player = FindAnyObjectByType<PlayerHealth>().transform;
        
        // states init
        var bubble1Spawn = new SpawnState(healBubble, _objectSpawner, 1, 2f, _player, 2f, 5f);
        var bubble2Spawn = new SpawnState(healBubble, _objectSpawner, 2, 0.5f, _player, 2f, 5f);
        var easyEnemy1Spawn = new SpawnState(easyEel, _objectSpawner, 1, 2f, _player, 15f, 15f);
        var easyEnemy2Spawn = new SpawnState(easyEel, _objectSpawner, 2, 1f, _player, 15f, 15f);
        var hardEnemySpawn = new SpawnState(hardEel, _objectSpawner, 1, 2f, _player, 15f, 15f);
        
        // state transitions
        At(bubble1Spawn, bubble2Spawn, bubble1Spawn.IsSpawnedGone);
        At(bubble2Spawn, easyEnemy1Spawn, bubble2Spawn.IsSpawnedGone);
        At(easyEnemy1Spawn, easyEnemy2Spawn, easyEnemy1Spawn.IsSpawnedGone);
        At(easyEnemy2Spawn, hardEnemySpawn, easyEnemy2Spawn.IsSpawnedGone);
        
        // set first state
        _stateMachine.SetState(bubble1Spawn);
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}

// (Start) Player starts with 1 hp
// (Start) 1 Bubble spawns near player [Object destroyed]
// 2 Bubbles spawns near player [Objects destroyed]

// easy version of Eel spawns [Object destroyed]
// upon death drops 2 Bubbles [Objects destroyed]

// hard version of Eel spawns
// upon death drops 5 bubbles [win]
