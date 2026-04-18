using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICardValueConfiguration
{
    public int GetValue(Frequency frequency);

    public Sprite GetIcon(Frequency frequency, Suit suit);
}

[CreateAssetMenu(fileName = "CardValueConfiguration", menuName = "Configurations/CardValueConfiguration")]
public class CardValueConfiguration : SerializedScriptableObject, ICardValueConfiguration
{
    [SerializeField] private Dictionary<Frequency, int> _frequenciesValues = new Dictionary<Frequency, int>();
    [SerializeField] private Dictionary<CardKey, Sprite> _icons = new Dictionary<CardKey, Sprite>();

    public Sprite GetIcon(Frequency frequency, Suit suit)
    {
        var pair = _icons.First(o => o.Key.Frequency == frequency && o.Key.Suit == suit);
        return pair.Value;
    }
    
    public int GetValue(Frequency frequency) => _frequenciesValues[frequency];

    [Serializable]
    private class CardKey
    {
        public Suit Suit;
        public Frequency Frequency;
    }

    [Button]
    private void Validate()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Frequency frequency in Enum.GetValues(typeof(Frequency)))
            {
                var key = _icons.FirstOrDefault(o => o.Key.Frequency == frequency && o.Key.Suit == suit).Key;
                
                if(key == null)
                    Debug.Log($"{suit.ToString()} {frequency.ToString()}");
                
                var pairs = _icons.Where(o => o.Key.Frequency == frequency && o.Key.Suit == suit);
                
                if(pairs.Count() > 1)
                    Debug.Log($"{suit.ToString()} {frequency.ToString()}");
            }
        }
    }
}