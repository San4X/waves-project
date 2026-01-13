using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : MonoBehaviour
{
    private InputAction _action;


    private void Awake()
    {
        _action = InputSystem.actions.FindAction("Ability");

        _action.performed += _ => ChangeTimeSpeed();
    }
    // charging by damaging if not charged yet
    // can trigger if charged: charging energy by spamming within time
    // after timer: addforce to player

    private void ChangeTimeSpeed()
    {
        if(Time.timeScale > 0.5f) Tween.Custom(Time.timeScale, 0.05f, 0.5f, f => Time.timeScale = f);
        else Tween.Custom(Time.timeScale, 1f, 0.5f, f => Time.timeScale = f);
    }
}
