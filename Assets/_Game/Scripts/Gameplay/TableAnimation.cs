using System.Collections;
using System.Collections.Generic;
using Assets.Source.Scripts.DI.Services.Boot;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TableAnimation : MonoBehaviour
{
    [Header("Animation settings")]
    [SerializeField] private float _scaleDuration = 0.5f;
    [SerializeField] private Vector3 _enlargeScale;
    [SerializeField] private Vector3 _smallScale;

    [SerializeField] private JokerManager _jokerManager;
    [SerializeField] private HandVisual _hand;
    [SerializeField] private ScorePreview _preview;
    [FormerlySerializedAs("_flyingText")] [SerializeField] private FlyingCombination _flyingCombination;

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

        foreach (TableCardVisual card in cards)
        {
            if (combo.Contains(card.Card) && card.IsBlocked == false)
            {
                yield return TriggerCard(card);

                if (_jokerManager.CanBoostThatSuit(card.Card.Suit, out List<JokerCard> suitJokers))
                {
                    foreach (JokerCard joker in suitJokers)
                    {
                        yield return joker.transform.DOShakePosition(0.25f, 15f, 50).WaitForCompletion();
                        yield return TriggerCard(card);
                    }
                }
                
                if (_jokerManager.CanBoostThatFrequency(card.Card.Frequency, out List<JokerCard> frequencyJokers))
                {
                    foreach (JokerCard joker in frequencyJokers)
                    {
                        yield return joker.transform.DOShakePosition(0.25f, 15f, 50).WaitForCompletion();
                        yield return TriggerCard(card);
                    }
                }
            }
        }

        float multiplier = _configuration.GetValue(_hand.Combination.Type).Multiplier;
        _preview.AddTargetValues(0, multiplier);
        yield return _flyingCombination.Show(_hand.Combination.Type);

        if (_jokerManager.CanBoostThatCombination(_hand.Combination.Type, out List<JokerCard> jokers))
        {
            foreach (JokerCard joker in jokers)
            {
                yield return joker.transform.DOShakePosition(0.25f, 15f, 30).WaitForCompletion();
                yield return joker.FlyingText.Show($"x2");
                _preview.AddTargetValues(0, 2, true);
            }
        }
    }

    private IEnumerator TriggerCard(TableCardVisual card)
    {
        yield return card.transform.DOShakePosition(0.4f, 30f).WaitForCompletion();
        int score = _configuration.GetValue(card.Card.Frequency);
        _preview.AddTargetValues(score, 0f);
        yield return card.FlyingScore.Show(score);
    }
}