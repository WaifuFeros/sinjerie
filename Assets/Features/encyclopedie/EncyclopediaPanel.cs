using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaPanel : MonoBehaviour
{
    [Header("UI List")]
    public Transform contentParent;
    public GameObject entryPrefab;

    [Header("UI Details")]
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDescription;
    public TextMeshProUGUI detailStats;

    void Start()
    {
        Populate();
    }

    void Populate()
    {
        Debug.Log("Encyclopedia populate");
        Debug.Log(ItemManager.Instance);
        ItemManager.Instance.WaitForItemDatabaseInitialization(() =>
        {
            Debug.Log("Encyclopedia populate complete");
            // Tri par rareté (Common → Legendary)
            var sortedList = new List<ObjetSO>(ItemManager.Instance.ItemsDatabase)
                .OrderBy(o => o.Rarity)
                .ToList();

            Debug.Log(sortedList.Count);

            foreach (var obj in sortedList)
            {
                Debug.Log("Spawn item Encyclopedia");
                var entryObj = Instantiate(entryPrefab, contentParent);
                var entry = entryObj.GetComponent<ObjetEntry>();
                entry.Setup(obj, this);
            }
        });
    }

    public void ShowDetails(ObjetSO obj)
    {
        detailIcon.sprite = obj.objetSprite;
        detailName.text = obj.objetName;
        detailDescription.text = obj.objetDescription;

        detailStats.text =
            $"Matériau : {obj.objetMaterialType}\n" +
            $"Poids : {obj.objetWeight}\n" +
            $"Type d'effet : {obj.objectType}\n" +
            $"Valeur d'effet : {obj.objectEffect}\n" +
            $"Rareté : {obj.Rarity}";
    }
}

