using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterSO))]
public class BuildDataEditor : Editor
{
    SerializedProperty itemsProperty;

    private void OnEnable()
    {
        itemsProperty = serializedObject.FindProperty("items");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        // Affichage classique
        //EditorGUILayout.PropertyField(itemsProperty, true);

        EditorGUILayout.Space(10);
        DrawStats();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStats()
    {
        CharacterSO character = (CharacterSO)target;

        int totalDamage = 0;
        int totalHeal = 0;
        int totalStamina = 0;

        foreach (var item in character.startDeck)
        {
            if (item == null)
                continue;

            if (item.objectType == ObjetEffectType.Attack)
                totalDamage += item.objectEffect;
            else if (item.objectType == ObjetEffectType.Heal)
                totalHeal += item.objectEffect;
            totalStamina += item.objetWeight;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Statistiques", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Total Damage", totalDamage.ToString());
        EditorGUILayout.LabelField("Total Heal", totalHeal.ToString());
        EditorGUILayout.LabelField("Total Stamina", totalStamina.ToString());

        EditorGUILayout.Space();

        foreach (ObjetMaterialType type in Enum.GetValues(typeof(ObjetMaterialType)))
        {
            int count = character.startDeck.Count(i =>
                i != null &&
                i.objetMaterialType == type);

            if (type != ObjetMaterialType.None && count > 0)
                EditorGUILayout.LabelField( $"{type}", count.ToString());
        }
    }
}