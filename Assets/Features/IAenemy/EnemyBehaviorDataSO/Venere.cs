using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/Vénère")]
public class Venere : EnemyBehaviorSO
{
    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int MaxHealth, int health, int stamina)
    {
        List<ObjetSO> chosenItems = new List<ObjetSO>();
        int staminaAvailable = stamina;

        foreach (var obj in objs)
        {
            ObjetSO selecObj = objs[Random.Range(0, objs.Length)];
            if (staminaAvailable - selecObj.objetWeight >= 0 && selecObj.objectType == ObjetEffectType.Attack)
            {
                chosenItems.Add(selecObj);
                staminaAvailable -= selecObj.objetWeight;
            }
        }
        return chosenItems.ToArray();
    }
}