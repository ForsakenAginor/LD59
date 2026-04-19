using System;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] private TMP_Text _cardFrequency;
    //[SerializeField] private TMP_Text _cardSuit;
    [SerializeField] private Image _image;
    [SerializeField] private float _movingDuration = 0.2f;
    [SerializeField] private SwitchableElement _blockingImage;
    
    private Card _card;
    private Tween _movingTween;
    private Tween _rotatingTween;
    private RectTransform _rectTransform;
    private ICardValueConfiguration _configuration;
    private bool _isBlocked;

    public bool IsBlocked => _isBlocked;
    
    public event Action<CardVisual> OnClick;
    
    public Card Card => _card;
    
    public RectTransform RectTransform => _rectTransform;

    [Inject]
    public void Construct(ICardValueConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Card card)
    {
        _card = card;
        _image.sprite = _configuration.GetIcon(_card.Frequency, _card.Suit);
        /*
        string freqStr = card.Frequency switch
        {
            Frequency._100Hz => "100Hz",
            Frequency._200Hz => "200Hz",
            Frequency._300Hz => "300Hz",
            Frequency._400Hz => "400Hz",
            Frequency._500Hz => "500Hz",
            Frequency._600Hz => "600Hz",
            Frequency._700Hz => "700Hz",
            Frequency._800Hz => "800Hz",
            Frequency._900Hz => "900Hz",
            Frequency._1000Hz => "1000Hz",
            Frequency._1200Hz => "1200Hz",
            Frequency._1500Hz => "1500Hz",
            Frequency._2000Hz => "2000Hz",
            _ => "?"
        };

        string suitStr = card.Suit switch
        {
            Suit.Sine => "~",
            Suit.Saw => "^",
            Suit.Digital => "!",
            _ => "?"
        };

        _cardFrequency.text = freqStr;
        _cardSuit.text = suitStr;
        */
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void BlockCard()
    {
        _blockingImage.Enable();
        _isBlocked = true;
    }

    public void Enlarge()
    {
        transform.localScale = Vector3.one * 1.2f;
    }

    public void Normalize()
    {
        transform.localScale = Vector3.one;
    }

    public void SetTargetToMove(Vector2 targetPosition, Quaternion targetRotation, float delay)
    {
        if (_movingTween != null)
        {
            _movingTween.Kill();
        }

        if (_rotatingTween != null)
        {
            _rotatingTween.Kill();
        }
        
        _movingTween = _rectTransform.DOAnchorPos(targetPosition, _movingDuration).SetDelay(delay);
        _rotatingTween = _rectTransform.DORotateQuaternion(targetRotation, _movingDuration).SetDelay(delay);
    }
}