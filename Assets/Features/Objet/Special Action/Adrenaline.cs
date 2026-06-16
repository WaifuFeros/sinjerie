using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Adrenaline")]
public class Adrenaline : SpecialActionSO
{
    public override void Execute()
    {
        PlayerManager.Instance.stats.currentStamina = PlayerManager.Instance.stats.maxStamina;
        StaminaUIManager.Instance.UpdateDisplay(PlayerManager.Instance.stats.currentStamina);
    }
}