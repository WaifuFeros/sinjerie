using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AfflictionEffect : MonoBehaviour
{
    [SerializeField] private Image _fireImage;
    [SerializeField] private TextMeshProUGUI _fireCountText;

    [SerializeField] private Image _wetImage;
    [SerializeField] private TextMeshProUGUI _wetCountText;

    [SerializeField] private Image _paralizeImage;
    [SerializeField] private TextMeshProUGUI _paralizeCountText;

    [SerializeField] private Image _freezeImage;
    [SerializeField] private Image _freezeFillImage;
    [SerializeField] private TextMeshProUGUI _freezeCountText;

    public void UpdateVisuals(int fireCounter, int wetCounter, int paralizeCounter, int freezeCounter)
    {
        _fireImage.gameObject.SetActive(fireCounter > 0);
        _fireCountText.text = fireCounter.ToString();

        _wetImage.gameObject.SetActive(wetCounter > 0);
        _wetCountText.text = wetCounter.ToString();

        _paralizeImage.gameObject.SetActive(paralizeCounter > 0);
        _paralizeCountText.text = paralizeCounter.ToString();

        _freezeImage.gameObject.SetActive(freezeCounter > 0);
        _freezeFillImage.fillAmount = Mathf.Clamp01((float)freezeCounter / CombatSystem.Instance.freezeProcThreshold);
        _freezeCountText.text = freezeCounter.ToString();
    }
}
