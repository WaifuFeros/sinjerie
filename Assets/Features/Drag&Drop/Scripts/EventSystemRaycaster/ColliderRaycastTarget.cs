using UnityEngine;

public class ColliderRaycastTarget : MonoBehaviour, IRaycastTarget
{
    /// <summary>
    /// is the collider enabled for raycasting ?
    /// </summary>
    public bool isEnabled { get => this._collider.enabled; set => this._collider.enabled = value; }

    /// <summary>
    /// the collider
    /// </summary>
    [SerializeField]
    private Collider _collider;
}
