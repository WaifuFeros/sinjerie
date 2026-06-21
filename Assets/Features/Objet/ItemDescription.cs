using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDescription : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private IItemObject _itemObject;
    private Coroutine _longPressCoroutine;

    private void Awake()
    {
        _itemObject = GetComponent<IItemObject>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _longPressCoroutine = StartCoroutine(WaitAndShowDescription());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopLongPress();
    }

    private IEnumerator WaitAndShowDescription()
    {
        yield return new WaitForSeconds(DescriptionManager.Instance.DescriptionPressTime);

        DescriptionManager.Instance.DisplayDescription(_itemObject.ItemData);
    }
    public void StopLongPress()
    {
        if (_longPressCoroutine != null)
            StopCoroutine(_longPressCoroutine);
    }
}
