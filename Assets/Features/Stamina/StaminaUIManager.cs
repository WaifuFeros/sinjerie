using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StaminaUIManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject staminaPointPrefab;
    public Sprite fullStaminaSprite;
    public Sprite emptyStaminaSprite;

    private List<Image> staminaPoints = new List<Image>();


    public void SetupStamina(int maxStamina)
    {
        // clear les anciens
        foreach (Transform child in transform) Destroy(child.gameObject);
        staminaPoints.Clear();

        // ajoute les nouveaux
        for (int i = 0; i < maxStamina; i++)
        {
            GameObject newPoint = Instantiate(staminaPointPrefab, transform);
            Image staminaImage = newPoint.GetComponent<Image>();
            staminaPoints.Add(staminaImage);
        }
    }

    // Met à jour l'affichage
    public void UpdateDisplay(int currentStamina)
    {
        for (int i = 0; i < staminaPoints.Count; i++)
        {
            if (i < currentStamina)
                staminaPoints[i].sprite = fullStaminaSprite;
            else
                staminaPoints[i].sprite = emptyStaminaSprite;
        }
    }
}