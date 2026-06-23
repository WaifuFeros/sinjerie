using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionManager : MonoBehaviour
{
    public static DescriptionManager Instance { get; private set; }

    [SerializeField] private DescriptionWindow _descriptionWindow;
    [field: SerializeField, Min(0)] public float DescriptionPressTime { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void DisplayDescription(ObjetSO item)
    {
        if (item == null)
            return;

        _descriptionWindow?.DisplayDescription(item);
    }
}
