using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JokerData
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public float Rarity;
    
    public int AddedBase;
    public float AddedMultiplier;
    public float Multiplier;
    public int HandSize;
    public int Rerolls;
    public int Signals;
    public List<Frequency> Frequencies = new List<Frequency>();
    public List<Suit> Suits = new List<Suit>();
    public List<CombinationType> Combinations = new List<CombinationType>();
}