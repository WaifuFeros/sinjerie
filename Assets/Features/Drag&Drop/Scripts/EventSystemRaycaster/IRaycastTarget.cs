using UnityEngine;

/// <summary>
/// Interface to handle the raycast target method of EventSystem
/// </summary>
public interface IRaycastTarget
{
    /// <summary>
    /// is the collider enabled for raycasting ?
    /// </summary>
    bool isEnabled { get; set; }
}
