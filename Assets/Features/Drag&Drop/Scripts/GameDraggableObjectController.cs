using UnityEngine;
using UnityEngine.EventSystems;

public class GameDraggableObjectController : GameInteractableObjectController, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// The current gameobject being dragged
    /// </summary>
    public static GameDraggableObjectController CurrentDraggedObject { get; set; }

    [SerializeField]
    protected bool _resetPositionOnEndDrag;

    /// <summary>
    /// The drag starting position of the gameobject
    /// </summary>
    protected Vector3 _startingPosition;

    /// <summary>
    /// The offset between the mouse in world position and the gameobject origin
    /// </summary>
    protected Vector3 _dragOffset;

    /// <summary>
    /// Is this gameobject being dragged
    /// </summary>
    protected bool _isDragging;

    /// <summary>
    /// Did the player drop this gameobject on a valid target
    /// </summary>
    protected bool _isDropSuccess;

    protected float _startingScale;

    protected Transform _parent;

    protected override void Awake()
    {
        base.Awake();

        this._startingPosition = transform.position;
    }

    #region Drag Interfaces

    /// <summary>
    /// The event for when the drag begins
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.BeginDrag(eventData.position);
    }

    /// <summary>
    /// Update when the gameobject is dragged
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        SetDragPosition(eventData.position);
    }

    /// <summary>
    /// The event for when the drag ends
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
    }
        
    #endregion

    /// <summary>
    /// Begin the drag interaction
    /// </summary>
    /// <param name="mousePosition"></param>
    public virtual void BeginDrag(Vector3 mousePosition)
    {
        if (!IsInteractable || !isActiveAndEnabled)
            return;

        this._isDragging = true;
        this._isDropSuccess = false;
        this._raycastTarget.isEnabled = false;

        if (DraggableManager.IsValid())
        {
            _parent = transform.parent;
            transform.SetParent(DraggableManager.Instance.DraggingParent);
        }

        this._startingPosition = transform.position;

        CurrentDraggedObject = this;
    }

    /// <summary>
    /// Set the position of the gameobject based on mouse position and an offset
    /// </summary>
    /// <param name="mousePosition"></param>
    public virtual void SetDragPosition(Vector3 mousePosition)
    {
        if (!IsInteractable || !isActiveAndEnabled)
            return;

        transform.position = mousePosition + _dragOffset;
    }

    /// <summary>
    /// End the drag interaction
    /// </summary>
    public virtual void EndDrag()
    {
        this._isDragging = false;
        this._raycastTarget.isEnabled = true;

        if (_parent != null)
            transform.SetParent(_parent);

        if (!this._isDropSuccess && _resetPositionOnEndDrag)
            this.SendBackToStartingPosition();

        CurrentDraggedObject = null;
        this.updateColliderState();
    }

    /// <summary>
    /// Set the drop interaction as successful or not
    /// </summary>
    /// <param name="success"></param>
    public virtual void OnDrop<T>(GameDroppableZoneController<T> droppableZone, bool success) where T : GameDraggableObjectController
    {
        this._isDropSuccess = success;
    }

    /// <summary>
    /// Set the position of the gameobject as before the drag interaction
    /// </summary>
    public void SendBackToStartingPosition()
    {
        this.transform.position = this._startingPosition;
    }

    /// <summary>
    /// Update the enabled state of the collider
    /// </summary>
    protected override void updateColliderState()
    {
        if (this._isDragging)
            return;

        base.updateColliderState();
    }

    protected override void Reset()
    {
        base.Reset();

        _resetPositionOnEndDrag = true;
    }
}
