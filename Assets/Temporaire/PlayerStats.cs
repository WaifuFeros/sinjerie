using UnityEngine;

[System.Serializable]
public class PlayerStatsData
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 5;
    public int staminaRegenPerTurn = 2;
    public int currentStamina;
    public int nbStartItem = 20;
    public int nbItemPerTurn = 20;

    [Header("Level & Experience")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;


    [Header("TEMPORAIRE")]
    [SerializeField] public ObjetSO[] Deck;

}

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] public PlayerStatsData stats;

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
    /// Le joueur prend des d�g�ts
    /// </summary>
    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;
        UpdateHealthBar();
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        stats.currentHealth = Mathf.Min(stats.maxHealth, stats.currentHealth + amount);
        UpdateHealthBar();
    }

    /// <summary>
    /// Ajoute de l'exp�rience et v�rifie le level up
    /// </summary>
    public void AddExperience(int exp)
    {
        stats.experience += exp;

        // V�rifier le level up
        while (stats.experience >= stats.experienceToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Augmente le niveau du joueur et am�liore ses stats
    /// </summary>
    private void LevelUp()
    {
        stats.level++;
        stats.experience -= stats.experienceToNextLevel;
        stats.experienceToNextLevel = Mathf.RoundToInt(stats.experienceToNextLevel * 1.5f);

        // Augmenter les stats (style Pok�mon)
        int healthIncrease = Random.Range(15, 25);
        int attackIncrease = Random.Range(3, 7);
        int defenseIncrease = Random.Range(2, 5);
        int speedIncrease = Random.Range(2, 5);

        stats.maxHealth += healthIncrease;
        stats.currentHealth = stats.maxHealth; // Soigne compl�tement au level up

        UpdateHealthBar();
        Debug.Log($" Level Up! Niveau {stats.level}");
        Debug.Log($"Stats am�lior�es - PV: +{healthIncrease}, Attaque: +{attackIncrease}, D�fense: +{defenseIncrease}, Vitesse: +{speedIncrease}");
    }

    /// <summary>
    /// Met � jour la barre de vie dans l'UI
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
    // Remet la stamina � son maximum
    public void refillStamina()
    {
        stats.currentStamina = Mathf.Min(stats.currentStamina + stats.staminaRegenPerTurn, stats.maxStamina);
        staminaUI.UpdateDisplay(stats.currentStamina);
    }




    // Getters
    public bool IsDead()
    {
        return stats.currentHealth <= 0;
    }


    public PlayerStatsData GetStats()
    {
        return stats;
    }
}