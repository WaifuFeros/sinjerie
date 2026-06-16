using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Rikiki")]
public class Rikiki : SpecialActionSO
{
    public override void Execute()
    {
        var items = ItemManager.Instance.GetAllItems();
        ItemManager.Instance.ChangeItemWeight(items, 1);
        foreach (var item in items)
        {
            List<ItemBrain> oneItemList = new List<ItemBrain>();
            oneItemList.Add(item);
            ItemManager.Instance.ChangeItemEffect(oneItemList, Mathf.CeilToInt((float)item.itemData.objectEffect / 2f));
        }

    }
}