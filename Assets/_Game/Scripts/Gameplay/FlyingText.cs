using System.Collections;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class FlyingText: SwitchableElement
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _targetPosition = 160;
    [SerializeField] private float _duration = 1f;

    private ICombinationsConfiguration _combinationsConfiguration;
    
    [Inject]
    public void Construct(ICombinationsConfiguration configuration)
    {
        _combinationsConfiguration = configuration;
    }
    
    public IEnumerator Show(CombinationType combinationType)
    {
        _text.text = $"{_combinationsConfiguration.GetValue(combinationType).Name}";
        transform.localPosition = Vector3.zero;
        Color color = _text.color;
        color.a = 1;
        _text.color = color;
        color.a = 0;
        _text.DOColor(color, _duration).SetEase(Ease.InCirc);
        yield return transform.DOLocalMoveY(_targetPosition, _duration).WaitForCompletion();
    }
}