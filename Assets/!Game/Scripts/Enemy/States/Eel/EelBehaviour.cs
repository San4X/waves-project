using System;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class EelBehaviour : MonoBehaviour
{
    [Header("Base")]
    private Transform target;
    [SerializeField] private Transform headBone;
    [SerializeField] private float maxSpeed; // when object far away
    private NavMeshAgent _navAgent;
    private StateMachine _stateMachine;
    private Tween _currentTween;
    private bool _isWavingLeft;

    private Vector3 _lastPos;
    public Vector3 RealVelocity { get; private set; }
    
    [Header("Stalk")]
    [SerializeField] private float stopDistance;
    [SerializeField] private float stalkSpeed; // when object in stalk area
    [SerializeField] private float maxSpeedDistance = 10f, stalkSpeedDistance = 7f;
    

    [Header("Dash")] 
    [SerializeField] private float dashTargetDelay = 1f;
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private float dashDuration = 1.2f;
    public Vector3 drawPoint;
    private Rigidbody _rb;
    
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _stateMachine = new StateMachine();
        _rb = GetComponent<Rigidbody>();
        _navAgent.stoppingDistance = stopDistance;
        target = FindAnyObjectByType<PlayerHealth>().transform;
        
        var stalkState = new EelStalkState(target, _navAgent, maxSpeed, stalkSpeed, maxSpeedDistance, stalkSpeedDistance, stopDistance);
        var dashState = new EelDashState(target, this, dashTargetDelay, _rb, maxSpeed, dashSpeedCurve, dashDuration);
        var relaxState = new EelRelaxState(transform, _navAgent);
        
        At(stalkState, dashState, stalkState.TargetReached);
        At(dashState, relaxState, dashState.Dashed);
        At(relaxState, stalkState, relaxState.TargetReached);
        
        _stateMachine.SetState(stalkState);
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }
    
    private void Update()
    {
        _stateMachine.Tick();
        AnimateBody();
    }

    private void FixedUpdate()
    {
        CalculateRealVelocity();
    }

    private void AnimateBody()
    {
        float velocityCoef = VelocityCoef();
        float timeScale = Mathf.Lerp(0f, 3f, velocityCoef);
        float angle = Mathf.Lerp(9f, 15f, velocityCoef);
        // if (!_navAgent.enabled)
        // {
        //     timeScale = 3f;
        //     angle = 10f;
        // }
        WaveAnimation(angle, 0.2f, timeScale);
    }
    
    private void WaveAnimation(float angleAmplitude, float baseSpeed, float timeScale)
    {
        if (_currentTween.isAlive)
        {
            _currentTween.timeScale = timeScale;
            return;
        }
        if (RealVelocity.magnitude <= 0.01f)  return;
        
        angleAmplitude = _isWavingLeft ? -angleAmplitude : angleAmplitude;
        _isWavingLeft = !_isWavingLeft;

        Vector3 rotation = new Vector3(0f, angleAmplitude, 0f);

        TweenSettings settings = new TweenSettings(baseSpeed, Ease.OutQuad, 2, CycleMode.Rewind);

        _currentTween = Tween.LocalRotation(headBone, rotation, settings);
    }

    private float VelocityCoef()
    {
        float velocity = RealVelocity.magnitude;
        float velocityCoef = Mathf.InverseLerp(0f, maxSpeed, velocity);

        return velocityCoef;
    }

    private void CalculateRealVelocity()
    {
        RealVelocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        _lastPos = transform.position;
    }
}

// 🎈 slow approaching (stalk) to Player
// if close enough, fast accelerating towards Player with slight aim adjustment and passing through
    // move to point that follows Player with delay, when reached continue moving along same trajectory but slower
// recover and slow fleeing to random spot -- VULNERABLE