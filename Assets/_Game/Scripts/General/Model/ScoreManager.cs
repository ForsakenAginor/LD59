using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScoreManager
{
    private readonly CombinationCalculator _calculator;
    private readonly ICardValueConfiguration _cardValueConfiguration;
    
    private int _currentScore;
    
    [Inject]
    public ScoreManager(ICardValueConfiguration cardValueConfiguration)
    {
        _calculator = new CombinationCalculator();
        _cardValueConfiguration = cardValueConfiguration;
        
        _currentScore = 0;
    }

    public event Action ScoreChanged; 
    
    public int CurrentScore => _currentScore;
    
    /// <summary>
    /// Рассчитывает очки для карт на столе и добавляет их к общему счёту
    /// </summary>
    /// <returns>Очки за эту комбинацию</returns>
    public float CalculateAndAddScore(List<Card> tableCards, float globalModifier)
    {
        if (tableCards == null || tableCards.Count == 0)
            return 0;
        
        var bestCombo = _calculator.GetBestCombination(tableCards);
        
        if (bestCombo.Multiplier <= 0)
            return 0;

        int tableCardsValue = 0;

        foreach (Card card in bestCombo.UsedCards)
        {
            tableCardsValue += _cardValueConfiguration.GetValue(card.Frequency);
        }
        
        Debug.Log(tableCardsValue);
        Debug.Log(bestCombo.Multiplier);
        
        float roundScore = tableCardsValue * bestCombo.Multiplier * globalModifier;
        Debug.Log(roundScore);
        _currentScore += (int)roundScore;
        ScoreChanged?.Invoke();
        
        return roundScore;
    }

    public void CalculatePreview(List<Card> tableCards, out int baseScore, out float multiplier, out CombinationType combinationType, out CombinationResult bestCombo)
    {
        if (tableCards == null)
            throw new ArgumentException("Table cards are required");
        
        bestCombo = _calculator.GetBestCombination(tableCards);
        baseScore = 0;
        
        foreach (Card card in bestCombo.UsedCards)
        {
            baseScore += _cardValueConfiguration.GetValue(card.Frequency);
        }
        
        multiplier = bestCombo.Multiplier;
        combinationType = bestCombo.Type;
    }
    
    public void ResetScore()
    {
        _currentScore = 0;
        ScoreChanged?.Invoke();
    }
}