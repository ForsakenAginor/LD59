
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICardValueConfiguration
{
    public int GetValue(Frequency frequency);
}

[CreateAssetMenu(fileName = "CardValueConfiguration", menuName = "Configurations/CardValueConfiguration")]
public class CardValueConfiguration : SerializedScriptableObject, ICardValueConfiguration
{
    [SerializeField] private Dictionary<Frequency, int> _frequenciesValues = new Dictionary<Frequency, int>();
    
    public int GetValue(Frequency frequency) => _frequenciesValues[frequency];
}