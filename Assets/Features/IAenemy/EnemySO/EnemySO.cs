using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NouvelEnemy", menuName = "Enemy/Nouvel enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public Sprite Sprite;
    public string Name;
    public string Description;
    public int MaxHealth;
    public int MaxStamina;
    public Object[] Items;
}
