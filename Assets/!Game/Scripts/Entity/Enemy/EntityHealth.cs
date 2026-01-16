using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

public class EntityHealth : Health
{
    [SerializeField] private int lootAmount;
    private DashAbility _abilityManager;


    protected override void Awake()
    {
        base.Awake();
        
        _abilityManager = FindAnyObjectByType<DashAbility>();
    }

    protected override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        
        _abilityManager.ChargeToUnlock();
        
        Vector3 popupPos = transform.position;
        popupPos.y += 1;
        WorldTextPopupFade.Create(popupPos, damageAmount.ToString());
    }

    // protected override void AnimateShake()
    // {
    //     Tween.ShakeLocalPosition(transform, new Vector3(1f, 0f, 0f) * 0.7f, shakeDuration, 10f);
    // }

    protected override void Death()
    {
        base.Death();
        SpawnLoot();
    }

    private void SpawnLoot()
    {
        for (int i = 0; i < lootAmount; i++)
        {
            ObjectSpawner.Instance.SpawnObject(PrefabManager.Instance.bubble, transform.position, 0f, 0.1f);
        }
    }
}
