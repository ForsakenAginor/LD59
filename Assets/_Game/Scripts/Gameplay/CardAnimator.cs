using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Float")]
    [SerializeField] private float _floatAmplitude = 5f;
    [SerializeField] private float _floatSpeed = 1.1f;

    [Header("Hover")]
    [SerializeField] private float _hoverLift = 25f;
    [SerializeField] private float _hoverDuration = 0.2f;

    [Header("Selected")]
    [SerializeField] private float _selectedLift = 40f;
    [SerializeField] private float _selectedBobAmplitude = 3f;
    [SerializeField] private float _selectedBobSpeed = 1.5f;

    private RectTransform _rect;
    private CardVisual _cardVisual;
    private float _floatOffset;
    private bool _isHovered;
    private bool _isSelected;
    private Tween _moveTween;
    private Tween _scaleTween;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        // Берём CardVisual с родителя — он на корневом Card
        _cardVisual = GetComponentInParent<CardVisual>();
        _floatOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Start()
    {
        if (_cardVisual != null)
            _cardVisual.OnClick += OnCardClicked;
    }

    private void OnDestroy()
    {
        if (_cardVisual != null)
            _cardVisual.OnClick -= OnCardClicked;
        _moveTween?.Kill();
        _scaleTween?.Kill();
    }

    private void Update()
    {
        // Не флоатим пока идёт tween анимация
        if (_moveTween != null && _moveTween.IsPlaying()) return;
        if (_isHovered) return;

        float amplitude = _isSelected ? _selectedBobAmplitude : _floatAmplitude;
        float speed     = _isSelected ? _selectedBobSpeed     : _floatSpeed;
        float baseY     = _isSelected ? _selectedLift : 0f;

        // Двигаем только локальную позицию — не трогаем якоря Layout Group
        float yOffset = Mathf.Sin(Time.time * speed + _floatOffset) * amplitude;
        float xOffset = Mathf.Sin(Time.time * speed * 0.5f + _floatOffset + 1f) * amplitude * 0.2f;
        float tiltZ   = Mathf.Sin(Time.time * speed * 0.6f + _floatOffset) * 2f;

        _rect.localPosition = new Vector3(xOffset, baseY + yOffset, 0f);
        _rect.localRotation = Quaternion.Euler(0f, 0f, tiltZ);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected) return;
        _isHovered = true;

        _moveTween?.Kill();
        _scaleTween?.Kill();

        _rect.localRotation = Quaternion.identity;
        _moveTween = _rect.DOLocalMoveY(_hoverLift, _hoverDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => _moveTween = null);
        _scaleTween = _rect.DOScale(1.06f, _hoverDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected) return;
        _isHovered = false;

        _moveTween?.Kill();
        _scaleTween?.Kill();

        _moveTween = _rect.DOLocalMoveY(0f, _hoverDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => _moveTween = null);
        _scaleTween = _rect.DOScale(1f, _hoverDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnCardClicked(CardVisual card)
    {
        _isSelected = !_isSelected;
        _isHovered = false;

        _moveTween?.Kill();
        _scaleTween?.Kill();

        if (_isSelected)
        {
            _moveTween = _rect.DOLocalMoveY(_selectedLift, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _moveTween = null);
            _scaleTween = _rect.DOScale(1.08f, 0.25f)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _moveTween = _rect.DOLocalMoveY(0f, 0.2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _moveTween = null);
            _scaleTween = _rect.DOScale(1f, 0.2f)
                .SetEase(Ease.OutQuad);
        }
    }

    public void Deactivate()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        _rect.localPosition = Vector3.zero;
        _rect.localRotation = Quaternion.identity;
        _rect.localScale = Vector3.one;
        enabled = false;
    }
}