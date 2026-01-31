using System;
using PrimeTween;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public EventHandler OnLevelUpdate;
    public int AbilityChargeStep { get; private set; }
    public int HealthBonus { get; private set; }
    
    [SerializeField] private AnimationCurve expCurve;
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private float expIntakeTime;

    private int _currentLevel, _totalExperience;
    private int _prevLevelsExp, _nextLevelExp;
    private int _maxLevel;
    private int _experiencePool;
    private float _expIntakeTimer;
    private Sequence _levelingSequence;

    
    private void Awake()
    {
        InputSystem.actions.FindAction("Attack").performed += _ => AddExperience(1);
        _maxLevel = (int)expCurve.keys[^1].time;
        _expIntakeTimer = expIntakeTime;
        _currentLevel = (int)expCurve.keys[0].time;
    }

    private void Start()
    {
        UpdateLevel();
        UpdateUI();
    }

    private void Update()
    {
        float x = Mathf.InverseLerp(0, 3, _experiencePool);
        float intakeTime = Mathf.Lerp(expIntakeTime, expIntakeTime / 10f, x);
        
        if(_expIntakeTimer < intakeTime) _expIntakeTimer += Time.unscaledDeltaTime;
        else if (_experiencePool > 0)
        {
            _expIntakeTimer = 0;
            ScoreExperience();
            CheckForLevelUp();
            UpdateLevel();
        }
        else if (_expIntakeTimer < intakeTime * 2) 
        {
            UpdateUI(); // in case if exp stops at threshold (like 5/5) so UI can update 
            _expIntakeTimer = intakeTime * 3;
        }
    } 

    public void AddExperience(int amount)
    {
        _experiencePool += amount;
    }

    private void ScoreExperience()
    {
        _experiencePool--;
        _totalExperience++;
        UpdateUI();
    }
    
    private void CheckForLevelUp()
    {
        if(_totalExperience < _nextLevelExp) return;
        if (_currentLevel < _maxLevel)
        {
            _currentLevel++;
            OnLevelUpdate?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateLevel()
    {
        _prevLevelsExp = (int)expCurve.Evaluate(_currentLevel);
        _nextLevelExp = (int)expCurve.Evaluate(_currentLevel + 1);

        AbilityChargeStep = _currentLevel + 1;
        HealthBonus = _currentLevel;
    }

    private void UpdateUI()
    {
        int start = _totalExperience - _prevLevelsExp;
        int end = _nextLevelExp - _prevLevelsExp;
        
        levelText.text = _currentLevel.ToString();
        expText.text = start + " / " + end + " exp ";

        float fill = (float)start / end;

        if (_currentLevel == _maxLevel)
        {
            expText.text = start + " exp";
            levelText.text = _currentLevel + "(MAX)";
            fill = 1f;
        }

        float startScale = expFill.transform.localScale.x;
        float endScale = fill;

        if (startScale > endScale) startScale = 0f;
        
        Tween.Custom(startScale, endScale, expIntakeTime, f =>
        {
            expFill.transform.localScale = new Vector3(f, 1, 1);
        });
    }

    private float EaseFunc(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }
}
