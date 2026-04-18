using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _cardFrequency;
    [SerializeField] private TMP_Text _cardSuit;

    private Card _card;
    
    public event Action<CardVisual> OnClick;
    
    public Card Card => _card;
    
    public void Init(Card card)
    {
        _card = card;
        
        string freqStr = card.Frequency switch
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
        
        string suitStr = card.Suit switch
        {
            Suit.Sine => "~",
            Suit.Saw => "ᛛ",
            Suit.Digital => "▤",
            _ => "?"
        };
        
        _cardFrequency.text = freqStr;
        _cardSuit.text = suitStr;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void Enlarge()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void Normalize()
    {
        transform.localScale = Vector3.one;
    }
}