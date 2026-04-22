using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemToBuy : MonoBehaviour
{

    private PlayerManager player = PlayerManager.Instance;
    private int price;
    private ObjetSO itemToBuySO;

    [Header("UI")]
    [SerializeField] private GameObject item;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Asset Rarity")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;

    void Awake()
    {
        // Choisi la rareté et le prix et applique l'affichage de la rareté
        int itemChoiceNb = Random.Range(0, 100);
        ObjetRarity itemRarity;
        if (itemChoiceNb < 5) {                  // 5% 
            itemRarity = ObjetRarity.Legendary;
            price = 150;
            itemBackground.sprite = _lengendarySprite;
        } else if (itemChoiceNb < 15){           // 10%
            itemRarity = ObjetRarity.Epic;
            price = 100;
            itemBackground.sprite = _epicSprite;
        } else if (itemChoiceNb < 35) {          // 20%
            itemRarity = ObjetRarity.Rare;
            price = 60;
            itemBackground.sprite = _rareSprite;
        } else if (itemChoiceNb < 60) {          // 25%
            itemRarity = ObjetRarity.Uncommon;
            price = 30;
            itemBackground.sprite = _uncommonSprite;
        } else {                                 // 40%
            itemRarity = ObjetRarity.Common;
            price = 20;
            itemBackground.sprite = _communSprite;
        }
        // Récup tout les items de la rareté choisie
        List<ObjetSO> objetSelection = new List<ObjetSO>(); ;
        foreach (var item in ItemManager.Instance.ItemsData)
        {
            if (item.Rarity == itemRarity)
            {
                objetSelection.Add(item);
            }
        }
        itemToBuySO = objetSelection[Random.Range(0, objetSelection.Count)];


        // Applique l'affichage
        itemIcon.sprite = itemToBuySO.objetSprite;

        effectImage.SetActive(true);
        if (itemToBuySO.objectType == ObjetEffectType.Heal)
            effectImage.GetComponent<Image>().color = Color.red;
        else if (itemToBuySO.objectType == ObjetEffectType.Attack)
            effectImage.GetComponent<Image>().color = Color.yellow;
        else if (itemToBuySO.objectType == ObjetEffectType.Special)
            effectImage.SetActive(false);
        effectText.text = itemToBuySO.objectEffect.ToString();
        weightText.text = itemToBuySO.objetWeight.ToString();
        priceText.text = price.ToString();
    }


    public void BuyItem()
    {
        if (player.stats.gold >= price)
        {
            // Ajoute l'item au deck du joueur et retire l'or
            player.removeGold(price);
            var deckList = new List<ObjetSO>(PlayerManager.Instance.stats.Deck);
            deckList.Add(itemToBuySO);
            player.stats.Deck = deckList.ToArray();

            // Affichage
            GetComponent<Button>().interactable = false;
            item.SetActive(false);
        }
    }
}
