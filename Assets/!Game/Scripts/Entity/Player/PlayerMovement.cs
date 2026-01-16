using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float forwardForce = 1f;
    public float sideForce = 1f;
    public AnimationCurve sideForceDumpCurve;
    public float rotationSpeed = 1f;
    
    private InputAction _moveAction;
    private Rigidbody _rb;
    
    Tween _rotationTween;
    Transform _visualModel;
    
    
    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
        _visualModel = transform.GetChild(0);
    }

    private void Update()
    {
        Accelerate();
        Rotate();
        // AdditiveRotation();
    }

    void FixedUpdate()
    {
        SideForce();
    }

    private void Accelerate()
    {
        if(!_moveAction.WasPerformedThisFrame()) return;
        
        ForwardImpulse(forwardForce);
    }

    private void SideForce()
    {
        Vector3 sideVel = new Vector3();
        float side = _moveAction.ReadValue<float>();
        
        // Evaluate turn side
        if(side < 0)
            sideVel = Vector3.left;
        else if(side > 0) 
            sideVel = Vector3.right;
        
        // Calculate side velocity
        float dump = sideForceDumpCurve.Evaluate(VelocityCoef(0f, forwardForce*2));
        
        sideVel *= sideForce * dump; // side velocity based on: local forward, const, dump coef (if forward vel > forward impulse value, coef = 1)
        
        _rb.AddRelativeForce(sideVel);
    }

    private void Rotate()
    {
        Vector3 vel = _rb.linearVelocity;
        vel.y = 0;
        
        if(vel.sqrMagnitude <= 0.01f) return; // bc of warning: look rotation vector is zero
        
        Quaternion targetRot = Quaternion.LookRotation(vel.normalized, Vector3.up);

        _rb.MoveRotation(targetRot);
        // this method in Update cuz .MoveRotation happens instantly and FixedUpdate frame rate affected by timeScale
    }

    private void AdditiveRotation()
    {
        // animate model when fast
        float inputSide = _moveAction.ReadValue<float>();
        float angle = Mathf.Lerp(0f, 30f, VelocityCoef(12f, 18f));
        angle *= inputSide;
        
        Quaternion target = Quaternion.Euler(0, angle, 0);
        _visualModel.localRotation = Quaternion.RotateTowards(
            _visualModel.localRotation,
            target,
            300f * Time.deltaTime
        );
    }
    
    private float VelocityCoef(float minThreshold, float maxThreshold)
    {
        float velocity = transform.InverseTransformDirection(_rb.linearVelocity).z;
        float velocityCoef = Mathf.InverseLerp(minThreshold, maxThreshold, velocity);

        return velocityCoef;
    }

    private void OnDrawGizmos()
    {
        if(!_rb) return;
        Gizmos.DrawRay(transform.position, _rb.linearVelocity);
        Gizmos.color = Color.white;
    }

    public void ForwardImpulse(float force)
    {
        _rb.AddRelativeForce(Vector3.forward * force, ForceMode.Impulse);
    }
}