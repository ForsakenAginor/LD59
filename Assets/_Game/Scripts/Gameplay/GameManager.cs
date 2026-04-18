using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button _playHandButton;
    [SerializeField] private Button _discardHandButton;

    [SerializeField] private FlyingScore _flyingScore;
    [SerializeField] private ResourceVisual _resourceVisual;
    [SerializeField] private HandVisual _handVisual;
    [SerializeField] private TargetScoreVisual _targetScoreVisual;
    [SerializeField] private int _handSize = 6;
    [SerializeField] private int _signalsMax = 5;
    [SerializeField] private int _rerollsMax = 5;

    private ScoreManager _scoreManager;
    private CardTransferManager _cardTransferManager;
    private Table _table;
    private int _targetScore;
    private ILevelDifficultyConfiguration _configuration;
    private LevelNumber _level;
    private int _signalsLeft;
    private int _rerollsLeft;

    public event Action PlayerWon;
    public event Action PlayerLose;
    public event Action ActionDone;

    public int RerollsLeft => _rerollsLeft;

    public int SignalsLeft => _signalsLeft;


    [Inject]
    public void Construct(ScoreManager scoreManager, CardTransferManager transferManager,
        ILevelDifficultyConfiguration levelDifficultyConfiguration)
    {
        _scoreManager = scoreManager;
        _cardTransferManager = transferManager;
        _table = _cardTransferManager.Table;
        _configuration = levelDifficultyConfiguration;

        _table.TableCommited += OnTableCommited;
        _playHandButton.onClick.AddListener(OnPlayButtonClicked);
        _discardHandButton.onClick.AddListener(OnDiscardButtonClicked);
    }

    private void OnDestroy()
    {
        _table.TableCommited -= OnTableCommited;
        _playHandButton.onClick.RemoveListener(OnPlayButtonClicked);
        _discardHandButton.onClick.RemoveListener(OnDiscardButtonClicked);
    }

    public void Init(LevelNumber levelNumber)
    {
        _level = levelNumber;
        _signalsLeft = _signalsMax;
        _rerollsLeft = _rerollsMax;
        _resourceVisual.Init();
        _scoreManager.ResetScore();
        _targetScoreVisual.Init(levelNumber);
        _cardTransferManager.Hand.SetNewMaxSize(_handSize);
        _cardTransferManager.RefillHand();
        _targetScore = _configuration.GetValue(levelNumber);
        _playHandButton.interactable = true;
        _discardHandButton.interactable = true;
    }


    private void OnDiscardButtonClicked()
    {
        if (_handVisual.CanPlayHand == false)
            return;

        if (_rerollsLeft <= 0)
            return;

        _rerollsLeft--;
        
        if (_rerollsLeft <= 0)
            _discardHandButton.interactable = false;
        
        _handVisual.DiscardSelectedCards();
        _cardTransferManager.RefillHand();
        ActionDone?.Invoke();
    }

    private void OnTableCommited()
    {
        StartCoroutine(CalculateScore());
    }

    private IEnumerator CalculateScore()
    {
        int score = (int) _scoreManager.CalculateAndAddScore(_table.SelectedCards.ToList(), 1f);
        yield return _flyingScore.Show(score);
        
        if (_scoreManager.CurrentScore >= _targetScore)
        {
            if (_level == LevelNumber.Fifth)
            {
                PlayerWon?.Invoke();
            }
            else
            {
                _level++;
                _cardTransferManager.Hand.ClearHand();
                _cardTransferManager.Deck.Fill();
                Init(_level);
            }
        }
        else
        {
            if (_signalsLeft > 0)
            {
                _cardTransferManager.RefillHand();
                _playHandButton.interactable = true;

                if (_rerollsLeft > 0)
                    _discardHandButton.interactable = true;
            }
            else
            {
                PlayerLose?.Invoke();
            }
        }
        
    }

    private void OnPlayButtonClicked()
    {
        if (_handVisual.CanPlayHand == false)
            return;

        _signalsLeft--;
        _playHandButton.interactable = false;
        _discardHandButton.interactable = false;
        _handVisual.PlaySelectedCards();
        ActionDone?.Invoke();
    }
}