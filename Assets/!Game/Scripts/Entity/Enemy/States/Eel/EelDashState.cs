using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EelDashState : IState
{
    private Transform _target;
    private Rigidbody _rb;
    private float _speed;
    private bool _moveInert;
    private float _timer;
    private Vector3 _dirToLook;
    private Transform _transform;
    private AnimationCurve _dashClip;
    private readonly float _dashDuration;
    
    // obsolete
    private List<Vector3> _positions = new();
    private Vector3 _delayedPosition;
    private EelBehaviour _manager;
    private float _dashTargetDelay;

    public EelDashState(Transform target, EelBehaviour manager, float dashTargetDelay, Rigidbody rb, float speed, AnimationCurve dashClip, float dashDuration)
    {
        _target = target;
        _manager = manager;
        _dashTargetDelay = dashTargetDelay;
        _rb = rb;
        _speed = speed * 7f;
        _transform = rb.transform;
        _dashClip = dashClip;
        _dashDuration = dashDuration;
    }
    
    public void Tick()
    {
        //CalcDelayedPosition();
        //MoveToTarget();
        Dash();
    }

    private void CalcDelayedPosition()
    {
        // Дізнаємось delayed позицію target
        Vector3 pos = new Vector3(_target.position.x, _target.position.y, _target.position.z);
        _positions.Insert(0, pos);

        int framesPerDelay = Mathf.FloorToInt(_dashTargetDelay / Time.deltaTime);
        
        if (_positions.Count > framesPerDelay) // frames per specific time
        {
            _positions.RemoveRange(framesPerDelay, _positions.Count - framesPerDelay);
            _delayedPosition = new Vector3(_positions[^1].x, _positions[^1].y, _positions[^1].z);
        }
        
        _manager.drawPoint = _delayedPosition;
    }

    private void MoveToTarget()
    {
        Vector3 dirToLook = _delayedPosition - _rb.position;
        
        // Once reached target
        if(Vector3.Distance(_rb.position, _delayedPosition) < 1f || _moveInert)
        {
            dirToLook = _rb.linearVelocity;
            _moveInert = true;
            _timer -= Time.deltaTime;
        }
        
        _rb.rotation = Quaternion.LookRotation(dirToLook.normalized);
        _rb.AddRelativeForce(Vector3.forward * (Time.deltaTime * _speed * 5f), ForceMode.VelocityChange);
    }

    private void Dash()
    {
        if (Vector3.Distance(_rb.position, _target.position) > 3f && !_moveInert) _dirToLook = _target.position - _transform.position;
        else if (!_moveInert) _moveInert = true;
        
        _timer += Time.deltaTime;
        float time = Mathf.InverseLerp(0f, _dashDuration, _timer);
        float speedMult = _dashClip.Evaluate(time);
        
        _rb.rotation = Quaternion.LookRotation(_dirToLook.normalized);
        _rb.AddRelativeForce(Vector3.forward * (Time.deltaTime * _speed * speedMult), ForceMode.VelocityChange);
    }

    public bool Dashed()
    {
        return _timer >= _dashDuration;
    }
    
    public void OnEnter()
    {
        _dirToLook = _target.position;
        _moveInert = false;
        _timer = 0f;
    }

    public void OnExit()
    {
        
    }
}

// continiously get point that is delayed position of Player
    // delay of 0.2 sec
    // mark with gizmo
    // using array of vector2s that will be filled with positions
// move fast to that point
// after reaching continue moving along vector of velocity