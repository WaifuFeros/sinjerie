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
    [SelectScene]
    [SerializeField] private string _gameSceneName;

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
        TransitionManager.Instance.TransitionWithAction(() =>
        {
            SceneLoadManager.Instance.LoadSceneAsActive(_gameSceneName);
        });
    }

    /// <summary>
    /// Étape 1: Commencer le jeu
    /// </summary>
    public void StartGame()
    {
        currentRoomNumber = 0;

        // Initialiser tout les Manager
        RoomManager.Instance.Initialize(() =>
        {
            ItemManager.Instance.WaitForItemDatabaseInitialization(() =>
            {
                CombatSystem.Instance.Initialize(() =>
                {
                    RewardSystem.Instance.Initialize(() =>
                    {                         // Tout est prêt, passer à la première salle
                        TransitionToNewRoom(true);
                    });

                });
                    
            });

        });
    }

    /// <summary>
    /// Étape 2: Nouvelle salle
    /// </summary>
    private void TransitionToNewRoom(bool addDelayToItemSpawn = false)
    {
        currentRoomNumber++;

        WeatherManager.Instance.UpdateWeather(() =>
        {
            // Générer une nouvelle salle (0 = normal, 1 = spécial)
            RoomType roomType = RoomManager.Instance.GenerateNewRoom(currentRoomNumber);

            if (roomType == RoomType.Enemy || roomType == RoomType.Boss)
            {
                // Mettre à jour l'UI
                UIManager.Instance.UpdateRoomCounter(currentRoomNumber);
                UIManager.Instance.ShowRoomUI();
                // Passer au combat
                StartCombat(addDelayToItemSpawn);
            }
        });
    }

    /// <summary>
    /// Étape 3: Ennemi - Démarrage du combat
    /// </summary>
    public void StartCombat(bool addDelayToItemSpawn = false)
    {
        CombatSystem.Instance.StartCombat(OnCombatVictory, OnCombatDefeat, addDelayToItemSpawn);
        UIManager.Instance.ShowCombatUI();

    }

    /// <summary>
    /// Étape 4a: Réussite (Victoire)
    /// </summary>
    private void OnCombatVictory()
    {
        Debug.Log("Réussite - Combat gagné!");

        if (CombatSystem.Instance.Enemy.EnemyStats.IsBoss)
            UIManager.Instance.ShowBananaRewardPanel();
        else
            UIManager.Instance.ShowRewardPanel();
        

        
    }

    /// <summary>
    /// Étape 4b: Défaite
    /// </summary>
    private void OnCombatDefeat()
    {
        UIManager.Instance.ShowDefeatPanel();

    }


    /// <summary>
    /// Callback après collecte des récompenses
    /// </summary>
    public void ExitRoom()
    {
        TransitionToNewRoom();
    }

    public int GetCurrentRoomNumber()
    {
        return currentRoomNumber;
    }
}