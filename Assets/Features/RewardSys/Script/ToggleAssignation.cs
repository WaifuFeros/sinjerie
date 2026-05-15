using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

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

        if (!hasEnoughSelected && !IsSelected)
        {
            print(_Item.objetName + " select");
            transform.localScale = Vector3.one * 1.2f;
            RewardSystem.ItemRewards.Add(_Item);
            IsSelected = true;
        }
        else
        {
            print(_Item.objetName + " unselect");
            transform.localScale = Vector3.one;
            RewardSystem.ItemRewards.Remove(_Item);
            IsSelected = false;
        }
    }
}