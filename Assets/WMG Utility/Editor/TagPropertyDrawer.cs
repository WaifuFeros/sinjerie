using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Tag))]
public class TagPropertyDrawer : PropertyDrawer
{
    bool test;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var tag = property.FindPropertyRelative("stringTag");
        test = EditorExtentionMethods.ToggleRight(position, test, out Rect rect);
        if (test)
        {
            if (string.IsNullOrWhiteSpace(tag.stringValue))
                tag.stringValue = "Untagged";
            tag.stringValue = EditorGUI.TagField(rect, label, tag.stringValue);
        }
        else
        {
            bool enabled = GUI.enabled;
            GUI.enabled = false;

            tag.stringValue = EditorGUI.TagField(rect, label, string.Empty);

            GUI.enabled = enabled;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
