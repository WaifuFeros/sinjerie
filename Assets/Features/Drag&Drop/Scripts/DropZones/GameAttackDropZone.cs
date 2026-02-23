using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameAttackDropZone : GameDroppableZoneController<ItemBrain>
{
    public UnityEvent<ItemBrain> OnDropItem;

    public string TestMessage;

    protected override void OnDropObject(ItemBrain draggable, PointerEventData eventData, bool isDropValid)
    {
        Debug.Log(draggable.GetType());
        base.OnDropObject(draggable, eventData, isDropValid);

        OnDropItem.Invoke(draggable);
    }

    protected override bool verifyDraggable(GameDraggableObjectController draggable)
    {
        Debug.Log(draggable.GetType());
        return draggable is ItemBrain;
    }
}
