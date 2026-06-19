using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaPanel : MonoBehaviour
{
    [Header("Database")]
    public ObjetDatabase database;

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
        // Tri par rareté (Common → Legendary)
        var sortedList = database.allObjects
            .OrderBy(o => o.Rarity)
            .ToList();

        foreach (var obj in sortedList)
        {
            var entryObj = Instantiate(entryPrefab, contentParent);
            var entry = entryObj.GetComponent<ObjetEntry>();
            entry.Setup(obj, this);
        }
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

