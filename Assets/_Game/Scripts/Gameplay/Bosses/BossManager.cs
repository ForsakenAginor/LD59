using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int _bossFrequency = 3;
    
    private IBossConfiguration _bossConfiguration;
    private LevelNumber _levelNumber = LevelNumber.First;
    private BossType _currentBossType;
    private BossData _currentBossData;
    public bool IsBossActive => _levelNumber != LevelNumber.First && (int)_levelNumber % _bossFrequency == 0;
    
    [Inject]
    public void Construct(IBossConfiguration config)
    {
        _bossConfiguration = config;
    }
    
    public void SetLevel(LevelNumber levelNumber)
    {
        _levelNumber = levelNumber;

        if (IsBossActive)
        {
            _currentBossType = (BossType) Random.Range(0, Enum.GetValues(typeof(BossType)).Length);
            _currentBossData = _bossConfiguration.GetBossData(_currentBossType);
        }
    }

    public string GetDescription()
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");
        
        return _currentBossData.Description;
    }

    public bool IsOneSignal()
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");
        
        return _currentBossData.IsOneSignal;
    }
    
    public bool IsNoRerolls()
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");
        
        return _currentBossData.IsNoReroll;
    }

    public int GetHandModifacator()
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");
        
        return _currentBossData.Hand;
    }

    public bool IsFrequencyBanned(Frequency frequency)
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");

        return _currentBossData.Frequencies.Contains(frequency);
    }
    
    public bool IsSuitBanned(Suit suit)
    {
        if (IsBossActive == false)
            throw new Exception("Boss is not active");

        return _currentBossData.Suits.Contains(suit);
    }
}