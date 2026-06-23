using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static WeatherManager;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public bool DataBaseInitialized => _databaseState == ItemDataBaseInizializationState.Initialized;

    [Header("Configuration")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Asset")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private Sprite _atkSprite;

    // Liste de tous les items
    public List<ItemBrain> activeItems { get; private set; } = new List<ItemBrain>();

    // Liste des Items SO
    public List<ObjetSO> ItemsDatabase { get; private set; } = new List<ObjetSO>();

    private ItemDataBaseInizializationState _databaseState = ItemDataBaseInizializationState.NotInizialized;

    private Action _onLoadItemsCompleteEvent;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        Initialize(null);

    }

    public void Initialize(Action onLoadCompleted)
    {
        if (_databaseState == ItemDataBaseInizializationState.Initializing)
            return;

        _databaseState = ItemDataBaseInizializationState.Initializing;
        _onLoadItemsCompleteEvent += onLoadCompleted;
        Addressables.LoadAssetsAsync<ObjetSO>("Items", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ItemsDatabase.AddRange(handle.Result);
                _databaseState = ItemDataBaseInizializationState.Initialized;
                _onLoadItemsCompleteEvent?.Invoke();
                _onLoadItemsCompleteEvent = null;
            }
        };

    }

    public void WaitForItemDatabaseInitialization(Action onLoadCompleted)
    {
        switch (_databaseState)
        {
            default:
            case ItemDataBaseInizializationState.NotInizialized:
                Initialize(onLoadCompleted);
                break;
            case ItemDataBaseInizializationState.Initializing:
                _onLoadItemsCompleteEvent += onLoadCompleted;
                break;
            case ItemDataBaseInizializationState.Initialized:
                onLoadCompleted?.Invoke();
                break;
        }
    }

    public void SpawnRandomItem()
    {
        if (!DataBaseInitialized || ItemsDatabase.Count == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, ItemsDatabase.Count);
        ObjetSO randomData = ItemsDatabase[randomIndex];
        SpawnItem(randomData);
    }
    public void SpawnItem(ObjetSO objetSO)
    {
        if (!DataBaseInitialized
            || InventoryManager.Instance == null
            || !InventoryManager.Instance.HasEmptySlot()
            || ItemsDatabase.Count == 0)
            return;

        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

        ItemBrain newItemBrain = Instantiate(itemPrefab, position, Quaternion.identity, InventoryManager.Instance.itemsParent).GetComponent<ItemBrain>();

        newItemBrain.InitItem(objetSO);
        activeItems.Add(newItemBrain);

        var slot = InventoryManager.Instance.GetEmptySlot();
        slot.SetItem(newItemBrain);
    }

    // Recup des items de l'inventaire
    public List<ItemBrain> GetAllItems()
    {
        return activeItems;
    }
    public List<ItemBrain> GetItemsOfType(ObjetEffectType type)
    {
        List<ItemBrain> result = new List<ItemBrain>();
        foreach (ItemBrain item in activeItems)
        {
            if (item.itemData.objectType == type)
            {
                result.Add(item);
            }
        }
        return result;
    }

    public List<ItemBrain> GetItemsOfMaterial(ObjetMaterialType type)
    {
        List<ItemBrain> result = new List<ItemBrain>();
        foreach (ItemBrain item in activeItems)
        {
            if (item.itemData.objetMaterialType == type)
            {
                result.Add(item);
            }
        }
        return result;
    }
    public List<ItemBrain> GetRandomItems(int nb)
    {
        List<ItemBrain> result = new List<ItemBrain>();
        if (nb <= 0 || activeItems.Count == 0)
        {
            return result;
        }
        List<ItemBrain> tempPool = new List<ItemBrain>(activeItems);
        int countToGet = Mathf.Min(nb, tempPool.Count);
        for (int i = 0; i < countToGet; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempPool.Count);
            result.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
        return result;
    }

    // Applique effet sur un item
    public void ChangeItemType(List<ItemBrain> items, ObjetEffectType type)
    {
        foreach (ItemBrain item in items)
        {
            item.itemData.objectType = type;
            item.TriggerVisualUpdate();
        }
    }
    public void ChangeItemWeight(List<ItemBrain> items, int Weight)
    {
        foreach (ItemBrain item in items)
        {
            item.itemData.objetWeight = Weight;
            item.TriggerVisualUpdate();
        }
    }
    public void ChangeItemEffect(List<ItemBrain> items, int Effect)
    {
        foreach (ItemBrain item in items)
        {
            item.itemData.objectEffect = Effect;
            item.TriggerVisualUpdate();
        }
    }

    public Sprite GetRaritySprite(ObjetRarity rarity)
    {
        switch (rarity)
        {
            default:
            case ObjetRarity.Common:
                return _communSprite;
            case ObjetRarity.Uncommon:
                return _uncommonSprite;
            case ObjetRarity.Rare:
                return _rareSprite;
            case ObjetRarity.Epic:
                return _epicSprite;
            case ObjetRarity.Legendary:
                return _lengendarySprite;
        }
    }

    public bool GetObjetTypeSprite(ObjetEffectType type, out Sprite result)
    {
        switch (type)
        {
            default:
            case ObjetEffectType.Attack:
                result = _atkSprite;
                return true;
            case ObjetEffectType.Heal:
                result = _healSprite;
                return true;
            case ObjetEffectType.Special:
                result = null;
                return false;
        }
    }

    internal enum ItemDataBaseInizializationState
    {
        NotInizialized,
        Initializing,
        Initialized
    }
    private bool ShouldItemReactToEnemy(ObjetMaterialType itemType, ObjetMaterialType enemyType)
    {
        switch (enemyType)
        {
            case ObjetMaterialType.Fire:
                return itemType == ObjetMaterialType.Fire ||
                       itemType == ObjetMaterialType.Wood;

            case ObjetMaterialType.Ice:
                return itemType == ObjetMaterialType.Ice ||
                       itemType == ObjetMaterialType.PerfectIce;

            case ObjetMaterialType.Electricity:
                return itemType == ObjetMaterialType.Metal;

            default:
                return false;
        }
    }

    private bool ShouldItemReactToWeather(ObjetMaterialType itemType, GameWeatherType weather)
    {
        switch (weather)
        {
            case GameWeatherType.Thunderstorm:
                return itemType == ObjetMaterialType.Metal;

            case GameWeatherType.Snow:
                return itemType == ObjetMaterialType.Ice ||
                       itemType == ObjetMaterialType.PerfectIce;

            case GameWeatherType.ClearSky:
            default:
                return false;
        }
    }

    private ObjetMaterialType GetEnemyElementType()
    {
        var enemy = CombatSystem.Instance.Enemy;

        if (enemy == null)
            return ObjetMaterialType.None;

        if (enemy.FireCounter > 0)
            return ObjetMaterialType.Fire;

        if (enemy.FreezeCounter > 0)
            return ObjetMaterialType.Ice;

        if (enemy.ParalyzeCounter > 0)
            return ObjetMaterialType.Electricity;

        if (enemy.WetCounter > 0)
            return ObjetMaterialType.Water;

        return ObjetMaterialType.None;
    }



    public void UpdateAllReactions(GameWeatherType weather)
    {
        ObjetMaterialType enemyType = GetEnemyElementType();

        foreach (var item in activeItems)
        {
            var wiggle = item.GetComponentInChildren<ItemWiggleDOTween>();
            if (wiggle == null) continue;

            bool weatherReact = ShouldItemReactToWeather(item.itemData.objetMaterialType, weather);
            bool enemyReact = ShouldItemReactToEnemy(item.itemData.objetMaterialType, enemyType);

            wiggle.SetWiggle(weatherReact || enemyReact);
        }
    }

    public void setActiveMetal()
    {
        foreach (var item in activeItems)
        {
            var _metalObject = item.GetComponent<ParticleMetalObject>();
            print(_metalObject);
            if (_metalObject == null) continue;
            else if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm && item.itemData.objetMaterialType == ObjetMaterialType.Metal)
            {
                _metalObject.enabled = true;
            }
        }
    }
}