using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Nécessaire pour le composant Image
using DG.Tweening;
using FMODUnity;    // Nécessaire pour DOTween

public class AnimalPress : MonoBehaviour
{
    public enum AnimalType { Owl, Frog }

    [Header("Type d'animal")]
    [SerializeField] private AnimalType animalType = AnimalType.Owl;

    [Header("Configuration visuelle")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private Vector3 pressedScale = new Vector3(0.85f, 0.85f, 1f);

    [Header("Audio")]
    [SerializeField] private EventReference owlSound;
    [SerializeField] private EventReference frogSound;

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

    public void OnAnimalClicked()
    {
        if (isAnimating) return;

        isAnimating = true;

        PlayAnimalSound();

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

    private void PlayAnimalSound()
    {
        switch (animalType)
        {
            case AnimalType.Owl:
                RuntimeManager.PlayOneShot("owlSound");
                break;

            case AnimalType.Frog:
                RuntimeManager.PlayOneShot("frogSound");
                break;
        }
    }
}