using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DashAbility : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chargeAmountText;
    [SerializeField] private float countdownTime;
    [SerializeField] private Image timerImage;
    [SerializeField] private int amountToUnlock;
    [SerializeField] private Image iconFillImage;
    
    private InputAction _abilityTriggerAction;
    private InputAction _chargeAction;
    private InputAction _moveAction;
    private PlayerMovementController _playerMovement;

    private int _chargeCount;
    private bool _abilityActivated;
    private float _timer;
    private Tween _timeScaleTween;
    private int _unlockPointsCount;
    private int _chargeStep = 1;


    private void Awake()
    {
        _abilityTriggerAction = InputSystem.actions.FindAction("Ability");
        _chargeAction = InputSystem.actions.FindAction("Charge");
        _moveAction = InputSystem.actions.FindAction("Move");
        _playerMovement = FindAnyObjectByType<PlayerMovementController>();
        
        AbilityDisable();
        
        _abilityTriggerAction.performed += _ => ActivateAbility();
        FindAnyObjectByType<ExperienceManager>().OnLevelUpdate += LevelUp_Event;
    }

    private void Update()
    {
        if(!_abilityActivated) return;
        
        ChargeDash();
        UpdateUI(true);

        _timer += Time.unscaledDeltaTime;
        if (_timer >= countdownTime)
        {
            Dash();
        }
    }

    private void ActivateAbility()
    {
        if (_abilityActivated)
        {
            AbilityDisable();
            return;
        }
        if(_unlockPointsCount < amountToUnlock) return;
        
        _abilityActivated = true;
        _moveAction.Disable();
        _chargeAction.Enable();
        ChangeTimeSpeed(true);
    }

    private void Dash()
    {
        _playerMovement.Dash(_chargeCount);

        _unlockPointsCount = 0;
        AbilityDisable();
    }

    private float _lastChargeButton;
    private void ChargeDash()
    {
        float chargeButton = _chargeAction.ReadValue<float>();
        
        if(Mathf.Approximately(chargeButton, _lastChargeButton) || chargeButton == 0) return; // charge button should be opposite from last
        _lastChargeButton = chargeButton;

        _chargeCount += _chargeStep;
    }

    private void AbilityDisable()
    {
        _abilityActivated = false;
        
        _moveAction.Enable();
        _chargeAction.Disable();
        
        _chargeCount = 0;
        _lastChargeButton = 0f;
        _timer = 0f;
        
        UpdateUI(false);
        UpdateIconUI();
        
        ChangeTimeSpeed(false);
    }
    
    private void ChangeTimeSpeed(bool slowDown)
    {
        _timeScaleTween.Complete();
        if(slowDown) _timeScaleTween = Tween.Custom(Time.timeScale, 0.02f, 0.5f, f => Time.timeScale = f);
        else _timeScaleTween = Tween.Custom(Time.timeScale, 1f, 0.1f, f => Time.timeScale = f);
    }

    private void UpdateUI(bool active)
    {
        timerImage.transform.parent.gameObject.SetActive(active);
        if(!active) return;
        
        chargeAmountText.text = _chargeCount.ToString();
        float timerFillAmount = Mathf.InverseLerp(countdownTime, 0f, _timer);
        timerImage.fillAmount = timerFillAmount;
    }

    public void ChargeToUnlock()
    {
        if(_unlockPointsCount < amountToUnlock) _unlockPointsCount++;
        UpdateIconUI();
    }

    private void UpdateIconUI()
    {
        iconFillImage.fillAmount = Mathf.InverseLerp(0f, amountToUnlock, _unlockPointsCount);
    }

    private void LevelUp_Event(object sender, EventArgs args)
    {
        var manager = (ExperienceManager)sender;
        _chargeStep = manager.AbilityChargeStep;
    }
}
