using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameDroppableSlotController : GameDroppableZoneController<GameDraggableObjectController>
{
    public GameDraggableObjectController CurrentDraggable { get; private set; }

    public bool IsEmpty => CurrentDraggable == null;

    /// <summary>
    /// The predetermined slots for cards
    /// </summary>
    [SerializeField]
    protected Transform _droppableSlot;

    protected override void onDropObject(GameDraggableObjectController draggable, PointerEventData eventData, bool isDropValid)
    {
        draggable.OnDrop(this, isDropValid);

        if (isDropValid)
        {
            var currentItem = RemoveDraggable();

            if (draggable.IsInSlot && !draggable.CurrentSlot.IsEmpty)
            {
                GameDroppableSlotController previousSlot = draggable.CurrentSlot;
                previousSlot.RemoveDraggable();
                previousSlot.SetItem(currentItem);
            }

            SetItem(draggable);
        }
    }

    public bool SetItem(GameDraggableObjectController draggable)
    {
        if (draggable == null)
            return false;

        CurrentDraggable = draggable;
        draggable.SetSlot(this);
        draggable.transform.position = _droppableSlot.position;

        return true;
    }

    public GameDraggableObjectController RemoveDraggable()
    {
        if (CurrentDraggable == null)
            return null;

        var draggable = CurrentDraggable;
        CurrentDraggable = null;
        return draggable;
    }
}
