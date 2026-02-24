using UnityEngine;


public abstract class EnemyBehaviorSO : ScriptableObject
{
    public abstract ObjetSO[] ChooseItem(ObjetSO[] objs , int health, int stamina);
}