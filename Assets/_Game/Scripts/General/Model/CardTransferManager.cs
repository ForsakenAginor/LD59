using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;

public class CardTransferManager
{
    private Deck _deck;
    private Hand _hand;
    private Table _table;
    
    public CardTransferManager()
    {
        _deck = new Deck();
        _hand = new Hand(5);
        _table = new Table();
    }

    public Deck Deck => _deck;
    public Hand Hand => _hand;
    public Table Table => _table;
    
    // Начальная раздача
    public void DealInitialHand()
    {
        _hand.Clear();
        var cards = _deck.Draw(5);
        _hand.AddCards(cards);
    }
    
    // Переложить карту из руки на стол по индексу
    public bool MoveToTable(int handIndex)
    {
        Card card = _hand.RemoveCard(handIndex);
        if (card != null)
        {
            _table.AddCard(card);
            return true;
        }
        return false;
    }
    
    // Переложить несколько карт из руки на стол
    public int MoveToTable(List<int> handIndices)
    {
        List<Card> movedCards = _hand.RemoveCards(handIndices);
        if (movedCards.Count > 0)
        {
            _table.AddCards(movedCards);
        }
        return movedCards.Count;
    }
    
    // Вернуть все карты со стола в руку
    public void ReturnAllToHand()
    {
        var tableCards = _table.GetCardsCopy();
        _table.Clear();
        _hand.AddCards(tableCards);
    }
    
    // Сбросить карты из руки (удалить и взять новые из колоды)
    public void DiscardAndDraw(List<int> handIndices)
    {
        var discarded = _hand.RemoveCards(handIndices);
        // Сброшенные карты возвращаются в колоду (перемешивать не обязательно)
        _deck.AddCards(discarded);
        
        // Добираем столько же новых
        var newCards = _deck.Draw(discarded.Count);
        _hand.AddCards(newCards);
    }
    
    // Зафиксировать комбинацию на столе (очистить стол и добить руку)
    public void CommitTableAndRefill()
    {
        // Очищаем стол (карты уходят в никуда — они сыграны)
        _table.Clear();
        
        // Добиваем руку до 5 карт
        int needed = _hand.MaxSize - _hand.Count;
        if (needed > 0)
        {
            var newCards = _deck.Draw(needed);
            _hand.AddCards(newCards);
        }
        
        // Если колода пуста — перемешиваем сброс? (опционально)
        if (_deck.IsEmpty)
        {
            // Здесь можно сделать reshuffle из отыгранных карт, но пока пропустим
        }
    }
    
    // Получить текущее состояние руки (для UI)
    public List<Card> GetHandCards() => _hand.Cards.ToList();
    
    // Получить текущее состояние стола
    public List<Card> GetTableCards() => _table.SelectedCards.ToList();
}