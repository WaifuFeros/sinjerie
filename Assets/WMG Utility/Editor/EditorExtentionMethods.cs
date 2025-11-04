using UnityEditor;
using UnityEngine;

public static class EditorExtentionMethods
{
    public static float singleLineWithSpaceHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    public static readonly int toggleRightSize = 18;

    public static readonly int toggleRightStandardSpace = 2;

    public static bool PropertyFieldWithCheck(Rect position, SerializedProperty property, GUIContent label, bool checkIn, out bool checkOut, GUIStyle checkStyle, bool includeChildren = false, int space = 2)
    {
        Rect fieldRect = new Rect(position);
        fieldRect.width -= toggleRightSize + space;

        bool returnValue = EditorGUI.PropertyField(fieldRect, property, label, includeChildren);

        Rect toggleRect = new Rect(position.position, Vector2.one * toggleRightSize);
        toggleRect.x = position.width + position.x - toggleRightSize;

        int indentation = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        checkOut = EditorGUI.Toggle(toggleRect, checkIn, checkStyle);
        EditorGUI.indentLevel = indentation;

        return returnValue;
    }

    public static bool PropertyFieldWithCheck(Rect position, SerializedProperty property, GUIContent label, bool checkIn, out bool checkOut, bool includeChildren = false, int space = 2)
    {
        return PropertyFieldWithCheck(position, property, label, checkIn, out checkOut, "Toggle", includeChildren, space);
    }

    public static bool ToggleRight(Rect position, bool value, out Rect rect)
    {
        Rect toggleRect = new Rect(position.position, Vector2.one * toggleRightSize);
        toggleRect.x = position.width + position.x - toggleRightSize;

        rect = new Rect(position);
        rect.width = toggleRect.x - position.x - toggleRightStandardSpace;

        int indentation = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        value = EditorGUI.Toggle(toggleRect, value, "Toggle");
        EditorGUI.indentLevel = indentation;

        return value;
    }

    public static bool ToggleRight(Rect position, bool value)
    {
        return ToggleRight(position, value, out Rect rect);
    }

    public static float GetSingleLinesOffset(int lines, int spacing, bool allowNegatives = false)
    {
        if (!allowNegatives)
        {
            if (lines < 0)
            {
                lines = 0;
                Debug.LogWarning($"{nameof(lines)} is lower than 0. Value has been clamped.");
            }
            if (spacing < 0)
            {
                spacing = 0;
                Debug.LogWarning($"{nameof(spacing)} is lower than 0. Value has been clamped.");
            } 
        }

        return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * spacing;
    }
}
