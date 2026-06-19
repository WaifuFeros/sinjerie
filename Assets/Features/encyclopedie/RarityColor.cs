using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RarityColor
{
    public static Color GetColor(ObjetRarity rarity)
    {
        switch (rarity)
        {
            case ObjetRarity.Common:
                return Color.white;

            case ObjetRarity.Uncommon:
                return new Color(0.3f, 1f, 0.3f); 

            case ObjetRarity.Rare:
                return new Color(0.3f, 0.5f, 1f); 

            case ObjetRarity.Epic:
                return new Color(0.7f, 0.3f, 1f); 

            case ObjetRarity.Legendary:
                return new Color(1f, 0.6f, 0f); 

            default:
                return Color.white;
        }
    }
}

