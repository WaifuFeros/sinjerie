using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BouncingLogo : MonoBehaviour
{
    [Header("Configuration du Mouvement")]
    [SerializeField] private float travelDistance = 0.5f;
    [SerializeField] private float duration = 1.5f;

    void Start()
    {
        float startY = transform.localPosition.y;
        transform.DOLocalMoveY(startY + travelDistance, duration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
        transform.DOScaleY(0.9f, duration + 1.3f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }
}