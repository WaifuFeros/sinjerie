using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private UnityEngine.UI.Image _enemyImage;

    [Header("WeatherEffect")]
    public int FireCounter = 0;
    public int FreezeCounter = 0;
    public int SwetCounter = 0;
    public int ParalyzeCounter = 0;

    public EnemySO EnemyStats;

    public int currentHealth;
    public void Initialize(EnemySO stats)
    {
        EnemyStats = stats;
        currentHealth = EnemyStats.MaxHealth;
        _enemyImage.sprite = EnemyStats.Sprite;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthBar();
    }
    public void Heal(int heal)
    {
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
}