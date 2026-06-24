using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BananaCount : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        UpdateBananaCount();
        MetaProgressionManager.Instance.OnBananaCountUpdated += UpdateBananaCount;
    }

    private void OnDestroy()
    {
        MetaProgressionManager.Instance.OnBananaCountUpdated -= UpdateBananaCount;
    }

    private void UpdateBananaCount()
    {
        text.text = MetaProgressionManager.Instance.CurrentBananaCount.ToString();
    }
}
