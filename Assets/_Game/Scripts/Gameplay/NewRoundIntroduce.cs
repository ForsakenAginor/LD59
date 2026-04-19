using System.Collections;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class NewRoundIntroduce : MonoBehaviour
{
    [SerializeField] private SwitchableElement _panel;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    [SerializeField] private float _duration = 2f;
    [SerializeField] private TMP_Text _scoreField;
    [SerializeField] private float _minTextSize = 35;
    [SerializeField] private float _maxTextSize = 70f;
    [SerializeField] private int _maxScore = 10000000;
    private int _current;
    private ILevelDifficultyConfiguration _configuration;

    [Inject]
    public void Construct(ILevelDifficultyConfiguration configuration)
    {
        _configuration = configuration;
        _current = _configuration.GetValue(LevelNumber.First);
        _scoreField.text = _current.ToString();
        _scoreField.fontSize = _minTextSize;
    }
    
    public IEnumerator SetNewThreshold(LevelNumber levelNumber)
    {
        _canvasGroup.alpha = 0f;
        _panel.Enable();
        yield return _canvasGroup.DOFade(1, _duration/2f).WaitForCompletion();
        int newThreshold = _configuration.GetValue(levelNumber);
        float remainingDuration = _duration;

        while (_current < newThreshold)
        {
            int goldToAdd = (int)((newThreshold - _current) * Time.deltaTime / remainingDuration);
            _current += goldToAdd;
            remainingDuration -= Time.deltaTime;

            if (_current > newThreshold)
                _current = newThreshold;

            _scoreField.text = _current.ToString();
            float fontSize = Mathf.Clamp(math.remap(0, _maxScore, _minTextSize, _maxTextSize, _current), _minTextSize, _maxTextSize);
            _scoreField.fontSize = fontSize;

            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
        _panel.Disable();
    }
}