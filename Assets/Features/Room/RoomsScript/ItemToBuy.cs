using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    private int price;
    private ObjetSO itemToBuySO;

    [Header("UI")]
    [SerializeField] private GameObject item;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private Image effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Color _notEnoughMoneyTextColor = Color.red;
    [SerializeField] private Color _notEnoughMoneyImageColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private Color _priceTextBaseColor;

    void Start()
    {
        _priceTextBaseColor = priceText.color;

        // Choisi la rareté et le prix et applique l'affichage de la rareté
        int itemChoiceNb = Random.Range(0, 100);
        ObjetRarity itemRarity;
        if (itemChoiceNb < 5) {                  // 5% 
            itemRarity = ObjetRarity.Legendary;
            price = 150;
        } else if (itemChoiceNb < 15){           // 10%
            itemRarity = ObjetRarity.Epic;
            price = 100;
        } else if (itemChoiceNb < 35) {          // 20%
            itemRarity = ObjetRarity.Rare;
            price = 60;
        } else if (itemChoiceNb < 60) {          // 25%
            itemRarity = ObjetRarity.Uncommon;
            price = 30;
        } else {                                 // 40%
            itemRarity = ObjetRarity.Common;
            price = 20;
        }

        itemBackground.sprite = ItemManager.Instance.GetRaritySprite(itemRarity);


        // Récup tout les items de la rareté choisie
        List<ObjetSO> objetSelection = new List<ObjetSO>(); ;
        foreach (var item in ItemManager.Instance.ItemsDatabase)
        {
            if (item.Rarity == itemRarity)
            {
                objetSelection.Add(item);
            }
        }
        itemToBuySO = objetSelection[Random.Range(0, objetSelection.Count)];


        // Applique l'affichage
        itemIcon.sprite = itemToBuySO.objetSprite;
        effectText.text = itemToBuySO.objectEffect.ToString();
        weightText.text = itemToBuySO.objetWeight.ToString();
        priceText.text = price.ToString();
        effectImage.gameObject.SetActive(ItemManager.Instance.GetObjetTypeSprite(itemToBuySO.objectType, out Sprite result));
        effectImage.sprite = result;
        UpdatePriceColor();

        PlayerManager.Instance.OnGoldUpdateEvent += UpdatePriceColor;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnGoldUpdateEvent -= UpdatePriceColor;
    }

    public void BuyItem()
    {
        if (PlayerManager.Instance.stats.gold >= price)
        {
            // Ajoute l'item au deck du joueur et retire l'or
            PlayerManager.Instance.removeGold(price);
            var deckList = new List<ObjetSO>(PlayerManager.Instance.stats.Deck);
            deckList.Add(itemToBuySO);
            PlayerManager.Instance.stats.Deck = deckList.ToArray();

            // Affichage
            GetComponent<Button>().interactable = false;
            item.SetActive(false);
        }
    }

    private void UpdatePriceColor()
    {
        bool hasEnoughMoney = PlayerManager.Instance.stats.gold >= price;
        priceText.color = hasEnoughMoney ? _priceTextBaseColor : _notEnoughMoneyTextColor;
        itemIcon.color = hasEnoughMoney ? Color.white : _notEnoughMoneyImageColor;
        itemBackground.color = hasEnoughMoney ? Color.white : _notEnoughMoneyImageColor;
    }
}
