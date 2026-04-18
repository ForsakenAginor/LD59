using System;
using System.Collections.Generic;

public class Table
{
    private readonly List<Card> _selectedCards = new();

    public event Action TableRefreshed;
    public event Action TableCleared;
    public event Action TableCommited;
    
    public IReadOnlyList<Card> SelectedCards => _selectedCards;
    
    public int Count => _selectedCards.Count;
    
    public void Clear()
    {
        _selectedCards.Clear();
        TableCleared?.Invoke();
    }

    public void Commit()
    {
        TableCommited?.Invoke();
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        _selectedCards.AddRange(cards);
        TableRefreshed?.Invoke();
    }
}