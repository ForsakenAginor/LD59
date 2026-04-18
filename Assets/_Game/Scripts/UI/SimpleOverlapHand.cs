using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

public class SimpleOverlapHand : MonoBehaviour
{
    private List<CardVisual> _cards = new List<CardVisual>();
    private List<CardVisual> _hand = new List<CardVisual>();

    [Header("Fan settings")]
    [SerializeField] private float _maxAngle = 15f; // Максимальный угол поворота крайних карт
    [SerializeField] private float _overlapAmount = 40f;
    [SerializeField] private RectTransform _deckRect;
    [SerializeField] private RectTransform _handRect;
    
    private Vector2 _deckPosition;
    private Vector2 _handPosition;
    private float _cardWidth = 0f;
    private bool _isSeted = false;
    private bool _needUpdate = false;

    private void Awake()
    {
        _deckPosition = _deckRect.anchoredPosition;
        _handPosition = _handRect.anchoredPosition;
    }

    private void LateUpdate()
    {
        if (_needUpdate)
            StartCoroutine(UpdateLayout());
    }

    public void SetNeedUpdate()
    {
        _needUpdate = true;
    }

    private IEnumerator UpdateLayout()
    {
        _needUpdate = false;
        yield return null;

        _hand.Clear();

        foreach (CardVisual card in _cards)
        {
            if(card != null)
                _hand.Add(card);
        }

        _cards.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            _cards.Add(child.GetComponent<CardVisual>());
        }

        yield return null;
        SetCardWidth();

        _cards = _cards.OrderByDescending(o => o.Card.Frequency).ToList();

        if (_cards.Count != 0)
        {
            // Автоматически вычисляем оверлап, если включено
            CalculateOverlapAmount();

            float totalWidth = _cardWidth + ((_cards.Count - 1) * (_cardWidth - _overlapAmount));
            float startX = -totalWidth / 2f;

            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                float angle = math.remap(0, _cards.Count - 1, _maxAngle, -_maxAngle, i);
                float x = startX + (i * (_cardWidth - _overlapAmount)) + _cardWidth / 2f;
                float yOffset = -Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * angle) * x);

                if (_hand.Contains(card))
                    card.RectTransform.anchoredPosition = _handPosition;
                else
                    card.RectTransform.anchoredPosition = _deckPosition;

                card.RectTransform.localRotation = Quaternion.identity;
                card.GetComponent<CardVisual>()
                    .SetTargetToMove(new Vector2(x, yOffset), Quaternion.Euler(0f, 0f, angle), i * 0.2f);
                // Применяем позицию и поворот
                //card.anchoredPosition = new Vector2(x, yOffset);
                //card.localRotation = Quaternion.Euler(0, 0, angle);

                card.RectTransform.SetAsLastSibling();
            }
        }
    }

    private void CalculateOverlapAmount()
    {
        if (_cards.Count <= 1)
        {
            _overlapAmount = 0f;
            return;
        }

        // Получаем доступную ширину контейнера
        var rectTransform = GetComponent<RectTransform>();
        float containerWidth = rectTransform.rect.width;

        // Если контейнер растянут и его ширина ещё не определена (0),
        // используем ширину родительского канваса
        if (containerWidth <= 0)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                containerWidth = canvas.GetComponent<RectTransform>().rect.width;
            }
        }

        // Минимальный зазор между картами (0 = плотная упаковка, можно сделать отрицательным для нахлёста)
        float minSpacing = 0f;

        // Вычисляем необходимый оверлап, чтобы все карты поместились
        // Формула: containerWidth = cardWidth + (cardsCount - 1) * (cardWidth - overlapAmount)
        // Решаем относительно overlapAmount:
        // overlapAmount = cardWidth - (containerWidth - cardWidth) / (cardsCount - 1)

        float requiredOverlap = _cardWidth - ((containerWidth - _cardWidth) / (_cards.Count - 1));

        // Ограничиваем оверлап разумными пределами
        // Максимальный оверлап (карты почти полностью перекрываются) - 90% ширины
        float maxOverlap = _cardWidth * 0.9f;
        // Минимальный оверлап (карты едва касаются) - 0 или небольшое отрицательное значение
        float minOverlap = -_cardWidth * 0.2f;

        _overlapAmount = Mathf.Clamp(requiredOverlap, minOverlap, maxOverlap);
    }

    private void SetCardWidth()
    {
        if (_isSeted)
            return;

        if (_cards.Count <= 0)
            return;

        var canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            float scaleFactor = canvas.scaleFactor;
            _cardWidth = _cards[0].RectTransform.rect.width * scaleFactor;
            _isSeted = true;
        }
    }
}