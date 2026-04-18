using TMPro;
using UnityEngine;
using Zenject;

public class TargetScoreVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    
    private ILevelDifficultyConfiguration _configuration;

    [Inject]
    public void Construct(ILevelDifficultyConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Init(LevelNumber levelNumber)
    {
        _score.text = _configuration.GetValue(levelNumber).ToString();
    }
}