using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 10;
    public int defense = 5;
    public int speed = 50;
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
        stats.attackDamage = Mathf.RoundToInt(stats.attackDamage * difficultyMultiplier);
        stats.defense = Mathf.RoundToInt(stats.defense * difficultyMultiplier);
        stats.speed = Mathf.RoundToInt(stats.speed * difficultyMultiplier);

        UpdateHealthBar();

        Debug.Log($"Ennemi configuré - PV: {stats.maxHealth}, Attaque: {stats.attackDamage}, Défense: {stats.defense}");
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - stats.defense);
        stats.currentHealth -= actualDamage;
        stats.currentHealth = Mathf.Max(0, stats.currentHealth);

        UpdateHealthBar();

        Debug.Log($"Ennemi prend {actualDamage} dégâts. PV restants: {stats.currentHealth}");
    }

    public int GetAttackDamage()
    {
        return stats.attackDamage;
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