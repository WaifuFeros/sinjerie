using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Transform roomContainer;
    [SerializeField] private GameObject[] roomPrefabs; // Différents types de salles

    [Header("Enemy Spawn")]
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private GameObject enemy;

    private GameObject currentRoom;
    private List<EnemySO> availableEnemyData = new List<EnemySO>();
    public RewardSystem rewardSystem;

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

    public IEnumerator GenerateNewRoom(int roomNumber)
    {

        // Détruire l'ancienne salle si elle existe
        if (currentRoom != null)
        {
            Destroy(currentRoom);
        }

        // Choisir un prefab de salle aléatoire ou selon le numéro
        GameObject roomToSpawn = roomPrefabs.Length > 0
            ? roomPrefabs[roomNumber % roomPrefabs.Length]
            : null;

        if (roomToSpawn != null && roomContainer != null)
        {
            currentRoom = Instantiate(roomToSpawn, roomContainer.position, roomContainer.rotation);
            currentRoom.transform.SetParent(roomContainer);
        }

        // Spawner un ennemi
        SpawnEnemy(roomNumber);

        yield return new WaitForSeconds(0.5f);
    }

    private void SpawnEnemy(int roomNumber)
    {
        if (enemy != null && enemySpawnPoint != null)
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
            rewardSystem._EnemySO = randomData;
        }
    }

    public GameObject GetEnemy()
    {
        return enemy;
    }
}