using UnityEngine;

namespace Assets.Source.Scripts.DI.Services.Boot
{
    [CreateAssetMenu(fileName = "ConfigurationsProvider", menuName = "Services/ConfigurationsProvider")]
    public class ConfigurationProvider : ScriptableObject, ICardValueConfiguration, ILevelDifficultyConfiguration
    {
        [SerializeField] private CardValueConfiguration _cardValueConfiguration;
        [SerializeField] private LevelDifficultyConfiguration _levelDifficultyConfiguration;
        
        public int GetValue(Frequency frequency)
        {
            return _cardValueConfiguration.GetValue(frequency);
        }
        
        public int GetValue(LevelNumber level) => _levelDifficultyConfiguration.GetValue(level);
    }
}
