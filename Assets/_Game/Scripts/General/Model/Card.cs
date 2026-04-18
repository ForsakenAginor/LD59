using System;

public class Card
{
    public Card(Suit suit, Frequency frequency)
    {
        Suit = suit;
        Frequency = frequency;
    }
    
    public Suit Suit { get; }
    public Frequency Frequency { get; }
    
    // Для отладки
    public override string ToString()
    {
        string freqStr = Frequency switch
        {
            Frequency._100Hz => "100",
            Frequency._200Hz => "200",
            Frequency._300Hz => "300",
            Frequency._400Hz => "400",
            Frequency._500Hz => "500",
            Frequency._600Hz => "600",
            Frequency._700Hz => "700",
            Frequency._800Hz => "800",
            Frequency._900Hz => "900",
            Frequency._1000Hz => "1000",
            Frequency._1200Hz => "1200",
            Frequency._1500Hz => "1500",
            Frequency._2000Hz => "2000",
            _ => "?"
        };
        
        string suitStr = Suit switch
        {
            Suit.Sine => "~",
            Suit.Saw => "ᛛ",
            Suit.Digital => "▤",
            _ => "?"
        };
        
        return $"{suitStr}{freqStr}Hz";
    }
    
}