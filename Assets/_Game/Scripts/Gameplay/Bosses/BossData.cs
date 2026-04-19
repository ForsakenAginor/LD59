using System;
using System.Collections.Generic;

[Serializable]
public class BossData
{
    public string Description = string.Empty;
    
    public List<Frequency> Frequencies = new List<Frequency>();

    public List<Suit> Suits = new List<Suit>();

    public int Hand = -2;
    
    public bool IsOneSignal = false;
        
    public bool IsNoReroll = false;
}