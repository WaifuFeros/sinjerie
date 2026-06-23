using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;



public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    [SerializeField]
    private VcaController gameplayMusicVCA;

    [Header("Reward Settings")]

    public int NumberOfToggle;

    public int NumberOfRewardToChoose;

    [Header("References")]

    [SerializeField]
    private GameObject _ToggleButton;

    [SerializeField]
    private GameObject _ButtonValidate;

    [SerializeField]
    private TextMeshProUGUI _Text;

    public EnemySO EnemySO;

    public Transform Panel;

    public List<ToggleAssignation> ToggleAssignations = new List<ToggleAssignation>();

    public List<ObjetSO> ItemRewards = new List<ObjetSO>();


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    public void Initialize(Action onLoadCompleted)
    {
        onLoadCompleted?.Invoke();
    }


    public void AddToggle()
    {

        for (int i = 0; i < NumberOfToggle; i++)
        {
            var indexAppendReward = UnityEngine.Random.Range(0, EnemySO.Items.Length);
            GameObject toggle = Instantiate(_ToggleButton, Panel);
            ToggleAssignation toggleAssignation = toggle.GetComponent<ToggleAssignation>();

            // _enemySo récup items et les assignes au toggle
            var valueItemReward = EnemySO.Items[indexAppendReward];
            toggleAssignation.Initialized(valueItemReward,this);
            ToggleAssignations.Add(toggleAssignation);
        }
        _Text.text = $"Choisissez {NumberOfRewardToChoose} récompenses ";
    }

    public void ValidateChoice()
    {
        TransitionManager.Instance.TransitionWithAction(() => {
            Panel.gameObject.SetActive(false);
            // Donne les items
            for (int i = 0; i < ItemRewards.Count; i++)
            {
                var deckList = new List<ObjetSO>(PlayerManager.Instance.stats.Deck);
                deckList.Add(ItemRewards[i]);
                PlayerManager.Instance.stats.Deck = deckList.ToArray();
            }
            // Donne les gold
            PlayerManager.Instance.AddGold(EnemySO.GoldReward);

            // Réinitialise le système de récompense pour la prochaine fois
            for (int i = 0; i < ToggleAssignations.Count; i++)
            {
                Destroy(ToggleAssignations[i].gameObject);
            }
            ItemRewards.Clear();
            ToggleAssignations.Clear();
            GameLoopManager.Instance.ExitRoom();
        });
    }
    

}