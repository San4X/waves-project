using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private new Renderer renderer;
    [SerializeField] private Material effectMaterial;
    [SerializeField] protected Color damageEffectColor;
    [SerializeField] private float flashDuration;
    [SerializeField] protected float shakeDuration;
    [SerializeField] private float invincibleCd = 1f;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected string damagingObjectTag;
    private Material _effectMat;
    private float _timer;
    protected int CurrentHealth;


    protected virtual void Awake()
    {
        var mats = renderer.materials.ToList();
        mats.Add(effectMaterial);
        renderer.materials = mats.ToArray();
        _effectMat = renderer.materials[^1];
        
        _timer = invincibleCd;
        CurrentHealth = maxHealth;
    }
    
    private void Update()
    {
        if (_timer <= invincibleCd)
        {
            _timer += Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag(damagingObjectTag)) return;
        if (_timer < invincibleCd) return;
        _timer = 0f;
        
        TakeDamage();
    }

    protected virtual void TakeDamage()
    {
        ChangeHealth(-1);
        
        AnimateShake();
        
        StopAllCoroutines();
        //StartCoroutine(AnimateColor(damageEffectColor));
        AnimateColor(damageEffectColor);
    }

    protected void ChangeHealth(int value)
    {
        CurrentHealth += value;
        if (CurrentHealth <= 0)
        {
            Death();
        }

        if (CurrentHealth > maxHealth) CurrentHealth = maxHealth;
    }

    protected virtual void Death()
    {
        Tween.Scale(transform, 0f, shakeDuration, Ease.InQuad);
        Destroy(gameObject, shakeDuration);
    }

    protected virtual void AnimateShake()
    {
        Tween.ShakeLocalPosition(transform, new Vector3(1f, 0f, 1f) * 0.7f, shakeDuration, 10f);
    }
    
    protected void AnimateColor(Color effectColor)
    {
        Tween.Custom(1f, 0f, 0.8f, f => ChangeAlpha(_effectMat, effectColor, f));
    }

    private void ChangeAlpha(Material materialToChange, Color newColor, float value)
    {
        newColor.a = value;
        materialToChange.color = newColor;
    }
}
