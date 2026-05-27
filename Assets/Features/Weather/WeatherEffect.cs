using System;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    [Header("Settings Effect")]
    [SerializeField] private float _fireResistance;
    [SerializeField] private int _fireDamage;

    private Enemy enemy;
    private Main main; //Class Main de Weather Manager
    public void OnFire()
    {
        if(enemy.FireCounter > 0)
        {
            enemy.TakeDamage(Convert.ToInt32(main.temp/7));
            enemy.FireCounter--;
            CombatSystem.Instance.CheckCombatEnd();
        }
    }
}
