using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;
using Unity.Mathematics;

public class TablePreview : MonoBehaviour
{
    [SerializeField] private float _maxMultiplier = 5f;
    [SerializeField] private float _maxBase = 5000;
    
    [SerializeField] private TMP_Text _comboName;
    [SerializeField] private TMP_Text _base;
    [SerializeField] private TMP_Text _multiplier;
    [SerializeField] private HandVisual _hand;

    [SerializeField] private float _duration;
    private int _currentBase = 0;
    private float _currentMultiplier = 1f;
    private Coroutine _baseCoroutine;
    private Coroutine _multiplierCoroutine;
    private int _targetBase = 0;
    private float _targetMultiplier = 1f;

    private ICombinationsConfiguration _combinationsConfiguration;

    [Inject]
    public void Construct(ICombinationsConfiguration configuration)
    {
        _combinationsConfiguration = configuration;
    }

    private void Start()
    {
        ClearPreview();
        _hand.SelectedCardsChanged += OnSelectedCardsChanged;
    }

    private void OnDestroy()
    {
        _hand.SelectedCardsChanged -= OnSelectedCardsChanged;
    }

    public void AddTargetValues(int targetBase, float targetMultiplier)
    {
        if (targetBase != 0)
        {
            _targetBase += targetBase;
            OnBaseChanged();
        }

        if (targetMultiplier != 0)
        {
            _targetMultiplier += targetMultiplier;
            OnMultiplierChanged();
        }
    }

    public void ClearPreview()
    {
        _comboName.text = "";
        _base.text = "0";
        _multiplier.text = "0";
        _currentBase = 0;
        _currentMultiplier = 0f;
        _base.fontSize = 20f;
        _multiplier.fontSize = 20f;
    }

    private void OnSelectedCardsChanged(PreviewData data)
    {
        if (data != null)
        {
            _comboName.text = _combinationsConfiguration.GetValue(data.Type).Name;
            _base.text = data.BaseValue.ToString();
            _multiplier.text = data.Multiplier.ToString("0.00");
        }
        else
        {
            ClearPreview();
        }
    }

    private void OnBaseChanged()
    {
        if (_baseCoroutine != null)
            StopCoroutine(_baseCoroutine);

        _baseCoroutine = StartCoroutine(ChangeBase());
    }

    private void OnMultiplierChanged()
    {
        if (_multiplierCoroutine != null)
            StopCoroutine(_multiplierCoroutine);

        _multiplierCoroutine = StartCoroutine(ChangeMultiplier());
    }

    private IEnumerator ChangeBase()
    {
        float remainingDuration = _duration;

        while (_currentBase < _targetBase)
        {
            int goldToAdd = (int)((_targetBase - _currentBase) * Time.deltaTime / remainingDuration);
            _currentBase += goldToAdd;
            remainingDuration -= Time.deltaTime;

            if (_currentBase > _targetBase)
                _currentBase = _targetBase;

            _base.text = _currentBase.ToString();
            _base.fontSize = math.remap(0, _maxBase, 20, 35, _currentBase);

            yield return null;
        }
    }

    private IEnumerator ChangeMultiplier()
    {
        float remainingDuration = _duration;

        while (_currentMultiplier < _targetMultiplier)
        {
            float goldToAdd = ((_targetMultiplier - _currentMultiplier) * Time.deltaTime / remainingDuration);
            _currentMultiplier += goldToAdd;
            remainingDuration -= Time.deltaTime;

            if (_currentMultiplier > _targetMultiplier)
                _currentMultiplier = _targetMultiplier;

            _multiplier.text = _currentMultiplier.ToString("0.00");
            _multiplier.fontSize = math.remap(0, _maxMultiplier, 20, 35, _currentMultiplier);

            yield return null;
        }
    }
}