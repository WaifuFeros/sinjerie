using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBrain : MonoBehaviour, IDragHandler
{
    [SerializeField] private ObjetScriptableObj itemData;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        GetComponent<Image>().sprite = itemData.objetSprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}