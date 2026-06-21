using FMODUnity;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action OnAfflictionUpdateEvent;
    [SerializeField] private EventReference iceSound;

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] public UnityEngine.UI.Image enemyHead;
    [SerializeField] private AfflictionEffect afflictionEffect;

    #region Counter logic variables

    public int FireCounter
    {
        get => _fireCounter;
        set
        {
            _fireCounter = Mathf.Max(0, value);
            _afflictionUpdated = true;
        }
    }
    public int WetCounter
    {
        get => _wetCounter;
        set
        {
            _wetCounter = Mathf.Max(0, value);
            _afflictionUpdated = true;
        }
    }
    public int ParalyzeCounter
    {
        get => _paralyzeCounter;
        set
        {
            _paralyzeCounter = Mathf.Max(0, value);
            _afflictionUpdated = true;
        }
    }
    public int FreezeCounter
    {
        get => _freezeCounter;
        set
        {
            _freezeCounter = Mathf.Max(0, value);
            _afflictionUpdated = true;
        }
    }

    public bool IsFrozen => FreezeCounter >= CombatSystem.Instance.freezeProcThreshold;

    private int _fireCounter;
    private int _freezeCounter;
    private int _wetCounter;
    private int _paralyzeCounter;
    private bool _afflictionUpdated;

    #endregion

    public EnemySO EnemyStats;

    public int currentHealth;
    public int currentStaminaMax;

    private void Awake()
    {
        OnAfflictionUpdateEvent += UpdateAfflictionIcons;
    }

    private void LateUpdate()
    {
        if (_afflictionUpdated)
        {
            OnAfflictionUpdateEvent?.Invoke();
            _afflictionUpdated = false;
        }
    }

    public void Initialize(EnemySO stats)
    {
        FireCounter = 0;
        FreezeCounter = 0;
        WetCounter = 0;
        ParalyzeCounter = 0;
        EnemyStats = stats;
        currentHealth = EnemyStats.MaxHealth;
        enemyHead.sprite = EnemyStats.Sprite;
        currentStaminaMax = EnemyStats.MaxStamina;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        VisualEffectManager.Instance.EnemyTakeDamage();
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthBar();
    }
    public void Heal(int heal)
    {
        VisualEffectManager.Instance.EnemyHeal();
        currentHealth = Mathf.Min(EnemyStats.MaxHealth, currentHealth + heal);
        UpdateHealthBar();
    }


    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void UpdateHealthBar()
    {
        
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / EnemyStats.MaxHealth;
        }
    }

    public EnemySO GetStats()
    {
        return EnemyStats;
    }

    public void UpdateAfflictionIcons()
    {
        afflictionEffect.UpdateVisuals(FireCounter, WetCounter, ParalyzeCounter, FreezeCounter);

        if (FireCounter > 0)
            VisualEffectManager.Instance.AddEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);
        else
            VisualEffectManager.Instance.RemoveEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

        if (WetCounter > 0)
            VisualEffectManager.Instance.AddEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Water);
        else
            VisualEffectManager.Instance.RemoveEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Water);

        if (ParalyzeCounter > 0)
            VisualEffectManager.Instance.AddEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Paralyze);
        else
            VisualEffectManager.Instance.RemoveEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Paralyze);

        if (IsFrozen)
        {
            RuntimeManager.PlayOneShot(iceSound);
            VisualEffectManager.Instance.AddEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);
        }
        else
            VisualEffectManager.Instance.RemoveEffect(enemyHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);
    }
}