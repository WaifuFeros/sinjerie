using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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

    [Header("Banana Reward")]
    [SerializeField] GameObject bananaPanel;
    [SerializeField] GameObject bananaBackground;
    [SerializeField] GameObject banana;

    [Header("Combat UI")]
    [SerializeField] 
    private TextMeshProUGUI roomCounterText;

    [SerializeField] 
    private TextMeshProUGUI combatStatusText;

    private CombatSystem combatSystem;
    private RewardSystem rewardSystem;

    [Header("Player")]
    [SerializeField] private TextMeshProUGUI goldUI;

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
        TransitionManager.Instance.TransitionWithAction(() =>
        {
            HideAllPanels();
            if (rewardPanel != null)
            {
                rewardPanel.SetActive(true);
                rewardSystem.AddToggle();

            }
        });
        
    }
    public void ShowBananaRewardPanel()
    {
        bananaPanel.SetActive(true);
        Button bgButton = bananaBackground.GetComponent<Button>();
        bgButton.interactable = true;
        Image panelImage = bananaPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            Color c = panelImage.color;
            c.a = 0f;
            panelImage.color = c;

            panelImage.DOFade(0.5f, 0.4f);
        }

        bananaBackground.transform.DOKill();
        bananaBackground.transform.localScale = Vector3.zero;

        bananaBackground.transform.DORotate(new Vector3(0, 0, 360), 5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        bananaBackground.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                bananaBackground.transform.DOScale(Vector3.one * 1.06f, 1.8f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
        
        banana.transform.DOKill();
        banana.transform.localScale = Vector3.zero;

        banana.transform.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                banana.transform.DOScale(Vector3.one * 1.1f, 1.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }
    public void OnBananaBackgroundClicked()
    {
        Image panelImage = bananaPanel.GetComponent<Image>();

        bananaBackground.transform.DOKill();
        banana.transform.DOKill();
        if (panelImage != null) panelImage.DOKill();

        Button bgButton = bananaBackground.GetComponent<Button>();
        if (bgButton != null) bgButton.interactable = false;
        Sequence closeSequence = DOTween.Sequence();

        closeSequence.Join(bananaBackground.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        closeSequence.Join(banana.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        if (panelImage != null)
        {
            closeSequence.Insert(0.15f, panelImage.DOFade(0f, 0.25f));
        }
        closeSequence.OnComplete(() =>
        {
            bananaPanel.SetActive(false);
            // donne la banane dans la save
            ShowRewardPanel();
        });

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

    public void UpdateGoldUI(int goldAmount)
    {
        if (goldUI != null)
        {
            goldUI.text = goldAmount.ToString();
        }
    }
}