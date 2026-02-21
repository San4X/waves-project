using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAimState : IState
{
    private Transform _target;
    private NavMeshAgent _navAgent;
    private float _angularSpeed, _baseAngularSpeed;
    private float _speed;


    public EnemyAimState(Transform target, NavMeshAgent navAgent, float angularSpeed, float speed)
    {
        _target = target;
        _navAgent = navAgent;
        _angularSpeed = angularSpeed;
        _speed = speed;
    }

    
    
    public void Tick()
    {
        _navAgent.SetDestination(_target.position);
    }

    public void OnEnter()
    {
        _baseAngularSpeed = _navAgent.angularSpeed;
        
        _navAgent.speed = _speed;
        _navAgent.angularSpeed = _angularSpeed;
    }

    public void OnExit()
    {
        _navAgent.angularSpeed = _baseAngularSpeed;
    }

    public bool StateCompleted()
    {
        Transform thisTransform = _navAgent.transform;
        Vector3 lookDir = thisTransform.TransformDirection(Vector3.forward);
        Vector3 dirToTarget = (_target.position - thisTransform.position).normalized;
        float angle = Vector3.Angle(lookDir, dirToTarget);

        return angle <= 5f;
    }
}
