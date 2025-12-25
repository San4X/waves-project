using PrimeTween;
using UnityEngine;
using UnityEngine.AI;

public class EelStalkState : IState
{
    private Transform _target;
    private NavMeshAgent _navAgent;
    private readonly float _maxSpeedDistance, _normalSpeedDistance, _stopDistance;
    private float _maxSpeed;
    private float _minSpeed;
    
    private float _lastUpdTime;
    private Tween _currentTween;
    private readonly float _navCooldown = 0.2f;


    public EelStalkState(Transform target, NavMeshAgent navAgent, float maxSpeed, float minSpeed, float maxSpeedDistance, float stalkSpeedDistance, float stopDistance)
    {
        _target = target;
        _navAgent = navAgent;
        _maxSpeed = maxSpeed;
        _minSpeed = minSpeed;

        _stopDistance = stopDistance;
        _maxSpeedDistance = maxSpeedDistance;
        _normalSpeedDistance = stalkSpeedDistance;
    }
    
    public void Tick()
    {
        Move();
    }

    private void Move()
    {
        AdjustMovementSpeed();
        if(Time.time - _lastUpdTime < _navCooldown) return;
        _lastUpdTime = Time.time;
        _navAgent.SetDestination(_target.position);
    }
    
    private void AdjustMovementSpeed()
    {
        float distanceToTraget = Vector3.Distance(_navAgent.transform.position, _target.position);
        float distanceCoef = Mathf.InverseLerp(_normalSpeedDistance, _maxSpeedDistance, distanceToTraget);

        float currentSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, distanceCoef);

        _navAgent.speed = currentSpeed;
    }
    
    // TODO: stop waving when turning
    
    public void OnEnter()
    {
        _navAgent.enabled = true;
        _navAgent.stoppingDistance = _stopDistance;
    }

    public void OnExit()
    {
        _navAgent.enabled = false;
    }
    
    public bool TargetReached()
    {
        return Vector3.Distance(_navAgent.transform.position, _target.position) <= _navAgent.stoppingDistance + 0.01f;
    }
}
