using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public bool DataBaseInitialized => _databaseState == ItemDataBaseInizializationState.Initialized;

    [Header("Configuration")]
    [SerializeField] private GameObject itemPrefab;

    // Liste de tous les items
    public List<ItemBrain> activeItems = new List<ItemBrain>();

    // Liste des Items SO
    public List<ObjetSO> ItemsData = new List<ObjetSO>();

    private ItemDataBaseInizializationState _databaseState = ItemDataBaseInizializationState.NotInizialized;

    private Action _onLoadItemsCompleteEvent;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        Initialize(() =>
        {
            _databaseState = ItemDataBaseInizializationState.Initialized;
        });
    }

    public void Initialize(Action onLoadCompleted)
    {
        _databaseState = ItemDataBaseInizializationState.Initializing;
        _onLoadItemsCompleteEvent = onLoadCompleted;
        Addressables.LoadAssetsAsync<ObjetSO>("Items", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ItemsData.AddRange(handle.Result);
                _onLoadItemsCompleteEvent?.Invoke();
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
                _onLoadItemsCompleteEvent = onLoadCompleted;
                break;
            case ItemDataBaseInizializationState.Initialized:
                onLoadCompleted?.Invoke();
                break;
        }
    }

    public void SpawnRandomItem()
    {
        if (!DataBaseInitialized || ItemsData.Count == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, ItemsData.Count);
        ObjetSO randomData = ItemsData[randomIndex];
        SpawnItem(randomData);
    }
    public void SpawnItem(ObjetSO objetSO)
    {
        if (!DataBaseInitialized
            || InventoryManager.Instance == null
            || !InventoryManager.Instance.HasEmptySlot()
            || ItemsData.Count == 0)
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

    internal enum ItemDataBaseInizializationState
    {
        NotInizialized,
        Initializing,
        Initialized
    }
}