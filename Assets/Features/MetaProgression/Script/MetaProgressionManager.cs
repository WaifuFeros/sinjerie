using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaProgressionManager : MonoBehaviour
{
    public static MetaProgressionManager Instance;

    public Action OnBananaCountUpdated;

    public int CurrentBananaCount
    {
        get
        {
            return _currentBananaCount;
        }

        set
        {
            _currentBananaCount = Mathf.Clamp(value, 0, TotalBananaCount);
            OnBananaCountUpdated?.Invoke();
        }
    }

    public int TotalBananaCount;

    [SerializeField] private StatUpgradeData[] _statUpgrades;

    private Dictionary<StatUpgradeType, int> _currentUpgradesDictionary = new Dictionary<StatUpgradeType, int>();

    private int _currentBananaCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            _currentUpgradesDictionary.Clear();
            foreach (var item in _statUpgrades)
            {
                _currentUpgradesDictionary.Add(item.type, 0);
            }

            _currentBananaCount = TotalBananaCount;

            return;
        }

        Destroy(gameObject);
    }

    public float GetValueByTypeAsFloat(StatUpgradeType type)
    {
        int upgradeLevel = _currentUpgradesDictionary[type];

        var upgradeData = GetUpgradeData(type);

        if (upgradeData != null)
        {
            return upgradeData.GetValue(upgradeLevel);
        }

        return 0;
    }

    public int GetValueByType(StatUpgradeType type)
    {
        int upgradeLevel = _currentUpgradesDictionary[type];
        var upgradeData = GetUpgradeData(type);

        if (upgradeData != null)
        {
            return Mathf.RoundToInt(upgradeData.GetValue(upgradeLevel));
        }

        return 0;
    }

    public void Upgrade(StatUpgradeType type)
    {
        Debug.Log("try update");
        var upgradeData = GetUpgradeData(type);

        if (upgradeData != null)
        {
            Debug.Log("upgrade found");
            Debug.Log(_currentUpgradesDictionary[type]);
            int upgradeLevel = _currentUpgradesDictionary[type];
            upgradeLevel++;
            _currentUpgradesDictionary[type] = Mathf.Clamp(upgradeLevel, 0, upgradeData.maxUpdateCount);
            Debug.Log(_currentUpgradesDictionary[type]);

            CurrentBananaCount -= upgradeData.upgradeCost;
        }
    }

    public void Downgrade(StatUpgradeType type)
    {
        var upgradeData = GetUpgradeData(type);

        if (upgradeData != null)
        {
            Debug.Log("upgrade found");
            Debug.Log(_currentUpgradesDictionary[type]);
            int upgradeLevel = _currentUpgradesDictionary[type];
            upgradeLevel--;
            _currentUpgradesDictionary[type] = Mathf.Clamp(upgradeLevel, 0, upgradeData.maxUpdateCount);
            Debug.Log(_currentUpgradesDictionary[type]);

            CurrentBananaCount += upgradeData.upgradeCost;
        }
    }

    public int GetUpgradeLevel(StatUpgradeType type)
    {
        return _currentUpgradesDictionary[type];
    }

    public StatUpgradeData GetUpgradeData(StatUpgradeType type)
    {
        foreach (var item in _statUpgrades)
        {
            if (item.type == type)
                return item;
        }

        return null;
    }

    public void AddBananaToTotal(int count)
    {
        TotalBananaCount += count;
        _currentBananaCount += count;
    }

    public bool HasEnoughBanana(int count)
    {
        return CurrentBananaCount >= count;
    }
}

public enum StatUpgradeType
{
    Health,
    Stamina,
    Damage,
    Heal,
    RewardCount,
    GoldIncrease
}

[Serializable]
public class StatUpgradeData
{
    public StatUpgradeType type;
    public float upgradeValue;
    [Min(1)] public int upgradeCost;
    [Min(1)] public int maxUpdateCount;

    public float GetValue(int upgradeLevel)
    {
        return upgradeValue * maxUpdateCount;
    }
}

[Serializable]
public struct StatUpgradeSave
{
    public StatUpgradeType type;
    public int upgradeLevel;
}