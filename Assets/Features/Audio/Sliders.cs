using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    private Slider slider;
    public string VcaName;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (slider != null && VcaController.Instance != null)
        {
            // 1. On récupère le volume sauvegardé
            float startVolume = VcaController.Instance.GetSavedVolume(VcaName);

            // 2. CORRECTION : On écoute D'ABORD les changements
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            // 3. CORRECTION : On applique la valeur ENSUITE. 
            // Cela va instantanément déclencher OnSliderValueChanged et réveiller FMOD !
            slider.value = startVolume;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        VcaController.Instance.SetVolume(VcaName, value);
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}