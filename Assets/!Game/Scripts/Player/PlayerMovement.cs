using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float forwardForce = 1f;
    public float turnForce = 0.5f;
    public AnimationCurve turnDumpCurve;
    
    private InputAction _moveAction;
    private Rigidbody _rb;
    
    
    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Accelerate();
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

        float forwardVelCoef = Mathf.InverseLerp(0f, forwardForce, forwardVel);
        float dump = turnDumpCurve.Evaluate(forwardVelCoef);
        
        sideVel *= forwardVel * turnForce * dump * Time.fixedDeltaTime; // side velocity based on: local forward, setting, dump coef (if forward vel > forward impulse value, coef = 1)
        
        _rb.AddRelativeForce(sideVel, ForceMode.VelocityChange);
        
        // Rotate body
        if(forwardVelCoef == 0f) return; // bc of warning: look rotation vector is zero
        Quaternion targetRot = Quaternion.LookRotation(_rb.linearVelocity.normalized, Vector3.up); // look at global velocity vector (move left -> -x -> look left)
        transform.rotation = targetRot;
    }

    private void OnDrawGizmos()
    {
        if(!_rb) return;
        Gizmos.DrawRay(transform.position, _rb.linearVelocity);
        Gizmos.color = Color.white;
    }
}