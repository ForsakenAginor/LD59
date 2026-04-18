using System.Collections.Generic;

public struct CombinationResult
{
    public CombinationType Type;
    public float Multiplier;
    public List<Card> UsedCards;
    
    public CombinationResult(CombinationType type, float multiplier, List<Card> usedCards)
    {
        Type = type;
        Multiplier = multiplier;
        UsedCards = usedCards;
    }
}