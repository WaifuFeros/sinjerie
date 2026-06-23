using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderParticle : MonoBehaviour
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

    public void Shake()
    {

        _rectTransform.DOShakeAnchorPos(_shakeDuration, Vector2.right * _shakeStrenght).OnKill(() =>
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        });
    }
}
