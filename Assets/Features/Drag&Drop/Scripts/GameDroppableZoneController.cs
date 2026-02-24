using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GameDroppableZoneController<T> : GameInteractableObjectController, IDropHandler where T : GameDraggableObjectController
{
    /// <summary>
    /// The event for when player stop dragging and drop an object on this object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        GameDraggableObjectController draggable = GameDraggableObjectController.CurrentDraggedObject;

        if (draggable == null)
            return;

        bool isDropValid = verifyDraggable(draggable);
        onDropObject(draggable, eventData, isDropValid);
    }

    protected abstract void onDropObject(GameDraggableObjectController draggable, PointerEventData eventData, bool isDropValid);

    /// <summary>
    /// Verify if the draggable object is valid
    /// </summary>
    /// <param name="draggable"></param>
    /// <returns></returns>
    protected virtual bool verifyDraggable(GameDraggableObjectController draggable)
    {
        return draggable is T;
    }

    protected virtual bool verifyDraggable(GameDraggableObjectController draggable, out T castedDraggable)
    {
        bool isValid = draggable is T;
        castedDraggable = draggable as T;
        return isValid;
    }
}
