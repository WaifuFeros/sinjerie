using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int maxHealth = 100;
    public int currentHealth;
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats stats;

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;

    private void Start()
    {
        stats.currentHealth = stats.maxHealth;
        UpdateHealthBar();
    }

    public void SetDifficulty(int roomNumber)
    {
        // Augmenter la difficulté selon le numéro de salle
        float difficultyMultiplier = 1f + (roomNumber * 0.2f);

        stats.maxHealth = Mathf.RoundToInt(stats.maxHealth * difficultyMultiplier);
        stats.currentHealth = stats.maxHealth;

        UpdateHealthBar();

        Debug.Log($"Ennemi configuré - PV: {stats.maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;
        UpdateHealthBar();

        Debug.Log($"Ennemi prend {damage} dégâts. PV restants: {stats.currentHealth}");
    }
    public void Heal(int heal)
    {
        stats.currentHealth += heal;
        UpdateHealthBar();
    }


    public bool IsDead()
    {
        return stats.currentHealth <= 0;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)stats.currentHealth / stats.maxHealth;
        }
    }

    public EnemyStats GetStats()
    {
        return stats;
    }
}