using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private float enemyAttackDelay = 2f; // Délai avant que l'ennemi attaque

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour


    private Enemy currentEnemy;
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    private bool combatActive = false;
    private bool isPlayerTurn = true;

    public void Initialize()
    {
        // Configurer le bouton de passage de tour
        SetupSkipTurnButton();
    }

    ///<summary>
    ///Configure le bouton de passage de tour
    ///</summary>
    private void SetupSkipTurnButton()
    {
        if (skipTurnButton != null)
        {
            skipTurnButton.onClick.RemoveAllListeners();
            skipTurnButton.onClick.AddListener(() => OnSkipTurnButtonClicked());
        }
    }

    /// <summary>
    /// Démarre un combat
    /// </summary>
    public void StartCombat(System.Action onVictory, System.Action onDefeat)
    {
        onVictoryCallback = onVictory;
        onDefeatCallback = onDefeat;
        combatActive = true;
        isPlayerTurn = true;

        // Récupérer l'ennemi actuel
        if (roomManager != null)
        {
            GameObject enemyObj = roomManager.GetEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        SetupSkipTurnButtonInteractable(true);

    }

    public void AttackPlayer(ObjetSO attack)
    {
        if (!combatActive)
            return;
        playerStats.TakeDamage(attack.objectEffect);
        CheckCombatEnd();
    }
    public void HealPlayer (ObjetSO healItem)
    {
        if (!combatActive)
            return;
        playerStats.Heal(healItem.objectEffect);
        CheckCombatEnd();
    }

    public void AttackEnemy(ObjetSO attack)
    {
        if (!combatActive || currentEnemy == null)
            return;
        currentEnemy.TakeDamage(attack.objectEffect);
        CheckCombatEnd();
    }
    public void HealEnemy(ObjetSO healItem)
    {
        if (!combatActive ||  currentEnemy == null)
            return;
        currentEnemy.Heal(healItem.objectEffect);
        CheckCombatEnd();
    }

    /// <summary>
    /// Appelé quand le bouton de passage de tour est cliqué
    /// </summary>
    private void OnSkipTurnButtonClicked()
    {
        if (!combatActive || !isPlayerTurn)
        {
            return;
        }
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);
        Debug.Log("Tour passé!");
        playerStats.refillStamina(); // Restaure la stamina du joueur à chaque tour passé
        // l'ennemis attack
        StartCoroutine(EnemyAttackSequence());
    }

    /// <summary>
    /// Vérifie si le bouton de passage de tour doit être interactif
    /// </summary>
    private void SetupSkipTurnButtonInteractable(bool interactable)
    {
        if (skipTurnButton != null)
        {
            skipTurnButton.interactable = interactable;
        }
    }

    /// <summary>
    /// Vérifie les conditions de fin de combat après un délai
    /// </summary>
    private void CheckCombatEnd()
    {
        if (currentEnemy != null && currentEnemy.IsDead())
        {
            EndCombat(true);
        }
        else if (playerStats.IsDead())
        {
            EndCombat(false);
        }
    }

    /// <summary>
    /// Séquence d'attaque de l'ennemi
    /// </summary>
    private IEnumerator EnemyAttackSequence()
    {


        yield return new WaitForSeconds(enemyAttackDelay);

        if (!combatActive)
            yield break;

        // L'ennemi attaque
        if (currentEnemy != null && playerStats != null)
        {
            playerStats.TakeDamage(3);
            Debug.Log($"L'ennemi inflige {3} dégâts!");
        }

        CheckCombatEnd();

        // Retour au tour du joueur
        isPlayerTurn = true;
        SetupSkipTurnButtonInteractable(true);
        Debug.Log("À vous de jouer! Touchez un bouton pour attaquer.");
    }

    private IEnumerator EndCombat(bool victory)
    {
        combatActive = false;
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);

        yield return new WaitForSeconds(0.5f);

        // Vérifier si l'ennemi est mort

        if (victory)
        {
            Debug.Log("Victoire au combat!");
            onVictoryCallback?.Invoke();
        }
        else
        {
            Debug.Log("Défaite au combat!");
            onDefeatCallback?.Invoke();
        }
        yield break;
    }

    public bool IsCombatActive()
    {
        return combatActive;
    }

    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }
}