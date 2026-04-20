using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.DI.Services.Global;
using TMPro;
using UnityEngine;
using Zenject;

public class HandVisual : MonoBehaviour
{
    private const int TableSize = 5;

    private readonly Dictionary<Card, CardVisual> _cards = new Dictionary<Card, CardVisual>();
    private readonly List<CardVisual> _selectedCards = new List<CardVisual>();

    [SerializeField] private TMP_Text _selectionText;
    [SerializeField] private BossManager _bossManager;
    [SerializeField] private CardVisual _cardVisualPrefab;
    [SerializeField] private Transform _cardsInHandContainer;
    [SerializeField] private SimpleOverlapHand _layout;
    
    private ScoreManager _scoreManager;
    private CardTransferManager _cardTransferManager;
    private Hand _hand;
    private bool _isBlocked;
    private CombinationResult _combination;
    private IZenjectInstantiateWrapper _instantiateWrapper;

    public event Action<PreviewData> SelectedCardsChanged;

    public bool CanPlayHand => _selectedCards.Count > 0;

    public CombinationResult Combination => _combination;
    
    [Inject]
    public void Construct(ScoreManager scoreManager, CardTransferManager transferManager, IZenjectInstantiateWrapper instantiateWrapper)
    {
        _scoreManager = scoreManager;
        _cardTransferManager = transferManager;
        _instantiateWrapper = instantiateWrapper;
        _hand = _cardTransferManager.Hand;

        _hand.CardAdded += OnCardAdded;
        _hand.CardRemoved += OnCardRemoved;
    }

    private void OnDestroy()
    {
        _hand.CardAdded -= OnCardAdded;
        _hand.CardRemoved -= OnCardRemoved;
    }

    public void PlaySelectedCards()
    {
        _isBlocked = true;
        IEnumerable<Card> cards = _selectedCards.Select(card => card.Card);
        _cardTransferManager.MoveToTable(cards);
        _selectedCards.Clear();
        _selectionText.text = "0/5";
        CallSelectedCardsChanged();
    }

    public void DiscardSelectedCards()
    {
        IEnumerable<Card> cards = _selectedCards.Select(card => card.Card);
        _hand.RemoveCards(cards);
        _selectedCards.Clear();
        _selectionText.text = "0/5";
        CallSelectedCardsChanged();
    }

    private void OnCardAdded(Card card)
    {
        var cardVisual = _instantiateWrapper.Instantiate(_cardVisualPrefab, _cardsInHandContainer);
        cardVisual.Init(card);
            
        if(_bossManager.IsBossActive && (_bossManager.IsFrequencyBanned(card.Frequency) || _bossManager.IsSuitBanned(card.Suit)))
        {
            cardVisual.BlockCard();
        }
        
        _cards.Add(card, cardVisual);
        cardVisual.OnClick += OnCardClick;
        _isBlocked = false;
        _layout.SetNeedUpdate();
    }

    private void OnCardRemoved(Card card)
    {
        if (_cards.ContainsKey(card) == false)
            throw new Exception("Card not found");

        _cards[card].OnClick -= OnCardClick;
        Destroy(_cards[card].gameObject);
        _cards.Remove(card);
        //_layout.SetNeedUpdate();
    }

    private void OnCardClick(CardVisual cardVisual)
    {
        if (_isBlocked)
            return;

        if (_selectedCards.Contains(cardVisual))
        {
            cardVisual.Normalize();
            _selectedCards.Remove(cardVisual);
            _selectionText.text = $"{_selectedCards.Count}/5";
            CallSelectedCardsChanged();
        }
        else if (_selectedCards.Count < TableSize)
        {
            cardVisual.Enlarge();
            _selectedCards.Add(cardVisual);
            _selectionText.text = $"{_selectedCards.Count}/5";
            CallSelectedCardsChanged();
        }
    }

    private void CallSelectedCardsChanged()
    {
        if (_selectedCards.Count > 0)
        {
            _scoreManager.CalculatePreview(_selectedCards.Select(o => o.Card).ToList(), out int baseScore,
                out float multiplier, out CombinationType combinationType, out CombinationResult combination);
            _combination = combination;
            SelectedCardsChanged?.Invoke(new PreviewData(combinationType, multiplier, baseScore));
        }
        else
        {
            SelectedCardsChanged?.Invoke(null);
        }
    }
}

public class PreviewData
{
    public readonly CombinationType Type;
    public readonly float Multiplier;
    public readonly float BaseValue;

    public PreviewData(CombinationType type, float multiplier, float baseValue)
    {
        Type = type;
        Multiplier = multiplier;
        BaseValue = baseValue;
    }
}