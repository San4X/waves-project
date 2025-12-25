using System;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealth : Health
{
    [SerializeField] private RectTransform healthImg;
    [SerializeField] private RectTransform dumpedHealthImg;
    [SerializeField] private Color healEffectColor;
    private GameObject _lastHealObject;
    

    public void Heal(int amount)
    {
        ChangeHealth(+1);
        
        StopAllCoroutines();
        AnimateColor(healEffectColor);
        
        UpdateUI();
    }
    
    protected override void TakeDamage()
    {
        ChangeHealth(-3);
        
        StopAllCoroutines();
        AnimateColor(damageEffectColor);
        
        UpdateUI();
    }

    protected override void Death()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void UpdateUI()
    {
        float newHealthPercent = Mathf.InverseLerp(0f, maxHealth, CurrentHealth);
        Vector3 newHealthImgScale = new Vector3(newHealthPercent, 1f, 1f);
        
        if(Mathf.Approximately(newHealthPercent, healthImg.localScale.x)) return;
        
        // if health reduces then front image scale will decrease instantly and back image scale will tween
        // if health increases then back image will increase instantly and front image will tween
        if (newHealthPercent < healthImg.localScale.x)
        {
            healthImg.localScale = newHealthImgScale;
            Tween.ScaleX(dumpedHealthImg, newHealthPercent, 0.5f, Ease.InQuad);
        }
        else
        {
            dumpedHealthImg.localScale = newHealthImgScale;
            Tween.ScaleX(healthImg, newHealthPercent, 0.5f, Ease.InQuad);
        }
    }
}
