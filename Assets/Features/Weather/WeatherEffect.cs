using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public static WeatherEffect Instance { get; private set; }

    [SerializeField] private Enemy enemy;
    [SerializeField, HideInInspector] private WeatherManager weather;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void OnFire(bool isPlayer)
    {  
       if (!isPlayer && enemy.FireCounter > 0)
       {
           enemy.TakeDamage(Convert.ToInt32(weather.temperature / 7));
           enemy.FireCounter--;
       }
       else if (PlayerManager.Instance.FireCounter > 0)
       {
           PlayerManager.Instance.TakeDamage(Convert.ToInt32(weather.temperature / 7));
           PlayerManager.Instance.FireCounter--;
       }
       CombatSystem.Instance.CheckCombatEnd();
       
    }

    public bool OnFreeze(bool isPlayer, int isFreeze)
    {
        if (!isPlayer && enemy.FreezeCounter > 0)
        {
            enemy.FreezeCounter--;
            enemy.WetCounter = 0;
            if (isFreeze >= 3)
            {
                isFreeze = 0;
                return true; //ennemi gele
            }
        }
        else if (PlayerManager.Instance.FreezeCounter > 0)
        {
            PlayerManager.Instance.FreezeCounter--;
            PlayerManager.Instance.WetCounter = 0;
            if (isFreeze >= 3)
            {
                isFreeze = 0;
                return true; //player gele
            }
        }
        return false;
    }

    public void OnWet(bool isPlayer)
    {   
        if (!isPlayer && enemy.WetCounter > 0)
        {
            enemy.FireCounter = 0;
        }
        else if (PlayerManager.Instance.WetCounter > 0)
        {
            PlayerManager.Instance.FireCounter = 0;
        }
    }


    public bool OnParalyze(bool isPlayer)
    {
        int paralyze = UnityEngine.Random.Range(0, 100);
        if (!isPlayer && enemy.ParalyzeCounter > 0 )
        {
            if (paralyze <= 25)
            {
                return true; //ennemi paralysé, il saute son tour
            }
            enemy.ParalyzeCounter--;
        }
        else if (PlayerManager.Instance.ParalyzeCounter > 0)
        {
            if (paralyze <= 25)
            {
                return true; //player paralysé, il saute son tour
            }
            PlayerManager.Instance.ParalyzeCounter--;
        }
        return false;
    }

    public void Thunder(bool isPlayer,bool isThunder, int damageThunder)
    {
        if (isThunder && !isPlayer)
        {
            enemy.TakeDamage(damageThunder);
        }
        else if (isThunder && isPlayer)
        {
            PlayerManager.Instance.TakeDamage(damageThunder);
        }
    }
}