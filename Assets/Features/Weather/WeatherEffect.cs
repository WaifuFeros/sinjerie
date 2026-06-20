using System;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public static WeatherEffect Instance { get; private set; }

    [SerializeField] private Enemy enemy;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void OnFire(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.FireCounter > 0)
            {
               PlayerManager.Instance.TakeDamage(Convert.ToInt32(WeatherManager.Instance.temperature / 7));
               PlayerManager.Instance.FireCounter--;
                if (PlayerManager.Instance.FireCounter == 0)
                    VisualEffectManager.Instance.RemoveEffect(CombatSystem.Instance._playerhead);
            }
        }
        else
        {
            if (enemy.FireCounter > 0)
            {
               enemy.TakeDamage(Convert.ToInt32(WeatherManager.Instance.temperature / 7));
               enemy.FireCounter--;
                if (enemy.FireCounter == 0)
                    VisualEffectManager.Instance.RemoveEffect(enemy.EnemyImage.gameObject);
            }
        }
        
        PlayerManager.Instance.UpdateAfflictionIcons();
        enemy.UpdateAfflictionIcons();
        CombatSystem.Instance.CheckCombatEnd();
    }

    public bool OnFreeze(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.FreezeCounter > 0)
            {
                PlayerManager.Instance.WetCounter = 0;
                if (PlayerManager.Instance.FreezeCounter >= 3)
                {
                    PlayerManager.Instance.FreezeCounter = 0;
                    PlayerManager.Instance.UpdateAfflictionIcons();
                    return true; //player gele
                }
            }
        }
        else
        {
            if (enemy.FreezeCounter > 0)
            {
                enemy.WetCounter = 0;
                if (enemy.FreezeCounter >= 3)
                {
                    enemy.FreezeCounter = 0;
                    enemy.UpdateAfflictionIcons();
                    return true; //ennemi gele
                }
            }
        }

        PlayerManager.Instance.UpdateAfflictionIcons();
        enemy.UpdateAfflictionIcons();
        return false;
    }
    //a tester :)
    public void isSnowing() 
    {
        PlayerManager.Instance.stats.currentStamina = PlayerManager.Instance.stats.maxStamina - 2;
        PlayerManager.Instance.OnStaminaUpdateEvent?.Invoke();
        enemy.currentStaminaMax = enemy.EnemyStats.MaxStamina - 2;
    }

    public void OnWet(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.WetCounter > 0)
            {
                PlayerManager.Instance.FireCounter = 0;
                PlayerManager.Instance.WetCounter -= 1;
                if (PlayerManager.Instance.WetCounter == 0)
                    VisualEffectManager.Instance.RemoveEffect(CombatSystem.Instance._playerhead);
            }
        }
        else
        {
            if (enemy.WetCounter > 0)
            {
                enemy.FireCounter = 0;
                enemy.WetCounter -= 1;
                if (enemy.WetCounter == 0)
                    VisualEffectManager.Instance.RemoveEffect(enemy.EnemyImage.gameObject);
            }
        }

        PlayerManager.Instance.UpdateAfflictionIcons();
        enemy.UpdateAfflictionIcons();
    }


    public bool OnParalyze(bool isPlayer)
    {
        int paralyze = UnityEngine.Random.Range(0, 100);
        if (isPlayer)
        {
            if (PlayerManager.Instance.ParalyzeCounter > 0)
            {
                PlayerManager.Instance.ParalyzeCounter--;
                PlayerManager.Instance.UpdateAfflictionIcons();
                if (paralyze <= 25)
                {
                    return true; //player paralyse, il saute son tour
                }
            }
        }
        else
        {
            if (enemy.ParalyzeCounter > 0)
            {
                enemy.ParalyzeCounter--;
                enemy.UpdateAfflictionIcons();
                if (paralyze <= 25)
                {
                    return true; //ennemi paralyse, il saute son tour
                }
            }
        }
        
        return false;
    }

    public void Thunder(bool isPlayer,bool isThunder, int damageThunder)
    {
        if (!isThunder)
            return;

        if (isPlayer)
            PlayerManager.Instance.TakeDamage(damageThunder);
        else
            enemy.TakeDamage(damageThunder);
    }
}