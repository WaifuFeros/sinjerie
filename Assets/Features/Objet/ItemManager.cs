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

    [SerializeField]
    private ObjetSO[] _startingItems;

    // Liste interne des SO chargés avec adressables
    private List<ObjetSO> availableItemData = new List<ObjetSO>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        LoadItemData();
    }

    private void LoadItemData()
    {
        Addressables.LoadAssetsAsync<ObjetSO>("Items", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                availableItemData.AddRange(handle.Result);
                Debug.Log($"{availableItemData.Count} items load");
            }

            foreach (ObjetSO item in _startingItems)
            {
                var itemBrain = SpawnItem(item);
                if (InventoryManager.Instance.HasEmptySlot())
                {
                    var slot = InventoryManager.Instance.GetEmptySlot();
                    slot.SetItem(itemBrain);
                }
            }
        };
    }

    public void SpawnRandomItem()
    {
        if (availableItemData.Count == 0)
        {
            return;
        }

        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
        int randomIndex = Random.Range(0, availableItemData.Count);
        ObjetSO randomData = availableItemData[randomIndex];

        ItemBrain newItemGo = Instantiate(itemPrefab, position, Quaternion.identity, itemParents).GetComponent<ItemBrain>();


        newItemGo.InitItem(randomData);
        activeItems.Add(newItemGo);
    }

    public ItemBrain SpawnItem(ObjetSO objetSO)
    {
        if (availableItemData.Count == 0)
        {
            return null;
        }

        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

        ItemBrain newItemBrain = Instantiate(itemPrefab, position, Quaternion.identity, itemParents).GetComponent<ItemBrain>();


        newItemBrain.InitItem(objetSO);
        activeItems.Add(newItemBrain);

        return newItemBrain;
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
            int randomIndex = Random.Range(0, tempPool.Count);
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
    public void ChangeItemEffect(List<ItemBrain> items, float Effect)
    {
        foreach (ItemBrain item in items)
        {
            item.itemData.objectEffect = Effect;
            item.TriggerVisualUpdate();
        }
    }
}