using UnityEngine;
using UnityEngine.AI;

public class EelRelaxState : IState
{
    private Transform _transform;
    private NavMeshAgent _navAgent;
    private readonly float _speed;
    
    
    public EelRelaxState(Transform transform, NavMeshAgent navAgent)
    {
        _transform = transform;
        _navAgent = navAgent;
    }
    public void Tick()
    {
     
    }

    public void OnEnter()
    {
        _navAgent.enabled = true;
        _navAgent.stoppingDistance = 0f;
        _navAgent.speed = 1.5f;
        Vector3 offset = FindRandOffset();
        Vector3 position = _transform.position + _transform.TransformDirection(offset);
        _navAgent.SetDestination(position);
    }

    public void OnExit()
    {
        _navAgent.enabled = false;
    }

    private Vector3 FindRandOffset()
    {
        float randX = Random.Range(-1.0f, 1.0f);
        float randZ = Random.value;

        Vector3 randOffset = new Vector3(randX, 0f, randZ);
        randOffset.Normalize();
        randOffset *= 8f;
        
        return randOffset;
    }

    public bool TargetReached()
    {
        return Vector3.Distance(_navAgent.transform.position, _navAgent.destination) <= _navAgent.stoppingDistance + 1;
    }
}
