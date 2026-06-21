using System;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public static WeatherEffect Instance { get; private set; }

    [SerializeField] public Enemy enemy; 

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void RefreshItemReactions()
    {
        if (enemy == null)
            return;

        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
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

        //RefreshItemReactions(); 
    }

    public bool OnFreeze(bool isPlayer)
    {
        bool result = false;

        if (isPlayer)
        {
            if (PlayerManager.Instance.FreezeCounter > 0)
            {
                PlayerManager.Instance.WetCounter = 0;

                if (PlayerManager.Instance.FreezeCounter >= 3)
                {
                    PlayerManager.Instance.FreezeCounter = 0;
                    PlayerManager.Instance.UpdateAfflictionIcons();
                    result = true; 
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
                    result = true; 
                }
            }
        }

        PlayerManager.Instance.UpdateAfflictionIcons();
        enemy.UpdateAfflictionIcons();

        //RefreshItemReactions();

        return result;
    }

    public void isSnowing()
    {
        PlayerManager.Instance.stats.currentStamina = PlayerManager.Instance.stats.maxStamina - 2;
        PlayerManager.Instance.OnStaminaUpdateEvent?.Invoke();

        enemy.currentStaminaMax = enemy.EnemyStats.MaxStamina - 2;

        //RefreshItemReactions();
    }

    public void OnWet(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.WetCounter > 0)
            {
                PlayerManager.Instance.FireCounter = 0;
                PlayerManager.Instance.WetCounter--;

                if (PlayerManager.Instance.WetCounter == 0)
                    VisualEffectManager.Instance.RemoveEffect(CombatSystem.Instance._playerhead);
            }
        }
        else
        {
            if (enemy.WetCounter > 0)
            {
                enemy.FireCounter = 0;
                enemy.WetCounter--;

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
        bool result = false;

        if (isPlayer)
        {
            if (PlayerManager.Instance.ParalyzeCounter > 0)
            {
                PlayerManager.Instance.ParalyzeCounter--;
                PlayerManager.Instance.UpdateAfflictionIcons();

                if (paralyze <= 25)
                    result = true; 
            }
        }
        else
        {
            if (enemy.ParalyzeCounter > 0)
            {
                enemy.ParalyzeCounter--;
                enemy.UpdateAfflictionIcons();

                if (paralyze <= 25)
                    result = true; 
            }
        }

        //RefreshItemReactions();

        return result;
    }

    public void Thunder(bool isPlayer, bool isThunder, int damageThunder)
    {
        if (!isThunder)
            return;

        if (isPlayer)
            PlayerManager.Instance.TakeDamage(damageThunder);
        else
            enemy.TakeDamage(damageThunder);

        //RefreshItemReactions();
    }
}
