using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float cooldown = 1f;
    
    private InputAction _attackAction;
    private float _cdTimer;
    private bool _onCooldown;


    private void Awake()
    {
        _attackAction = InputSystem.actions.FindAction("Attack");
        _attackAction.performed += _ => Attack();
    }

    private void Update()
    {
        if(!_onCooldown) return;
        
        _cdTimer += Time.deltaTime;
        if (_cdTimer >= cooldown)
        {
            _onCooldown = false;
            _cdTimer = 0;
        }
    }

    private void Attack()
    {
        // handle cooldown
        if(_onCooldown) return;
        _onCooldown = true;
        
        // spawn bullet
        var bullet = PrefabManager.Instance.fishBullet;
        Instantiate(bullet, transform.position, transform.rotation);
        
        // svfx
    }
}
