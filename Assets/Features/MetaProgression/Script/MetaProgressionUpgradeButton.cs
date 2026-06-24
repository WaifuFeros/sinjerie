using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaProgressionUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _downgradeButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private StatUpgradeType _upgradeType;
    [SerializeField] private TextMeshProUGUI _upgradeLevelText;

    private void Start()
    {
        MetaProgressionManager.Instance.OnBananaCountUpdated += UpdateButtons;
        //UpdateButtons();
    }

    private void OnEnable()
    {
        UpdateButtons();
    }

    private void OnDestroy()
    {
        MetaProgressionManager.Instance.OnBananaCountUpdated -= UpdateButtons;
    }

    public void UpdateButtons()
    {
        var upgradeData = MetaProgressionManager.Instance.GetUpgradeData(_upgradeType);
        int currentUpgradeLevel = MetaProgressionManager.Instance.GetUpgradeLevel(_upgradeType);

        _downgradeButton.interactable = currentUpgradeLevel > 0;
        _upgradeButton.interactable = MetaProgressionManager.Instance.HasEnoughBanana(upgradeData.upgradeCost) && currentUpgradeLevel < upgradeData.maxUpdateCount;
        _upgradeLevelText.text = MetaProgressionManager.Instance.GetUpgradeLevel(_upgradeType).ToString();
    }

    public void Upgrade()
    {
        MetaProgressionManager.Instance.Upgrade(_upgradeType);
        _upgradeLevelText.text = MetaProgressionManager.Instance.GetUpgradeLevel(_upgradeType).ToString();
    }
    public void Downgrade()
    {
        MetaProgressionManager.Instance.Downgrade(_upgradeType);
        _upgradeLevelText.text = MetaProgressionManager.Instance.GetUpgradeLevel(_upgradeType).ToString();
    }
}
