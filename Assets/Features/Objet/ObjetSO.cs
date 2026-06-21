using UnityEngine;


public enum ObjetMaterialType
{
    None,
    Metal,
    Wood,
    Fire,
    Ice,
    PerfectIce,
    Electricity,
    Water,
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
    public ObjetMaterialType objetMaterialType;
    public int objetWeight;
    public ObjetEffectType objectType;
    public int objectEffect;
    public SpecialActionSO specialAction;
    public ObjetRarity Rarity;
}