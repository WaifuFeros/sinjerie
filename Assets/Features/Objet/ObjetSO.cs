using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ObjetMatiralType
{
    Metal,
    Wood,
    Plastic,
    Glass,
    Stone,
    Organic,
    fire
}
public enum ObjetEffectType
{
    Heal,
    Attack,
    Special
}

public enum ObjetRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NouvelObjet", menuName = "Objets/Nouvel Objet", order = 1)]
public class ObjetSO : ScriptableObject
{
    public Sprite objetSprite;
    public string objetName;
    public string objetDescription;
    public ObjetMatiralType objetMatiralType;
    public int objetWeight;
    public ObjetEffectType objectType;
    public int objectEffect;
    public SpecialActionSO specialAction;
    public ObjetRarity objetRarity;
}