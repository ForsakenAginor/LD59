using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private float _duration = 2f;

    
    private ScoreManager _scoreManager;
    private int _currentScore;
    private Coroutine _coroutine;

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

    public void Reset()
    {
        _currentScore = 0;
        _score.text = _currentScore.ToString();
    }

    private void OnScoreChanged()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(ChangeScore());
    }

    private IEnumerator ChangeScore()
    {
        float remainingDuration = _duration;

        while (_currentScore < _scoreManager.CurrentScore)
        {
            int goldToAdd = (int)((_scoreManager.CurrentScore - _currentScore) * Time.deltaTime / remainingDuration);
            _currentScore += goldToAdd;
            remainingDuration -= Time.deltaTime;

            if (_currentScore > _scoreManager.CurrentScore)
                _currentScore = _scoreManager.CurrentScore;

            _score.text = _currentScore.ToString();

            yield return null;
        }
    }
}