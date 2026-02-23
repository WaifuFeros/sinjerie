using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/ConvertirHealEnAttack")]
public class HealToAttack : SpecialActionSO
{
    public override void Execute()
    {
        var atkItems = ItemManager.Instance.GetItemsOfType(ObjetEffectType.Attack);
        var healItems = ItemManager.Instance.GetItemsOfType(ObjetEffectType.Heal);
        ItemManager.Instance.ChangeItemType(atkItems, ObjetEffectType.Heal);
        ItemManager.Instance.ChangeItemType(healItems, ObjetEffectType.Attack);
        Debug.Log("HealToAttack");
    }
}