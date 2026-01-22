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


    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
        _velocityDampTimer = stopTime;
    }

    private void Update()
    {
        CalculateVelocity();
        Rotate();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = transform.InverseTransformDirection(_rb.linearVelocity);
        velocity.z = _calcForwardVelocity;
        velocity.x = _calcSideVelocity;
        _rb.linearVelocity = transform.TransformDirection(velocity);
    }

    private void CalculateVelocity()
    {
        ForwardVelocity();
        ForwardVelocityDamp();
        SideVelocity();
        
        _calcForwardVelocity = Mathf.Lerp(
            _calcForwardVelocity,
            _targetForwardVelocity,
            forwardSmooth * Time.deltaTime
        );
    }
    
    private void ForwardVelocity()
    {
        if (!_moveAction.WasPerformedThisFrame()) return;
        _targetForwardVelocity += velocityStep;
        if (_targetForwardVelocity > maxForwardVelocity) _targetForwardVelocity = maxForwardVelocity;
    }

    private float _maxReachedVelocity;
    private void ForwardVelocityDamp()
    {
        if (_moveAction.WasPerformedThisFrame())
        {
            _velocityDampTimer = 0f;
            _maxReachedVelocity = _targetForwardVelocity;
        }
        _velocityDampTimer += Time.deltaTime;

        float curveTime = Mathf.InverseLerp(0f, stopTime, _velocityDampTimer);
        float curveValue = velocityDamp.Evaluate(curveTime);
        float newForwardVelocity = Mathf.Lerp(0f, _maxReachedVelocity, curveValue);

        _targetForwardVelocity = newForwardVelocity;
    }
    
    private float _sideVelToForwardDamp = 0;
    private float _sideVelToTimeDamp = 0;
    private void SideVelocity()
    {
        float sideVel = _moveAction.ReadValue<float>(); // -1 / 0 / 1
        
        // Time based damp
        if (_moveAction.WasPerformedThisFrame())
        {
            _sideVelocityDampTween = Tween.Custom(0f, 1f, 1f, f =>
            {
                _sideVelToTimeDamp = sideVelMult_Time.Evaluate(f);
            });
        }
        
        // Forward velocity based damp
        _sideVelToForwardDamp = sideVelMult_ForwardVel.Evaluate(VelocityCoef(0f, maxForwardVelocity));

        _calcSideVelocity = sideVel * sideForce * _sideVelToForwardDamp * _sideVelToTimeDamp;
    }
    
    private void Rotate()
    {
        Vector3 vel = _rb.linearVelocity;
        vel.y = 0;
        
        if(vel.sqrMagnitude <= 0.01f) return; // bc of warning: look rotation vector is zero
        
        Quaternion targetRot = Quaternion.LookRotation(vel.normalized, Vector3.up);
        
        _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, Time.deltaTime * rotationSpeed));
        // this method in Update cuz .MoveRotation happens instantly and FixedUpdate frame rate affected by timeScale
    }
    
    private float VelocityCoef(float minThreshold, float maxThreshold)
    {
        float speed = _calcForwardVelocity;
        float velocityCoef = Mathf.InverseLerp(minThreshold, maxThreshold, speed);

        return velocityCoef;
    }

    public void Dash(float force)
    {
        _velocityDampTimer = 0f;
        _calcForwardVelocity += force;
        _maxReachedVelocity = _calcForwardVelocity;
    }
}
