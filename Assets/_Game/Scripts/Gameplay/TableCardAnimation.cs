using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TableCardAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _flyInDuration = 0.3f;
    [SerializeField] private float _flyOutDuration = 0.4f;
    [SerializeField] private float _flyOutDistance = 600f;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    // Карта влетает снизу с пружинкой
    public void PlayFlyIn(float delay)
    {
        _canvasGroup.alpha = 0f;
        Vector2 targetPos = _rect.anchoredPosition;
        _rect.anchoredPosition = targetPos + new Vector2(0, -150f);
        _rect.localScale = Vector3.one * 0.75f;
        _rect.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

        Sequence seq = DOTween.Sequence();
        seq.SetDelay(delay);
        seq.Append(_canvasGroup.DOFade(1f, _flyInDuration * 0.4f));
        seq.Join(_rect.DOAnchorPos(targetPos, _flyInDuration).SetEase(Ease.OutBack));
        seq.Join(_rect.DOScale(1f, _flyInDuration).SetEase(Ease.OutBack));
        seq.Join(_rect.DOLocalRotate(Vector3.zero, _flyInDuration).SetEase(Ease.OutBack));
    }

    // Карта улетает вверх и исчезает
    public IEnumerator PlayFlyOut()
    {
        float randomX = Random.Range(-60f, 60f);
        float randomRot = Random.Range(-20f, 20f);

        Sequence seq = DOTween.Sequence();
        seq.Append(_rect.DOAnchorPosY(_rect.anchoredPosition.y + _flyOutDistance, _flyOutDuration)
            .SetEase(Ease.InBack));
        seq.Join(_rect.DOAnchorPosX(_rect.anchoredPosition.x + randomX, _flyOutDuration)
            .SetEase(Ease.InQuad));
        seq.Join(_rect.DOScale(0.6f, _flyOutDuration).SetEase(Ease.InBack));
        seq.Join(_rect.DOLocalRotate(new Vector3(0, 0, randomRot), _flyOutDuration));
        seq.Join(_canvasGroup.DOFade(0f, _flyOutDuration * 0.5f)
            .SetDelay(_flyOutDuration * 0.5f));

        yield return seq.WaitForCompletion();
        Destroy(gameObject);
    }
}