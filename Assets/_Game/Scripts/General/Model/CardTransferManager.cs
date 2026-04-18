using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTransferManager
{
    private Deck _deck;
    private Hand _hand;
    private Table _table;
    
    public CardTransferManager()
    {
        _deck = new Deck();
        _hand = new Hand(8);
        _table = new Table();
    }

    public Deck Deck => _deck;
    public Hand Hand => _hand;
    public Table Table => _table;
    
    public void MoveToTable(IEnumerable<Card> cards)
    {
        if(cards == null || cards.Count() == 0)
            throw new Exception("Empty cards");
        
        _hand.RemoveCards(cards);
        _table.AddCards(cards);
    }
    
    // Зафиксировать комбинацию на столе (очистить стол и добить руку)
    public void CommitTable()
    {
        _table.Commit();
        _table.Clear();
    }

    public void RefillHand()
    {
        int needed = _hand.MaxSize - _hand.Count;
        
        if (needed <= _deck.Count)
        {
            needed = _deck.Count;
            var newCards = _deck.Draw(needed);
            _hand.AddCards(newCards);
            
            _deck.Fill();
            needed = _hand.MaxSize - _hand.Count;
            newCards = _deck.Draw(needed);
            _hand.AddCards(newCards);
        }
        else
        {
            var newCards = _deck.Draw(needed);
            _hand.AddCards(newCards);
        }
    }
}