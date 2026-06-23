using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HangingSignRotation : MonoBehaviour
{
    [SerializeField] private float duration = 2f;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 0, -10f);

        transform.DOLocalRotate(new Vector3(0, 0, 10f), duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}