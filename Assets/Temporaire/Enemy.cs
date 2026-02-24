using System;
using System.Runtime.CompilerServices;
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
        print($"Ennemi initialisé - PV: {EnemyStats.MaxHealth}");
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
        currentHealth -= damage;
        UpdateHealthBar();
        Debug.Log($"Ennemi prend {damage} dégâts. PV restants: {currentHealth}");
    }
    public void Heal(int heal)
    {
        currentHealth += heal;
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