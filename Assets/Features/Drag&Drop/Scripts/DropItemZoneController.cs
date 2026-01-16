using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemZoneController : GameDroppableZoneController<DraggableItem>
{
    [SerializeField]
    private DraggableItem startingItem;

    //protected override void Awake()
    //{
    //    base.Awake();

    //    if (startingItem == null)
    //        return;

    //    tryAddDraggable(startingItem);
    //    startingItem.OnDropCallback += RemoveItem;
    //}
}
