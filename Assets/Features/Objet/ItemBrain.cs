using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemBrain : GameDraggableObjectController, IItemObject
{
    public ObjetSO ItemData => itemData;

    [Header("Data")]
    [SerializeField] public ObjetSO itemData;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private Color invalidWeightTextColor = Color.red;
    [SerializeField] private Color invalidWeightImagesColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private ItemDescription description;
    [SerializeField] public ItemWiggleDOTween wiggle;

    [Header("Smoke Effect")]
    [SerializeField] private Animator smokeAnimator;
    [SerializeField] private float delayBeforeChange = 0.021f;

    private CanvasGroup canvasGroup;
    private Coroutine updateCoroutine;

    private Color _weightTextBaseColor;

    void Start()
    {
        name = $"Item: {itemData.objetName}";

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

        if (itemData.objectType == ObjetEffectType.Attack)
            itemData.objectEffect += MetaProgressionManager.Instance.GetValueByType(StatUpgradeType.Damage);
        else if (itemData.objectType == ObjetEffectType.Heal)
            itemData.objectEffect += MetaProgressionManager.Instance.GetValueByType(StatUpgradeType.Heal);

        TriggerVisualUpdate();
    }

    public override void BeginDrag(Vector3 mousePosition)
    {
        base.BeginDrag(mousePosition);
        description.StopLongPress();
    }

    public override void EndDrag()
    {
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