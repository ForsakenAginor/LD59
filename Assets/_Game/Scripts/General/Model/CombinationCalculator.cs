using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class CombinationCalculator
{
    // Порядок частот для проверки стритов
    private static readonly List<Frequency> FrequencyOrder = new List<Frequency>
    {
        Frequency._100Hz, Frequency._200Hz, Frequency._300Hz, Frequency._400Hz,
        Frequency._500Hz, Frequency._600Hz, Frequency._700Hz, Frequency._800Hz,
        Frequency._900Hz, Frequency._1000Hz, Frequency._1200Hz, Frequency._1500Hz,
        Frequency._2000Hz
    };
    
    private static readonly List<Frequency> RoyalFrequencies = new List<Frequency>
    {
        Frequency._900Hz, Frequency._1000Hz, Frequency._1200Hz,
        Frequency._1500Hz, Frequency._2000Hz
    };

    private readonly ICombinationsConfiguration _configuration;

    [Inject]
    public CombinationCalculator(ICombinationsConfiguration config)
    {
        _configuration = config;
    }
    
    /// <summary>
    /// Рассчитывает лучшую комбинацию из выложенных на стол карт
    /// </summary>
    public CombinationResult GetBestCombination(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
            return new CombinationResult(CombinationType.Single, 1f, new List<Card>());
        
        var allCombinations = new List<CombinationResult>();
        
        // Генерируем все возможные подмножества от 3 до 5 карт (плюс одиночная на всякий)
        for (int size = 1; size <= Math.Min(5, cards.Count); size++)
        {
            var subsets = GetCombinations(cards, size);
            foreach (var subset in subsets)
            {
                var combo = EvaluateCombination(subset);
                if (combo.Multiplier > 0)
                    allCombinations.Add(combo);
            }
        }
        /*
        // Если нет ни одной комбинации из 3+ карт — берём самую "высокую" одиночную карту
        if (allCombinations.Count == 0 && cards.Count >= 1)
        {
            var singleCombo = EvaluateSingle(cards);
            allCombinations.Add(singleCombo);
        }*/
        
        // Возвращаем комбинацию с наибольшим множителем
        return allCombinations.OrderByDescending(c => c.Multiplier).ThenByDescending(c => GetCombinationValue(c.UsedCards)).FirstOrDefault();
    }
    
    private int GetCombinationValue(List<Card> cards)
    {
        // Сумма индексов частот (чем выше — тем лучше)
        return cards.Sum(c => FrequencyOrder.IndexOf(c.Frequency));
    }
    
    private CombinationResult EvaluateCombination(List<Card> cards)
    {
        if (IsRoyalStraightFlush(cards))
            return new CombinationResult(CombinationType.RoyalStraightFlush, _configuration.GetValue(CombinationType.RoyalStraightFlush).Multiplier, cards);
        
        if (IsStraightFlush(cards))
            return new CombinationResult(CombinationType.StraightFlush, _configuration.GetValue(CombinationType.StraightFlush).Multiplier, cards);
        
        if (IsStraight(cards))
            return new CombinationResult(CombinationType.Straight, _configuration.GetValue(CombinationType.Straight).Multiplier, cards);
        
        if (IsFourOfAKind(cards, out var fourCards))
            return new CombinationResult(CombinationType.FourOfAKind, _configuration.GetValue(CombinationType.FourOfAKind).Multiplier, fourCards);
        
        if (IsFullHouse(cards, out var fullHouseCards))
            return new CombinationResult(CombinationType.FullHouse, _configuration.GetValue(CombinationType.FullHouse).Multiplier, fullHouseCards);
        
        if (IsFullFlush(cards))
            return new CombinationResult(CombinationType.FullFlush, _configuration.GetValue(CombinationType.FullFlush).Multiplier, cards);
        
        if (IsBigFlush(cards))
            return new CombinationResult(CombinationType.BigFlush, _configuration.GetValue(CombinationType.BigFlush).Multiplier, cards);
        
        if (IsSet(cards, out var setCards))
            return new CombinationResult(CombinationType.Set, _configuration.GetValue(CombinationType.Set).Multiplier, setCards);
        
        if (IsSmallFlush(cards))
            return new CombinationResult(CombinationType.SmallFlush, _configuration.GetValue(CombinationType.SmallFlush).Multiplier, cards);
        
        if (IsTwoPairs(cards, out var twoPairsCards))
            return new CombinationResult(CombinationType.TwoPairs, _configuration.GetValue(CombinationType.TwoPairs).Multiplier, twoPairsCards);
        
        if (IsPair(cards, out var pairCards))
            return new CombinationResult(CombinationType.Pair, _configuration.GetValue(CombinationType.Pair).Multiplier, pairCards);
        
        GetHighestCard(cards, out var highestCard);
        return new CombinationResult(CombinationType.Single, _configuration.GetValue(CombinationType.Single).Multiplier, highestCard);
    }

    private void GetHighestCard(List<Card> cards, out List<Card> highestCards)
    {
        if (cards == null || cards.Count == 0)
        {
            highestCards = new List<Card>();
            return;
        }
    
        var bestCard = cards.OrderByDescending(c => FrequencyOrder.IndexOf(c.Frequency)).First();
        highestCards = new List<Card> { bestCard };
    }
    
    private CombinationResult EvaluateSingle(List<Card> cards)
    {
        // Самая "высокая" по частоте карта
        var bestCard = cards.OrderByDescending(c => FrequencyOrder.IndexOf(c.Frequency)).First();
        return new CombinationResult(CombinationType.Single, 1f, new List<Card> { bestCard });
    }
    
    // Проверки комбинаций
    
    private bool IsPair(List<Card> cards, out List<Card> pairCards)
    {
        pairCards = null;
        var freqGroups = cards.GroupBy(c => c.Frequency);
        var pair = freqGroups.FirstOrDefault(g => g.Count() == 2);
        
        if (pair != null && cards.Count == 2)
        {
            pairCards = pair.ToList();
            return true;
        }
        return false;
    }
    
    private bool IsTwoPairs(List<Card> cards, out List<Card> twoPairsCards)
    {
        twoPairsCards = null;
        var freqGroups = cards.GroupBy(c => c.Frequency);
        var pairs = freqGroups.Where(g => g.Count() == 2).Take(2).ToList();
        
        if (pairs.Count == 2 && cards.Count == 4)
        {
            twoPairsCards = pairs.SelectMany(p => p).ToList();
            return true;
        }
        return false;
    }
    
    private bool IsSet(List<Card> cards, out List<Card> setCards)
    {
        setCards = null;
        var freqGroups = cards.GroupBy(c => c.Frequency);
        var set = freqGroups.FirstOrDefault(g => g.Count() == 3);
        
        if (set != null && cards.Count == 3)
        {
            setCards = set.ToList();
            return true;
        }
        return false;
    }
    
    private bool IsSmallFlush(List<Card> cards)
    {
        return cards.Count == 3 && cards.GroupBy(c => c.Suit).Count() == 1;
    }
    
    private bool IsBigFlush(List<Card> cards)
    {
        return cards.Count == 4 && cards.GroupBy(c => c.Suit).Count() == 1;
    }
    
    private bool IsFullFlush(List<Card> cards)
    {
        return cards.Count == 5 && cards.GroupBy(c => c.Suit).Count() == 1;
    }
    
    private bool IsStraight(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        
        var indices = cards.Select(c => FrequencyOrder.IndexOf(c.Frequency)).OrderBy(i => i).ToList();
        
        for (int i = 0; i < indices.Count - 1; i++)
        {
            if (indices[i + 1] != indices[i] + 1)
                return false;
        }
        return true;
    }
    
    private bool IsStraightFlush(List<Card> cards)
    {
        return IsStraight(cards) && cards.GroupBy(c => c.Suit).Count() == 1;
    }
    
    private bool IsFullHouse(List<Card> cards, out List<Card> fullHouseCards)
    {
        fullHouseCards = null;
        if (cards.Count != 5) return false;
        
        var freqGroups = cards.GroupBy(c => c.Frequency).ToList();
        var hasSet = freqGroups.Any(g => g.Count() == 3);
        var hasPair = freqGroups.Any(g => g.Count() == 2);
        
        if (hasSet && hasPair)
        {
            fullHouseCards = cards;
            return true;
        }
        return false;
    }
    
    private bool IsFourOfAKind(List<Card> cards, out List<Card> fourCards)
    {
        fourCards = null;
        if (cards.Count != 4) return false;
        
        var freqGroups = cards.GroupBy(c => c.Frequency);
        var four = freqGroups.FirstOrDefault(g => g.Count() == 4);
        
        if (four != null)
        {
            fourCards = four.ToList();
            return true;
        }
        return false;
    }
    
    private bool IsRoyalStraightFlush(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        
        var freqs = cards.Select(c => c.Frequency).OrderBy(f => FrequencyOrder.IndexOf(f)).ToList();
        bool hasRoyalFreqs = RoyalFrequencies.All(rf => freqs.Contains(rf));
        bool oneSuit = cards.GroupBy(c => c.Suit).Count() == 1;
        
        return hasRoyalFreqs && oneSuit;
    }
    
    // Вспомогательный метод для генерации сочетаний
    private List<List<Card>> GetCombinations(List<Card> cards, int size)
    {
        var result = new List<List<Card>>();
        Combine(cards, size, 0, new List<Card>(), result);
        return result;
    }
    
    private void Combine(List<Card> cards, int size, int start, List<Card> current, List<List<Card>> result)
    {
        if (current.Count == size)
        {
            result.Add(new List<Card>(current));
            return;
        }
        
        for (int i = start; i < cards.Count; i++)
        {
            current.Add(cards[i]);
            Combine(cards, size, i + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
}