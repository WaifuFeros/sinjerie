using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GameDroppableZoneController<T> : GameInteractableObjectController, IDropHandler where T : GameDraggableObjectController
{
    /// <summary>
    /// The predetermined slots for cards
    /// </summary>
    [SerializeField]
    protected Transform[] _droppablesSlot;

    [SerializeField]
    protected bool _setDraggablePositionOnDropValid;

    [SerializeField]
    protected bool _transferOwnershipOnDrop;

    protected Dictionary<T, Transform> _currentDraggables;

    protected Stack<Transform> _remainingSlots;

    protected override void Awake()
    {
        base.Awake();
        this._currentDraggables = new Dictionary<T, Transform>();
        this._remainingSlots = new Stack<Transform>();

        for (int i = _droppablesSlot.Length - 1; i >= 0; i--)
        {
            _remainingSlots.Push(_droppablesSlot[i]);
        }
    }

    /// <summary>
    /// The event for when player stop dragging and drop an object on this object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        GameDraggableObjectController draggable = GameDraggableObjectController.CurrentDraggedObject;

        bool isDropValid = verifyDraggable(draggable);

        OnDropObject((T)draggable, eventData, isDropValid);
    }

    protected virtual void OnDropObject(T draggable, PointerEventData eventData, bool isDropValid)
    {
        if (isDropValid && _setDraggablePositionOnDropValid)
        {
            draggable.transform.position = _currentDraggables[draggable].position;
        }

        draggable.OnDrop(this, isDropValid, _transferOwnershipOnDrop);

        draggable.OnDropCallback += RemoveItem;
    }

    private void RemoveItem(GameDraggableObjectController draggable, bool transferOwnership)
    {
        draggable.OnDropCallback -= RemoveItem;

        if (transferOwnership)
            RemoveDraggable((T)draggable);
    }

    /// <summary>
    /// Verify if the draggable object is valid
    /// </summary>
    /// <param name="draggable"></param>
    /// <returns></returns>
    protected virtual bool verifyDraggable(GameDraggableObjectController draggable)
    {
        if (draggable is T castedDraggable)
        {
            return this.tryAddDraggable(castedDraggable);
        }

        return false;
    }

    /// <summary>
    /// Try to add a draggable objects
    /// </summary>
    /// <param name="draggable"></param>
    /// <returns></returns>
    protected bool tryAddDraggable(T draggable)
    {
        if (isStackableObject(draggable))
        {
            StackObject(draggable);
            return true;
        }
        else if (this._currentDraggables.Count < this._droppablesSlot.Length && !this._currentDraggables.ContainsKey(draggable))
        {
            this._currentDraggables.Add(draggable, _remainingSlots.Pop());

            return true;
        }

        return false;
    }

    public void RemoveDraggable(T draggable)
    {
        _remainingSlots.Push(this._currentDraggables[draggable]);
        this._currentDraggables.Remove(draggable);
    }

    protected virtual bool isStackableObject(T draggable)
    {
        return false;
    }

    protected virtual void StackObject(T draggable)
    {
        Destroy(draggable);
    }
}
