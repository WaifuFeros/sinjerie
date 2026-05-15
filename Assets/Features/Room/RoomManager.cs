using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room Settings")]
    [SerializeField] private Transform roomContainer;
    [SerializeField] private GameObject roomShop; // La salle de shop
    [SerializeField] private GameObject[] roomPrefabs; // Différents types de salles

    [Header("Enemy Spawn")]
    [SerializeField] private GameObject enemy;

    private GameObject currentRoom;
    private List<EnemySO> availableEnemyData = new List<EnemySO>();
    private RewardSystem rewardSystem;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        rewardSystem = RewardSystem.Instance;
    }
    public void Initialize(Action onLoadCompleted)
    {
        Addressables.LoadAssetsAsync<EnemySO>("Enemy", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                availableEnemyData.AddRange(handle.Result);
                onLoadCompleted?.Invoke();
            }
        };
    }

    // Retourne le type de salle (0 = normal, 1 = spécial)
    public int GenerateNewRoom(int roomNumber)
    {
        // Détruire l'ancienne salle si elle existe
        if (currentRoom != null)
        {
            Destroy(currentRoom);
        }

        if ((roomNumber + 1) % 8 == 0) // room spécial
        {
            GameObject roomToSpawn = roomPrefabs[UnityEngine.Random.Range(0,roomPrefabs.Length)];
            currentRoom = Instantiate(roomToSpawn, roomContainer.position, roomContainer.rotation);
            currentRoom.transform.SetParent(roomContainer);
            return 1;
        }
        else if ((roomNumber + 1) % 4 == 0) // room shop
        {
            currentRoom = Instantiate(roomShop, roomContainer.position, roomContainer.rotation);
            currentRoom.transform.SetParent(roomContainer);
            return 1;
        }
        else
        {
            SpawnEnemy(roomNumber);
            return 0;
        }
    }

    private void SpawnEnemy(int roomNumber)
    {
        if (enemy != null)
        {
            // Configurer l'ennemi selon le numéro de salle (difficulté progressive)
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            int randomIndex = UnityEngine.Random.Range(0, availableEnemyData.Count);
            EnemySO randomData = availableEnemyData[randomIndex];
            enemyScript.Initialize(randomData);
            if (enemyScript != null)
            {
                enemyScript.SetDifficulty(roomNumber);
            }
            rewardSystem.EnemySO = randomData;
        }

    }

    public GameObject GetEnemy()
    {
        return enemy;
    }
}