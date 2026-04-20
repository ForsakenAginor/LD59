using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text _rerolls;
    [SerializeField] private TMP_Text _signals;
    
    [SerializeField] private GameManager _gameManager;

    public void Init()
    {
        _gameManager.ActionDone -= OnGameActionDone;
        OnGameActionDone();
        _gameManager.ActionDone += OnGameActionDone;
    }

    private void OnDestroy()
    {
        _gameManager.ActionDone -= OnGameActionDone;
    }

    private void OnGameActionDone()
    {
        _rerolls.text = $"{_gameManager.RerollsLeft}/{_gameManager.RerollsMax}";
        _signals.text = $"{_gameManager.SignalsLeft}/{_gameManager.SignalsMax}";
    }

}