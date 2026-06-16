using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objets/Actions/Piece")]
public class Piece : SpecialActionSO
{
    public override void Execute()
    {
        if (Random.Range(0, 2) == 0)
            PlayerManager.Instance.TakeDamage(10);
        else
            CombatSystem.Instance.currentEnemy.TakeDamage(15);
        CombatSystem.Instance.CheckCombatEnd();
    }
}