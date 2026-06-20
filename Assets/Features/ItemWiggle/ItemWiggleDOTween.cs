using UnityEngine;
using DG.Tweening;

public class ItemWiggleDOTween : MonoBehaviour
{
    public bool IsWiggling = false;

    private RectTransform rect;
    private Tween wiggleTween;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (IsWiggling)
            StartWiggle();
    }

    void OnDisable()
    {
        StopWiggle();
    }

    public void StartWiggle()
    {
        if (wiggleTween != null && wiggleTween.IsActive())
            return;

        wiggleTween = rect.DOShakeAnchorPos(
            duration: 1f,
            strength: new Vector2(10f, 10f),
            vibrato: 20,
            randomness: 90,
            snapping: false,
            fadeOut: true
        ).SetLoops(-1, LoopType.Restart);
    }

    public void StopWiggle()
    {
        if (wiggleTween != null)
        {
            wiggleTween.Kill();
            wiggleTween = null;
        }

        rect.anchoredPosition = Vector2.zero;
    }

    public void SetWiggle(bool state)
    {
        IsWiggling = state;

        if (state)
            StartWiggle();
        else
            StopWiggle();
    }
}
