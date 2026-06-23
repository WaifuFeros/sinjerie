using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyBrowser : GenericScriptableObjectBrowser<EnemySO>
{
    [MenuItem("Tools/Enemies Browser")]
    public static void Open()
    {
        GetWindow<EnemyBrowser>();
    }

    protected override bool EnableGrouping => true;

    protected override IComparable GetGroupKey(EnemySO item)
    {
        if (item.name.ToLower().Contains("debug"))
        {
            return "3.Debug";
        }
        else
        {
            if (item.IsBoss)
                return "2.Boss";
            else
                return "1.Ennemis";
        }
    }
}
