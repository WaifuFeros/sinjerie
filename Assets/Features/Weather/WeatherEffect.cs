using System;
using System.Collections;
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

    private void Start()
    {
        enemy.OnAfflictionUpdateEvent += RefreshItemReactions;
    }

    private void RefreshItemReactions()
    {
        if (enemy == null)
            return;

        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
    }

    public IEnumerator OnFire(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.FireCounter > 0)
            {
                PlayerManager.Instance.TakeDamage(FireDamage());
                PlayerManager.Instance.FireCounter--;
                VisualEffectManager.Instance.TriggerBurst(PlayerManager.Instance.playerHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            if (enemy.FireCounter > 0)
            {
                enemy.TakeDamage(FireDamage());
                enemy.FireCounter--;
                VisualEffectManager.Instance.TriggerBurst(enemy.enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

                yield return new WaitForSeconds(1);
            }
        }

        CombatSystem.Instance.CheckCombatEnd();

        //RefreshItemReactions(); 
    }

    public bool OnFreeze(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.FreezeCounter > 0)
            {
                if (PlayerManager.Instance.FreezeCounter >= CombatSystem.Instance.freezeProcThreshold)
                    return true;
            }
        }
        else
        {
            if (enemy.FreezeCounter > 0)
            {
                if (enemy.FreezeCounter >= CombatSystem.Instance.freezeProcThreshold)
                    return true; 
            }
        }

        //RefreshItemReactions();

        return false;
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
            }
        }
        else
        {
            if (enemy.WetCounter > 0)
            {
                enemy.FireCounter = 0;
                enemy.WetCounter--;
            }
        }
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

                if (paralyze <= 25)
                    result = true; 
            }
        }
        else
        {
            if (enemy.ParalyzeCounter > 0)
            {
                enemy.ParalyzeCounter--;

                if (paralyze <= 25)
                    result = true; 
            }
        }

        //RefreshItemReactions();

        return result;
    }

    public IEnumerator PlayParalyzeAnimation()
    {
        VisualEffectManager.Instance.TriggerBurst(enemy.enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Paralyze);

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator PlayFrozenAnimation()
    {
        VisualEffectManager.Instance.TriggerBurst(enemy.enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);

        yield return new WaitForSeconds(1f);

        VisualEffectManager.Instance.RemoveEffect(enemy.enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);
    }

    public void Thunder(bool isPlayer,bool isThunder, int damageThunder)
    {
        if (!isThunder)
            return;

        if (isPlayer)
            PlayerManager.Instance.TakeDamage(damageThunder);
        else
            enemy.TakeDamage(damageThunder);

        //RefreshItemReactions();
    }

    public int FireDamage()
    {
        return Convert.ToInt32(WeatherManager.Instance.temperature / 7);
    }
}
