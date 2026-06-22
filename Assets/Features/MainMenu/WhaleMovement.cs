using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WhaleMovement : MonoBehaviour
{
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] private float rotationAngle;
    [SerializeField] private float rotationDuration;

    void Start()
    {
        float startX = transform.localPosition.x;
        transform.DOLocalMoveX(startX + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        float startY = transform.localPosition.y;
        transform.DOLocalMoveY(startY + (moveDistance * 0.5f), moveDuration * 1.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalRotate(new Vector3(rotationAngle, 0, 0), rotationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}