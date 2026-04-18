using System;
using System.Collections.Generic;

public class Table
{
    private List<Card> _selectedCards;
    
    public Table()
    {
        _selectedCards = new List<Card>();
    }
    
    public event Action TableRefreshed;
    
    public IReadOnlyList<Card> SelectedCards => _selectedCards;
    
    public int Count => _selectedCards.Count;
    
    public void Clear()
    {
        _selectedCards.Clear();
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        _selectedCards.AddRange(cards);
        TableRefreshed?.Invoke();
    }
}