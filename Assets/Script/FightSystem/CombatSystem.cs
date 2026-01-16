using System.Collections;
using UnityEngine;
using System;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private float enemyAttackDelay = 2f; // Délai avant que l'ennemi attaque

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Attack Buttons")]
    [SerializeField] private AttackButtonData[] attackButtons; // Les boutons d'attaque configurés dans l'Inspector

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour

    [System.Serializable]
    public class AttackButtonData
    {
        public UnityEngine.UI.Button button;
        public AttackData attackData;
    }

    private Enemy currentEnemy;
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    private bool combatActive = false;
    private bool waitingForPlayerAction = true;

    public void Initialize()
    {
        Debug.Log("CombatSystem initialisé");

        // Configurer les boutons d'attaque
        SetupAttackButtons();

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
    /// Configure les boutons d'attaque
    /// </summary>
    private void SetupAttackButtons()
    {
        if (attackButtons == null) return;

        for (int i = 0; i < attackButtons.Length; i++)
        {
            if (attackButtons[i].button != null && attackButtons[i].attackData != null)
            {
                AttackData attack = attackButtons[i].attackData;
                int index = i; // Capture pour le closure

                attackButtons[i].button.onClick.RemoveAllListeners();
                attackButtons[i].button.onClick.AddListener(() => OnAttackButtonClicked(attack));

                // Mettre à jour le texte/icône du bouton si nécessaire
                UpdateAttackButtonUI(attackButtons[i].button, attack);
            }
        }
    }

    /// <summary>
    /// Met à jour l'UI d'un bouton d'attaque
    /// </summary>
    private void UpdateAttackButtonUI(UnityEngine.UI.Button button, AttackData attack)
    {
        // Mettre à jour le texte si présent
        TMPro.TextMeshProUGUI text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.text = attack.attackName;
        }

        // Mettre à jour l'icône si présent
        UnityEngine.UI.Image icon = button.GetComponentInChildren<UnityEngine.UI.Image>();
        if (icon != null && attack.attackIcon != null)
        {
            icon.sprite = attack.attackIcon;
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
        waitingForPlayerAction = true;

        // Récupérer l'ennemi actuel
        if (roomManager != null)
        {
            GameObject enemyObj = roomManager.GetCurrentEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        // Activer les boutons d'attaque
        SetAttackButtonsInteractable(true);
        SetupSkipTurnButtonInteractable(true);

        Debug.Log("Combat démarré! Touchez un bouton pour attaquer.");
    }

    /// <summary>
    /// Appelé quand un bouton d'attaque est cliqué
    /// </summary>
    private void OnAttackButtonClicked(AttackData attack)
    {
        if (!combatActive || !waitingForPlayerAction)
        {
            return;
        }

        waitingForPlayerAction = false;
        SetAttackButtonsInteractable(false);
        SetupSkipTurnButtonInteractable(false);

        Debug.Log($"Attaque utilisée: {attack.attackName}");

        // Infliger les dégâts à l'ennemi
        if (currentEnemy != null)
        {
            currentEnemy.TakeDamage(attack.damageAmount);
            Debug.Log($"L'ennemi prend {attack.damageAmount} dégâts!");
        }

        // Vérifier les conditions de fin de combat
        StartCoroutine(CheckCombatEndAfterDelay());
    }

    /// <summary>
    /// Appelé quand le bouton de passage de tour est cliqué
    /// </summary>
    private void OnSkipTurnButtonClicked()
    {
        if (!combatActive || !waitingForPlayerAction)
        {
            return;
        }
        waitingForPlayerAction = false;
        SetAttackButtonsInteractable(false);
        SetupSkipTurnButtonInteractable(false);
        Debug.Log("Tour passé!");
        // l'ennemis attack
        StartCoroutine(EnemyAttackSequence());
    }


    /// <summary>
    /// Active ou désactive les boutons d'attaque
    /// </summary>
    private void SetAttackButtonsInteractable(bool interactable)
    {
        if (attackButtons == null) return;

        foreach (var attackButton in attackButtons)
        {
            if (attackButton.button != null)
            {
                attackButton.button.interactable = interactable;
            }
        }
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
    private IEnumerator CheckCombatEndAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        // Vérifier si l'ennemi est mort
        if (currentEnemy != null && currentEnemy.IsDead())
        {
            EndCombat(true);
            yield break;
        }

        // L'ennemi attaque après un délai
        StartCoroutine(EnemyAttackSequence());
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
            int enemyDamage = currentEnemy.GetAttackDamage();
            playerStats.TakeDamage(enemyDamage);
            Debug.Log($"L'ennemi inflige {enemyDamage} dégâts!");
        }

        // Vérifier si le joueur est mort
        if (playerStats != null && playerStats.IsDead())
        {
            EndCombat(false);
            yield break;
        }

        // Retour au tour du joueur
        waitingForPlayerAction = true;
        SetAttackButtonsInteractable(true);
        SetupSkipTurnButtonInteractable(true);
        Debug.Log("À vous de jouer! Touchez un bouton pour attaquer.");
    }

    private void EndCombat(bool victory)
    {
        combatActive = false;
        waitingForPlayerAction = false;
        SetAttackButtonsInteractable(false);
        SetupSkipTurnButtonInteractable(false);

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