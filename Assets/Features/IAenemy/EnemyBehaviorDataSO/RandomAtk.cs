using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/RandomAtk")]
public class RandomAtk : EnemyBehaviorSO
{
    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int health, int stamina)
    {
        List<ObjetSO> inventaireAttaque = new List<ObjetSO>();
        int staminaAvailable = stamina;

        foreach (var obj in objs)
        {
            ObjetSO selecObj = objs[Random.Range(0, objs.Length)];
            if (staminaAvailable - selecObj.objetWeight >= 0)
            {
                inventaireAttaque.Add(selecObj);
                staminaAvailable -= selecObj.objetWeight;
            }
        }
        return inventaireAttaque.ToArray();
    }
}