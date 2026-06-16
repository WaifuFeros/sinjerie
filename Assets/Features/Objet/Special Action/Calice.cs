using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Calice")]
public class Calice : SpecialActionSO
{
    public override void Execute()
    {
        PlayerManager.Instance.stats.maxHealth += 5;
        PlayerManager.Instance.UpdateHealthBar();
    }
}