using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [Header("Panels")]
    [SerializeField] 
    private GameObject roomPanel;
 
    [SerializeField] 
    private GameObject combatPanel;

    [SerializeField] 
    private GameObject victoryPanel;

    [SerializeField] 
    private GameObject rewardPanel;

    [SerializeField] 
    private GameObject defeatPanel;


    [Header("Combat UI")]
    [SerializeField] 
    private TextMeshProUGUI roomCounterText;

    [SerializeField] 
    private TextMeshProUGUI combatStatusText;

    private CombatSystem combatSystem;
    private RewardSystem rewardSystem;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        combatSystem = CombatSystem.Instance;
        rewardSystem = RewardSystem.Instance;
        // Masquer tous les panels au démarrage
        HideAllPanels();
    }

    public void ShowRoomUI()
    {
        HideAllPanels();
        if (roomPanel != null)
            roomPanel.SetActive(true);
    }

    public void ShowCombatUI()
    {
        HideAllPanels();
        if (combatPanel != null)
            combatPanel.SetActive(true);

        // Mettre à jour le message de statut (adapté pour mobile)
        if (combatStatusText != null)
        {
            combatStatusText.text = "Touchez un bouton pour attaquer!";
        }
    }

    public void ShowVictoryPanel()
    {
        HideAllPanels();
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void ShowDefeatPanel()
    {
        HideAllPanels();
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
    }

    public void ShowRewardPanel()
    {
        HideAllPanels();
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
            rewardSystem.AddToggle();

        }
    }

    private void HideAllPanels()
    {
        if (roomPanel != null) roomPanel.SetActive(false);
        if (combatPanel != null) combatPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    public void UpdateRoomCounter(int roomNumber)
    {
        if (roomCounterText != null)
        {
            roomCounterText.text = $"Salle #{roomNumber}";
        }
    }

    /// <summary>
    /// Met à jour le message de statut du combat
    /// </summary>
    public void UpdateCombatStatus(string message)
    {
        if (combatStatusText != null)
        {
            combatStatusText.text = message;
        }
    }
}