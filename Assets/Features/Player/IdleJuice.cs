using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class IdleJuice : MonoBehaviour
{

    private float idleScaleMultiplier = 1.05f;
    private float idleDuration = 1.5f;
    private Tween idleTween;

    void Start()
    {
        transform.DOScale(Vector3.one * idleScaleMultiplier, idleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }
}