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
    public List<ScriptableObject> items;
    
}

public class RewardSystem : MonoBehaviour
{

    [Header("Reward Settings")]
    [SerializeField] 
    private Reward baseReward;

    [SerializeField]
    private int _NumberOfToggle;

    [SerializeField]
    private int _NumberOfRewardToChoose;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [SerializeField]
    private GameObject _ToggleButton;

    [SerializeField]
    private GameObject _ButtonValidate;

    [SerializeField]
    private Transform _Panel;

    [SerializeField]
    private TextMeshProUGUI _Text;

    public EnemySO _EnemySO;

    public void Initialize()
    {
        baseReward.items = new List<ScriptableObject>();

    }

    public void AddToggle()
    {

        for (int i = 0; i < _NumberOfToggle; i++)
        {
            var indexAppendReward = Random.Range(0, _EnemySO.Items.Length);
            GameObject toggle = Instantiate(_ToggleButton, _Panel);
            ToggleAssignation toggleAssignation = toggle.GetComponent<ToggleAssignation>();

            // _enemySo récup items et les assignes au toggle
            var valueItemReward = _EnemySO.Items[indexAppendReward];
            baseReward.items.Add(valueItemReward);
            toggleAssignation.Initialized(valueItemReward);
            print(baseReward.items[i].name);
        }
        _Text.text = $"Choisissez {_NumberOfRewardToChoose} récompenses ";
    }

    public void ValidateChoice()
    {
        _Panel.gameObject.SetActive(false);
        for(int i = 0; i < _Panel.childCount; i++)
        {
            string Name = "Item(Clone)";
            if(Name == _Panel.GetChild(i).name)
            {
                print("test");
                Destroy(_Panel.GetChild(i).gameObject);
            }
            baseReward.items.Clear();
        }
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
        reward.gold = _EnemySO.GoldReward;
        return reward;
    }
}