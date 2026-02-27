using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAssignation : MonoBehaviour
{
    public ObjetSO _Item { get; private set; }

    public RewardSystem RewardSystem;

    public Toggle Toggle;

    public void Initialized(ObjetSO itemsInToggle, RewardSystem rewardSystem)
    {
        _Item = itemsInToggle;
        RewardSystem = rewardSystem;
    }

    public void DesibleToggle() 
    { 
        int count = 0;
        foreach (var toggleAssignation in RewardSystem.ToggleAssignations)
        {
            if (toggleAssignation.Toggle.isOn)
                count++;
        }

        bool hasEnoughSelected = count >= RewardSystem.NumberOfRewardToChoose;
        for (int i = 0; i < RewardSystem.ToggleAssignations.Count; i++)
        {
            if (hasEnoughSelected == true && RewardSystem.ToggleAssignations[i].Toggle.isOn == false)
            {
                //print(count);
                RewardSystem.ToggleAssignations[i].Toggle.interactable = false;
            }
            else
            {
                RewardSystem.ToggleAssignations[i].Toggle.interactable = true;
            }
        }
        //if (count >= RewardSystem.NumberOfRewardToChoose)
        //{
        //}
        //count++;
    }
}
