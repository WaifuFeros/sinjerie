using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAssignation : MonoBehaviour
{
    public ObjetSO _Item;

    private RewardSystem RewardSystem;

    public Toggle Toggle;

    public Image ItemImage;

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
            if (toggleAssignation.Toggle.isOn)
                count++;
        }

        bool hasEnoughSelected = count > RewardSystem.NumberOfRewardToChoose;
        print("has enough selected : " + hasEnoughSelected);
        print(Toggle.isOn);
        if (!hasEnoughSelected && Toggle.isOn)
        {
            print(_Item.objetName + " select");
            transform.localScale = Vector3.one * 1.2f;
            RewardSystem.ItemRewards.Add(_Item);
        }
        else
        {
            print(_Item.objetName + " unselect");
            transform.localScale = Vector3.one;
            RewardSystem.ItemRewards.Remove(_Item);
        }
        
        //if (count >= RewardSystem.NumberOfRewardToChoose)
        //{
        //}
        //count++;
    }
}
