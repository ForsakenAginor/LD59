using System;
using Assets.Source.Scripts.AudioLogic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightCatcher : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioPlayer.Instance.PlayCardHighlight();
    }
}