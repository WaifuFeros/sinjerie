using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AfflictionEffect : MonoBehaviour
{
    public static AfflictionEffect Instance;

    [SerializeField] private Image _fireImage;
    [SerializeField] private TextMeshProUGUI _fireCountText;

    [SerializeField] private Image _wetImage;
    [SerializeField] private TextMeshProUGUI _wetCountText;

    [SerializeField] private Image _paralyzeImage;
    [SerializeField] private TextMeshProUGUI _paralizeCountText;

    [SerializeField] private Image _freezeImage;
    [SerializeField] private TextMeshProUGUI _freezeCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public void UpdatePlayerVisual()
    {

    }
}
