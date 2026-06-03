using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThirdEnemyAnimations : MonoBehaviour
{
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrenght;

    private BlobBody _body;
    private List<Transform> _allThorns;
    private List<Transform> _alwaysActiveThorns;
    private float _baseThornScale;


    private void Awake()
    {
        _body = GetComponent<BlobBody>();
        _body.OnWeaponCreated += BodyOnOnWeaponCreated;

        _allThorns = new List<Transform>();
        _alwaysActiveThorns = new List<Transform>();
    }

    private void BodyOnOnWeaponCreated(object sender, EventArgs e)
    {
        _allThorns = _body._thorns;
        
        for (int i = 0; i < _allThorns.Count; i++)
        {
            if(i % 3 == 0) _alwaysActiveThorns.Add(_allThorns[i]);
        }

        _baseThornScale = _allThorns[0].localScale.x;
        
        //Chill();
        Agro();
    }

    public void Agro()
    {
        // shake
        ShakeBody(shakeDuration, shakeStrenght);
        // scale up all thorns
        ScaleWeapon(_allThorns, _baseThornScale*0.8f, _baseThornScale*1.5f, shakeDuration/3f, shakeDuration/2f);
    }

    public void Chill()
    {
        // short shake
        ShakeBody(shakeDuration/2f, shakeStrenght);
        
        // hide thorns
        ScaleWeapon(_allThorns, 0.1f, 0.1f, shakeDuration);
        
        // set 1/3 weapon half/active
        ScaleWeapon(_alwaysActiveThorns, _baseThornScale/2f, _baseThornScale/2f, shakeDuration);
    }

    private void ShakeBody(float duration, float strenght)
    {
        Tween.ShakeLocalPosition(transform, new Vector3(1f, 0, 1f) * strenght, duration, 25f);
    }

    private void ScaleWeapon(List<Transform> list, float minValue, float maxValue, float duration, float startDelay = 0f)
    {
        foreach (var weapon in list)
        {
            float endValue = Random.Range(minValue, maxValue);
            
            Tween.ScaleZ(weapon, endValue, duration, startDelay: startDelay, ease: Ease.OutExpo);
        }
    }
}
