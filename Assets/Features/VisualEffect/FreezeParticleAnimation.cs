using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeParticleAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.75f;
    [SerializeField] private float _startingScale = 0.6f;
    [SerializeField] private float _shakeDuration = 0.75f;
    [SerializeField] private float _shakeStrenght = 15;

    private void Start()
    {
        _canvasGroup.alpha = 0f;
        _rectTransform.localScale = Vector3.one * _startingScale;
    }

    private void OnDestroy()
    {
        DOTween.Kill(_rectTransform);
    }

    public void Display()
    {
        DOTween.Kill(_rectTransform);

        var seq = DOTween.Sequence();

        seq.SetTarget(_rectTransform);
        seq.Append(_canvasGroup.DOFade(1, _fadeDuration));
        seq.Join(_rectTransform.DOScale(1, _fadeDuration));

        seq.Play();
    }

    public void Hide()
    {
        DOTween.Kill(_rectTransform);

        var seq = DOTween.Sequence();

        seq.SetTarget(_rectTransform);
        seq.Append(_canvasGroup.DOFade(0, _fadeDuration));
        seq.Join(_rectTransform.DOScale(_startingScale, _fadeDuration));

        seq.Play();
    }

    public void Shake()
    {
        _rectTransform.DOShakeAnchorPos(_shakeDuration, Vector2.right * _shakeStrenght).OnKill(() =>
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        });
    }
}
