using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Behavior/Tuto Behaviour")]
public class TutoBehaviour : EnemyBehaviorSO
{
    public override ObjetSO[] ChooseItem(ObjetSO[] objs, int MaxHealth, int health, int stamina)
    {
        List<ObjetSO> chosenItems = new List<ObjetSO>();

        chosenItems.Add(objs[0]);

        return chosenItems.ToArray();
    }
}