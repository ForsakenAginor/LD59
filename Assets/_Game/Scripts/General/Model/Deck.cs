using System.Collections.Generic;

public class Deck
{
    private List<Card> _cards;
    private System.Random _random;
    
    public int Count => _cards.Count;
    public bool IsEmpty => _cards.Count == 0;
    
    public Deck()
    {
        _random = new System.Random();
        Fill();
    }
    
    public void Fill()
    {
        _cards = new List<Card>();
        
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Frequency freq in System.Enum.GetValues(typeof(Frequency)))
            {
                _cards.Add(new Card(suit, freq));
            }
        }
        
        Shuffle();
    }
    
    // Перемешивание
    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }
    
    // Взять карту сверху
    public Card Draw()
    {
        if (IsEmpty)
            return null;
            
        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }
    
    // Взять несколько карт
    public List<Card> Draw(int count)
    {
        List<Card> drawn = new List<Card>();
        
        for (int i = 0; i < count && !IsEmpty; i++)
        {
            drawn.Add(Draw());
        }
        
        return drawn;
    }
    
    // Добавить карту обратно (например, при сбросе)
    public void AddCard(Card card)
    {
        _cards.Insert(0, card);
    }
    
    public void AddCards(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
            _cards.Insert(0, card);
    }
}