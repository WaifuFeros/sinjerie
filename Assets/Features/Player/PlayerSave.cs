using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSave
{
    public int currentBanana;
    public int maxBanana;

    public StatUpgradeSave[] statUpgrades;

    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }
}
