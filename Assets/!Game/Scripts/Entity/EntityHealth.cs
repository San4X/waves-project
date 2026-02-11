using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

public class EntityHealth : Health
{
    [SerializeField] private int lootAmount;
    [SerializeField] private int expAmount;
    private DashAbility _abilityManager;
    private ExperienceManager _experienceManager;


    protected virtual void Awake()
    {
        _abilityManager = FindAnyObjectByType<DashAbility>();
        _experienceManager = FindAnyObjectByType<ExperienceManager>();
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
        _experienceManager.AddExperience(expAmount);
    }

    private void SpawnLoot()
    {
        for (int i = 0; i < lootAmount; i++)
        {
            ObjectSpawner.Instance.SpawnObject(PrefabManager.Instance.bubble, transform.position, 0f, 0.1f);
        }
    }
}
