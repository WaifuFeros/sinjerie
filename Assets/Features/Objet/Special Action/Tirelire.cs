using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Tirelire")]
public class Tirelire : SpecialActionSO
{
    public override void Execute()
    {
        PlayerManager.Instance.AddGold(20);
        PlayerManager.Instance.TakeDamage(10);
    }
}