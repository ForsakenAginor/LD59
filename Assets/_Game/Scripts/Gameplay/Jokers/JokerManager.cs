using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Scripts.DI.Services.Global;
using Assets.Source.Scripts.Utility;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class JokerManager : MonoBehaviour
{
    private readonly List<JokerCard> _jokers = new();
    private List<string> _jokersNames = new List<string>();

    [SerializeField] private JokerTooltip _jokerTooltip;
    [SerializeField] private ScorePreview _preview;
    [SerializeField] private Transform _jokerContainer;
    [SerializeField] private JokerCard _jokerPrefab;

    [Header("Selection")]
    [SerializeField] private TMP_Text _levelBypassText;
    [SerializeField] private SwitchableElement _chooseJokerText;
    [SerializeField] private JokerSelectionCard[] _jokerSelectionCards;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private SwitchableElement _jokerSelectionPanel;

    private IJokerConfiguration _configuration;
    private IZenjectInstantiateWrapper _instantiateWrapper;
    private bool _isJokerSelected;

    [Inject]
    public void Construct(IJokerConfiguration configuration, IZenjectInstantiateWrapper instantiateWrapper)
    {
        _configuration = configuration;
        _instantiateWrapper = instantiateWrapper;
        _jokersNames = _configuration.GetJokerNames();

        foreach (JokerSelectionCard jokerSelectionCard in _jokerSelectionCards)
        {
            jokerSelectionCard.JokerSelected += AddJoker;
        }
    }

    private void OnDestroy()
    {
        foreach (JokerSelectionCard jokerSelectionCard in _jokerSelectionCards)
        {
            jokerSelectionCard.JokerSelected -= AddJoker;
        }
    }

    public IEnumerator SelectJokers()
    {
        _isJokerSelected = false;
        List<string> jokersNames = new List<string>();

        for (int i = 0; i < _jokerSelectionCards.Length; i++)
        {
            string randomJokerName = _jokersNames[Random.Range(0, _jokersNames.Count)];

            while (jokersNames.Contains(randomJokerName))
            {
                randomJokerName = _jokersNames[Random.Range(0, _jokersNames.Count)];
            }

            jokersNames.Add(randomJokerName);
            _jokerSelectionCards[i].Init(randomJokerName);
            _jokerSelectionCards[i].SetInteractable(false);
        }

        _levelBypassText.transform.localScale = Vector3.one * 20f;
        _chooseJokerText.Disable();
        _jokerSelectionPanel.Enable();
        yield return _levelBypassText.transform.DOScale(Vector3.one, 1f).WaitForCompletion();
        _chooseJokerText.Enable();

        for (int i = 0; i < _jokerSelectionCards.Length; i++)
        {
            yield return _jokerSelectionCards[i].transform.DOScale(Vector3.one, _animationDuration)
                .WaitForCompletion();
        }

        for (int i = 0; i < _jokerSelectionCards.Length; i++)
        {
            _jokerSelectionCards[i].SetInteractable(true);
        }

        yield return new WaitWhile(() => _isJokerSelected == false);
        _isJokerSelected = false;

        for (int i = 0; i < _jokerSelectionCards.Length; i++)
        {
            _jokerSelectionCards[i].SetInteractable(false);
        }

        Transform selected = null;

        for (int i = 0; i < _jokerSelectionCards.Length; i++)
        {
            if (_jokerSelectionCards[i].IsSelected == false)
                _jokerSelectionCards[i].transform.DOScale(Vector3.zero, _animationDuration);
            else
                selected = _jokerSelectionCards[i].transform;
        }

        yield return selected.DOScale(Vector3.zero, _animationDuration).SetDelay(_animationDuration)
            .WaitForCompletion();

        _jokerSelectionPanel.Disable();
    }

    [Button]
    private void AddJoker(string id)
    {
        var joker = _instantiateWrapper.Instantiate(_jokerPrefab, _jokerContainer);
        _jokers.Add(joker);
        joker.Init(id, _jokerTooltip);
        _isJokerSelected = true;
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
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f, 30).WaitForCompletion();
                _preview.AddTargetValues(data.AddedBase, 0f);
                yield return jokerCard.FlyingScore.Show(data.AddedBase);
            }

            if (data.AddedMultiplier != 0)
            {
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f, 30).WaitForCompletion();
                _preview.AddTargetValues(0, data.AddedMultiplier);
                yield return jokerCard.FlyingText.Show($"+{data.AddedMultiplier:0.00}");
            }

            if (Mathf.Approximately(data.Multiplier, 1) == false)
            {
                yield return jokerCard.transform.DOShakePosition(0.5f, 15f, 30).WaitForCompletion();
                _preview.AddTargetValues(0, data.Multiplier, true);
                yield return jokerCard.FlyingText.Show($"x{data.Multiplier:0.00}");
            }
        }
    }
}