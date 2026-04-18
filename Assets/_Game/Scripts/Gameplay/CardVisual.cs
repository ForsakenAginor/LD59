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
            Frequency._100Hz => "100Hz",
            Frequency._200Hz => "200Hz",
            Frequency._300Hz => "300Hz",
            Frequency._400Hz => "400Hz",
            Frequency._500Hz => "500Hz",
            Frequency._600Hz => "600Hz",
            Frequency._700Hz => "700Hz",
            Frequency._800Hz => "800Hz",
            Frequency._900Hz => "900Hz",
            Frequency._1000Hz => "1000Hz",
            Frequency._1200Hz => "1200Hz",
            Frequency._1500Hz => "1500Hz",
            Frequency._2000Hz => "2000Hz",
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