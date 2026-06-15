using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau Character", menuName = "Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    public Sprite characterSprite;
    public string characterName;
    public string characterDescription;
    public int price;
    public ObjetSO[] startDeck;
}
