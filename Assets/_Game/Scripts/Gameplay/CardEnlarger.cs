using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEnlarger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _selectionBorder;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_selectionBorder.enabled == false)
            transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_selectionBorder.enabled == false)
            transform.localScale = Vector3.one * 1f;
    }
}