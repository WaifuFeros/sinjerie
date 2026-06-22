using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    [Header("GameObjects à animer")]
    [SerializeField] private GameObject wheelback;
    [SerializeField] private GameObject wheelgen;

    [Header("Idle")]
    [SerializeField] private float idleScaleY = 1.1f;
    [SerializeField] private float idleDuration = 1f;

    [Header("Clic (Front Wheel)")]
    [SerializeField] private float clickScaleMultiplier = 1.2f;
    [SerializeField] private float clickDuration = 0.3f;

    [Header("Rotation (Back Wheel)")]
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float rotationAngleMin = -360f;
    [SerializeField] private float rotationAngleMax = -720;

    private Button wheelButton;
    private Vector3 wheelgenInitialPosition;

    void Start()
    {
        wheelButton = GetComponent<Button>();

        if (wheelgen != null)
        {
            wheelgenInitialPosition = wheelgen.transform.localPosition;
        }

        StartIdleAnimation(wheelgen);
    }

    public void WheelClick()
    {
        if (PlayerManager.Instance.stats.gold >= 5)
        {
            PlayerManager.Instance.removeGold(5);
            wheelButton.interactable = false;
            TriggerBackWheelRotation(wheelback);
            TriggerFrontClickAnimation(wheelgen);
        }
        else
        {
            TriggerNotEnoughGoldAnimation(wheelgen);
        }
    }

    private void TriggerNotEnoughGoldAnimation(GameObject target)
    {
        target.transform.DOKill();

        target.transform.localPosition = wheelgenInitialPosition;
        target.transform.localScale = Vector3.one;
        target.transform.DOShakePosition(0.5f, new Vector3(30f, 0f, 0f), 9, 90, false, true)
            .OnComplete(() =>
            {
                target.transform.localPosition = wheelgenInitialPosition;
                StartIdleAnimation(target);
            });
    }

    private void StartIdleAnimation(GameObject target)
    {
        target.transform.localScale = Vector3.one;

        target.transform.DOScaleY(idleScaleY, idleDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(target.GetInstanceID() + "_idle");
    }

    private void TriggerBackWheelRotation(GameObject target)
    {
        target.transform.DOKill();

        int rotationAngle = (int)Random.Range(rotationAngleMin, rotationAngleMax);
        target.transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => CheckWheelResult(target));
    }

    private void TriggerFrontClickAnimation(GameObject target)
    {
        target.transform.DOKill();

        target.transform.localPosition = wheelgenInitialPosition;
        target.transform.localScale = Vector3.one;

        Vector3 punchScale = new Vector3(clickScaleMultiplier - 1, clickScaleMultiplier - 1, clickScaleMultiplier - 1);

        target.transform.DOPunchScale(punchScale, clickDuration, 5, 0.5f)
            .OnComplete(() =>
            {
                StartIdleAnimation(target);
            });
    }

    private void CheckWheelResult(GameObject target)
    {
        wheelButton.interactable = true;
        float currentAngle = target.transform.localEulerAngles.z;
        currentAngle = (currentAngle % 360 + 360) % 360;

        if (currentAngle >= 335f || currentAngle <= 25f)
        {
            PlayerManager.Instance.Heal(10);
        }
        else if (currentAngle >= 151f && currentAngle <= 205f)
        {
            PlayerManager.Instance.AddGold(20);
        }
    }
}