using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("Extra")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    public int gold = 0;

    [Header("TEMPORAIRE")]
    [SerializeField] public ObjetSO[] Deck;

}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Action OnStaminaUpdateEvent;
    public Action OnGoldUpdateEvent;
 
    [Header("Player Stats")]
    [SerializeField] public PlayerStatsData stats;

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private StaminaUIManager staminaUI;

    [Header("WeatherEffect")]
    [SerializeField,HideInInspector] public int FireCounter = 0;
    [SerializeField,HideInInspector] public int FreezeCounter = 0;
    [SerializeField,HideInInspector] public int WetCounter = 0;
    [SerializeField,HideInInspector] public int ParalyzeCounter = 0;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        stats.currentHealth = stats.maxHealth;
        stats.currentStamina = stats.maxStamina;

        UpdateHealthBar();
        staminaUI.SetupStamina(stats.maxStamina);
        staminaUI.UpdateDisplay(stats.currentStamina);
    }

    private void OnDestroy()
    {
        OnStaminaUpdateEvent = null;
    }

    /// <summary>
    /// Le joueur prend des degats
    /// </summary>
    public void TakeDamage(int damage)
    {
        VisualEffectManager.Instance.ShakeUI(0.3f, 20f, 15);
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
    /// Ajoute de l'experience et verifie le level up
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

        // Augmenter les stats (style Pokemon)
        int healthIncrease = Random.Range(15, 25);
        int attackIncrease = Random.Range(3, 7);
        int defenseIncrease = Random.Range(2, 5);
        int speedIncrease = Random.Range(2, 5);

        stats.maxHealth += healthIncrease;
        stats.currentHealth = stats.maxHealth; // Soigne completement au level up

        UpdateHealthBar();
        Debug.Log($" Level Up! Niveau {stats.level}");
        Debug.Log($"Stats am�lior�es - PV: +{healthIncrease}, Attaque: +{attackIncrease}, D�fense: +{defenseIncrease}, Vitesse: +{speedIncrease}");
    }

    /// <summary>
    /// Met a jour la barre de vie dans l'UI
    /// </summary>
    public void UpdateHealthBar()
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
            OnStaminaUpdateEvent?.Invoke();
            return true;
        }
        return false;
    }
    // Remet la stamina a son maximum
    public void refillStamina(int staminaPenaly = 0)
    {
        int staminaToRegen = Mathf.Max(stats.staminaRegenPerTurn - staminaPenaly, 0);
        stats.currentStamina = Mathf.Min(stats.currentStamina + staminaToRegen, stats.maxStamina);
        staminaUI.UpdateDisplay(stats.currentStamina);
        OnStaminaUpdateEvent?.Invoke();
    }

    public void AddGold(int amount)
    {
        stats.gold += amount;
        OnGoldUpdateEvent?.Invoke();
        //UIManager.Instance.UpdateGoldUI(stats.gold);
        // + une anim de gain de piece
    }
    public void removeGold(int amount)
    {
        stats.gold -= amount;
        OnGoldUpdateEvent?.Invoke();
        //UIManager.Instance.UpdateGoldUI(stats.gold);
        // + une anim de perte de piece
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