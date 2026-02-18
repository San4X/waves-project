using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class FirstEnemyBehavior : MonoBehaviour
{
    [Header("Follow State")]
    [SerializeField] private float followSpeed;
    [SerializeField] private float farAwayDistance; // used for speed increasing
    
    [Header("Attack State")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackAcceleration;
    [SerializeField] private float overkillDistance;

    [Header("Tired State")] 
    [SerializeField] private float tiredSpeed;
    [SerializeField] private float tiredRotationSpeed;
    [SerializeField] private float tiredAcceleration;
    [SerializeField] private float tiredTime;
    
    private Transform _target;
    private NavMeshAgent _navAgent;
    private StateMachine _stateMachine;
    private Rigidbody _rb;

    [SerializeField] private TextMeshProUGUI debugText;

    // follow
    // chose point that is ? units more that players position
    // fast follow to that point
    // continue slow moving

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _stateMachine = new StateMachine();
        _rb = GetComponent<Rigidbody>();
        _target = FindAnyObjectByType<PlayerHealth>().transform;
        if(!_target) {Debug.Log("NoPlayer");return;}

        var followState = new EnemyFollowState(_target, _navAgent, attackDistance, farAwayDistance, followSpeed);
        var dashState = new EnemyAttackState(_target, _navAgent, attackSpeed, attackAcceleration, overkillDistance);
        var tiredState = new EnemyTiredFollowState(_target, _navAgent, tiredSpeed, tiredRotationSpeed,
            tiredAcceleration, tiredTime);
        
        At(followState, dashState, followState.StateCompleted);
        At(dashState, tiredState, dashState.StateCompleted);
        At(tiredState, followState, tiredState.StateCompleted);
        
        _stateMachine.SetState(followState);
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void Update()
    {
        _stateMachine.Tick();
        debugText.text = _stateMachine.GetCurrentStateText();
    }

    public Vector3 drawPoint;
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(drawPoint, Vector3.one);
        Gizmos.color = Color.black;
    }
}
