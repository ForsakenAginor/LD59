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
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Fifth = 5,
    Sixth = 6,
    Seventh = 7,
    Eighth = 8,
    Ninth = 9,
    Tenth = 10,
    Eleventh = 11,
    Twelveth = 12,
}