using System;
using System.Collections.Generic;
using System.Linq;

public class Hand
{
    private readonly List<Card> _cards;
    
    public Hand(int maxSize = 8)
    {
        MaxSize = maxSize;
        _cards = new List<Card>();
    }
    
    public event Action<Card> CardAdded;
    public event Action<Card> CardRemoved;
    
    public IReadOnlyList<Card> Cards => _cards;
    
    public int Count => _cards.Count;
    
    public int MaxSize { get; private set; }

    public void SetNewMaxSize(int newMaxSize)
    {
        if(newMaxSize <= 0)
            throw new ArgumentException("Max size must be greater than 0", nameof(newMaxSize));
        
        MaxSize = newMaxSize;
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
        {
            if (_cards.Count >= MaxSize)
                break;
            
            _cards.Add(card);
            CardAdded?.Invoke(card);
        }
    }
    
    public Card RemoveCard(int index)
    {
        if (index < 0 || index >= _cards.Count)
            return null;
            
        Card card = _cards[index];
        _cards.RemoveAt(index);
        CardRemoved?.Invoke(card);
        return card;
    }
    
    public bool RemoveCard(Card card)
    {
        int index = _cards.FindIndex(c => c.Equals(card));
        if (index == -1)
            return false;
            
        _cards.RemoveAt(index);
        CardRemoved?.Invoke(card);
        return true;
    }
    
    public List<Card> RemoveCards(List<int> indices)
    {
        List<Card> removed = new List<Card>();
        // Сортируем в обратном порядке, чтобы индексы не сбивались
        foreach (int index in indices.OrderByDescending(i => i))
        {
            if (index >= 0 && index < _cards.Count)
            {
                removed.Add(_cards[index]);
                CardRemoved?.Invoke(_cards[index]);
                _cards.RemoveAt(index);
            }
        }
        removed.Reverse(); // Восстанавливаем исходный порядок
        return removed;
    }
    
    public List<Card> RemoveCards(IEnumerable<Card> cardsToRemove)
    {
        List<Card> removed = new List<Card>();
        
        foreach (var cardToRemove in cardsToRemove)
        {
            int index = _cards.FindIndex(c => c.Equals(cardToRemove));
            if (index != -1)
            {
                removed.Add(_cards[index]);
                CardRemoved?.Invoke(_cards[index]);
                _cards.RemoveAt(index);
            }
        }
        
        return removed;
    }
}