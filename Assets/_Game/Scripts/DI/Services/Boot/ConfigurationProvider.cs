using UnityEngine;

namespace Assets.Source.Scripts.DI.Services.Boot
{
    [CreateAssetMenu(fileName = "ConfigurationsProvider", menuName = "Services/ConfigurationsProvider")]
    public class ConfigurationProvider : ScriptableObject, ICardValueConfiguration, ILevelDifficultyConfiguration, ICombinationsConfiguration,
        IJokerConfiguration
    {
        [SerializeField] private CardValueConfiguration _cardValueConfiguration;
        [SerializeField] private LevelDifficultyConfiguration _levelDifficultyConfiguration;
        [SerializeField] private CombinationsConfiguration _combinationsConfiguration;
        [SerializeField] private JokerConfiguration _jokerConfiguration;
        
        public int GetValue(Frequency frequency)
        {
            return _cardValueConfiguration.GetValue(frequency);
        }
        
        public int GetValue(LevelNumber level) => _levelDifficultyConfiguration.GetValue(level);
        
        public CombinationData GetValue(CombinationType combination)
        {
            return _combinationsConfiguration.GetValue(combination);
        }
        
        public Sprite GetIcon(Frequency frequency, Suit suit) => _cardValueConfiguration.GetIcon(frequency, suit);
        
        public JokerData GetJokerData(string jokerName)
        {
            return _jokerConfiguration.GetJokerData(jokerName);
        }
    }
}
