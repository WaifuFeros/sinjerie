using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class ToggleAssignation : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public ObjetSO Item;
    public bool IsSelected;

    [Header("Asset")]
    [SerializeField] private Sprite _itemHide;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private Image effectImage;
    [SerializeField] private Image weightImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField, Min(0)] private float descriptionPressTime;

    private Coroutine longPressCoroutine;
    public void Initialized(ObjetSO itemsInToggle, RewardSystem rewardSystem)
    {
        Item = itemsInToggle;

        // Ajoute les visuels
        if (WeatherManager.Instance.effetMeteorologique != GameWeatherType.Mist) {
            itemIcon.sprite = Item.objetSprite;
            itemBackground.sprite = ItemManager.Instance.GetRaritySprite(Item.Rarity);
            effectImage.gameObject.SetActive(ItemManager.Instance.GetObjetTypeSprite(Item.objectType, out Sprite result));
            effectImage.sprite = result;

            effectText.text = Item.objectEffect.ToString();
            weightText.text = Item.objetWeight.ToString();
        }
        else
        {
            // Affiche une icone de brouillard
            itemIcon.gameObject.SetActive(false);
            effectImage.gameObject.SetActive(false);
            weightImage.gameObject.SetActive(false);

            itemBackground.sprite = _itemHide;
        }
    }

    public void PressToggle()
    {
        int count = 0;
        foreach (var toggleAssignation in RewardSystem.Instance.ToggleAssignations)
        {
            if (toggleAssignation.IsSelected)
                count++;
        }

        bool hasEnoughSelected = count >= RewardSystem.Instance.NumberOfRewardToChoose;

        // Annule les anim en cours
        transform.DOKill();
        transform.localRotation = Quaternion.identity;


        if (!IsSelected && !hasEnoughSelected)
        {
            transform.DOScale(Vector3.one * 1.4f, 0.2f).SetEase(Ease.OutBack);
            RewardSystem.Instance.ItemRewards.Add(Item);
            IsSelected = true;
        }
        else if (IsSelected)
        {
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            RewardSystem.Instance.ItemRewards.Remove(Item);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        longPressCoroutine = StartCoroutine(WaitAndShowDescription());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopLongPress();
    }

    private IEnumerator WaitAndShowDescription()
    {
        yield return new WaitForSeconds(descriptionPressTime);
        DescriptionManager.Instance.DisplayDescription(Item);
    }
    private void StopLongPress()
    {
        if (longPressCoroutine != null) StopCoroutine(longPressCoroutine);
    }
}