using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class GoldChest : MonoBehaviour
{
    [Header("Gold")]
    [SerializeField] private int goldGiven = 20;
    [Header("Anim")]
    [SerializeField] private float bounceDuration = 0.6f;
    [SerializeField] private float bounceStrength = 20f;
    [SerializeField] private float clickScaleMultiplier = 1.3f;
    [SerializeField] private float clickDuration = 0.2f;

    [Header("Audio")]
    [SerializeField] private EventReference chestSound;

    void Start()
    {
        transform.DOLocalMoveY(transform.localPosition.y + bounceStrength, bounceDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnChestClicked()
    {
        RuntimeManager.PlayOneShot(chestSound);
        PlayerManager.Instance.AddGold(goldGiven);
        transform.DOKill();
        transform.DOScale(transform.localScale * clickScaleMultiplier, clickDuration / 2)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, clickDuration / 2)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => Destroy(gameObject));
            });
    }
}