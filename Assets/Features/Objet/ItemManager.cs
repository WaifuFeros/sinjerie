using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject itemPrefab;

    // Liste de tous les items
    public List<GameObject> activeItems = new List<GameObject>();
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

        GameObject newItemGo = Instantiate(itemPrefab, position, Quaternion.identity, transform);


        newItemGo.GetComponent<ItemBrain>().InitItem(randomData);
        activeItems.Add(newItemGo);

    }



    // Recup des items de l'inventaire
    public List<GameObject> GetAllItem()
    {
        return activeItems;
    }

    public List<GameObject> GetHealItem()
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject item in activeItems)
        {
            ItemBrain itemBrain = item.GetComponent<ItemBrain>();
            if (itemBrain.itemData.objectType == ObjetEffectType.Heal)
            {
                result.Add(item);
            }
        }
        return result;
    }

    public List<GameObject> GetAttackItem()
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject item in activeItems)
        {
            ItemBrain itemBrain = item.GetComponent<ItemBrain>();
            if (itemBrain.itemData.objectType == ObjetEffectType.Attack)
            {
                result.Add(item);
            }
        }
        return result;
    }
}