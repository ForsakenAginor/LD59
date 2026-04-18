using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ILevelDifficultyConfiguration
{
    public int GetValue(LevelNumber level);
}

[CreateAssetMenu(fileName = "LevelDifficulty", menuName = "Configurations/LevelDifficulty")]
public class LevelDifficultyConfiguration : SerializedScriptableObject, ILevelDifficultyConfiguration
{
    [SerializeField] private Dictionary<LevelNumber, int> _targetScores = new Dictionary<LevelNumber, int>();
    
    public int GetValue(LevelNumber level) => _targetScores[level];
}

public enum LevelNumber
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
}