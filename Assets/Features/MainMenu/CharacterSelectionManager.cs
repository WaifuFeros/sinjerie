using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSO> characterList;
    [SerializeField] private GameObject characterPrefab;

    [Header("UI")]
    [SerializeField] private RectTransform characterSelection;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 10f;

    [Header("Visual Effects")]
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float fadeDistance = 400f;

    private List<CanvasGroup> spawnedCanvasGroups = new List<CanvasGroup>();
    private List<RectTransform> spawnedCharacters = new List<RectTransform>();

    private int currentCharacterIndex = 0;
    private Vector2 targetPosition;
    private float screenCenter;

    private void Start()
    {
        screenCenter = Screen.width / 2f;

        // 1. Instanciation des personnages
        foreach (CharacterSO character in characterList)
        {
            GameObject oneCharacter = Instantiate(characterPrefab, characterSelection);
            oneCharacter.GetComponent<Image>().sprite = character.characterSprite;

            spawnedCharacters.Add(oneCharacter.GetComponent<RectTransform>());

            CanvasGroup cg = oneCharacter.GetComponent<CanvasGroup>();
            if (cg == null) cg = oneCharacter.AddComponent<CanvasGroup>();
            spawnedCanvasGroups.Add(cg);
        }

        // 2. LIGNE CORRECTIVE : On force Unity à calculer la position des prefabs immédiatement
        LayoutRebuilder.ForceRebuildLayoutImmediate(characterSelection);

        // 3. Assigner les fonctions aux flèches
        if (leftButton != null) leftButton.onClick.AddListener(SelectPreviousCharacter);
        if (rightButton != null) rightButton.onClick.AddListener(SelectNextCharacter);

        // 4. Initialiser la position de départ sur le premier perso (maintenant que les positions sont connues)
        UpdateTargetPosition();
        UpdateButtonVisibility();

        // Optionnel : On téléporte directement le menu sur le premier perso au démarrage sans transition
        characterSelection.anchoredPosition = targetPosition;
    }

    private void Update()
    {
        // Déplacement fluide vers le personnage sélectionné
        characterSelection.anchoredPosition = Vector2.Lerp(characterSelection.anchoredPosition, targetPosition, Time.deltaTime * transitionSpeed);

        // L'effet d'alpha reste fluide pendant le déplacement
        UpdateCharactersAlpha();
    }

    public void SelectPreviousCharacter()
    {
        if (currentCharacterIndex > 0)
        {
            currentCharacterIndex--;
            UpdateTargetPosition();
            UpdateButtonVisibility();
            OnCharacterChanged(currentCharacterIndex);
        }
    }

    public void SelectNextCharacter()
    {
        if (currentCharacterIndex < spawnedCharacters.Count - 1)
        {
            currentCharacterIndex++;
            UpdateTargetPosition();
            UpdateButtonVisibility();
            OnCharacterChanged(currentCharacterIndex);
        }
    }

    private void UpdateTargetPosition()
    {
        if (spawnedCharacters.Count == 0) return;

        // On prend la position locale X du personnage cible
        float characterOffset = spawnedCharacters[currentCharacterIndex].localPosition.x;

        // La cible du conteneur est l'inverse de la position du perso pour le ramener au centre
        targetPosition = new Vector2(-characterOffset, characterSelection.anchoredPosition.y);
    }

    private void UpdateButtonVisibility()
    {
        if (leftButton != null) leftButton.interactable = (currentCharacterIndex > 0);
        if (rightButton != null) rightButton.interactable = (currentCharacterIndex < spawnedCharacters.Count - 1);
    }

    private void UpdateCharactersAlpha()
    {
        for (int i = 0; i < spawnedCharacters.Count; i++)
        {
            float distance = Mathf.Abs(spawnedCharacters[i].position.x - screenCenter);
            float alpha = Mathf.Lerp(1f, minAlpha, distance / fadeDistance);
            spawnedCanvasGroups[i].alpha = alpha;
        }
    }

    private void OnCharacterChanged(int index)
    {
        CharacterSO selectedCharacter = characterList[index];
        Debug.Log("Personnage sélectionné au milieu : " + selectedCharacter.name);
    }
}