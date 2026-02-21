using UnityEngine;
using UnityEngine.AI;

public class EnemyTiredFollowState : IState
{
    // has tired timer
    // sets speed, rotation speed, acceleration to tired
    // updates SetDestination as usual
    private Transform _target;
    private NavMeshAgent _navAgent;
    private float _speed, _angularSpeed, _acceleration;
    private float _tiredTime;
    private float _stateTimer;
    private float _navCooldown = 0.2f;

    private float _baseAngularSpeed, _baseAcceleration;
    
    
    public EnemyTiredFollowState(Transform target, NavMeshAgent navAgent, float tiredSpeed, float tiredAngular,
        float tiredAcceleration, float tiredTime)
    {
        _target = target;
        _navAgent = navAgent;
        _speed = tiredSpeed;
        _angularSpeed = tiredAngular;
        _acceleration = tiredAcceleration;
        _tiredTime = tiredTime;
    }

    private float _lastUpdTime;
    private void Follow()
    {
        if(Time.time - _lastUpdTime < _navCooldown) return;
        _lastUpdTime = Time.time;
        _navAgent.SetDestination(_target.position);
    }
    
    public void Tick()
    {
        Follow();
        _stateTimer -= Time.deltaTime;
    }

    public void OnEnter()
    {
        _baseAngularSpeed = _navAgent.angularSpeed;
        _baseAcceleration = _navAgent.acceleration;
        _stateTimer = _tiredTime;
        
        _navAgent.speed = _speed;
        _navAgent.angularSpeed = _angularSpeed;
        //_navAgent.acceleration = _acceleration;
    }

    public void OnExit()
    {
        _navAgent.angularSpeed = _baseAngularSpeed;
        _navAgent.acceleration = _baseAcceleration;
    }

    public bool StateCompleted()
    {
        return _stateTimer <= 0;
    }
}
