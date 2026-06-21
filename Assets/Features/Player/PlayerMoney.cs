using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        PlayerManager.Instance.OnGoldUpdateEvent += UpdateGoldText;
    }

    private void OnEnable()
    {
        UpdateGoldText();
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnGoldUpdateEvent -= UpdateGoldText;
    }

    private void UpdateGoldText()
    {
        if (PlayerManager.Instance != null)
            _text.text = PlayerManager.Instance.stats.gold.ToString();
    }
}
