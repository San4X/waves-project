using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Forward")]
    public float velocityStep; // by what value increase velocity when move action triggered
    public float maxForwardVelocity; // player cant accelerate more that that by move action (but can with ability)
    public float stopTime; // time need to pass to full stop
    public AnimationCurve velocityDamp; // how should velocity decrease from start to stopTime
    public float forwardSmooth; // how smooth it accelerates
    
    [Header("Side")]
    public float sideForce = 1f;
    public AnimationCurve sideVelMult_ForwardVel;
    public AnimationCurve sideVelMult_Time;
    public float rotationSpeed;

    private InputAction _moveAction;
    private Rigidbody _rb;
    private float _targetForwardVelocity;
    private float _calcForwardVelocity;
    private float _calcSideVelocity;
    private Tween _sideVelocityDampTween;
    private Tween _rotationTween;
    private float _velocityDampTimer; // from 0 to stopTime, 0 when move action triggered. Used in velocityDecreaseCurve
    private bool _dashing;
    private Vector3 _targetVelocity;


    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
        _velocityDampTimer = stopTime;
    }

    private void Update()
    {
        CalculateVelocity();
    }

    private void FixedUpdate()
    {
        
        _rb.linearVelocity = _targetVelocity;
        Rotate();
        // RotateDva();
    }

    private void CalculateVelocity()
    {
        ForwardAcceleration();
        ForwardDamp();
        SideVelocity();
        
        _calcForwardVelocity = Mathf.Lerp(
            _calcForwardVelocity,
            _targetForwardVelocity,
            forwardSmooth * Time.deltaTime
        );
        
        _targetVelocity.z = _calcForwardVelocity;
        _targetVelocity.x = _calcSideVelocity;
        _targetVelocity = transform.TransformDirection(_targetVelocity);
        
        if (_targetForwardVelocity < maxForwardVelocity) _dashing = false;
    }
    
    private void ForwardAcceleration()
    {
        if (!_moveAction.WasPerformedThisFrame() || _dashing) return;
        _targetForwardVelocity += velocityStep;
        if (_targetForwardVelocity > maxForwardVelocity) _targetForwardVelocity = maxForwardVelocity;
    }

    private float _maxReachedVelocity;
    private void ForwardDamp()
    {
        // begin of damp
        if (_moveAction.WasPerformedThisFrame() && !_dashing)
        {
            _velocityDampTimer = 0f;
            _maxReachedVelocity = _targetForwardVelocity;
        }
        
        // progression
        float delta = _moveAction.IsPressed() ? Time.deltaTime : Time.deltaTime * 2.5f;
        _velocityDampTimer += delta;

        // evaluation
        float curveTime = Mathf.InverseLerp(0f, stopTime, _velocityDampTimer);
        float curveValue = velocityDamp.Evaluate(curveTime);
        float newForwardVelocity = Mathf.Lerp(0f, _maxReachedVelocity, curveValue);

        _targetForwardVelocity = newForwardVelocity;
    }
    
    private float _sideVelToForwardDamp = 0;
    // private float _sideVelToTimeDamp = 0;
    private void SideVelocity()
    {
        float sideVel = _moveAction.ReadValue<float>(); // -1 / 0 / 1
        
        // Time based damp
        // if (_moveAction.WasPerformedThisFrame())
        // {
        //     _sideVelocityDampTween = Tween.Custom(0f, 1f, 1f, f =>
        //     {
        //         _sideVelToTimeDamp = sideVelMult_Time.Evaluate(f);
        //     });
        // }
        
        // Forward velocity based damp
        _sideVelToForwardDamp = sideVelMult_ForwardVel.Evaluate(VelocityCoef(0f, maxForwardVelocity));

        _calcSideVelocity = sideVel * sideForce * _sideVelToForwardDamp;
    }
    
    private void Rotate()
    {
        var vel = _rb.linearVelocity;
        if(vel.sqrMagnitude <= 0.01f) return; // bc of warning: look rotation vector is zero
        Quaternion targetRot = Quaternion.LookRotation(vel, Vector3.up);
        _rb.MoveRotation(targetRot);

        // this method in Update cuz .MoveRotation happens instantly and FixedUpdate frame rate affected by timeScale
    }

    private void RotateDva()
    {
        _rb.AddRelativeTorque(Vector3.up * 0.1f, ForceMode.Acceleration);
    }
    
    private float VelocityCoef(float minThreshold, float maxThreshold)
    {
        float speed = _calcForwardVelocity;
        float velocityCoef = Mathf.InverseLerp(minThreshold, maxThreshold, speed);

        return velocityCoef;
    }

    public void Dash(float force)
    {
        _dashing = true;
        _velocityDampTimer = 0f;
        _targetForwardVelocity += force;
        _maxReachedVelocity = _targetForwardVelocity;
    }
    
    private void OnDrawGizmos()
    {
        if(!_rb) return;
        Gizmos.DrawRay(transform.position, _rb.linearVelocity);
        Gizmos.color = Color.white;
    }
}
