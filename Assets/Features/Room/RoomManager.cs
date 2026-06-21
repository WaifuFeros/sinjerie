using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private VcaController gameplayMusicVCA;

    [Header("Room Settings")]
    [SerializeField] private Transform roomContainer;
    [SerializeField] private GameObject roomShop; // La salle de shop
    [SerializeField] private GameObject[] roomPrefabs; // Différents types de salles

    [SerializeField] private RoomType[] _roomsLoop;

    private GameObject currentRoom;
    private List<EnemySO> availableEnemyData = new List<EnemySO>();
    private List<EnemySO> availableBossData = new List<EnemySO>();
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
                availableEnemyData.AddRange(handle.Result.Where(x => x.IsActive && !x.IsBoss));
                availableBossData.AddRange(handle.Result.Where(x => x.IsActive && x.IsBoss));
                onLoadCompleted?.Invoke();
            }
        };
    }

    // Retourne le type de salle (0 = normal, 1 = spécial)
    public RoomType GenerateNewRoom(int roomNumber)
    {
        // Détruire l'ancienne salle si elle existe
        if (currentRoom != null)
        {
            Destroy(currentRoom);
        }

        int roomNumberMod = (roomNumber - 1) % _roomsLoop.Length;

        RoomType roomType = _roomsLoop[roomNumberMod];

        switch (roomType)
        {
            default:
            case RoomType.None:
            case RoomType.Enemy:
                SpawnEnemy(false);
                return roomType;
            case RoomType.Boss:
                SpawnEnemy(false);
                return roomType;
            case RoomType.Shop:
                currentRoom = Instantiate(roomShop, roomContainer.position, roomContainer.rotation);
                currentRoom.transform.SetParent(roomContainer, false);
                gameplayMusicVCA.FadeLowerMusicVolume(2f, 0.2f);
                return roomType;
            case RoomType.Special:
                GameObject roomToSpawn = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];
                currentRoom = Instantiate(roomToSpawn, roomContainer.position, roomContainer.rotation);
                currentRoom.transform.SetParent(roomContainer, false);
                gameplayMusicVCA.FadeLowerMusicVolume(2f, 0.2f);
                return roomType;
        }
    }

    private void SpawnEnemy(bool isBoss)
    {
        // Configurer l'ennemi selon le numéro de salle (difficulté progressive)
        int randomIndex = UnityEngine.Random.Range(0, availableEnemyData.Count);
        EnemySO randomData = availableEnemyData[randomIndex];
        CombatSystem.Instance.Enemy.Initialize(randomData);
        rewardSystem.EnemySO = randomData;
    }
}

public enum RoomType
{
    None,
    Enemy,
    Boss,
    Shop,
    Special,
}