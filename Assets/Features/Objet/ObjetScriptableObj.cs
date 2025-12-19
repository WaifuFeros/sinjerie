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
    Organic
}

[CreateAssetMenu(fileName = "NouvelObjet", menuName = "Objets/Nouvel Objet", order = 1)]
public class ObjetScriptableObj : ScriptableObject
{
    public Sprite objetSprite;
    public string objetName;
    public string objetDescription;
    public ObjetMatiralType objetMatiralType;
    public float objetWeight;
    public float objectDamage;
    public float objectHeal;
}