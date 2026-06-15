using UnityEngine;

[CreateAssetMenu(fileName = "SelectedCharacter", menuName = "Storage")]
public class SelectedCharacterData : ScriptableObject
{
    public CharacterSO selectedCharacter;
}