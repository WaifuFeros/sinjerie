using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/Malin")]
public class Malin : EnemyBehaviorSO
{
    [Range(0,1)]
    public float healthRatio;

    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int MaxHealth, int health, int stamina)
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

        int futurHealth = health;


        foreach (var obj in objs)
        {
            ObjetSO selecObj;
            if (futurHealth < MaxHealth * healthRatio)
                selecObj = healItems[Random.Range(0, healItems.Count)];
            else
                selecObj = atkItems[Random.Range(0, atkItems.Count)];
            Debug.Log(selecObj);
            if (staminaAvailable - selecObj.objetWeight >= 0)
            {
                if (selecObj.objectType == ObjetEffectType.Heal)
                    futurHealth += selecObj.objectEffect;
                Debug.Log(futurHealth);
                chosenItems.Add(selecObj);
                staminaAvailable -= selecObj.objetWeight;
            }
        }
        return chosenItems.ToArray();
    }
}