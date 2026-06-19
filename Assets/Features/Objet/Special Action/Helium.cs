using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Helium")]
public class Helium : SpecialActionSO
{
    public override void Execute()
    {
        var items = ItemManager.Instance.GetAllItems();
        foreach (var item in items)
        {
            if (item.itemData.objetWeight == 1)
            {
                List<ItemBrain> oneItemList = new List<ItemBrain>();
                oneItemList.Add(item);
                ItemManager.Instance.ChangeItemWeight(oneItemList, 0);
            }
        }
        

    }
}