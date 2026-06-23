using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

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
            strength: new Vector2(5f, 5f),
            vibrato: 10,
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
