using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using UnityEngine.UI;

public class GoldChest : MonoBehaviour
{
    [SerializeField] private GameObject particule;
    [Header("Gold")]
    [SerializeField] private int goldGiven = 20;
    [Header("Anim")]
    [SerializeField] private float bounceDuration = 0.6f;
    [SerializeField] private float bounceStrength = 20f;
    [SerializeField] private float clickScaleMultiplier = 1.3f;
    [SerializeField] private float clickDuration = 0.6f;

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
        VisualEffectManager.Instance.TriggerBurst(particule, VisualEffectManager.ParticleEffectType.Gold);
        PlayerManager.Instance.AddGold(goldGiven);
        transform.DOKill();
        transform.DOScale(transform.localScale * clickScaleMultiplier, clickDuration / 2)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, clickDuration / 2)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                        Destroy(gameObject)
                    );
            });
    }
}