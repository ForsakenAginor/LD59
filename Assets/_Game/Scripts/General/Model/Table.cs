using System.Collections.Generic;

public class Table
{
    private List<Card> _selectedCards;
    public IReadOnlyList<Card> SelectedCards => _selectedCards;
    public int Count => _selectedCards.Count;
    
    public Table()
    {
        _selectedCards = new List<Card>();
    }
    
    public void Clear()
    {
        _selectedCards.Clear();
    }
    
    public void AddCard(Card card)
    {
        _selectedCards.Add(card);
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        _selectedCards.AddRange(cards);
    }
    
    public bool RemoveCard(Card card)
    {
        return _selectedCards.Remove(card);
    }
    
    // Получить копию для расчётов
    public List<Card> GetCardsCopy()
    {
        return new List<Card>(_selectedCards);
    }
}