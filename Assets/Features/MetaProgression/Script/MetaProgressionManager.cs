using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MetaProgressionManager : MonoBehaviour
{
    public static MetaProgressionManager Instance;

    [SerializeField] private StatUpgradeData[] _statUpgrades;

    //private StatUpgradeSave[] _currentUpgrades;
    private Dictionary<StatUpgradeType, int> _currentUpgradesDictionary = new Dictionary<StatUpgradeType, int>();

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

            return;
        }

        Destroy(gameObject);
    }

    public float GetValueByType(StatUpgradeType type)
    {
        int upgradeLevel = _currentUpgradesDictionary[type];

        foreach (var item in _statUpgrades)
        {
            if (item.type == type)
            {
                return item.GetValue(upgradeLevel);
            }
        }

        return 0;
    }
}

public enum StatUpgradeType
{
    Health,
    Stamina,
    Damage,
    RewardCount,
    GoldIncrease
}

[Serializable]
public struct StatUpgradeData
{
    public StatUpgradeType type;
    public float upgrade;
    [Min(1)] public int maxUpdateCount;

    public float GetValue(int upgradeLevel)
    {
        return upgrade * maxUpdateCount;
    }
}

[Serializable]
public struct StatUpgradeSave
{
    public StatUpgradeType type;
    public int upgradeLevel;
}