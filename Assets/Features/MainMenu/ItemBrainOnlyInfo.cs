using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class ItemBrainOnlyInfo : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] public ObjetSO itemData;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;


    [Header("Asset")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private Sprite _atkSprite;


    void Start()
    {
        name = $"Item: {itemData.objetName}";
        ApplyVisuals();
    }

    public void ApplyVisuals()
    {
        if (itemData == null) return;

        itemIcon.sprite = itemData.objetSprite;
        switch (itemData.Rarity)
        {
            case ObjetRarity.Common:
                itemBackground.sprite = _communSprite;
                break;
            case ObjetRarity.Uncommon:
                itemBackground.sprite = _uncommonSprite;
                break;
            case ObjetRarity.Rare:
                itemBackground.sprite = _rareSprite;
                break;
            case ObjetRarity.Epic:
                itemBackground.sprite = _epicSprite;
                break;
            case ObjetRarity.Legendary:
                itemBackground.sprite = _lengendarySprite;
                break;
        }

        effectImage.SetActive(true);

        if (itemData.objectType == ObjetEffectType.Heal)
            effectImage.GetComponent<Image>().sprite = _healSprite;
        else if (itemData.objectType == ObjetEffectType.Attack)
            effectImage.GetComponent<Image>().sprite = _atkSprite;
        else if (itemData.objectType == ObjetEffectType.Special)
            effectImage.SetActive(false);

        effectText.text = itemData.objectEffect.ToString();
        weightText.text = itemData.objetWeight.ToString();
    }

}