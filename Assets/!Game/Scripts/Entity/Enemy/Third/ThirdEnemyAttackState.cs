using UnityEngine;
using UnityEngine.AI;

public class ThirdEnemyAttackState : IState
{
    private Transform _target;
    private NavMeshAgent _navAgent;
    private ThirdEnemyAnimations _animations;

    private float _stateDuration = 4f;
    private float _timer;
    private float _navUpdateRate = 1f;

    

    public ThirdEnemyAttackState(Transform target, NavMeshAgent navAgent, ThirdEnemyAnimations animations)
    {
        _target = target;
        _navAgent = navAgent;
        _animations = animations;
    }
    
    private float _lastUpdTime;
    public void Tick()
    {
        _timer -= Time.deltaTime;
        
        if(Time.time - _lastUpdTime < _navUpdateRate) return;
        _lastUpdTime = Time.time;
        _navAgent.SetDestination(_target.position);
    }

    public void OnEnter()
    {
        _timer = _stateDuration;
        _navAgent.speed = 3f;
        
        _animations.Agro();
    }

    public void OnExit()
    {
        _animations.Chill();
    }

    public bool StateCompleted()
    {
        return _timer <= 0f;
    }
}
