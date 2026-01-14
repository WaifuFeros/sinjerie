using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class ItemBrain : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public ObjetSO itemData;                  // Public provi --> creer des fonctions pour stocker les données
    [SerializeField] private GameObject descritptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject effectImage; 
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI weightText;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Coroutine longPressCoroutine;
    private bool isDragging;

    void Start()
    {

        isDragging = false;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

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

        descritptionPanel.SetActive(false);

    }





    // Fonction appelée par le Manager juste après le Instantiate
    public void InitItem(ObjetSO data)
    {
        itemData = Instantiate(data);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
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

    private void OnDestroy()
    {
        if (ItemManager.Instance != null)
            ItemManager.Instance.activeItems.Remove(gameObject);
        Destroy(itemData);
    }





    // Interactions UI

    public void OnPointerDown(PointerEventData eventData)
    {
        longPressCoroutine = StartCoroutine(WaitAndShowDescription());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopLongPress();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    private IEnumerator WaitAndShowDescription()
    {
        yield return new WaitForSeconds(1.5f);
        descritptionPanel.SetActive(true);
    }

    private void StopLongPress()
    {
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
        }
        descritptionPanel.SetActive(false);
        isDragging = false;
    }
}