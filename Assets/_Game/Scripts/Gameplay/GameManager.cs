using System;
using System.Collections;
using System.Linq;
using Assets.Source.Scripts.AudioLogic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button _playHandButton;
    [SerializeField] private Button _discardHandButton;

    [SerializeField] private BossManager _bossManager;
    [SerializeField] private NewRoundIntroduce _newRoundIntroduce;
    [SerializeField] private ScoreVisual _scoreVisual;
    [SerializeField] private JokerManager _jokerManager;
    [FormerlySerializedAs("_tablePreview")] [SerializeField] private ScorePreview _scorePreview;
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
    private LevelNumber _level = LevelNumber.First;
    private int _signalsLeft;
    private int _rerollsLeft;

    public event Action PlayerWon;
    public event Action PlayerLose;
    public event Action ActionDone;

    public int RerollsLeft => _rerollsLeft;

    public int SignalsLeft => _signalsLeft;
    
    public int RerollsMax => _rerollsMax + _jokerManager.GetRerollsModificator();
    
    public int SignalsMax => _signalsMax + _jokerManager.GetSignalsModificator();


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

        _signalsLeft = _signalsMax + _jokerManager.GetSignalsModificator();
        _signalsLeft = Mathf.Clamp(_signalsLeft, 1, int.MaxValue);
        _rerollsLeft = _rerollsMax + _jokerManager.GetRerollsModificator();
        _rerollsLeft = Mathf.Clamp(_rerollsLeft, 0, int.MaxValue);
        
        if (_bossManager.IsBossActive)
        {
            _signalsLeft = _bossManager.IsOneSignal() ? 2 : _signalsLeft;
            _rerollsLeft = _bossManager.IsNoRerolls() ? 0 : _rerollsLeft;
        }
        
        _resourceVisual.Init();
        _scoreManager.ResetScore();
        _scoreVisual.Reset();
        _targetScoreVisual.Init(levelNumber);
        
        int newHandSize = _handSize + _jokerManager.GetHandModificator();
        newHandSize = _bossManager.IsBossActive ? newHandSize + _bossManager.GetHandModifacator() : newHandSize;
        
        _cardTransferManager.Hand.SetNewMaxSize(newHandSize);
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
        yield return _jokerManager.PlayJokerFinalAnimations();
        int score = (int)(_scorePreview.TargetBase * _scorePreview.TargetMultiplier);
        _scoreManager.CalculateAndAddScore(score);
        
        yield return _flyingScore.Show(score);
        _scorePreview.ClearPreview();
        
        if (_scoreManager.CurrentScore >= _targetScore)
        {
            if (_level == LevelNumber.Twelveth)
            {
                PlayerWon?.Invoke();
                AudioPlayer.Instance.PlayWinGame();
            }
            else
            {
                _level++;
                _bossManager.SetLevel(_level);
                _cardTransferManager.Hand.ClearHand();
                _cardTransferManager.Deck.Fill();
                AudioPlayer.Instance.PlayWinRound();
                yield return _jokerManager.SelectJokers(_level);
                yield return _newRoundIntroduce.SetNewThreshold(_level);
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
                AudioPlayer.Instance.PlayLoseRound();
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