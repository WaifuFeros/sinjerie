using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSO> characterList;
    [SerializeField] private GameObject characterPrefab;

    [Header("Data Storage")]
    [SerializeField] private SelectedCharacterData dataStorage;

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

        foreach (CharacterSO character in characterList)
        {
            GameObject oneCharacter = Instantiate(characterPrefab, characterSelection);
            oneCharacter.GetComponent<Image>().sprite = character.characterSprite;

            spawnedCharacters.Add(oneCharacter.GetComponent<RectTransform>());

            CanvasGroup cg = oneCharacter.GetComponent<CanvasGroup>();
            if (cg == null) cg = oneCharacter.AddComponent<CanvasGroup>();
            spawnedCanvasGroups.Add(cg);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(characterSelection);

        if (leftButton != null) leftButton.onClick.AddListener(SelectPreviousCharacter);
        if (rightButton != null) rightButton.onClick.AddListener(SelectNextCharacter);

        UpdateTargetPosition();
        UpdateButtonVisibility();

        characterSelection.anchoredPosition = targetPosition;

        SaveSelection();
    }

    private void Update()
    {
        characterSelection.anchoredPosition = Vector2.Lerp(characterSelection.anchoredPosition, targetPosition, Time.deltaTime * transitionSpeed);
        UpdateCharactersAlpha();
    }

    public void SelectPreviousCharacter()
    {
        if (currentCharacterIndex > 0)
        {
            currentCharacterIndex--;
            UpdateTargetPosition();
            UpdateButtonVisibility();
            SaveSelection();
        }
    }

    public void SelectNextCharacter()
    {
        if (currentCharacterIndex < spawnedCharacters.Count - 1)
        {
            currentCharacterIndex++;
            UpdateTargetPosition();
            UpdateButtonVisibility();
            SaveSelection();
        }
    }

    private void SaveSelection()
    {
        if (dataStorage != null && characterList.Count > 0)
        {
            dataStorage.selectedCharacter = characterList[currentCharacterIndex];
        }
    }

    private void UpdateTargetPosition()
    {
        if (spawnedCharacters.Count == 0) return;
        float characterOffset = spawnedCharacters[currentCharacterIndex].localPosition.x;
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
}