using System;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bubble : Health
{
    private PlayerHealth _playerHealth;
    private PlayerMovement _playerMovement;


    protected override void Awake()
    {
        base.Awake();

        transform.localScale = Vector3.zero;
        AnimateSpawn();
    }

    private void Start()
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
        _playerMovement = _playerHealth.GetComponent<PlayerMovement>();
    }

    protected override void TakeDamage()
    {
        HealPlayer();
        
        int damageAmount = (int)_playerMovement.GetDamageValue();
        ChangeHealth(-damageAmount);
        
        Vector3 popupPos = transform.position;
        popupPos.y += 1;
        WorldTextPopupFade.Create(popupPos, damageAmount.ToString());
        
        // same
        AnimateShake();
        AnimateColor(damageEffectColor);
    }

    private void HealPlayer()
    {
        _playerHealth.Heal(1);
    }

    private void AnimateSpawn()
    {
        var randX = Random.Range(-1f, 1f);
        var randZ = Random.Range(-1f, 1f);
        Vector3 newPosition = transform.position + new Vector3(randX, 0f ,randZ) * 1.5f;
        
        Tween.LocalPosition(transform, newPosition, 0.7f, Ease.OutExpo);
        Tween.Scale(transform, 1f, 0.7f, Ease.OutBack);
    }
}
