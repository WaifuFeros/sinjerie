using UnityEngine;
using UnityEngine.EventSystems;

public class GameAttackDropZone : GameDroppableZoneController<DraggableItem>
{
    public string TestMessage;

    protected override void OnDropObject(DraggableItem draggable, PointerEventData eventData, bool isDropValid)
    {
        base.OnDropObject(draggable, eventData, isDropValid);

        Destroy(draggable.gameObject);
        Debug.Log(TestMessage);
    }

    protected override bool verifyDraggable(GameDraggableObjectController draggable)
    {
        return draggable is DraggableItem;
    }
}
