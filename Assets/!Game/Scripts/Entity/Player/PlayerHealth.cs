using System;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealth : Health
{
    [SerializeField] private RectTransform healthImg;
    [SerializeField] private RectTransform dumpedHealthImg;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color healEffectColor;
    private GameObject _lastHealObject;
    private int _baseMaxHealth;
    private int _healthBonus;


    private void Awake()
    {
        _baseMaxHealth = currentMaxHealth;
        FindAnyObjectByType<ExperienceManager>().OnLevelUpdate += LevelUp_Event;
    }

    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    public void Heal(int value)
    {
        ChangeHealth(value);
        
        AnimateColor(healEffectColor);
        
        UpdateUI();
    }

    protected override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        UpdateUI();
    }

    protected override void Death()
    {
        base.Death();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void UpdateUI()
    {
        healthText.text = CurrentHealth + "/" + currentMaxHealth;
        
        float newHealthPercent = Mathf.InverseLerp(0f, currentMaxHealth, CurrentHealth);
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

    private void LevelUp_Event(object sender, EventArgs args)
    {
        var manager = (ExperienceManager)sender;
        _healthBonus = manager.HealthBonus;
        currentMaxHealth = _baseMaxHealth + _healthBonus;
        Heal(1);
    }
}
