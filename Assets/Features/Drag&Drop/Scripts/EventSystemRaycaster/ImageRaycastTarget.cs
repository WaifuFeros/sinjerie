using UnityEngine;
using UnityEngine.UI;

public class ImageRaycastTarget : MonoBehaviour, IRaycastTarget
{
    /// <summary>
    /// is the collider enabled for raycasting ?
    /// </summary>
    public bool isEnabled
    {
        get
        {
            return this._useRaycastTargetVariable ? this._image.raycastTarget : this._image.enabled;
        }
        set
        {
            if (this._useRaycastTargetVariable)
                this._image.raycastTarget = value;
            else
                this._image.enabled = value;
        }
    }

    /// <summary>
    /// The UI Image
    /// </summary>
    [SerializeField]
    private Image _image;

    /// <summary>
    /// Does it uses the raycast target variable ?
    /// </summary>
    [SerializeField]
    private bool _useRaycastTargetVariable;
}
