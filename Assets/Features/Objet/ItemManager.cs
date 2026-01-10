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
    public List<ItemBrain> activeItems = new List<ItemBrain>();
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
                Debug.Log($"{availableItemData.Count} items chargés !");
            }
        };
    }

    public void SpawnRandomItem(Vector3 position)
    {
        if (availableItemData.Count == 0) return;

        int randomIndex = Random.Range(0, availableItemData.Count);
        ObjetSO randomData = availableItemData[randomIndex];

        GameObject newItemGo = Instantiate(itemPrefab, position, Quaternion.identity, transform);

        ItemBrain brain = newItemGo.GetComponent<ItemBrain>();
        if (brain != null)
        {
            brain.InitItem(randomData);

            activeItems.Add(brain);
        }
    }


}