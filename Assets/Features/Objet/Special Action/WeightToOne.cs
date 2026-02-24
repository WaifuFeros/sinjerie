using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/MettreLePoidA1")]
public class WeightToOne : SpecialActionSO
{
    public override void Execute()
    {
        var items = ItemManager.Instance.GetAllItems();
        ItemManager.Instance.ChangeItemWeight(items, 1);
        Debug.Log("WeightToOne");
    }
}