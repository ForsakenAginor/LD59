using System.Collections.Generic;
using System.Linq;

public class Hand
{
    private List<Card> _cards;
    public IReadOnlyList<Card> Cards => _cards;
    public int Count => _cards.Count;
    public int MaxSize { get; }
    
    public Hand(int maxSize = 5)
    {
        MaxSize = maxSize;
        _cards = new List<Card>();
    }
    
    public bool AddCard(Card card)
    {
        if (_cards.Count >= MaxSize)
            return false;
            
        _cards.Add(card);
        return true;
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
        {
            if (_cards.Count >= MaxSize)
                break;
            _cards.Add(card);
        }
    }
    
    public Card RemoveCard(int index)
    {
        if (index < 0 || index >= _cards.Count)
            return null;
            
        Card card = _cards[index];
        _cards.RemoveAt(index);
        return card;
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
                _cards.RemoveAt(index);
            }
        }
        removed.Reverse(); // Восстанавливаем исходный порядок
        return removed;
    }
    
    public void Clear()
    {
        _cards.Clear();
    }
    
    public Card GetCard(int index)
    {
        if (index < 0 || index >= _cards.Count)
            return null;
        return _cards[index];
    }
}