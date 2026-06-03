using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : IState
{
    // sets speed, acceleration to attack
    private Transform _target;
    private NavMeshAgent _navAgent;
    private float _speed;
    private float _acceleration, _baseAcceleration;
    private float _overkillDistance;


    public EnemyAttackState(Transform target, NavMeshAgent navAgent, float attackSpeed, float attackAcceleration, float overkillDistance)
    {
        _target = target;
        _navAgent = navAgent;
        _speed = attackSpeed;
        _acceleration = attackAcceleration;
        _overkillDistance = overkillDistance;
    }

    private Vector3 CalculateDestination()
    {
        var thisPosition = _navAgent.transform.position;
        var targetPosition = _target.position;

        var dir = (targetPosition - thisPosition).normalized;
        float distance = Vector3.Distance(thisPosition, targetPosition) + _overkillDistance;
        dir *= distance;

        return dir + thisPosition;
    }

    private void Dash(Vector3 destination)
    {
        //_navAgent.enabled = true;
        _navAgent.speed = _speed;
        _navAgent.acceleration = _acceleration;
        _navAgent.SetDestination(destination);
    }
    
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        _baseAcceleration = _navAgent.acceleration;
        
        var destination = CalculateDestination();
        Dash(destination);
    }

    public void OnExit()
    {
        _navAgent.acceleration = _baseAcceleration;
    }
    
    public bool StateCompleted()
    {
        return _navAgent.desiredVelocity.magnitude <= 0f;
    }
}
