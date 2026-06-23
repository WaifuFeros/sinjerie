using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Nécessaire pour le composant Image
using DG.Tweening;    // Nécessaire pour DOTween

public class OwlPress : MonoBehaviour
{
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private float duration = 0.15f; 
    [SerializeField] private Vector3 pressedScale = new Vector3(0.85f, 0.85f, 1f);

    private Sprite originalSprite;
    private Vector3 originalScale;
    private bool isAnimating = false;
    private Image targetImage;

    void Start()
    {
        targetImage = GetComponent<Image>();
        originalSprite = targetImage.sprite;
        originalScale = targetImage.transform.localScale;
    }

    public void OnOwlClicked()
    {
        isAnimating = true;

        targetImage.sprite = pressedSprite;
        targetImage.transform.DOScale(pressedScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetImage.transform.DOScale(originalScale, duration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        targetImage.sprite = originalSprite;
                        isAnimating = false;
                    });
            });
    }
}