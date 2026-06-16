using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Room Settings")]
    [SerializeField] private int currentRoomNumber = 0;

    [Header("Scene Settings")]
    [SerializeField] private string _gameSceneName;



    private CombatSystem combatSystem;
    private RoomManager roomManager;
    private RewardSystem rewardSystem;
    private ItemManager itemManager;
    private UIManager uiManager;
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

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(_gameSceneName);
    }

    /// <summary>
    /// Étape 1: Commencer le jeu
    /// </summary>
    public void StartGame()
    {
        currentRoomNumber = 0;

        // Récup tout les Manager
        roomManager = RoomManager.Instance;
        itemManager = ItemManager.Instance;
        combatSystem = CombatSystem.Instance;
        rewardSystem = RewardSystem.Instance;
        uiManager = UIManager.Instance;

        // Initialiser tout les Manager
        roomManager.Initialize(() =>
        {
            itemManager.Initialize(() =>
            {
                combatSystem.Initialize(() =>
                {
                    rewardSystem.Initialize(() =>
                    {                         // Tout est prêt, passer à la première salle
                        StartCoroutine(TransitionToNewRoom());
                    });

                });
                    
            });

        });
    }

    /// <summary>
    /// Étape 2: Nouvelle salle
    /// </summary>
    private IEnumerator TransitionToNewRoom()
    {
        currentRoomNumber++;

        // Générer une nouvelle salle (0 = normal, 1 = spécial)
        int roomType = roomManager.GenerateNewRoom(currentRoomNumber);
        
        if (roomType == 0)
        {
            // Mettre à jour l'UI
            uiManager.UpdateRoomCounter(currentRoomNumber);
            uiManager.ShowRoomUI();
            // Passer au combat
            StartCombat();
        }
        yield return null;
    }

    /// <summary>
    /// Étape 3: Ennemi - Démarrage du combat
    /// </summary>
    public void StartCombat()
    {
        combatSystem.StartCombat(OnCombatVictory, OnCombatDefeat);
        uiManager.ShowCombatUI();

    }

    /// <summary>
    /// Étape 4a: Réussite (Victoire)
    /// </summary>
    private void OnCombatVictory()
    {
        Debug.Log("Réussite - Combat gagné!");

        if (CombatSystem.Instance.currentEnemy.EnemyStats.IsBoss)
            uiManager.ShowBananaRewardPanel();
        else
            uiManager.ShowRewardPanel();
        

        
    }

    /// <summary>
    /// Étape 4b: Défaite
    /// </summary>
    private void OnCombatDefeat()
    {
        uiManager.ShowDefeatPanel();

    }


    /// <summary>
    /// Callback après collecte des récompenses
    /// </summary>
    public void ExitRoom()
    {
        StartCoroutine(TransitionToNewRoom());
    }

    public int GetCurrentRoomNumber()
    {
        return currentRoomNumber;
    }
}