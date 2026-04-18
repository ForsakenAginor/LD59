using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HandVisual : MonoBehaviour
{
    private const int TableSize = 5;
    
    private readonly Dictionary<Card, CardVisual> _cards = new Dictionary<Card, CardVisual>();
    private readonly List<CardVisual> _selectedCards = new List<CardVisual>();
    
    [SerializeField] private CardVisual _cardVisualPrefab;
    [SerializeField] private Transform _cardsInHandContainer;
    
    private CardTransferManager _cardTransferManager;
    private Hand _hand;

    private bool _isBlocked;
    
    public bool CanPlayHand => _selectedCards.Count > 0;
    
    [Inject]
    public void Construct(CardTransferManager transferManager)
    {
        _cardTransferManager = transferManager;
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
    }

    public void DiscardSelectedCards()
    {
        IEnumerable<Card> cards = _selectedCards.Select(card => card.Card);
        _hand.RemoveCards(cards);
        _selectedCards.Clear();
    }

    private void OnCardAdded(Card card)
    {
            var cardVisual = Instantiate(_cardVisualPrefab, _cardsInHandContainer);
            cardVisual.Init(card);
            _cards.Add(card, cardVisual);
            cardVisual.OnClick += OnCardClick;
            _isBlocked = false;
    }

    private void OnCardRemoved(Card card)
    {
        if(_cards.ContainsKey(card) == false)
            throw new Exception("Card not found");
        
        _cards[card].OnClick -= OnCardClick;
        Destroy(_cards[card].gameObject);
        _cards.Remove(card);
    }
    
    private void OnCardClick(CardVisual cardVisual)
    {
        if(_isBlocked)
            return;
        
        if (_selectedCards.Contains(cardVisual))
        {
            cardVisual.Normalize();
            _selectedCards.Remove(cardVisual);
        }
        else if (_selectedCards.Count < TableSize)
        {
            cardVisual.Enlarge();
            _selectedCards.Add(cardVisual);
        }
    }

}