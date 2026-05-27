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

    public void OnFire(bool isPlayer)
    {
        if (enemy.FireCounter > 0 || PlayerManager.Instance.FireCounter > 0)
        {
            if (!isPlayer)
            {
                enemy.TakeDamage(Convert.ToInt32(weather.temperature / 7));
                enemy.FireCounter--;
            }
            else
            {
                PlayerManager.Instance.TakeDamage(Convert.ToInt32(weather.temperature / 7));
                PlayerManager.Instance.FireCounter--;
            }
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

    //todo ameliorer la logique demain
    public void OnWet(bool isPlayer)
    {
        if (enemy.WetCounter > 0 || PlayerManager.Instance.WetCounter > 0)
        {
            if (!PlayerManager.Instance && weather.effetMeteorologique == "Rain" )
            {
                enemy.FireCounter = 0;
            }
            else if (weather.effetMeteorologique == "Rain")
            {
                PlayerManager.Instance.FireCounter = 0;
            }
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