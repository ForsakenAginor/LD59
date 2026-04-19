using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IJokerConfiguration
{
    public JokerData GetJokerData(string jokerName);
}

[CreateAssetMenu(fileName = "JokerConfiguration", menuName = "Configurations/JokerConfiguration")]
public class JokerConfiguration : SerializedScriptableObject, IJokerConfiguration
{
    [SerializeField] private Dictionary<string, JokerData> _data = new Dictionary<string, JokerData>();
    
    public JokerData GetJokerData(string jokerName) => _data[jokerName];

}