using System.Collections;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Transform roomContainer;
    [SerializeField] private GameObject[] roomPrefabs; // Différents types de salles

    [Header("Enemy Spawn")]
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private GameObject enemyPrefab;

    private GameObject currentRoom;
    private GameObject currentEnemy;

    public void Initialize()
    {
        Debug.Log("RoomManager initialisé");
    }

    public IEnumerator GenerateNewRoom(int roomNumber)
    {
        Debug.Log($"Génération de la salle #{roomNumber}");

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
        if (enemyPrefab != null && enemySpawnPoint != null)
        {
            // Détruire l'ancien ennemi si il existe
            if (currentEnemy != null)
            {
                Destroy(currentEnemy);
            }

            currentEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);

            // Configurer l'ennemi selon le numéro de salle (difficulté progressive)
            Enemy enemyScript = currentEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.SetDifficulty(roomNumber);
            }
        }
    }

    public GameObject GetCurrentEnemy()
    {
        return currentEnemy;
    }
}