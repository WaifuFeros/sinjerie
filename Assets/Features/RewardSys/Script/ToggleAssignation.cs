using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class ToggleAssignation : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public ObjetSO Item;
    public bool IsSelected;

    [Header("Asset")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private Sprite _atkSprite;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    private RewardSystem RewardSystem;
    public void Initialized(ObjetSO itemsInToggle, RewardSystem rewardSystem)
    {
        Item = itemsInToggle;
        RewardSystem = rewardSystem;

        // Ajoute les visuels
        itemIcon.sprite = Item.objetSprite;
        switch (Item.Rarity)
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
        if (Item.objectType == ObjetEffectType.Heal)
            effectImage.GetComponent<Image>().sprite = _healSprite;
        else if (Item.objectType == ObjetEffectType.Attack)
            effectImage.GetComponent<Image>().sprite = _atkSprite;
        else if (Item.objectType == ObjetEffectType.Special)
            effectImage.SetActive(false);

        effectText.text = Item.objectEffect.ToString();
        weightText.text = Item.objetWeight.ToString();
    }

    public void PressToggle()
    {
        int count = 0;
        foreach (var toggleAssignation in RewardSystem.ToggleAssignations)
        {
            if (toggleAssignation.IsSelected)
                count++;
        }

        bool hasEnoughSelected = count >= RewardSystem.NumberOfRewardToChoose;

        // Annule les anim en cours
        transform.DOKill();
        transform.localRotation = Quaternion.identity;


        if (!IsSelected && !hasEnoughSelected)
        {
            transform.DOScale(Vector3.one * 1.4f, 0.2f).SetEase(Ease.OutBack);
            RewardSystem.ItemRewards.Add(Item);
            IsSelected = true;
        }
        else if (IsSelected)
        {
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            RewardSystem.ItemRewards.Remove(Item);
            IsSelected = false;
        }
        else
        {
            transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.3f, 13, 1);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSelected) return;
        transform.DOScale(Vector3.one * 1.1f, 0.15f).SetEase(Ease.OutQuad);
    }

    // 3. Détecte quand la souris SORT du bouton
    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsSelected) return;
        transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
    }
}