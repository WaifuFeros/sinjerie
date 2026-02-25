using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class ItemBrain : GameDraggableObjectController, IPointerDownHandler, IPointerUpHandler
{
    [Header("Data")]
    [SerializeField] public ObjetSO itemData;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    [Header("Smoke Effect")]
    [SerializeField] private Animator smokeAnimator;
    [SerializeField] private float delayBeforeChange = 0.021f;

    [Header("Asset Rarity")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Coroutine longPressCoroutine;
    private Coroutine updateCoroutine;
    private bool isDragging;

    void Start()
    {
        name = $"Item: {itemData.objetName}";
        isDragging = false;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        descritptionPanel.SetActive(false);
    }

    public void TriggerVisualUpdate()
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(VisualUpdateRoutine());
    }

    private IEnumerator VisualUpdateRoutine()
    {
        if (smokeAnimator != null)
        {
            float delay = Random.Range(0.01f, 0.2F);
            yield return new WaitForSeconds(delay);
            smokeAnimator.SetTrigger("StartSmoke");
        }
        yield return new WaitForSeconds(delayBeforeChange);

        ApplyVisuals();
    }

    public void ApplyVisuals()
    {
        if (itemData == null) return;

        itemIcon.sprite = itemData.objetSprite;
        descriptionText.text = itemData.objetDescription;
        if (itemData.objetRarity == ObjetRarity.Common) itemBackground.sprite = _communSprite;
        else if (itemData.objetRarity == ObjetRarity.Uncommon) itemBackground.sprite = _uncommonSprite;
        else if (itemData.objetRarity == ObjetRarity.Rare) itemBackground.sprite = _rareSprite;
        else if (itemData.objetRarity == ObjetRarity.Epic) itemBackground.sprite = _epicSprite;
        else if (itemData.objetRarity == ObjetRarity.Legendary) itemBackground.sprite = _lengendarySprite;

        effectImage.SetActive(true);

        if (itemData.objectType == ObjetEffectType.Heal)
            effectImage.GetComponent<Image>().color = Color.red;
        else if (itemData.objectType == ObjetEffectType.Attack)
            effectImage.GetComponent<Image>().color = Color.yellow;
        else if (itemData.objectType == ObjetEffectType.Special)
            effectImage.SetActive(false);

        effectText.text = itemData.objectEffect.ToString();
        weightText.text = itemData.objetWeight.ToString();
    }

    public void InitItem(ObjetSO data)
    {

        if (itemData != null) Destroy(itemData);

        itemData = Instantiate(data);
        TriggerVisualUpdate();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        longPressCoroutine = StartCoroutine(WaitAndShowDescription());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopLongPress();
    }
    private IEnumerator WaitAndShowDescription()
    {
        yield return new WaitForSeconds(1.5f);
        if (!isDragging) descritptionPanel.SetActive(true);
    }
    private void StopLongPress()
    {
        if (longPressCoroutine != null) StopCoroutine(longPressCoroutine);
        descritptionPanel.SetActive(false);
    }

    public override void BeginDrag(Vector3 mousePosition)
    {
        isDragging = true;
        base.BeginDrag(mousePosition);
    }

    public override void EndDrag()
    {
        isDragging = false;
        base.EndDrag();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (ItemManager.Instance != null)
            ItemManager.Instance.activeItems.Remove(this);

        if (itemData != null) Destroy(itemData);
    }
}