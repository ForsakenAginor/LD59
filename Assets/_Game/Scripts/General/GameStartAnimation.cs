using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameStartAnimation : MonoBehaviour
{
    [SerializeField] private Image _startButtonImage;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Button _startButton;
    
    [SerializeField] private Button[] _otherButtons;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private SwitchableElement _tooltip;
    
    private Tween _tween;
    private Color _originalColor;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        _startButton.onClick.AddListener(PlayAnimation);
    }

    private void Start()
    {
        _originalColor = _startButtonImage.color;
        _tween = _startButtonImage.DOColor(_targetColor, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        _coroutine = StartCoroutine(ShowTooltip());
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(PlayAnimation);
        
    }

    private IEnumerator ShowTooltip()
    {
        yield return new WaitForSeconds(10f);
        _tooltip.Enable();
    }

    private void PlayAnimation()
    {
        _startButton.interactable = false;
        _tween.Kill();
        StopCoroutine(_coroutine);
        _tooltip.Disable();
        _startButtonImage.color = _originalColor;
        _canvasGroup.DOFade(0f, 2f).SetEase(Ease.Linear);

        foreach (Button button in _otherButtons)
        {
            button.interactable = true;
        }
    }
}
