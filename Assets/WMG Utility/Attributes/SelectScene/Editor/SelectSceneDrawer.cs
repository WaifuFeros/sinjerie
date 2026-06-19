using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// \class
/// \author capot_r
/// <summary>
/// Custom drawer to display the scenes list in popup form.
/// </summary>
[CustomPropertyDrawer(typeof(SelectSceneAttribute))]
public class SelectSceneDrawer : PropertyDrawer
{
    /// \property
    /// <summary>
    /// The custom scenes attriute.
    /// </summary>
    SelectSceneAttribute SelectScene { get { return (SelectSceneAttribute)attribute; } }

    /// <summary>
    /// Manage the GUI part for the display in inspector.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] sceneNames = GetEnabledSceneNames();

        if (sceneNames.Length == 0)
        {
            EditorGUI.LabelField(position, ObjectNames.NicifyVariableName(property.name), "Scene is Empty");
            return;
        }

        int[] sceneNumbers = new int[sceneNames.Length];

        SetSceneNumbers(sceneNumbers, sceneNames);

        if (!string.IsNullOrEmpty(property.stringValue))
            SelectScene.SelectedValue = GetIndex(sceneNames, property.stringValue);

        SelectScene.SelectedValue = EditorGUI.IntPopup(position, label.text, SelectScene.SelectedValue, sceneNames, sceneNumbers);
        property.stringValue = sceneNames[SelectScene.SelectedValue];
    }

    /// <summary>
    /// Get all scenes names.
    /// </summary>
    /// <returns></returns>
    string[] GetEnabledSceneNames()
    {
        List<EditorBuildSettingsScene> scenes = (SelectScene.EnableOnly ? EditorBuildSettings.scenes.Where(scene => scene.enabled) : EditorBuildSettings.scenes).ToList();
        HashSet<string> sceneNames = new HashSet<string>();
        scenes.ForEach(scene =>
        {
            sceneNames.Add(scene.path.Substring(scene.path.LastIndexOf("/") + 1).Replace(".unity", string.Empty));
        });
        return sceneNames.ToArray();
    }

    /// <summary>
    /// Assign the scenes number
    /// </summary>
    /// <param name="sceneNumbers"></param>
    /// <param name="sceneNames"></param>
    void SetSceneNumbers(int[] sceneNumbers, string[] sceneNames)
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            sceneNumbers[i] = i;
        }
    }

    /// <summary>
    /// Get the index scene
    /// </summary>
    /// <param name="sceneNames"></param>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    int GetIndex(string[] sceneNames, string sceneName)
    {
        int result = 0;
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneName == sceneNames[i])
            {
                result = i;
                break;
            }
        }
        return result;
    }
}