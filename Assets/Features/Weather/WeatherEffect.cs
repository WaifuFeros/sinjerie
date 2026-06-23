using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    [SerializeField] private EventReference fireSound;
    [SerializeField] private EventReference paralyzeSound;

    public static WeatherEffect Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        CombatSystem.Instance.Enemy.OnAfflictionUpdateEvent += RefreshItemReactions;
    }

    private void RefreshItemReactions()
    {
        if (CombatSystem.Instance.Enemy == null)
            return;

        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
    }

    public IEnumerator OnFire(bool isPlayer)
    {
        if (isPlayer)
        {
            if (PlayerManager.Instance.FireCounter > 0)
            {
                PlayerManager.Instance.TakeDamage(ComputeFireDamage());
                PlayerManager.Instance.FireCounter--;
                VisualEffectManager.Instance.TriggerBurst(PlayerManager.Instance.playerHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            if (CombatSystem.Instance.Enemy.FireCounter > 0)
            {
                CombatSystem.Instance.Enemy.TakeDamage(ComputeFireDamage());
                CombatSystem.Instance.Enemy.FireCounter--;
                VisualEffectManager.Instance.TriggerBurst(CombatSystem.Instance.Enemy.enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

                yield return new WaitForSeconds(1);
            }
        }

        CombatSystem.Instance.CheckCombatEnd();

        //RefreshItemReactions(); 
    }

    public bool CheckFreeze(bool isPlayer)
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
            if (CombatSystem.Instance.Enemy.FreezeCounter > 0)
            {
                if (CombatSystem.Instance.Enemy.FreezeCounter >= CombatSystem.Instance.freezeProcThreshold)
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

        CombatSystem.Instance.Enemy.currentStaminaMax = CombatSystem.Instance.Enemy.EnemyStats.MaxStamina - 2;

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
            if (CombatSystem.Instance.Enemy.WetCounter > 0)
            {
                CombatSystem.Instance.Enemy.FireCounter = 0;
                CombatSystem.Instance.Enemy.WetCounter--;
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
            if (CombatSystem.Instance.Enemy.ParalyzeCounter > 0)
            {
                CombatSystem.Instance.Enemy.ParalyzeCounter--;

                if (paralyze <= 25)
                    result = true; 
            }
        }

        return result;
    }

    public IEnumerator PlayParalyzeAnimation(bool isPlayer)
    {
        GameObject target = isPlayer ? PlayerManager.Instance.playerHead.gameObject : CombatSystem.Instance.Enemy.enemyHead.gameObject;

        RuntimeManager.PlayOneShot(paralyzeSound);
        VisualEffectManager.Instance.TriggerBurst(target, VisualEffectManager.ParticleEffectType.Paralyze);

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator PlayFrozenAnimation(bool isPlayer)
    {
        GameObject target = isPlayer ? PlayerManager.Instance.playerHead.gameObject : CombatSystem.Instance.Enemy.enemyHead.gameObject;

        VisualEffectManager.Instance.TriggerBurst(target, VisualEffectManager.ParticleEffectType.Freeze);

        yield return new WaitForSeconds(1f);

        VisualEffectManager.Instance.RemoveEffect(target, VisualEffectManager.ParticleEffectType.Freeze);
        if (isPlayer)
            VisualEffectManager.Instance.RemoveEffect(InventoryManager.Instance.itemsParent.gameObject, VisualEffectManager.ParticleEffectType.FreezeInventory);

        yield return new WaitForSeconds(1f);
    }

    public void Thunder(bool isPlayer,bool isThunder, int damageThunder)
    {
        if (!isThunder)
            return;

        if (isPlayer)
            PlayerManager.Instance.TakeDamage(damageThunder);
        else
            CombatSystem.Instance.Enemy.TakeDamage(damageThunder);

        //RefreshItemReactions();
    }

    private int ComputeFireDamage()
    {
        RuntimeManager.PlayOneShot(fireSound);
        return Convert.ToInt32(WeatherManager.Instance.temperature / 7);
    }
}
