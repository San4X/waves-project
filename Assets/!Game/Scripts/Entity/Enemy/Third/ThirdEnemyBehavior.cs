using System;
using UnityEngine;
using UnityEngine.AI;

public class ThirdEnemyBehavior : MonoBehaviour
{
    [Header("Follow State")] 
    [SerializeField] private float speed;
    
    private Transform _target;
    private NavMeshAgent _navAgent;
    private StateMachine _stateMachine;
    private ThirdEnemyAnimations _animations;


    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _stateMachine = new StateMachine();
        _target = FindAnyObjectByType<PlayerHealth>().transform;
        _animations = GetComponent<ThirdEnemyAnimations>();
        if(!_target) {Debug.Log("NoPlayer");return;}
        
        var followState = new EnemyFollowState(_target, _navAgent, 3f, 15f, speed, speed*3f);
        var attackState = new ThirdEnemyAttackState(_target, _navAgent, _animations);
        
        At(followState, attackState, followState.StateCompleted);
        At(attackState, followState, attackState.StateCompleted);
        
        _stateMachine.SetState(followState);
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}
