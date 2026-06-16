using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/Suicidaire")]
public class Suicidaire : EnemyBehaviorSO
{
    ////////// NE PAS UTILISER NE MARCHE PAS
    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int MaxHealth, int health, int stamina)
    {
        List<ObjetSO> chosenItems = new List<ObjetSO>();
        int staminaAvailable = stamina;

        // tri item de heal
        List<ObjetSO> atkItems = new List<ObjetSO>();
        foreach (var obj in objs)
        {
            switch (obj.objectType)
            {
                case ObjetEffectType.Attack:
                    atkItems.Add(obj);
                    break;
            }
        }

        foreach (var obj in objs)
        {
            ObjetSO selecObj = atkItems[Random.Range(0, atkItems.Count)];
            if (staminaAvailable - selecObj.objetWeight >= 0)
            {
                chosenItems.Add(selecObj);
                staminaAvailable -= selecObj.objetWeight;
            }
        }
        return chosenItems.ToArray();
    }
}