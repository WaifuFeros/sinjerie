using System;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public static WeatherEffect Instance { get; private set; }

    [SerializeField] private Enemy enemy;
    [SerializeField] private WeatherManager weather;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void OnFire()
    {
        if (enemy.FireCounter > 0)
        {
            enemy.TakeDamage(Convert.ToInt32(weather.temperature / 7));
            enemy.FireCounter--;
            CombatSystem.Instance.CheckCombatEnd();
        }
    }

    public void OnFreeze()
    {
        if (enemy.FreezeCounter > 0)
        {
            //TODO: Geler l'ennemi ou lui faire sauter un tour)
        }
    }

    public void OnWet()
    {
        if (enemy.WetCounter > 0)
        {
            //todo: Rendre l'ennemi moins vulnérable au feu
        }
    }


    public void OnParalyze()
    {
        if (enemy.ParalyzeCounter > 0)
        {
            //todo: possibilité de faire sauter un tour à l'ennemi
        }
    }
}