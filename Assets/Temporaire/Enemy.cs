using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] public UnityEngine.UI.Image EnemyImage;
    [SerializeField] private AfflictionEffect afflictionEffect;

    [Header("WeatherEffect")]
    [SerializeField] public int FireCounter = 0;
    [SerializeField] public int FreezeCounter = 0;
    [SerializeField] public int WetCounter = 0;
    [SerializeField] public int ParalyzeCounter = 0;

    public EnemySO EnemyStats;

    public int currentHealth;
    public int currentStaminaMax;

    public void Initialize(EnemySO stats)
    {
        FireCounter = 0;
        FreezeCounter = 0;
        WetCounter = 0;
        ParalyzeCounter = 0;
        EnemyStats = stats;
        currentHealth = EnemyStats.MaxHealth;
        EnemyImage.sprite = EnemyStats.Sprite;
        currentStaminaMax = EnemyStats.MaxStamina;
        VisualEffectManager.Instance.RemoveEffect(EnemyImage.gameObject);
        UpdateHealthBar();
        UpdateAfflictionIcons();
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
    }
}