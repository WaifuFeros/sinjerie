using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Starting,
    InRoom,
    InCombat,
    Victory,
    Defeat,
    CollectingRewards
}

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private RewardSystem rewardSystem;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ItemManager itemManager;

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Starting;

    [Header("Room Settings")]
    [SerializeField] private int currentRoomNumber = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Commencer le jeu
        StartGame();
    }

    /// <summary>
    /// Étape 1: Commencer le jeu
    /// </summary>
    public void StartGame()
    {
        currentState = GameState.Starting;
        currentRoomNumber = 0;

        // Initialiser les systèmes
        roomManager.Initialize(() =>
        {
            itemManager.Initialize(() =>
            {
                if (combatSystem != null)
                    combatSystem.Initialize();

                if (rewardSystem != null)
                    rewardSystem.Initialize();
            });
            // Passer à la première salle
            StartCoroutine(TransitionToNewRoom()); 

        });
    }

    /// <summary>
    /// Étape 2: Nouvelle salle
    /// </summary>
    private IEnumerator TransitionToNewRoom()
    {
        currentRoomNumber++;
        currentState = GameState.InRoom;


        // Générer une nouvelle salle
        if (roomManager != null)
        {
            yield return StartCoroutine(roomManager.GenerateNewRoom(currentRoomNumber));
        }

        // Mettre à jour l'UI
        if (uiManager != null)
        {
            uiManager.UpdateRoomCounter(currentRoomNumber);
            uiManager.ShowRoomUI();
        }

        // Attendre un peu avant de commencer le combat
        yield return new WaitForSeconds(1f);

        // Passer au combat
        StartCombat();
    }

    /// <summary>
    /// Étape 3: Ennemi - Démarrage du combat
    /// </summary>
    public void StartCombat()
    {
        currentState = GameState.InCombat;

        if (combatSystem != null)
        {
            combatSystem.StartCombat(OnCombatVictory, OnCombatDefeat);
        }

        if (uiManager != null)
        {
            uiManager.ShowCombatUI();
        }
    }

    /// <summary>
    /// Étape 4a: Réussite (Victoire)
    /// </summary>
    private void OnCombatVictory()
    {
        Debug.Log("Réussite - Combat gagné!");
        currentState = GameState.Victory;

        if (uiManager != null)
        {
            uiManager.ShowRewardPanel();
        }

        // Passer aux récompenses après un délai
        StartCoroutine(DelayedRewardCollection(true));
    }

    /// <summary>
    /// Étape 4b: Défaite
    /// </summary>
    private void OnCombatDefeat()
    {
        Debug.Log("Défaite - Combat perdu!");
        currentState = GameState.Defeat;

        if (uiManager != null)
        {
            uiManager.ShowDefeatPanel();
        }
    }

    /// <summary>
    /// Délai avant de collecter les récompenses
    /// </summary>
    private IEnumerator DelayedRewardCollection(bool victory)
    {
        yield return new WaitForSeconds(2f);
        CollectRewards(victory);
    }

    /// <summary>
    /// Étape 5: Récupère les récompenses
    /// </summary>
    private void CollectRewards(bool victory)
    {
        Debug.Log("Récupération des récompenses");
    }

    /// <summary>
    /// Callback après collecte des récompenses
    /// </summary>
    private void OnRewardsCollected()
    {
        Debug.Log("Récompenses collectées");

        StartCoroutine(TransitionToNewRoom());
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public int GetCurrentRoomNumber()
    {
        return currentRoomNumber;
    }
}