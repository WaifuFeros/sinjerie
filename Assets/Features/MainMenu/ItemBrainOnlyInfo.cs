using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBrainOnlyInfo : MonoBehaviour, IItemObject
{
    public ObjetSO ItemData => itemData;

    [Header("Data")]
    [SerializeField] public ObjetSO itemData;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private Image effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    void Start()
    {
        name = $"Item: {itemData.objetName}";
        ApplyVisuals();
    }

    public void ApplyVisuals()
    {
        if (itemData == null) return;

        itemIcon.sprite = itemData.objetSprite;
        itemBackground.sprite = ItemManager.Instance.GetRaritySprite(itemData.Rarity);

        effectImage.gameObject.SetActive(ItemManager.Instance.GetObjetTypeSprite(itemData.objectType, out Sprite result));
        effectImage.sprite = result;

        effectText.text = itemData.objectEffect.ToString();
        weightText.text = itemData.objetWeight.ToString();
    }

}