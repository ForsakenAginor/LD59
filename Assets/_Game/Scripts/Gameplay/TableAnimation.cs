using System.Collections;
using System.Collections.Generic;
using Assets.Source.Scripts.DI.Services.Boot;
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
    [SerializeField] private TablePreview _preview;
    [SerializeField] private FlyingText _flyingText;

    private ConfigurationProvider _configuration;

    [Inject]
    public void Construct(ConfigurationProvider config)
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
                int score = _configuration.GetValue(card.Card.Frequency);
                _preview.AddTargetValues(score, 0f);
                yield return card.FlyingScore.Show(score);
            }
        }

        float multiplier = _configuration.GetValue(_hand.Combination.Type).Multiplier;
        _preview.AddTargetValues(0, multiplier);
        yield return _flyingText.Show(_hand.Combination.Type);
    }
}