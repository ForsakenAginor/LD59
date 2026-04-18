using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class TableAnimation : MonoBehaviour
{
    [Header("Animation settings")]
    [SerializeField] private float _scaleDuration = 0.5f;
    [SerializeField] private Vector3 _enlargeScale;
    [SerializeField] private Vector3 _smallScale;

    [SerializeField] private HandVisual _hand;

    private ICardValueConfiguration _configuration;

    [Inject]
    public void Construct(ICardValueConfiguration config)
    {
        _configuration = config;
    }
    
    public IEnumerator Animate(List<TableCardVisual> cards)
    {
        yield return null;
        var combo = _hand.Combination.UsedCards;

        foreach (var card in cards)
        {
            if (combo.Contains(card.Card))
                card.transform.DOScale(_enlargeScale, _scaleDuration);
            else
                card.transform.DOScale(_smallScale, _scaleDuration);
        }
        
        yield return new WaitForSeconds(_scaleDuration);

        foreach (var card in cards)
        {
            if (combo.Contains(card.Card))
            {
                yield return card.transform.DOShakePosition(0.5f, 15f).WaitForCompletion();
                yield return card.FlyingScore.Show(_configuration.GetValue(card.Card.Frequency));
            }
        }

    }
}