using System;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    
    private ScoreManager _scoreManager;

    [Inject]
    public void Construct(ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
        _score.text = _scoreManager.CurrentScore.ToString();

        _scoreManager.ScoreChanged += OnScoreChanged;
    }

    private void OnDestroy()
    {
        _scoreManager.ScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        _score.text = _scoreManager.CurrentScore.ToString();
    }
}