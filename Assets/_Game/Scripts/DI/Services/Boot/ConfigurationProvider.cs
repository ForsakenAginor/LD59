using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.DI.Services.Boot
{
    [CreateAssetMenu(fileName = "ConfigurationsProvider", menuName = "Services/ConfigurationsProvider")]
    public class ConfigurationProvider : ScriptableObject, ICardValueConfiguration, ILevelDifficultyConfiguration, ICombinationsConfiguration,
        IJokerConfiguration, IBossConfiguration
    {
        [SerializeField] private CardValueConfiguration _cardValueConfiguration;
        [SerializeField] private LevelDifficultyConfiguration _levelDifficultyConfiguration;
        [SerializeField] private CombinationsConfiguration _combinationsConfiguration;
        [SerializeField] private JokerConfiguration _jokerConfiguration;
        [SerializeField] private BossConfiguration _bossConfiguration;
        
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
        
        public List<string> GetJokerNames() => _jokerConfiguration.GetJokerNames();
        
        public BossData GetBossData(BossType bossType)
        {
            return _bossConfiguration.GetBossData(bossType);
        }
    }
}
