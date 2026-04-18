using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TableVisual : MonoBehaviour
{
    private readonly List<CardVisual> _cards = new List<CardVisual>();
    
    [SerializeField] private CardVisual _cardVisualPrefab;
    [SerializeField] private Transform _cardsContainer;
    
    private CardTransferManager _cardTransferManager;
    private Table _table;
    
    [Inject]
    public void Construct(CardTransferManager transferManager)
    {
        _cardTransferManager = transferManager;
        _table = _cardTransferManager.Table;

        _table.TableRefreshed += OnTableRefreshed;
    }

    private void OnDestroy()
    {
        _table.TableRefreshed -= OnTableRefreshed;
    }

    private void OnTableRefreshed()
    {
        var collection = _table.SelectedCards;

        foreach (Card card in collection)
        {
            var cardVisual = Instantiate(_cardVisualPrefab, _cardsContainer);
            _cards.Add(cardVisual);
        }
    }
}