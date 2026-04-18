using System.Collections.Generic;

public class ScoreManager
{
    private float _currentScore;
    private CombinationCalculator _calculator;
    
    public float CurrentScore => _currentScore;
    
    public ScoreManager()
    {
        _currentScore = 0;
        _calculator = new CombinationCalculator();
    }
    
    /// <summary>
    /// Рассчитывает очки для карт на столе и добавляет их к общему счёту
    /// </summary>
    /// <returns>Очки за эту комбинацию</returns>
    public float CalculateAndAddScore(List<Card> tableCards, float globalModifier = 1f)
    {
        if (tableCards == null || tableCards.Count == 0)
            return 0;
        
        var bestCombo = _calculator.GetBestCombination(tableCards);
        
        if (bestCombo.Multiplier <= 0)
            return 0;
        
        float roundScore = bestCombo.Multiplier * globalModifier;
        _currentScore += roundScore;
        
        return roundScore;
    }
    
    /// <summary>
    /// Только рассчитать, не добавляя к счёту (для превью)
    /// </summary>
    public float CalculatePreview(List<Card> tableCards, float globalModifier = 1f)
    {
        if (tableCards == null || tableCards.Count == 0)
            return 0;
        
        var bestCombo = _calculator.GetBestCombination(tableCards);
        return bestCombo.Multiplier * globalModifier;
    }
    
    public void ResetScore()
    {
        _currentScore = 0;
    }
}