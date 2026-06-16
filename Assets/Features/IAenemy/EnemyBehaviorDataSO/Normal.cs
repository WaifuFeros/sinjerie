using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/Normal")]
public class Normal : EnemyBehaviorSO
{
    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int health, int stamina)
    {
        List<ObjetSO> chosenItems = new List<ObjetSO>();
        int staminaAvailable = stamina;

        // tri item de heal et atk
        List<ObjetSO> healItems = new List<ObjetSO>();
        List<ObjetSO> atkItems = new List<ObjetSO>();
        foreach (var obj in objs)
        {
            switch (obj.objectType)
            {
                case ObjetEffectType.Heal:
                    healItems.Add(obj);
                    break;
                case ObjetEffectType.Attack:
                    atkItems.Add(obj);
                    break;
            }
        }


        foreach (var obj in objs)
        {
            ObjetSO selecObj;
            if (Random.Range(0, 6)<= 2)
                selecObj = atkItems[Random.Range(0, atkItems.Count)];
            else
                selecObj = healItems[Random.Range(0, healItems.Count)];

            if (staminaAvailable - selecObj.objetWeight >= 0)
            {
                chosenItems.Add(selecObj);
                staminaAvailable -= selecObj.objetWeight;
            }
        }
        return chosenItems.ToArray();
    }
}