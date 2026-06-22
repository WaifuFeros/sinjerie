using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonJuice : MonoBehaviour, IPointerClickHandler
{

    private float idleScaleMultiplier = 1.05f;
    private float idleDuration = 1.5f;
    private float clickScaleMultiplier = 0.15f;
    private float clickDuration = 0.2f;
    private int clickVibrato = 5;

    private Tween idleTween;

    void Start()
    {
        StartIdleAnimation();
    }

    void StartIdleAnimation()
    {
        idleTween = transform.DOScale(Vector3.one * idleScaleMultiplier, idleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        idleTween.Pause();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(new Vector3(-clickScaleMultiplier, -clickScaleMultiplier, 0), clickDuration, clickVibrato)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                idleTween.Play();
            });
    }

    private void OnDestroy()
    {
        idleTween.Kill();
    }
}