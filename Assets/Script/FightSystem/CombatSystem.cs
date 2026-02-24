using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private float enemyAttackDelay = 2f; // D�lai avant que l'ennemi attaque

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
    /// D�marre un combat
    /// </summary>
    public void StartCombat(System.Action onVictory, System.Action onDefeat)
    {
        onVictoryCallback = onVictory;
        onDefeatCallback = onDefeat;
        combatActive = true;
        isPlayerTurn = true;

        // R�cup�rer l'ennemi actuel
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
        public UnityEngine.UI.Button button;
        public AttackData attackData;
    }

    private Enemy currentEnemy;
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    private bool combatActive = false;
    private bool isPlayerTurn = true;

    public void Initialize()
    {
        Debug.Log("CombatSystem initialis�");

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
    /// D�marre un combat
    /// </summary>
    public void StartCombat(System.Action onVictory, System.Action onDefeat)
    {
        onVictoryCallback = onVictory;
        onDefeatCallback = onDefeat;
        combatActive = true;
        isPlayerTurn = true;

        // R�cup�rer l'ennemi actuel
        if (roomManager != null)
        {
            GameObject enemyObj = roomManager.GetCurrentEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        SetupSkipTurnButtonInteractable(true);

        Debug.Log("Combat d�marr�! Touchez un bouton pour attaquer.");
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
    /// Appel� quand le bouton de passage de tour est cliqu�
    /// </summary>
    private void OnSkipTurnButtonClicked()
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
    /// S�quence d'attaque de l'ennemi
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
            Debug.Log($"L'ennemi inflige {3} d�g�ts!");
        }

        CheckCombatEnd();

        // Retour au tour du joueur
        isPlayerTurn = true;
        SetupSkipTurnButtonInteractable(true);
        Debug.Log("� vous de jouer! Touchez un bouton pour attaquer.");
    }

    private IEnumerator EndCombat(bool victory)
    {
        if (skipTurnButton != null)
        {
            skipTurnButton.interactable = interactable;
        }
    }

    /// <summary>
    /// V�rifie les conditions de fin de combat apr�s un d�lai
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
    /// S�quence d'attaque de l'ennemi
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
            Debug.Log($"L'ennemi inflige {3} d�g�ts!");
        }

        CheckCombatEnd();

        // Retour au tour du joueur
        isPlayerTurn = true;
        SetupSkipTurnButtonInteractable(true);
        Debug.Log("� vous de jouer! Touchez un bouton pour attaquer.");
    }

    private IEnumerator EndCombat(bool victory)
    {
        combatActive = false;
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);

        yield return new WaitForSeconds(0.5f);

        // V�rifier si l'ennemi est mort

        if (victory)
        {
            Debug.Log("Victoire au combat!");
            onVictoryCallback?.Invoke();
        }
        else
        {
            Debug.Log("D�faite au combat!");
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