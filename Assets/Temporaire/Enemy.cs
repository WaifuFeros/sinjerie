using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;


public class Enemy : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private UnityEngine.UI.Image _enemyImage;

    public EnemySO EnemyStats;

    public int currentHealth;
    public void Initialize(EnemySO stats)
    {
        EnemyStats = stats;
        currentHealth = EnemyStats.MaxHealth;
        _enemyImage.sprite = EnemyStats.Sprite;
        UpdateHealthBar();
    }

    public void SetDifficulty(int roomNumber)
    {
        // Augmenter la difficulté selon le numéro de salle
        /*
        float difficultyMultiplier = 1f + (roomNumber * 0.2f);

        stats.maxHealth = Mathf.RoundToInt(stats.maxHealth * difficultyMultiplier);
        currentHealth = EnemyStats.MaxHealth;

        UpdateHealthBar();

        Debug.Log($"Ennemi configuré - PV: {EnemyStats.MaxHealth}");
        */
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