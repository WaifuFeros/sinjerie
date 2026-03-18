using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


[System.Serializable]
public class Reward
{
    public int gold = 10;
    public List<ObjetSO> items;
    
}

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    [Header("Reward Settings")]
    [SerializeField] 
    private Reward baseReward;

    public int NumberOfToggle;

    public int NumberOfRewardToChoose;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [SerializeField]
    private GameObject _ToggleButton;

    [SerializeField]
    private GameObject _ButtonValidate;

    [SerializeField]
    private TextMeshProUGUI _Text;

    public EnemySO EnemySO;

    public Transform Panel;

    public List<ToggleAssignation> ToggleAssignations = new List<ToggleAssignation>();


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    public void Initialize()
    {
        baseReward.items = new List<ObjetSO>();

    }

    public void AddToggle()
    {

        for (int i = 0; i < NumberOfToggle; i++)
        {
            var indexAppendReward = Random.Range(0, EnemySO.Items.Length);
            GameObject toggle = Instantiate(_ToggleButton, Panel);
            ToggleAssignation toggleAssignation = toggle.GetComponent<ToggleAssignation>();

            // _enemySo récup items et les assignes au toggle
            var valueItemReward = EnemySO.Items[indexAppendReward];
            baseReward.items.Add(valueItemReward);
            toggleAssignation.Initialized(valueItemReward,this);
            print(baseReward.items[i].name);
            print(baseReward.items.Count);
            ToggleAssignations.Add(toggleAssignation);
        }
        _Text.text = $"Choisissez {NumberOfRewardToChoose} récompenses ";
    }

    public void ValidateChoice()
    {
        Panel.gameObject.SetActive(false);
        for(int i = 0; i < ToggleAssignations.Count; i++)
        {
            string Name = "Item(Clone)";
            if(Name == Panel.GetChild(i).name)
            {
            }
                print("test");
                if (ToggleAssignations[i].Toggle.isOn == true)
                {
                    ItemManager.Instance.SpawnItem(baseReward.items[i]);
                    print(baseReward.items[i].name);

                }

                Destroy(ToggleAssignations[i].gameObject);
        }
        baseReward.items.Clear();
        ToggleAssignations.Clear();
    }
    

    public void GiveRewards(bool victory, int roomNumber, System.Action onComplete)
    {
        Reward reward = CalculateRewards();
    }

    private Reward CalculateRewards()
    {
        Reward reward = new Reward();

        ////Base reward
        reward.gold = baseReward.gold;

        //Gold que tu gagnes.
        reward.gold = EnemySO.GoldReward;
        return reward;
    }
}