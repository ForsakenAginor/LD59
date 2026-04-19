using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IBossConfiguration
{
    public BossData GetBossData(BossType bossType);
}

[CreateAssetMenu(fileName = "BossConfiguration", menuName = "Configurations/BossConfiguration")]
public class BossConfiguration : SerializedScriptableObject, IBossConfiguration
{
    [SerializeField] private Dictionary<BossType, BossData> _data = new Dictionary<BossType, BossData>();
    
    public BossData GetBossData(BossType bossType) => _data[bossType];

}