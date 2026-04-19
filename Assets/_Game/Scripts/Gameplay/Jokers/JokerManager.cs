using System.Collections;
using System.Collections.Generic;
using Assets.Source.Scripts.DI.Services.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class JokerManager : MonoBehaviour
{
    private readonly List<JokerCard> _jokers = new();
    
    [SerializeField] private JokerTooltip _jokerTooltip;
    [SerializeField] private ScorePreview _preview;
    [SerializeField] private Transform _jokerContainer;
    [SerializeField] private JokerCard _jokerPrefab;
    
    private IJokerConfiguration _configuration;
    private IZenjectInstantiateWrapper _instantiateWrapper;
    
    [Inject]
    public void Construct(IJokerConfiguration configuration, IZenjectInstantiateWrapper instantiateWrapper)
    {
        _configuration = configuration;
        _instantiateWrapper = instantiateWrapper;
    }

    [Button]
    private void AddJoker(string id)
    {
        var joker = _instantiateWrapper.Instantiate(_jokerPrefab, _jokerContainer);
        _jokers.Add(joker);
        joker.Init(id, _jokerTooltip);
    }

    public bool CanBoostThatFrequency(Frequency frequency, out List<JokerCard> jokers)
    {
        bool isBoost = false;
        jokers = new List<JokerCard>();
        
        foreach (JokerCard joker in _jokers)
        {
            JokerData data = _configuration.GetJokerData(joker.Id);

            if (data.Frequencies != null && data.Frequencies.Contains(frequency))
            {
                isBoost = true;
                jokers.Add(joker);
            }
        }
        
        return isBoost;
    }
    
    public bool CanBoostThatSuit(Suit suit, out List<JokerCard> jokers)
    {
        bool isBoost = false;
        jokers = new List<JokerCard>();
        
        foreach (JokerCard joker in _jokers)
        {
            JokerData data = _configuration.GetJokerData(joker.Id);

            if (data.Suits != null && data.Suits.Contains(suit))
            {
                isBoost = true;
                jokers.Add(joker);
            }
        }
        
        return isBoost;
    }
    
    public bool CanBoostThatCombination(CombinationType combination, out List<JokerCard> jokers)
    {
        bool isBoost = false;
        jokers = new List<JokerCard>();
        
        foreach (JokerCard joker in _jokers)
        {
            JokerData data = _configuration.GetJokerData(joker.Id);

            if (data.Combinations != null && data.Combinations.Contains(combination))
            {
                isBoost = true;
                jokers.Add(joker);
            }
        }
        
        return isBoost;
    }

    public int GetHandModificator()
    {
        int modificator = 0;

        foreach (var jokerCard in _jokers)
        {
            JokerData data = _configuration.GetJokerData(jokerCard.Id);
            modificator += data.HandSize;
        }
        
        return modificator;
    }
    
    public int GetSignalsModificator()
    {
        int modificator = 0;

        foreach (var jokerCard in _jokers)
        {
            JokerData data = _configuration.GetJokerData(jokerCard.Id);
            modificator += data.Signals;
        }
        
        return modificator;
    }

    public int GetRerollsModificator()
    {
        int modificator = 0;

        foreach (var jokerCard in _jokers)
        {
            JokerData data = _configuration.GetJokerData(jokerCard.Id);
            modificator += data.Rerolls;
        }
        
        return modificator;
    }

    public IEnumerator PlayJokerFinalAnimations()
    {
        foreach (var jokerCard in _jokers)
        {
            JokerData data = _configuration.GetJokerData(jokerCard.Id);
            
            if (data.AddedBase != 0)
            {
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f,30).WaitForCompletion();
                _preview.AddTargetValues(data.AddedBase, 0f);
                yield return jokerCard.FlyingScore.Show(data.AddedBase);
            }
            
            if (data.AddedMultiplier != 0)
            {
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f,30).WaitForCompletion();
                _preview.AddTargetValues(0, data.AddedMultiplier);
                yield return jokerCard.FlyingText.Show($"+{data.AddedMultiplier:0.00}");
            }
            
            if (Mathf.Approximately(data.Multiplier, 1) == false)
            {
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f,30).WaitForCompletion();
                _preview.AddTargetValues(0, data.Multiplier, true);
                yield return jokerCard.FlyingText.Show($"x{data.AddedMultiplier:0.00}");
            }
        }
    }
}