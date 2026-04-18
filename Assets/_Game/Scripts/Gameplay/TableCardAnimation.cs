using DG.Tweening;
using UnityEngine;

public class TableCardAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _duration = 0.5f;
    
    private void Start()
    {
        _canvasGroup.DOFade(1f, _duration);
    }
}