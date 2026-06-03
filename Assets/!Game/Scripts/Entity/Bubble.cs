using System;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bubble : EntityHealth
{
    private PlayerHealth _playerHealth;


    protected override void Awake()
    {
        base.Awake();
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    protected override void Start()
    {
        base.Start();
        AnimateSpawn();
    }

    protected override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        
        HealPlayer();
    }

    private void HealPlayer()
    {
        _playerHealth.Heal(1);
    }

    private void AnimateSpawn()
    {
        transform.localScale = Vector3.zero;
        
        var randX = Random.Range(-1f, 1f);
        var randZ = Random.Range(-1f, 1f);
        Vector3 newPosition = transform.position + new Vector3(randX, 0f ,randZ) * 1.5f;
        
        Tween.LocalPosition(transform, newPosition, 0.7f, Ease.OutExpo);
        Tween.Scale(transform, 1f, 0.7f, Ease.OutBack);
    }
}
