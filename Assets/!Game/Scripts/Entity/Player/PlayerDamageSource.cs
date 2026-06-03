using System;
using UnityEngine;

public class PlayerDamageSource : DamageSource
{
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override int GetDemageValue()
    {
        float velocity = _rb.linearVelocity.magnitude;
        float damage = Mathf.Ceil(velocity / 8f);
        return (int)damage;
    }
}
