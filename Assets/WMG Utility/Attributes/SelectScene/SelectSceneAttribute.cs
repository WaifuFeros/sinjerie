using UnityEngine;

/// \class
/// \author capot_r
/// <summary>
/// Custom attribute used to list all scenes.
/// To be display, the scene must be active in the build setting.
/// </summary>
public class SelectSceneAttribute : PropertyAttribute
{
    /// \property
    /// <summary>
    /// Selected property.
    /// </summary>
    public int SelectedValue { get; set; }

    /// \property
    /// <summary>
    /// Define if we display only enabled scenes.
    /// </summary>
    public bool EnableOnly { get; set; }

    /// <summary>
    /// Initialize the list of scenes names.
    /// </summary>
    public SelectSceneAttribute(bool enableOnly = true)
    {
        SelectedValue = 0;
        EnableOnly = enableOnly;
    }
}