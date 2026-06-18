using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        return item.IsBoss;
    }
}
