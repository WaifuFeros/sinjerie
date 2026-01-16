using UnityEngine;

public class Collider2DRaycastTarget : MonoBehaviour, IRaycastTarget
{
    /// <summary>
    /// is the collider enabled for raycasting ?
    /// </summary>
    public bool isEnabled { get => this._collider2D.enabled; set => this._collider2D.enabled = value; }

    /// <summary>
    /// the collider 2D
    /// </summary>
    [SerializeField]
    private Collider2D _collider2D;
}
