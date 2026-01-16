using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;


public class ItemBrain : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] public ObjetSO itemData;
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject effectImage;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Coroutine longPressCoroutine;
    private bool isDragging;

    void Start()
    {
        isDragging = false;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        UpdateVisuals();
        descritptionPanel.SetActive(false);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
    }


    public void InitItem(ObjetSO data)
    {
        itemData = Instantiate(data);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (itemData == null) return;

        GetComponent<Image>().sprite = itemData.objetSprite;
        descriptionText.text = itemData.objetDescription;

        if (itemData.objectType == ObjetEffectType.Heal)
            effectImage.GetComponent<Image>().color = Color.red;
        else if (itemData.objectType == ObjetEffectType.Attack)
            effectImage.GetComponent<Image>().color = Color.yellow;
        else if (itemData.objectType == ObjetEffectType.Special)
            effectImage.SetActive(false);

        effectText.text = itemData.objectEffect.ToString();
        weightText.text = itemData.objetWeight.ToString();
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
        descritptionPanel.SetActive(true);
    }

    private void StopLongPress()
    {
        if (longPressCoroutine != null) StopCoroutine(longPressCoroutine);
        descritptionPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (ItemManager.Instance != null)
            ItemManager.Instance.activeItems.Remove(gameObject);
        Destroy(itemData);
    }
}