using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private Transform itemParents;

    // Liste de tous les items
    public List<ItemBrain> activeItems = new List<ItemBrain>();

    // Liste interne des SO chargés avec adressables
    private List<ObjetSO> availableItemData = new List<ObjetSO>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }


    public void Initialize(Action onLoadCompleted)
    {
        Addressables.LoadAssetsAsync<ObjetSO>("Items", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                availableItemData.AddRange(handle.Result);
                onLoadCompleted?.Invoke();
            }
        };

    }

    public void SpawnRandomItem()
    {
        if (availableItemData.Count == 0)
            return;


        int randomIndex = UnityEngine.Random.Range(0, availableItemData.Count);
        ObjetSO randomData = availableItemData[randomIndex];
        SpawnItem(randomData);
    }

    public void SpawnItem(ObjetSO objetSO)
    {
        if (availableItemData.Count == 0 || !InventoryManager.Instance.HasEmptySlot())
            return;

        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

        ItemBrain newItemBrain = Instantiate(itemPrefab, position, Quaternion.identity, itemParents).GetComponent<ItemBrain>();

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
}