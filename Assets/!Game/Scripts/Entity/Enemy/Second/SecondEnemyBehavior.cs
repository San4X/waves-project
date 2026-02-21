using System;
using UnityEngine;
using UnityEngine.AI;

public class SecondEnemyBehavior : MonoBehaviour
{
    [Header("Aim State")] 
    [SerializeField] private float aimStateSpeed = 1;
    [SerializeField] private float aimStateRotationSpeed = 200;

    [Header("Attack State")] 
    [SerializeField] private float attackStateSpeed = 100;
    [SerializeField] private float attackStateAcceleration = 200;
    [SerializeField] private float overkillDistance = 8;
    
    private Transform _target;
    private NavMeshAgent _navAgent;
    private StateMachine _stateMachine;
    
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _stateMachine = new StateMachine();
        _target = FindAnyObjectByType<PlayerHealth>().transform;
        if(!_target) {Debug.Log("NoPlayer");return;}

        var aimState = new EnemyAimState(_target, _navAgent, aimStateRotationSpeed, aimStateSpeed);
        var attackState = new EnemyAttackState(_target, _navAgent, attackStateSpeed, attackStateAcceleration, overkillDistance);
        
        At(aimState, attackState, aimState.StateCompleted);
        At(attackState, aimState, attackState.StateCompleted);
        
        _stateMachine.SetState(aimState);
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}
