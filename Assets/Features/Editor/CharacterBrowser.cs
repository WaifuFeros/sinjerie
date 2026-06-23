using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterBrowser : GenericScriptableObjectBrowser<CharacterSO>
{
    [MenuItem("Tools/Character Browser")]
    public static void Open()
    {
        GetWindow<CharacterBrowser>();
    }

    protected override bool EnableGrouping => true;
}
