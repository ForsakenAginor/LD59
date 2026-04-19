using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICombinationsConfiguration
{
    public CombinationData GetValue(CombinationType combination);
}

[CreateAssetMenu(fileName = "CombinationsConfiguration", menuName = "Configurations/CombinationsConfiguration")]
public class CombinationsConfiguration : SerializedScriptableObject, ICombinationsConfiguration
{
    [SerializeField] private Dictionary<CombinationType, CombinationData> _data = new Dictionary<CombinationType, CombinationData>();
    
    public CombinationData GetValue(CombinationType combination) => _data[combination];

}

[Serializable]
public class CombinationData
{
    public string Name;
    public float Multiplier;
}