using UnityEngine;
using UnityEngine.EventSystems;

public class GameAttackDropZone : GameDroppableZoneController<ItemBrain>
{
    public string TestMessage;

    protected override void OnDropObject(ItemBrain draggable, PointerEventData eventData, bool isDropValid)
    {
        base.OnDropObject(draggable, eventData, isDropValid);

        Destroy(draggable.gameObject);
        Debug.Log(TestMessage);
    }

    protected override bool verifyDraggable(GameDraggableObjectController draggable)
    {
        return draggable is ItemBrain;
    }
}
