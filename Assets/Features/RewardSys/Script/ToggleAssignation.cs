using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleAssignation : MonoBehaviour
{
    public ObjetSO _Item;
    public Image ItemImage;
    public bool IsSelected;

    private RewardSystem RewardSystem;
    public void Initialized(ObjetSO itemsInToggle, RewardSystem rewardSystem)
    {
        _Item = itemsInToggle;
        RewardSystem = rewardSystem;
        ItemImage.sprite = _Item.objetSprite;
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
            RewardSystem.ItemRewards.Add(_Item);
            IsSelected = true;
        }
        else if (IsSelected)
        {
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            RewardSystem.ItemRewards.Remove(_Item);
            IsSelected = false;
        }
        else
        {
            transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.3f, 13, 1);
        }
    }
}