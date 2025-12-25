using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private int lootAmount;
    
    protected override void AnimateShake()
    {
        Tween.ShakeLocalPosition(transform, new Vector3(1f, 0f, 0f) * 0.7f, shakeDuration, 10f);
    }

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
