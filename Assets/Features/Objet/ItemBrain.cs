using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class ItemBrain : GameDraggableObjectController, IPointerDownHandler, IPointerUpHandler
{
    [Header("Data")]
    [SerializeField] public ObjetSO itemData;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField, Min(0)] private float descriptionPressTime;
    [SerializeField] private Image effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private Color invalidWeightTextColor = Color.red;
    [SerializeField] private Color invalidWeightImagesColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Smoke Effect")]
    [SerializeField] private Animator smokeAnimator;
    [SerializeField] private float delayBeforeChange = 0.021f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Coroutine longPressCoroutine;
    private Coroutine updateCoroutine;
    private bool isDragging;

    private Color _weightTextBaseColor;

    void Start()
    {
        name = $"Item: {itemData.objetName}";
        isDragging = false;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        descritptionPanel.SetActive(false);

        _weightTextBaseColor = weightText.color;
        PlayerManager.Instance.OnStaminaUpdateEvent += UpdateWeightVisual;
    }

    private void UpdateWeightVisual()
    {
        weightText.color = PlayerManager.Instance.stats.currentStamina >= itemData.objetWeight ? _weightTextBaseColor : invalidWeightTextColor;
        itemIcon.color = PlayerManager.Instance.stats.currentStamina >= itemData.objetWeight ? Color.white : invalidWeightImagesColor;
        itemBackground.color = PlayerManager.Instance.stats.currentStamina >= itemData.objetWeight ? Color.white : invalidWeightImagesColor;
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
        itemBackground.sprite = ItemManager.Instance.GetRaritySprite(itemData.Rarity);

        effectImage.gameObject.SetActive(ItemManager.Instance.GetObjetTypeSprite(itemData.objectType, out Sprite result));
        effectImage.sprite = result;

        effectText.text = itemData.objectEffect.ToString();
        weightText.text = itemData.objetWeight.ToString();

        UpdateWeightVisual();
    }

    public void InitItem(ObjetSO data)
    {

        if (itemData != null) Destroy(itemData);

        itemData = Instantiate(data);
        TriggerVisualUpdate();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        StaminaUIManager.Instance.DisplayStaminaPreview(PlayerManager.Instance.stats.currentStamina, itemData.objetWeight);
        longPressCoroutine = StartCoroutine(WaitAndShowDescription());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StaminaUIManager.Instance.HideStaminaPreview();
        StopLongPress();
    }
    private IEnumerator WaitAndShowDescription()
    {
        yield return new WaitForSeconds(descriptionPressTime);
        //if (!isDragging) descritptionPanel.SetActive(true);
        DescriptionManager.Instance.DisplayDescription(itemData);
    }
    private void StopLongPress()
    {
        if (longPressCoroutine != null) StopCoroutine(longPressCoroutine);
        //descritptionPanel.SetActive(false);
    }

    public override void BeginDrag(Vector3 mousePosition)
    {
        isDragging = true;
        base.BeginDrag(mousePosition);
        StopLongPress();
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

        PlayerManager.Instance.OnStaminaUpdateEvent -= UpdateWeightVisual;

        if (itemData != null) Destroy(itemData);
    }
}