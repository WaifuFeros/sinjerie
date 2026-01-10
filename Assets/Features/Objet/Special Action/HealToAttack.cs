using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/ConvertirHealEnAttack")]
public class HealToAttack : SpecialActionSO
{
    public override void Execute()
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAH ça marche hein");
    }
}