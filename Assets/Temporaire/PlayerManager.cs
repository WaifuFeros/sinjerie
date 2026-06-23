using FMODUnity;
using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private EventReference iceSound;

    public bool HasTakenDamage { get; private set; }

    public Action OnStaminaUpdateEvent;
    public Action OnGoldUpdateEvent;
    public Action OnAfflictionUpdateEvent;
 
    [Header("Player Stats")]
    [SerializeField] public PlayerStatsData stats;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private StaminaUIManager staminaUI;
    [SerializeField] private AfflictionEffect afflictionEffect;
    [SerializeField] public Image playerHead;

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

        OnAfflictionUpdateEvent += UpdateAfflictionIcons;
        UpdateAfflictionIcons();
    }

    private void LateUpdate()
    {
        if (_afflictionUpdated)
        {
            OnAfflictionUpdateEvent?.Invoke();
            _afflictionUpdated = false;
        }
    }

    private void OnDestroy()
    {
        OnStaminaUpdateEvent = null;
        OnGoldUpdateEvent = null;
        OnAfflictionUpdateEvent = null;
    }

    /// <summary>
    /// Le joueur prend des degats
    /// </summary>
    public void TakeDamage(int damage)
    {
        VisualEffectManager.Instance.ShakeUI(0.3f, 20f, 15);
        stats.currentHealth -= damage;
        if(stats.currentHealth < 0)
            stats.currentHealth = 0;
        UpdateHealthBar();

        HasTakenDamage = true;
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

    public void UpdateAfflictionIcons()
    {
        afflictionEffect.UpdateVisuals(FireCounter, WetCounter, ParalyzeCounter, FreezeCounter);

        if (FireCounter > 0)
            VisualEffectManager.Instance.AddEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);
        else
            VisualEffectManager.Instance.RemoveEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Fire);

        if (WetCounter > 0)
            VisualEffectManager.Instance.AddEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Water);
        else
            VisualEffectManager.Instance.RemoveEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Water);

        if (ParalyzeCounter > 0)
            VisualEffectManager.Instance.AddEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Paralyze);
        else
            VisualEffectManager.Instance.RemoveEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Paralyze);

        if (IsFrozen)
        {
            RuntimeManager.PlayOneShot(iceSound);
            VisualEffectManager.Instance.AddEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);
        }

        else
            VisualEffectManager.Instance.RemoveEffect(playerHead.gameObject, VisualEffectManager.ParticleEffectType.Freeze);
    }
}