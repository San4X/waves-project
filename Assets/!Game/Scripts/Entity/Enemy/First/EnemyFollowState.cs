using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowState : IState
{
    // sets speed, rotation speed, acceleration to follow
    // updates SetDestination as usual
    private Transform _target;
    private NavMeshAgent _navAgent;
    private float _attackDistance, _farAwayDistance;
    private float _followSpeed;
    private float _navCooldown = 0.2f;
    private float _maxSpeed;


    public EnemyFollowState(Transform target, NavMeshAgent navAgent, float attackDistance, float farAwayDistance,
        float followSpeed, float maxSpeed)
    {
        _target = target;
        _navAgent = navAgent;
        _attackDistance = attackDistance;
        _farAwayDistance = farAwayDistance;
        _followSpeed = followSpeed;
        _maxSpeed = maxSpeed;
    }

    private float _lastUpdTime;
    private void Follow()
    {
        if(Time.time - _lastUpdTime < _navCooldown) return;
        _lastUpdTime = Time.time;
        _navAgent.SetDestination(_target.position);
    }

    private void AdjustSpeed()
    {
        float distanceToTarget = _navAgent.remainingDistance;
        float percent = Mathf.InverseLerp(_farAwayDistance / 2f, _farAwayDistance, distanceToTarget);
        float x = percent * percent * percent * percent;
        float newSpeed = Mathf.Lerp(_followSpeed, _maxSpeed, x);
        _navAgent.speed = newSpeed;
    }
    
    public virtual void Tick()
    {
        Follow();
        AdjustSpeed();
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public bool StateCompleted()
    {
        float distance = Vector3.Distance(_navAgent.transform.position, _target.position);
        return distance <= _attackDistance;
    }
}
