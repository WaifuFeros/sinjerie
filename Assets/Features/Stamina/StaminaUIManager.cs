using UnityEngine;
using System.Collections.Generic;

public class StaminaUIManager : MonoBehaviour
{
    public static StaminaUIManager Instance { get; private set; }

    [Header("Settings")]
    public GameObject staminaPointPrefab;

    private List<StaminaPoint> staminaPoints = new List<StaminaPoint>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void SetupStamina(int maxStamina)
    {
        // clear les anciens
        foreach (Transform child in transform) Destroy(child.gameObject);
        staminaPoints.Clear();

        // ajoute les nouveaux
        for (int i = 0; i < maxStamina; i++)
        {
            GameObject newPoint = Instantiate(staminaPointPrefab, transform);
            StaminaPoint staminaImage = newPoint.GetComponent<StaminaPoint>();
            staminaPoints.Add(staminaImage);
        }
    }

    // Met à jour l'affichage
    public void UpdateDisplay(int currentStamina)
    {
        for (int i = 0; i < staminaPoints.Count; i++)
        {
            staminaPoints[i].SetStamina(i < currentStamina);
        }
    }

    public void DisplayStaminaPreview(int currentStamina, int neededStamina)
    {
        bool isEnough = currentStamina >= neededStamina;

        int startingPoint = 0;
        if (isEnough)
            startingPoint = currentStamina - neededStamina;

        if (isEnough)
        {
            for (int i = 0; i < staminaPoints.Count; i++)
            {
                if (i < startingPoint || i >= currentStamina)
                    staminaPoints[i].HideOutline();
                else
                    staminaPoints[i].DisplayOutline(isEnough);
            }
        }
        else
        {
            for (int i = 0; i < staminaPoints.Count; i++)
            {
                if (i < neededStamina)
                    staminaPoints[i].DisplayOutline(isEnough);
                else
                    staminaPoints[i].HideOutline();
            }
        }
    }

    public void HideStaminaPreview()
    {
        for (int i = 0; i < staminaPoints.Count; i++)
        {
            staminaPoints[i].HideOutline();
        }
    }
}