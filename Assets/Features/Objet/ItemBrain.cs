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
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    [Header("Smoke Effect")]
    [SerializeField] private Animator smokeAnimator;
    [SerializeField] private float delayBeforeChange = 0.25f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Coroutine longPressCoroutine;
    private Coroutine updateCoroutine;
    private bool isDragging;

    void Start()
    {
        isDragging = false;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        ApplyVisuals();
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

        GetComponent<Image>().sprite = itemData.objetSprite;
        descriptionText.text = itemData.objetDescription;

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
        ApplyVisuals();
    }



    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    isDragging = true;
    //    canvasGroup.blocksRaycasts = false;
    //    transform.SetAsLastSibling();
    //}
    //public void OnDrag(PointerEventData eventData)
    //{
    //    rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    //}
    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    isDragging = false;
    //    canvasGroup.blocksRaycasts = true;
    //}
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

    private void OnDestroy()
    {
        if (ItemManager.Instance != null)
            ItemManager.Instance.activeItems.Remove(this);

        if (itemData != null) Destroy(itemData);
    }
}