using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class TableVisual : MonoBehaviour
{
    private readonly List<TableCardVisual> _cards = new List<TableCardVisual>();

    [SerializeField] private TableCardVisual _cardVisualPrefab;
    [SerializeField] private Transform _cardsContainer;
    [SerializeField] private TableAnimation _tableAnimation;

    private CardTransferManager _cardTransferManager;
    private ScoreManager _scoreManager;
    private Table _table;

    [Inject]
    public void Construct(ScoreManager scoreManager, CardTransferManager transferManager)
    {
        _scoreManager = scoreManager;
        _cardTransferManager = transferManager;
        _table = _cardTransferManager.Table;

        _table.TableRefreshed += OnTableRefreshed;
        _table.TableCleared += OnTableCleared;
    }

    private void OnDestroy()
    {
        _table.TableRefreshed -= OnTableRefreshed;
        _table.TableCleared -= OnTableCleared;
    }

    private void OnTableCleared()
    {
        for(int i = 0; i < _cards.Count; i++)
            Destroy(_cards[i].gameObject);

        _cards.Clear();
    }

    private void OnTableRefreshed()
    {
        var collection = _table.SelectedCards;

        foreach (Card card in collection)
        {
            var cardVisual = Instantiate(_cardVisualPrefab, _cardsContainer);
            cardVisual.Init(card);
            _cards.Add(cardVisual);
        }

        StartCoroutine(PlayComboCoroutine());
    }

    private IEnumerator PlayComboCoroutine()
    {
        
        yield return new WaitForSeconds(0.5f);
        yield return _tableAnimation.Animate(_cards);
        _cardTransferManager.CommitTable();
    }
}