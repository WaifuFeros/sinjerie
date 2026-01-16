using UnityEngine;

public abstract class GameInteractableObjectController : MonoBehaviour
{
    /// <summary>
    /// Getter for isInteractable
    /// </summary>
    public bool IsInteractable => _isInteractable;

    /// <summary>
    /// Is the object interactable
    /// </summary>
    [SerializeField]
    protected bool _isInteractable;

    /// <summary>
    /// The collider for the drag interraction
    /// </summary>
    protected IRaycastTarget _raycastTarget;

    protected virtual void Awake()
    {
        _raycastTarget = GetComponent<IRaycastTarget>();

        this.updateColliderState();
    }

    protected virtual void OnEnable()
    {
        this.updateColliderState();
    }

    protected virtual void OnDisable()
    {
        this.updateColliderState();
    }

    /// <summary>
    /// Set the gameobject as draggable or not
    /// </summary>
    /// <param name="isInteractable"></param>
    public virtual void SetIsInteractable(bool isInteractable)
    {
        this._isInteractable = isInteractable;

        this.updateColliderState();
    }

    /// <summary>
    /// Update the enabled state of the collider
    /// </summary>
    protected virtual void updateColliderState()
    {
        this._raycastTarget.isEnabled = this.isActiveAndEnabled && this._isInteractable;
    }

    protected virtual void Reset()
    {
        _isInteractable = true;
    }
}