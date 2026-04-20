using System.Collections;
using Assets.Source.Scripts.AudioLogic;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NewRoundIntroduce : MonoBehaviour
{
    [SerializeField] private Image _interferenceBorder;
    [SerializeField] private TMP_Text _interferenceText;
    [SerializeField] private SwitchableElement _interferenceAlert;
    [SerializeField] private BossManager _bossManager;
    [SerializeField] private SwitchableElement _panel;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _duration = 2f;
    [SerializeField] private TMP_Text _scoreField;
    [SerializeField] private float _minTextSize = 35;
    [SerializeField] private float _maxTextSize = 70f;
    [SerializeField] private int _maxScore = 10000000;
    private int _current;
    private ILevelDifficultyConfiguration _configuration;
    private bool _isSetInterference = false;

    private Tween _interferenceTween;
    private Color _interferenceColor;

    [Inject]
    public void Construct(ILevelDifficultyConfiguration configuration)
    {
        _configuration = configuration;
        _interferenceColor = _interferenceBorder.color;
        _current = _configuration.GetValue(LevelNumber.First);
        _interferenceText.text = "";
        _scoreField.text = _current.ToString();
        _scoreField.fontSize = _minTextSize;
    }

    public IEnumerator SetNewThreshold(LevelNumber levelNumber)
    {
        _isSetInterference = false;
        _canvasGroup.alpha = 0f;
        _panel.Enable();
        yield return _canvasGroup.DOFade(1, _duration / 3f).SetEase(Ease.Linear).WaitForCompletion();
        int newThreshold = _configuration.GetValue(levelNumber);
        float remainingDuration = _duration;
        AudioPlayer.Instance.PlayNewRoundPrepare();

        while (_current < newThreshold)
        {
            int goldToAdd = (int)((newThreshold - _current) * Time.deltaTime / remainingDuration);
            _current += goldToAdd;
            remainingDuration -= Time.deltaTime;

            if (_current > newThreshold)
                _current = newThreshold;

            if (_isSetInterference == false && _current > newThreshold * 0.7f)
                SetInterference();

            _scoreField.text = _current.ToString();
            float fontSize = Mathf.Clamp(math.remap(0, _maxScore, _minTextSize, _maxTextSize, _current), _minTextSize,
                _maxTextSize);
            _scoreField.fontSize = fontSize;

            yield return null;
        }

        SetInterference();


        yield return new WaitForSeconds(1f);
        _panel.Disable();
        _interferenceAlert.Disable();
    }

    private void SetInterference()
    {
        if (_bossManager.IsBossActive)
        {
            AudioPlayer.Instance.PlayInterferenceAlert();
            _interferenceAlert.Enable();
            _interferenceText.text = _bossManager.GetDescription();

            if (_interferenceTween != null)
            {
                _interferenceTween.Kill();
            }

            _interferenceBorder.color = Color.white;
            _interferenceTween = _interferenceBorder.DOColor(Color.red, 0.5f).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _interferenceAlert.Disable();
            _interferenceText.text = "";
            _interferenceBorder.color = _interferenceColor;

            if (_interferenceTween != null)
            {
                _interferenceTween.Kill();
            }
        }

        _isSetInterference = true;
    }
}