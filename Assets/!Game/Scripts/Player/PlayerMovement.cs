using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float forwardForce = 1f;
    public float turnForce = 0.5f;
    public AnimationCurve turnDumpCurve;
    
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
        RotateModel();
    }

    void FixedUpdate()
    {
        Turn();
    }

    private void Accelerate()
    {
        if(!_moveAction.WasPerformedThisFrame()) return;
        
        _rb.AddRelativeForce(Vector3.forward * forwardForce, ForceMode.Impulse);
    }

    private void Turn()
    {
        if (!_moveAction.IsPressed()) return;
        
        Vector3 sideVel = new Vector3();
        float side = _moveAction.ReadValue<float>();
        
        // Evaluate turn side
        if(side < 0)
            sideVel = Vector3.left;
        else if(side > 0) 
            sideVel = Vector3.right;
        
        // Calculate side velocity
        float forwardVel = transform.InverseTransformDirection(_rb.linearVelocity).z;// швидкість вперед по локальній сітці

        float forwardVelCoef = Mathf.InverseLerp(0f, forwardForce*2, forwardVel);
        float dump = turnDumpCurve.Evaluate(forwardVelCoef);
        
        sideVel *= turnForce * dump * Time.fixedDeltaTime; // side velocity based on: local forward, const, dump coef (if forward vel > forward impulse value, coef = 1)
        
        _rb.AddRelativeForce(sideVel, ForceMode.VelocityChange);
        // Rotate body
        if(forwardVelCoef <= 0.1f) return; // bc of warning: look rotation vector is zero
        Quaternion targetRot = Quaternion.LookRotation(_rb.linearVelocity.normalized, Vector3.up); // look at global velocity vector (move left -> -x -> look left)
        transform.rotation = targetRot;
    }

    private void RotateModel()
    {
        // more velocity - more rotation angle
        // speed of rotation покішо const
        //if (!_moveAction.IsPressed()) return;
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
}