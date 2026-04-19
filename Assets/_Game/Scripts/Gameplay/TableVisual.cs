using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.DI.Services.Global;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class TableVisual : MonoBehaviour
{
    private readonly List<TableCardVisual> _cards = new List<TableCardVisual>();

    [SerializeField] private TableCardVisual _cardVisualPrefab;
    [SerializeField] private RectTransform _cardsContainer;
    [SerializeField] private TableAnimation _tableAnimation;
    [SerializeField] private BossManager _bossManager;
    
    private IZenjectInstantiateWrapper _instantiateWrapper;

    [Header("Layout")]
    [SerializeField] private float _cardWidth = 200f;
    [SerializeField] private float _cardSpacing = 24f;
    [SerializeField] private float _flyInInterval = 0.08f;
    [SerializeField] private float _flyOutInterval = 0.06f;

    private CardTransferManager _cardTransferManager;
    private ScoreManager _scoreManager;
    private Table _table;

    [Inject]
    public void Construct(ScoreManager scoreManager, CardTransferManager transferManager, IZenjectInstantiateWrapper instantiateWrapper)
    {
        _scoreManager = scoreManager;
        _cardTransferManager = transferManager;
        _table = _cardTransferManager.Table;
        _instantiateWrapper = instantiateWrapper;

        _table.TableRefreshed += OnTableRefreshed;
        _table.TableCleared += OnTableCleared;
    }

    private void OnDestroy()
    {
        _table.TableRefreshed -= OnTableRefreshed;
        _table.TableCleared -= OnTableCleared;
    }

    private void OnTableCleared()
    {
        foreach (var card in _cards)
            if (card != null) Destroy(card.gameObject);
        _cards.Clear();
    }

    private void OnTableRefreshed()
    {
        var collection = _table.SelectedCards;
        int count = collection.Count;

        // Считаем стартовую X позицию чтобы карты были по центру
        float totalWidth = count * _cardWidth + (count - 1) * _cardSpacing;
        float startX = -totalWidth / 2f + _cardWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            var cardVisual = _instantiateWrapper.Instantiate(_cardVisualPrefab, _cardsContainer);
            cardVisual.Init(collection[i]);
            
            if(_bossManager.IsBossActive && (_bossManager.IsFrequencyBanned(collection[i].Frequency) || _bossManager.IsSuitBanned(collection[i].Suit)))
            {
                cardVisual.BlockCard();
            }

            // Выставляем позицию карты
            RectTransform rect = cardVisual.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + i * (_cardWidth + _cardSpacing), 0f);

            // Запускаем анимацию влёта с задержкой
            var anim = cardVisual.GetComponent<TableCardAnimation>();
            if (anim != null)
                anim.PlayFlyIn(i * _flyInInterval);


            _cards.Add(cardVisual);
        }

        StartCoroutine(PlayComboCoroutine());
    }

    private IEnumerator PlayComboCoroutine()
    {
        // Ждём пока все карты влетят
        float waitTime = _cards.Count * _flyInInterval + 0.45f;
        yield return new WaitForSeconds(waitTime);

        // Анимация комбо — шейк и очки
        yield return _tableAnimation.Animate(_cards);

        yield return new WaitForSeconds(0.2f);

        // Карты улетают по очереди слева направо
        for (int i = 0; i < _cards.Count; i++)
        {
            var anim = _cards[i].GetComponent<TableCardAnimation>();
            if (anim != null)
                StartCoroutine(anim.PlayFlyOut());

            yield return new WaitForSeconds(_flyOutInterval);
        }

        // Ждём пока последняя улетит
        yield return new WaitForSeconds(0.5f);

        _cardTransferManager.CommitTable();
    }
}