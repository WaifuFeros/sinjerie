
using UnityEngine;

[System.Serializable]
public class PlayerStatsData
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int attack = 15;
    public int defense = 10;
    public int speed = 50;
    public int maxStamina = 5;
    public int currentStamina;

    [Header("Level & Experience")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
}

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private PlayerStatsData stats;

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private StaminaUIManager staminaUI;

    private void Start()
    {
        stats.currentHealth = stats.maxHealth;
        stats.currentStamina = stats.maxStamina;

        UpdateHealthBar();
        staminaUI.SetupStamina(stats.maxStamina);
        staminaUI.UpdateDisplay(stats.currentStamina);
    }

    /// <summary>
    /// Le joueur prend des dégâts
    /// </summary>
    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - stats.defense);
        stats.currentHealth -= actualDamage;
        stats.currentHealth = Mathf.Max(0, stats.currentHealth);

        UpdateHealthBar();

        Debug.Log($"Joueur prend {actualDamage} dégâts. PV restants: {stats.currentHealth}");
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        stats.currentHealth = Mathf.Min(stats.maxHealth, stats.currentHealth + amount);
        UpdateHealthBar();
        Debug.Log($"Joueur soigné de {amount} PV. PV actuels: {stats.currentHealth}/{stats.maxHealth}");
    }

    /// <summary>
    /// Ajoute de l'expérience et vérifie le level up
    /// </summary>
    public void AddExperience(int exp)
    {
        stats.experience += exp;

        // Vérifier le level up
        while (stats.experience >= stats.experienceToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Augmente le niveau du joueur et améliore ses stats
    /// </summary>
    private void LevelUp()
    {
        stats.level++;
        stats.experience -= stats.experienceToNextLevel;
        stats.experienceToNextLevel = Mathf.RoundToInt(stats.experienceToNextLevel * 1.5f);

        // Augmenter les stats (style Pokémon)
        int healthIncrease = Random.Range(15, 25);
        int attackIncrease = Random.Range(3, 7);
        int defenseIncrease = Random.Range(2, 5);
        int speedIncrease = Random.Range(2, 5);

        stats.maxHealth += healthIncrease;
        stats.currentHealth = stats.maxHealth; // Soigne complètement au level up
        stats.attack += attackIncrease;
        stats.defense += defenseIncrease;
        stats.speed += speedIncrease;

        UpdateHealthBar();
        Debug.Log($" Level Up! Niveau {stats.level}");
        Debug.Log($"Stats améliorées - PV: +{healthIncrease}, Attaque: +{attackIncrease}, Défense: +{defenseIncrease}, Vitesse: +{speedIncrease}");
    }

    /// <summary>
    /// Met à jour la barre de vie dans l'UI
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)stats.currentHealth / stats.maxHealth;
        }
    }


    // Enleve de la stamina, retourne true si l'action est possible, false sinon
    public bool removeStamina(int nb)
    {
        if (stats.currentStamina - nb >= 0)
        {
            stats.currentStamina -= nb;
            staminaUI.UpdateDisplay(stats.currentStamina);
            return true;
        }
        return false;
    }
    // Remet la stamina à son maximum
    public void refillStamina()
    {
        stats.currentStamina = stats.maxStamina;
        staminaUI.UpdateDisplay(stats.currentStamina);
    }




    // Getters
    public bool IsDead()
    {
        return stats.currentHealth <= 0;
    }

    public int GetAttack()
    {
        return stats.attack;
    }

    public int GetDefense()
    {
        return stats.defense;
    }

    public int GetSpeed()
    {
        return stats.speed;
    }

    public PlayerStatsData GetStats()
    {
        return stats;
    }
}