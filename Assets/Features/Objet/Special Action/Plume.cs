using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Plume")]
public class Plume : SpecialActionSO
{
    public override void Execute()
    {
        var items = ItemManager.Instance.GetRandomItems(3);
        ItemManager.Instance.ChangeItemWeight(items, 0);

    }
}