using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThunderParticle : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeDuration = 0.75f;
    [SerializeField] private float _startingScale = 0.6f;
    [SerializeField] private float _shakeDuration = 0.75f;
    [SerializeField] private float _shakeStrenght = 15;

    [Header("Thunder Settings")]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private Color _endColor = new Color(0.5f, 0.8f, 1f, 0f);

    private Sequence _thunderSequence;

    private void Start()
    {
        ResetVisuals();
    }

    private void OnDestroy()
    {
        _thunderSequence?.Kill();
        DOTween.Kill(_rectTransform);
        DOTween.Kill(_canvasGroup);
        if (_image != null) DOTween.Kill(_image);
    }

    private void ResetVisuals()
    {
        _canvasGroup.alpha = 0f;
        _rectTransform.localScale = Vector3.one * _startingScale;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Shake()
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH        2");
        _rectTransform.DOShakeAnchorPos(_shakeDuration, Vector2.right * _shakeStrenght).OnKill(() =>
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        });
    }

    public void Thunder()
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH");
        _thunderSequence?.Kill(true);

        ResetVisuals();

        _thunderSequence = DOTween.Sequence();

        _thunderSequence.Append(_canvasGroup.DOFade(1f, 0.05f))
            .Join(_rectTransform.DOScale(1.2f, 0.05f).SetEase(Ease.OutQuad)) 
            .Join(_image.DOColor(_flashColor, 0.05f)); 

        _thunderSequence.Append(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InQuad))
            .Join(_rectTransform.DOScale(_startingScale, _fadeDuration).SetEase(Ease.InQuad))
            .Join(_image.DOColor(_endColor, _fadeDuration).SetEase(Ease.InQuad));

        _thunderSequence.OnComplete(() =>
        {
            ResetVisuals();
        });
    }
}