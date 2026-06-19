using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemBrowser : GenericScriptableObjectBrowser<ObjetSO>
{
    [MenuItem("Tools/Items Browser")]
    public static void Open()
    {
        GetWindow<ItemBrowser>();
    }

    protected override bool EnableGrouping => true;

    protected override IComparable GetGroupKey(ObjetSO item)
    {
        return item.objectType;
    }

    protected override IComparable GetSortKey(ObjetSO item)
    {
        return item.Rarity;
    }
}
